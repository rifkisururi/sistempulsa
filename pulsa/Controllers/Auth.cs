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

namespace pulsa.Controllers
{
    public class Auth : Controller
    {
        FirebaseAuthProvider auth;

        private readonly Client _supabaseClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Auth(Supabase.Client supabaseClient, IHttpContextAccessor httpContextAccessor)
        {
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCd9Qw1xwXaxB1LIIByISX_c--aclzhaF0"));
            _supabaseClient = supabaseClient;
            _httpContextAccessor = httpContextAccessor; 
        }


        [HttpGet]
        public IActionResult Index()
        {
            var signInUrlGoogle = _supabaseClient.Auth.SignIn(Provider.Google);
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
            string fullname = userMetaData.Where(a => a.Key == "full_name").SingleOrDefault().Value.ToString();
            var picture = userMetaData.Where(a => a.Key == "picture").SingleOrDefault().Value.ToString();

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

        [HttpPost]
        public async Task<IActionResult> Registration(FMLogin loginModel)
        {
            try
            {
                //create the user
                await auth.CreateUserWithEmailAndPasswordAsync(loginModel.username, loginModel.password);
                //log in the new user
                var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(loginModel.username, loginModel.password);
                string token = fbAuthLink.FirebaseToken;

                //saving the token in a session variable
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    return RedirectToAction("Index");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(loginModel);
            }

            return View();

        }


        [HttpPost]
        public async Task<IActionResult> Index(FMLogin _modelLogin)
        {
            try
            {
                //log in an existing user
                var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(_modelLogin.username, _modelLogin.password);
                string token = fbAuthLink.FirebaseToken;
                //save the token to a session variable
                if (token != null)
                {
                    Guid id = Guid.Parse("52bdd191-e448-4a83-a6bf-0029e7debb44");

                    HttpContext.Session.SetString("_UserToken", token);
                    HttpContext.Session.SetString("_Id", Convert.ToString(id));

                    List<Claim> claims = new List<Claim>() {
                            new Claim(ClaimTypes.NameIdentifier, _modelLogin.username),
                            new Claim(ClaimTypes.NameIdentifier, _modelLogin.username),
                            new Claim("Nama", "Rifki"),
                            new Claim("Role", "Admin"),
                            new Claim("Id", Convert.ToString(id)),
                        };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = _modelLogin.keepLogin
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View();
            }
            return View();
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
