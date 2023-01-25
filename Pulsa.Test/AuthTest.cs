using Newtonsoft.Json;
using pulsa.Models;
using System.Text;

namespace Pulsa.Test
{
    public class AuthTest
    {
        private readonly HttpClient _client;
        private readonly String baseUrl = "https://localhost:44305/";

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var loginModel = new FMLogin { username = "test@email.com", password = "1" };

            // Act
            var response = await _client.PostAsync(
                            "https://localhost:44305/auth/Index",
                            new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json")
                        );

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("OK", response.StatusCode.ToString());
        }
    }
}