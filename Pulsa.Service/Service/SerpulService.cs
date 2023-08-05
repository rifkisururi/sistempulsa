using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Newtonsoft.Json;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.Domain.Entities;
using Pulsa.Helper;
using Pulsa.Service.Interface;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Pulsa.Service.Service
{
   
    public class SerpulService : ISerpulService
    {
        IUnitOfWork _unitOfWork;
        ITagihanDetailRepository _tagihanDetailRepository;
        ITagihanMasterRepository _tagihanMasterRepository;
        ISupplier_produkRepository _supplier_produkRepository;
        IPenggunaTransaksiRepository _penggunaTransaksi;

        private IMapper _mapper;
        private readonly PulsaDataContext _context;
        private readonly IConfiguration _configuration;
        public string _baseUrl { get; }
        public string _apiKey { get; }
        private readonly HttpClient _client;
        

        public SerpulService(
                IUnitOfWork unitOfWork,
                IConfiguration configuration,
                HttpClient client,
                ITagihanDetailRepository tagihanDetailRepository,
                ITagihanMasterRepository tagihanMasterRepository,
                ISupplier_produkRepository Supplier_produkRepository,
                IPenggunaTransaksiRepository penggunaTransaksi,
                IMapper mapper,
                PulsaDataContext context
        ) {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _client = client;
            _tagihanDetailRepository = tagihanDetailRepository;
            _tagihanMasterRepository = tagihanMasterRepository;
            _supplier_produkRepository = Supplier_produkRepository;
            _penggunaTransaksi = penggunaTransaksi;
            _mapper = mapper;
            _context = context;
            _apiKey = configuration.GetSection("serpul_apikey").Value;
            _baseUrl = configuration.GetSection("serpul_url").Value;
        }
        public int getSaldo()
        {
            HttpClient _httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "account");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", _apiKey);

            var response = _httpClient.Send(request);
            var responseCode = response.EnsureSuccessStatusCode();

            var todo = response.Content.ReadFromJsonAsync<SerpulRespondAccount>().Result;
            return todo.responseData.balance;
        }

        public async Task<string> getTagihan(TagihanMasterDTO tm)
        {
            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
            var cekData = _tagihanDetailRepository
                .Find(a => a.id_tagihan_master == tm.id && a.tanggal_cek >= awalBulan).FirstOrDefault();
            if (cekData == null)
            {

                var client = new HttpClient();
                String ref_id = "serpul_" + tm.id_tagihan;
                var data = new
                {
                    no_pelanggan = tm.id_tagihan,
                    product_id = tm.type_tagihan,
                    ref_id = ref_id
                };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", _apiKey);
                var response = await client.PostAsync(_baseUrl+"pascabayar/check", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var tagihan = JsonConvert.DeserializeObject<SerpulRespondStatus>(responseString);
                if (tagihan.responseCode == 200)
                {
                    var tagihanListrik = JsonConvert.DeserializeObject<SerpulRespondTagihanListrik>(responseString);

                    // save nama pelanggan
                    if (tm.nama_pelanggan == null || tm.nama_pelanggan == "")
                    {
                        var tagihanMaster = _tagihanMasterRepository.Find(a => a.id == tm.id).FirstOrDefault();
                        tagihanMaster.nama_pelanggan = tagihanListrik.responseData.nama_pelanggan;
                        _tagihanMasterRepository.Update(tagihanMaster);
                        _tagihanMasterRepository.Save();
                    }

                    // todo save to tagihanDetail
                    Domain.Entities.Tagihan_detail td = new Tagihan_detail();
                    td.id_tagihan_master = tm.id;
                    td.ref_id = tagihanListrik.responseData.ref_id;
                    td.periode_tagihan = tagihanListrik.responseData.periode;
                    td.jumlah_tagihan = Convert.ToInt32(tagihanListrik.responseData.jumlah_tagihan);
                    td.admin_tagihan = Convert.ToInt32(tagihanListrik.responseData.biaya_admin);

                    var dataTagihan = _tagihanDetailRepository.Find(a => a.id_tagihan_master == td.id_tagihan_master && a.tanggal_cek >= awalBulan).FirstOrDefault();
                    if (dataTagihan == null) {
                        _tagihanDetailRepository.Add(td);
                        _tagihanDetailRepository.Save();
                        //var saveData = _unitOfWork.Complete();
                    }
                    return responseString;
                }
                else { 
                    return "Belum tersedia";
                }
            }
            return "sudah ada";

        }

        public async Task<string> PayTagihan(TagihanMasterDTO tm)
        {
            var client = new HttpClient();
            String yearMonth = Convert.ToString(DateTime.Now.Year) + Convert.ToString(DateTime.Now.Month);

            String ref_id = "serpul_"+ yearMonth+ "_" + tm.id_tagihan ;
            var data = new
            {
                no_pelanggan = tm.id_tagihan,
                product_id = tm.type_tagihan,
                ref_id = ref_id
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", _apiKey);
            var responseCheck = await client.PostAsync(_baseUrl+"pascabayar/check", content);
            var responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            var tagihanCheck = JsonConvert.DeserializeObject<SerpulRespondStatus>(responseStringCheck);
            if (tagihanCheck.responseCode == 200)
            {
                var response = await client.PostAsync(_baseUrl + "pascabayar/pay", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var tagihan = JsonConvert.DeserializeObject<SerpulRespondStatus>(responseString);
                if (tagihan.responseCode == 200)
                {
                    var tagihanListrik = JsonConvert.DeserializeObject<SerpulRespondPaymentBill>(responseString);

                    DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
                    var dtTagihan = _tagihanDetailRepository
                        .Find(a => a.id_tagihan_master == tm.id && a.tanggal_cek >= awalBulan).FirstOrDefault();
                    // todo save to tagihanDetail
                    dtTagihan.request_bayar = true;
                    dtTagihan.ref_id = ref_id;
                    dtTagihan.tanggal_bayar = DateTime.Now;
                    _tagihanDetailRepository.Update(dtTagihan);
                    _tagihanDetailRepository.Save();
                    return responseString;
                }
                else
                {
                    return "Belum tersedia";
                }
            }
            else {
                return responseStringCheck;
            }
        }

        public async Task<string> orderPrabayar(string produkId, string dest, string refId) {
            var client = new HttpClient();
            var data = new
            {
                destination = dest,
                product_id = produkId,
                ref_id = refId
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", _apiKey);
            var responseCheck = await client.PostAsync(_baseUrl + "prabayar/order", content);
            var responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            var serpulRespondStatus = JsonConvert.DeserializeObject<SerpulRespondStatus>(responseStringCheck);
            return responseStringCheck;
        }

        public async Task<List<Supplier_produk>> refressProduk() {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", _apiKey);
            var responseCheckCategory = await client.GetAsync(_baseUrl+ "prabayar/category");
            var responseCategoryCheck = await responseCheckCategory.Content.ReadAsStringAsync();
            var category = JsonConvert.DeserializeObject<responseDataCategory>(responseCategoryCheck);
            string updated_at  = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            List<Supplier_produk> sp = new List<Supplier_produk>();
            foreach(var c in category.responseData) {
                if (c.status.ToLower() == "active") {
                    var responseCategoryProduk = await client.GetAsync(_baseUrl + "prabayar/operator?product_id=" + c.product_id);
                    var responseCategoryProdukCheck = await responseCategoryProduk.Content.ReadAsStringAsync();
                    var categoryProduk = JsonConvert.DeserializeObject<responseDataProdukOperator>(responseCategoryProdukCheck);

                    foreach (var p in categoryProduk.responseData)
                    {
                        if (p.status.ToLower() == "active")
                        {
                            var responsePrabayarProduk = await client.GetAsync(_baseUrl + "prabayar/product?product_id=" + p.product_id);
                            var responsePrabayarProdukCheck = await responsePrabayarProduk.Content.ReadAsStringAsync();
                            var prabayarProduk = JsonConvert.DeserializeObject<responsePrabayarProduk>(responsePrabayarProdukCheck);
                            foreach (var pp in prabayarProduk.responseData) {
                                //int? dp = null;
                                var dataInsert = _mapper.Map<Supplier_produk>(pp);
                                dataInsert.supplier = "serpul";
                                dataInsert.updated_at = updated_at;
                                sp.Add(dataInsert);
                            }
                        }
                    }
                }
            }

            return sp;
        }

        public bool saveProduk(List<Supplier_produk> dt){
            _supplier_produkRepository.AddRange(dt);
            _supplier_produkRepository.Save();
            return true;
        }


        public List<Tagihan_detail> cekTransaksiPascaPending() {
            var dtTagihan = _tagihanDetailRepository.Find(a => a.request_bayar == true && a.status_bayar == false).AsNoTracking().ToList();
            return dtTagihan;
        }

        public List<Pengguna_Traksaksi> cekTransaksiPrabayarPending()
        {
            var dtTagihan = _penggunaTransaksi.Find(a => a.status_transaksi == 1 && a.suppliyer == "serpul").AsNoTracking().ToList();
            return dtTagihan;
        }

        public async Task<string> cekTransaksiPendingPasca(string refId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", _apiKey);
            var cekPendingPasca = await client.GetAsync(_baseUrl + "pascabayar/history/"+ refId);
            var responseStringPendingPasca = await cekPendingPasca.Content.ReadAsStringAsync();
            var respond = JsonConvert.DeserializeObject<SerpulRespondStatus>(responseStringPendingPasca);
            if (respond.responseCode == 200)
            {
                var respondCekBill = JsonConvert.DeserializeObject<SerpulRespondCekBillPasca>(responseStringPendingPasca);

                var dtTagihan = _tagihanDetailRepository.Find(a => a.ref_id == refId ).FirstOrDefault();
                dtTagihan.jumlah_tagihan = respondCekBill.responseData.bill_tagihan;
                dtTagihan.tanggal_bayar = DateTime.Now;
                dtTagihan.admin_transaksi = respondCekBill.responseData.bill_admin - respondCekBill.responseData.bill_fee;
                dtTagihan.status_bayar = true;
                dtTagihan.sn = respondCekBill.responseData.serial_number;

                _tagihanDetailRepository.Update(dtTagihan);
                _tagihanDetailRepository.Save();
            }
            else {
                return respond.responseMessage;
            }
            return "";
        }

        public async Task<string> cekTransaksiPendingPrabayar(string refId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", _apiKey);
            var cekPendingPasca = await client.GetAsync(_baseUrl + "prabayar/history/" + refId);
            var responseStringPendingPasca = await cekPendingPasca.Content.ReadAsStringAsync();
            var respond = JsonConvert.DeserializeObject<SerpulRespondStatus>(responseStringPendingPasca);
            if (respond.responseCode == 200)
            {
                var respondCekBill = JsonConvert.DeserializeObject<SerpulRespondCekBillPrabayar>(responseStringPendingPasca);
                var transaksiPending = _penggunaTransaksi.GetById(Guid.Parse(refId));
                if (respondCekBill.responseData.status == "SUCCESS" && transaksiPending.status_transaksi != 3) {
                    transaksiPending.sn = respondCekBill.responseData.serial_number;
                    transaksiPending.status_transaksi = 2;
                    transaksiPending.harga = respondCekBill.responseData.price;
                }
                else if(respondCekBill.responseData.status == "FAILED" && transaksiPending.status_transaksi != 2)
                {
                    transaksiPending.status_transaksi = 3;
                    // todo kembalikan deposit
                }
                _penggunaTransaksi.Update(transaksiPending);
                _penggunaTransaksi.Save();
            }
            else
            {
                return respond.responseMessage;
            }
            return "";
        }


        public async Task<string> cekPln(string noPln)
        {
            string namaPln = "Tidak ditemukan";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", _apiKey);

            var cekPLN = await client.GetAsync(_baseUrl + "checkplnprepaid?no_pelanggan=" + noPln);
            var responseCekPLN = await cekPLN.Content.ReadAsStringAsync();
            var respond = JsonConvert.DeserializeObject<RespondCekPLN>(responseCekPLN);

            if (respond.status == "success")
            {
                namaPln = respond.data.nama_pelanggan+ " " + respond.data.tarif_daya;
            }
            return namaPln;
        }

    }
}

