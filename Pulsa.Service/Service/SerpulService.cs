using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.Domain.Entities;
using Pulsa.Helper;
using Pulsa.Service.Interface;
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
        IConfiguration _configuration;
        ITagihanDetailRepository _tagihanDetailRepository;
        ITagihanMasterRepository _tagihanMasterRepository;

        public string apiKey;
        public string _baseUrl = "https://api.serpul.co.id/";
        public string _apiKey = "57a20250296598dd9f079e2b05f09f24";
        private readonly HttpClient _client;

        public SerpulService(
                IUnitOfWork unitOfWork,
                IConfiguration configuration,
                HttpClient client,
                ITagihanDetailRepository tagihanDetailRepository,
                ITagihanMasterRepository tagihanMasterRepository
        ) {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            apiKey = _configuration["email_template_path"];
            _client = client;
            _tagihanDetailRepository = tagihanDetailRepository;
            _tagihanMasterRepository = tagihanMasterRepository;
        }
        public int getSaldo()
        {
            HttpClient _httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "account");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", _apiKey);

            var response = _httpClient.Send(request);
            var responseCode = response.EnsureSuccessStatusCode();

            var todo = response.Content.ReadFromJsonAsync<RespondStatus>().Result;
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
                var response = await client.PostAsync("https://api.serpul.co.id/pascabayar/check", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var tagihan = JsonConvert.DeserializeObject<RespondSerpul>(responseString);
                if (tagihan.responseCode == 200)
                {
                    var tagihanListrik = JsonConvert.DeserializeObject<RespondStatusSerpulTagihanListrik>(responseString);

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

                    _tagihanDetailRepository.Add(td);
                    _tagihanDetailRepository.Save();
                    //var saveData = _unitOfWork.Complete();
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
            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
            var cekData = _tagihanDetailRepository
                .Find(a => a.id_tagihan_master == tm.id && a.tanggal_cek >= awalBulan).FirstOrDefault();

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
            var responseCheck = await client.PostAsync("https://api.serpul.co.id/pascabayar/check", content);
            var responseStringCheck = await responseCheck.Content.ReadAsStringAsync();
            var tagihanCheck = JsonConvert.DeserializeObject<RespondSerpul>(responseStringCheck);
            if (tagihanCheck.responseCode == 200)
            {
                var response = await client.PostAsync("https://api.serpul.co.id/pascabayar/pay", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var tagihan = JsonConvert.DeserializeObject<RespondSerpul>(responseString);
                if (tagihan.responseCode == 200)
                {
                    var tagihanListrik = JsonConvert.DeserializeObject<RespondStatusSerpulTagihanListrik>(responseString);

                    // save nama pelanggan
                    if (tm.nama_pelanggan == null || tm.nama_pelanggan == "")
                    {
                        var tagihanMaster = _tagihanMasterRepository.Find(a => a.id == tm.id).FirstOrDefault();
                        tagihanMaster.nama_pelanggan = tagihanListrik.responseData.nama_pelanggan;
                        //_tagihanMasterRepository.Update(tagihanMaster);
                        //_tagihanMasterRepository.Save();
                    }

                    // todo save to tagihanDetail
                    Domain.Entities.Tagihan_detail td = new Tagihan_detail();
                    td.id_tagihan_master = tm.id;
                    td.ref_id = tagihanListrik.responseData.ref_id;
                    td.periode_tagihan = tagihanListrik.responseData.periode;
                    td.jumlah_tagihan = Convert.ToInt32(tagihanListrik.responseData.jumlah_tagihan);
                    td.admin_tagihan = Convert.ToInt32(tagihanListrik.responseData.biaya_admin);

                    //_tagihanDetailRepository.Add(td);
                    //_tagihanDetailRepository.Save();
                    //var saveData = _unitOfWork.Complete();
                    return responseString;
                }
                else
                {
                    return "Belum tersedia";
                }
            }
            else {
                return "Belum tersedia";
            }
        }

    }
}

public class SerpulAccount
{
    public string? id { get; set; }
    public string? name { get; set; }
    public string? company_name { get; set; }
    public string? phone { get; set; }
    public int balance { get; set; }
}

public class RespondStatus
{
    public string? responseStatus { get; set; }
    public string? responseMessage { get; set; }
    public int? responseCode { get; set; }
    public SerpulAccount? responseData { get; set; }
}

public class RespondSerpul
{
    public string? responseStatus { get; set; }
    public string? responseMessage { get; set; }
    public int? responseCode { get; set; }
}

public class RespondStatusSerpulTagihanListrik
{
    public string? responseStatus { get; set; }
    public string? responseMessage { get; set; }
    public Int32 responseCode { get; set; }
    public SerpulTagihanListrik responseData { get; set; }
}

public class SerpulTagihanListrik {
    public string? ref_id { get; set; }
    public string? no_pelanggan { get; set; }
    public string? nama_pelanggan { get; set; }
    public string? periode { get; set; }
    public string? multiplier { get; set; }
    public string? jumlah_tagihan { get; set; }
    public string? biaya_admin { get; set; }
    public string? total_tagihan { get; set; }
    public string? fee { get; set; }
    public string? total_bayar { get; set; }
    public List<DetailTagihan>? detail { get; set; }
    public string? description { get; set; }

}

public class DetailTagihan {
    public string? key { get; set; }
    public string? value { get; set; }
}