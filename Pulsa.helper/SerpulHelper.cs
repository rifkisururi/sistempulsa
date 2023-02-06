using Microsoft.Graph;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Helper
{
    public class SerpulHelper
    {
        public string _baseUrl = "https://api.serpul.co.id/";
        public string _apiKey = "57a20250296598dd9f079e2b05f09f24";
        
        private readonly HttpClient _httpClient;

        public SerpulHelper()
        {
            _httpClient = new HttpClient();
        }

        public static string GetClientInfo(string clientIP)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(clientIP);
                return $"{hostEntry.HostName}::{clientIP}";
            }
            catch (Exception ex)
            {
                return clientIP;
            }

        }
        public async Task<int> getSaldo()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl+ "account");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var todo = await response.Content.ReadFromJsonAsync<SerpulAccount>();
            return todo.balance;
        }

        public class SerpulAccount
        {
            public string? id { get; set; }
            public string? name { get; set; }
            public string? company_name { get; set; }
            public string? phone { get; set; }
            public int balance { get; set; }
        }
    }
}
