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

namespace Pulsa.Service.Service
{
   
    public class DflashService : IDflashService
    {
        IUnitOfWork _unitOfWork;
        ITagihanDetailRepository _tagihanDetailRepository;
        ITagihanMasterRepository _tagihanMasterRepository;
        ISupplier_produkRepository _supplier_produkRepository;
        IPenggunaTransaksiRepository _penggunaTransaksi;

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
                        //var saveData = _unitOfWork.Complete();
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

        public static string CalculateSign(string template)
        {
            string sign = template.TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return sign;
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

