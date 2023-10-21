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
using Pulsa.ViewModel.Dflash;
using Pulsa.ViewModel.tagihan;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static Microsoft.Graph.Constants;
using Pulsa.DataAccess.Repository;
using System.Collections.Generic;

namespace Pulsa.Service.Service
{
   
    public class DflashService : IDflashService
    {
        IUnitOfWork _unitOfWork;
        ITagihanDetailRepository _tagihanDetailRepository;
        ITagihanMasterRepository _tagihanMasterRepository;
        Pulsa.DataAccess.Interface.ISupplier_produkRepository _suppliyerProduk;
        IPenggunaTransaksiRepository _penggunaTransaksi;
        ISupplier_produkRepository _supplier_produkRepository;
        ISaldoRefundRepository _saldoRefund;
        ITopUpService _topUpService;
        IPenggunaRepository _penggunaRepository;

        private IMapper _mapper;
        private readonly PulsaDataContext _context;
        private readonly IConfiguration _configuration;
        public String _baseUrl { get; }
        public String _memberId { get; }
        public String _password { get; }
        public String _ipTransit { get; }
        public String _pin { get; }
        private readonly HttpClient _client;


        public DflashService(
                IUnitOfWork unitOfWork,
                IConfiguration configuration,
                HttpClient client,
                ITagihanDetailRepository tagihanDetailRepository,
                ITagihanMasterRepository tagihanMasterRepository,
                ISupplier_produkRepository suppliyerProduk,
                IPenggunaTransaksiRepository penggunaTransaksi,
                IMapper mapper,
                PulsaDataContext context,
                ISupplier_produkRepository Supplier_produkRepository,
                ITopUpService topUpService,
                ISaldoRefundRepository saldoRefund,
                IPenggunaRepository penggunaRepository


        ) {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _client = client;
            _tagihanDetailRepository = tagihanDetailRepository;
            _tagihanMasterRepository = tagihanMasterRepository;
            _suppliyerProduk = suppliyerProduk;
            _penggunaTransaksi = penggunaTransaksi;
            _mapper = mapper;
            _context = context;
            _supplier_produkRepository = Supplier_produkRepository;
            _topUpService = topUpService;
            _saldoRefund = saldoRefund;
            _penggunaRepository = penggunaRepository;

            _baseUrl = configuration.GetSection("dflah_ip").Value;
            _memberId = configuration.GetSection("dflah_id").Value;
            _password = configuration.GetSection("dflah_password").Value;
            _pin = configuration.GetSection("dflah_pin").Value;
            _ipTransit = configuration.GetSection("ip_transit").Value;
        }
        public async Task<int> getSaldo()
        {
            HttpClient _httpClient = new HttpClient();
            string sign = CalculateSign(CalculateTemplate());
            string fullUrl = _baseUrl + "balance?memberID=" + _memberId + "&sign=" + sign;

            var data = new
            {
                destUrl = fullUrl,
                method = "get"
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var responseCheck = await _httpClient.PostAsync(_ipTransit, content);
            var responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            
            var respond = JsonConvert.DeserializeObject<DflashSaldoDTO>(responseStringCheck);

            return respond.saldo;
        }

        public async Task<string> getTagihan(TagihanMasterDTO tm)
        {
            HttpClient _httpClient = new HttpClient();
            String ref_id = tm.id_tagihan+ "_"+DateTime.Now.Month + DateTime.Now.Day;
            string codeProduk = "";
            if (tm.type_tagihan == "telkom")
            {
                codeProduk = "CTEL";
            }
            else if(tm.type_tagihan == "pln")
            {
                codeProduk = "CPLN";
            }
            string sign = CalculateSign(CalculateTemplate(codeProduk, tm.id_tagihan, ref_id));
            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");

            string fullUrl = _baseUrl + "trx?memberID=" + _memberId + "&product="+ codeProduk + "&dest=" + tm.id_tagihan + "&refID=" + ref_id + "&sign=" + sign;
            string responseStringCheck = "";
            int maxRetries = 3;
            TimeSpan retryDelay = TimeSpan.FromMilliseconds(200);

            if (_ipTransit == "")
            {
                HttpResponseMessage responseCheck = await _httpClient.GetAsync(fullUrl);
                responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            }
            else
            {
                var data = new
                {
                    destUrl = fullUrl,
                    method = "get"
                };
                try {
                    HttpResponseMessage responseCheck = await RetryHttpPostAsync(_ipTransit, data, maxRetries, retryDelay);
                    responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }

            var tagihan = JsonConvert.DeserializeObject<DflashRespond>(responseStringCheck);
            if (tagihan == null)
                return "";

            if ((tagihan.status == 20 || tagihan.status == 22) && tagihan.sn != null)
            {
                // cek tagihan sukses
                string[] segments = tagihan.sn.Split('/');
                string[] keterangan = tagihan.keterangan.Split('/');
                if (tm.nama_pelanggan == null || tm.nama_pelanggan == "")
                {
                    var tagihanMaster = _tagihanMasterRepository.Find(a => a.id == tm.id).FirstOrDefault();
                    tagihanMaster.nama_pelanggan = segments[0];
                    lock (_tagihanMasterRepository)
                    {
                        _tagihanMasterRepository.Update(tagihanMaster);
                        _tagihanMasterRepository.Save();
                    }
                }

                // Find the segment that starts with "TAG:" to get "80619".
                string tagSegment = Array.Find(segments, s => s.StartsWith("TAG:"));
                string tagValue = tagSegment != null ? tagSegment.Substring(4) : null;

                // todo save to tagihanDetail
                string btketerangan = Array.Find(keterangan, s => s.StartsWith("BT="));
                string btValue = btketerangan != null ? btketerangan.Substring(3) : null;


                Domain.Entities.Tagihan_detail td = new Tagihan_detail();
                td.id_tagihan_master = tm.id;
                td.ref_id = ref_id;
                td.periode_tagihan = btValue;
                td.jumlah_tagihan = Convert.ToInt32(tagValue);
                td.tanggal_cek = DateTime.Now.Date;
                //td.admin_tagihan = Convert.ToInt32(tagihanListrik.responseData.biaya_admin);

                lock (_tagihanDetailRepository)
                {
                    var dataTagihan = _tagihanDetailRepository.Find(a => a.id_tagihan_master == td.id_tagihan_master && a.tanggal_cek >= awalBulan).FirstOrDefault();
                    if (dataTagihan == null)
                    {
                        _tagihanDetailRepository.Add(td);
                        _tagihanDetailRepository.Save();
                    }
                }

                return responseStringCheck;
            }
            else if (tagihan.status == 43)
            {
                // transaksi gagal
            }
            return "gagal";
        }

        public async Task<string> PayTagihan(TagihanMasterDTO tm)
        {
            var client = new HttpClient();
            HttpClient _httpClient = new HttpClient();
            String ref_id = tm.id_tagihan + "_" + DateTime.Now.Month + DateTime.Now.Day;
            string codeProduk = "";
            if (tm.type_tagihan == "telkom")
            {
                codeProduk = "CTEL";
            }
            else if (tm.type_tagihan == "pln")
            {
                codeProduk = "CPLN";
            }
            string sign = CalculateSign(CalculateTemplate(codeProduk, tm.id_tagihan, ref_id));
            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");

            string fullUrl = _baseUrl + "trx?memberID=" + _memberId + "&product=" + codeProduk + "&dest=" + tm.id_tagihan + "&refID=" + ref_id + "&sign=" + sign;
            string responseStringCheck = "";
            int maxRetries = 3;
            TimeSpan retryDelay = TimeSpan.FromMilliseconds(200);

            if (_ipTransit == "")
            {
                HttpResponseMessage responseCheck = await _httpClient.GetAsync(fullUrl);
                responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            }
            else
            {
                var data = new
                {
                    destUrl = fullUrl,
                    method = "get"
                };
                try
                {
                    HttpResponseMessage responseCheck = await RetryHttpPostAsync(_ipTransit, data, maxRetries, retryDelay);
                    responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }

            var tagihan = JsonConvert.DeserializeObject<DflashRespond>(responseStringCheck);
            if (tagihan == null)
                return "";

            if ((tagihan.status == 20 || tagihan.status == 22) && tagihan.sn != null)
            {
                // cek tagihan sukses

                string[] segments = tagihan.sn.Split('/');
                string[] keterangan = tagihan.keterangan.Split('/');
                if (tm.nama_pelanggan == null || tm.nama_pelanggan == "")
                {
                    var tagihanMaster = _tagihanMasterRepository.Find(a => a.id == tm.id).FirstOrDefault();
                    tagihanMaster.nama_pelanggan = segments[0];
                    lock (_tagihanMasterRepository)
                    {
                        _tagihanMasterRepository.Update(tagihanMaster);
                        _tagihanMasterRepository.Save();
                    }
                }

                // Find the segment that starts with "TAG:" to get "80619".
                string tagSegment = Array.Find(segments, s => s.StartsWith("TAG:"));
                string tagValue = tagSegment != null ? tagSegment.Substring(4) : null;

                // todo save to tagihanDetail
                string btketerangan = Array.Find(keterangan, s => s.StartsWith("BT="));
                string btValue = btketerangan != null ? btketerangan.Substring(3) : null;


                Domain.Entities.Tagihan_detail td = new Tagihan_detail();
                td.id_tagihan_master = tm.id;
                td.ref_id = ref_id;
                td.periode_tagihan = btValue;
                td.jumlah_tagihan = Convert.ToInt32(tagValue);
                td.tanggal_cek = DateTime.Now.Date;
                //td.admin_tagihan = Convert.ToInt32(tagihanListrik.responseData.biaya_admin);

                lock (_tagihanDetailRepository)
                {
                    var dataTagihan = _tagihanDetailRepository.Find(a => a.id_tagihan_master == td.id_tagihan_master && a.tanggal_cek >= awalBulan).FirstOrDefault();
                    if (dataTagihan == null)
                    {
                        _tagihanDetailRepository.Add(td);
                        _tagihanDetailRepository.Save();
                    }
                }

                return responseStringCheck;
            }
            else if (tagihan.status == 43)
            {
                // transaksi gagal
            }
            return "gagal";

        }

        public async Task<List<Supplier_produk>> refressProduk(string dateUpdate)
        {
            List<Supplier_produk> listSp = new List<Supplier_produk>();
            HttpClient _httpClient = new HttpClient();
            string fullUrl = "https://dflash.co.id/harga/pricelist_json.php";
            string responseStringCheck = "";
            HttpResponseMessage responseCheck = await _httpClient.GetAsync(fullUrl);
            responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            if (responseStringCheck != "") {


                List<DflashProviderData> result = JsonConvert.DeserializeObject<List<DflashProviderData>>(responseStringCheck);
                

                foreach (var providerData in result)
                {
                    foreach (var product in providerData.data)
                    {
                        Supplier_produk sp = new Supplier_produk();
                        sp.supplier = "dflash";
                        sp.operator_name = providerData.provider;
                        sp.category = providerData.kategori;

                        sp.supplierkey = "dflash" + product.kode;
                        sp.product_id = product.kode;
                        sp.product_name = product.nama;
                        sp.product_price = product.harga;
                        sp.product_id = product.kode;
                        sp.status = Convert.ToString(product.status);
                        sp.updated_at = dateUpdate;
                        listSp.Add(sp);
                    }
                }
            }
            return listSp;
        }

        private string CalculateTemplate(string product = "", string dest = "", string refID = "")
        {
            string input = $"OtomaX|{_memberId}|{product}|{dest}|{refID}|{_pin}|{_password}";
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            using (SHA1 sha1 = new SHA1Managed())
            {
                byte[] hashBytes = sha1.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public void saveProduk(string dateUpdate, List<Supplier_produk> listSp) {
            lock (_supplier_produkRepository)
            {
                // remove produk
                var removeProduk = _supplier_produkRepository.Find(a => a.updated_at != dateUpdate && a.supplier == "dflash");;
                _supplier_produkRepository.RemoveRange(removeProduk);

                _supplier_produkRepository.AddRange(listSp);
                _supplier_produkRepository.Save();

                //int batchSize = 100;
                //// add produk
                //for (int i = 0; i < listSp.Count; i += batchSize)
                //{
                //    var batch = listSp.Skip(i).Take(batchSize).ToList();
                //    _supplier_produkRepository.AddRange(batch);
                //    _supplier_produkRepository.Save();
                //}
            }
        }

        public static string CalculateSign(string template)
        {
            string sign = template.TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return sign;
        }
        public async Task<string> order(string produkId, string dest, string refId)
        {
            HttpClient _httpClient = new HttpClient();

            string sign = CalculateSign(CalculateTemplate(produkId, dest, refId));
            string fullUrl = _baseUrl + "trx?memberID=" + _memberId + "&product=" + produkId + "&dest=" + dest + "&refID=" + refId + "&sign=" + sign;
            string responseStringCheck = "";
            if (_ipTransit == "")
            {
                HttpResponseMessage responseCheck = await _httpClient.GetAsync(fullUrl);
                responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            }
            else
            {
                int maxRetries = 3;
                TimeSpan retryDelay = TimeSpan.FromMilliseconds(200);
                var data = new
                {
                    destUrl = fullUrl,
                    method = "get"
                };
                try
                {
                    HttpResponseMessage responseCheck = await RetryHttpPostAsync(_ipTransit, data, maxRetries, retryDelay);
                    responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
            return responseStringCheck;
        }
        public async Task<string> cekTransaksiPending(Pengguna_Traksaksi pengguna_Traksaksi)
        {
            HttpClient _httpClient = new HttpClient();
            string produkId = pengguna_Traksaksi.product_id;
            string dest = pengguna_Traksaksi.tujuan;
            string refId = pengguna_Traksaksi.ref_id;
            string sign = CalculateSign(CalculateTemplate(produkId, dest, refId));
            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
            string fullUrl = _baseUrl + "check?memberID=" + _memberId + "&product=" + produkId + "&dest=" + dest + "&refID=" + refId + "&sign=" + sign;
            string responseStringCheck = "";
            if (_ipTransit == "")
            {
                HttpResponseMessage responseCheck = await _httpClient.GetAsync(fullUrl);
                responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            }
            else
            {
                int maxRetries = 3;
                TimeSpan retryDelay = TimeSpan.FromMilliseconds(200);
                var data = new
                {
                    destUrl = fullUrl,
                    method = "get"
                };
                try
                {
                    HttpResponseMessage responseCheck = await RetryHttpPostAsync(_ipTransit, data, maxRetries, retryDelay);
                    responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    return "";
                }
            }

            var respond = JsonConvert.DeserializeObject<DflashCekTransaksi>(responseStringCheck);
            if (respond == null) {
                return "";
            }
            var transaksiPending = _penggunaTransaksi.GetById(pengguna_Traksaksi.id);
            if (respond.status == 0 || respond.status == 1 || respond.status == 2 || respond.status == 22)
            {
                transaksiPending.status_transaksi = 1;
            }
            else if (respond.status == 20)
            {
                transaksiPending.status_transaksi = 2;
                transaksiPending.sn = respond.sn;
                transaksiPending.harga = Convert.ToInt32(respond.harga);
            }
            else {
                if (transaksiPending.status_transaksi != 3) {
                    lock (_penggunaRepository)
                    {
                        var pengguna = _penggunaRepository.Find(a => a.id == pengguna_Traksaksi.pengguna).SingleOrDefault();
                        int saldoAwal = pengguna.saldo;
                        pengguna.saldo = saldoAwal + transaksiPending.harga_jual;

                        Saldo_refund sr = new Saldo_refund();
                        sr.jumlah = transaksiPending.harga_jual;
                        sr.idpengguna = pengguna_Traksaksi.pengguna;
                        sr.note = respond.keterangan;
                        sr.idtransaksi = pengguna_Traksaksi.id;
                        sr.saldo_awal = saldoAwal;
                        sr.saldo_akhir = pengguna.saldo;

                        _penggunaRepository.Update(pengguna);
                        _penggunaRepository.Save();

                        _saldoRefund.Add(sr);
                    }
                    transaksiPending.status_transaksi = 3;
                }
            }

            _penggunaTransaksi.Update(transaksiPending);
            _penggunaTransaksi.Save();


            return "";

        }
        private async Task<HttpResponseMessage> RetryHttpPostAsync(string url, object data, int maxRetries, TimeSpan retryDelay)
        {
            HttpClient _httpClient = new HttpClient();
            int attempts = 0;
            while (attempts < maxRetries)
            {
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    var response = await _httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                }
                catch (HttpRequestException)
                {
                    // Handle the exception, log it, or perform other actions as needed.
                }

                // If the request was not successful or an exception occurred, wait before retrying.
                await Task.Delay(retryDelay);
                attempts++;
            }

            // If maxRetries is reached, you may want to throw an exception or return a default response.
            Console.WriteLine("Max retries exceeded, and the request was not successful.");
            Console.WriteLine("url " + url);
            Console.WriteLine("data " + data);
            throw new Exception("Max retries exceeded, and the request was not successful.");
        }
    }
}

