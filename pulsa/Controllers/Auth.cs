using Firebase.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using pulsa.Models;
using Pulsa.Web.Models;
using Serilog;
using System.Net;
using System.Security.Claims;
using Supabase;
using Supabase.Interfaces;
using static Supabase.Gotrue.Constants;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Pulsa.Service.Interface;
using Pulsa.Service.Service;

namespace pulsa.Controllers
{
    public class Auth : Controller
    {
        FirebaseAuthProvider auth;

        private readonly Client _supabaseClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IPenggunaService _penggunaService;
        public Auth(Supabase.Client supabaseClient, IHttpContextAccessor httpContextAccessor, IPenggunaService pengguna)
        {
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCd9Qw1xwXaxB1LIIByISX_c--aclzhaF0"));
            _supabaseClient = supabaseClient;
            _httpContextAccessor = httpContextAccessor;
            _penggunaService = pengguna;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var fullUrl = GetFullUrl(_httpContextAccessor.HttpContext);
            var signInUrlGoogle = _supabaseClient.Auth.SignIn(Provider.Google, new Supabase.Gotrue.SignInOptions { RedirectTo = fullUrl });

            var urlGoogle = signInUrlGoogle?.Result?.Uri?.ToString();

            ClaimsPrincipal claimsUser = HttpContext.User;
            if (claimsUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.nama = "silahkan masuk dulu, agar kita saling mengenal";
            ViewBag.urlGoogle = urlGoogle;
            return View();
        }

        public async Task<IActionResult> callBackSupabase(string access_token, string expires_in, string provider_token, string refresh_token, string token_type)
        {
            var fullUrl = GetFullUrl(_httpContextAccessor.HttpContext);
            Uri siteUri = new Uri(fullUrl+"#"+ "access_token=" + access_token + "&expires_in="+ expires_in + "" + "&provider_token=" + provider_token + "&refresh_token="+ refresh_token + "&token_type=" + token_type);
            var session = await _supabaseClient.Auth.GetSessionFromUrl(siteUri);
            var userMetaData = session.User.UserMetadata;
            string fullname = userMetaData.Where(a => a.Key == "full_name").FirstOrDefault().Value.ToString();
            var picture = userMetaData.Where(a => a.Key == "picture").FirstOrDefault().Value.ToString();

            _penggunaService.cekPengguna(Guid.Parse(session.User.Id), fullname, session.User.Email);
            List<Claim> claims = new List<Claim>() {
                            new Claim(ClaimTypes.Email, session.User.Email),
                            new Claim(ClaimTypes.Uri, siteUri.ToString()),
                            new Claim("Nama", fullname),
                            new Claim("picture", picture),
                            new Claim("Role", "Admin"),
                            new Claim("Id", session.User.Id),
                            new Claim("_supabaseTokenType", session.TokenType),
                            new Claim("_supabaseAccessToken", session.CreatedAt.ToShortTimeString()),
                            new Claim("_supabaseExpiresIn", session.ExpiresIn.ToString()),
                            new Claim("_supabaseRefreshToken", session.RefreshToken),
                        };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);
            
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("Index", "Auth");
        }

        private string GetAccessTokenFromUrl(string url)
        {
            Uri uri = new Uri(url);
            string fragment = uri.Fragment;

            if (!string.IsNullOrEmpty(fragment))
            {
                fragment = fragment.TrimStart('#');

                // Parsing query string
                var queryParameters = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(fragment);

                // Mendapatkan access token
                string accessToken = queryParameters["access_token"];

                return accessToken;
            }

            return null;
        }
        public string GetFullUrl(HttpContext context)
        {
            var session = _supabaseClient.Auth.CurrentSession;
            var request = context.Request;
            var host = request.Scheme + "://" + request.Host;
            var fullUrl = host;
            return fullUrl;
        }

        [HttpPost]
        public async Task<IActionResult> loginSupabase(string fullUri)
        {
            Uri siteUri = new Uri(fullUri);
            var session = await _supabaseClient.Auth.GetSessionFromUrl(siteUri);
            return null;
        }

    }
}
