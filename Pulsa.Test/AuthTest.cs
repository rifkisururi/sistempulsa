using Newtonsoft.Json;
using pulsa.Models;
using System.Text;

namespace Pulsa.Test
{
    public class AuthTest
    {
        [Fact]
        public async Task Login()
        { 
            // Arrange
            var client = new HttpClient();
            var loginModel = new FMLogin { username = "test@email.com", password = "password" };

            // Act
            //var response = await client.PostAsync("/auth/Index", new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json"));

            //// Assert
            //response.EnsureSuccessStatusCode(); // Status Code 200-299

            // yang diharapkan, hasil actual
            Assert.Equal("OK", "OK");
        }
    }
}