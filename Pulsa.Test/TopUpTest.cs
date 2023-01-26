using Newtonsoft.Json;
using System.Text;

namespace Pulsa.Test
{
    public class TopUpTest
    {
        private readonly HttpClient _client;
        private readonly String baseUrl = "https://localhost:44305/";

        [Fact]
        public async Task addMetode()
        {
            // Arrange
            var data = new Domain.Entities.TopUp_metode { name = "bank jago (otomatis)", no_rek = "1234122"};

            // Act
            var response = await _client.PostAsync(
                            "https://localhost:44305/auth/Index",
                            new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                        );

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("OK", response.StatusCode.ToString());
        }
    }
}