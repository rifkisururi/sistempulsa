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
using System.Text.Json.Serialization;

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
    }
}

