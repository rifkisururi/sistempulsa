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

namespace pulsa.Controllers
{
    public class Auth : Controller
    {
        FirebaseAuthProvider auth;

        private readonly Client _supabaseClient;
        public Auth(Supabase.Client supabaseClient)
        {
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCd9Qw1xwXaxB1LIIByISX_c--aclzhaF0"));
            _supabaseClient = supabaseClient;
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

        public async Task<IActionResult> callBack()
        {
            string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJhdXRoZW50aWNhdGVkIiwiZXhwIjoxNjg1OTY1MTE3LCJzdWIiOiI2N2Y4N2VjNC05NGJiLTRmYzAtOTU0Ni1hN2UyNWQ2NTViNWEiLCJlbWFpbCI6InJpZmtpc3VydXJpMjdAZ21haWwuY29tIiwicGhvbmUiOiIiLCJhcHBfbWV0YWRhdGEiOnsicHJvdmlkZXIiOiJnb29nbGUiLCJwcm92aWRlcnMiOlsiZ29vZ2xlIl19LCJ1c2VyX21ldGFkYXRhIjp7ImF2YXRhcl91cmwiOiJodHRwczovL2xoMy5nb29nbGV1c2VyY29udGVudC5jb20vYS9BQWNIVHRjbkhxMlJQTFFMUThyZTJyYUg2VTZuZ3I5b2NfWk5PVzZDVUdrPXM5Ni1jIiwiZW1haWwiOiJyaWZraXN1cnVyaTI3QGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJmdWxsX25hbWUiOiJyaWZraSBhaG1hZCBzdXJ1cmkiLCJpc3MiOiJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS91c2VyaW5mby92Mi9tZSIsIm5hbWUiOiJyaWZraSBhaG1hZCBzdXJ1cmkiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUFjSFR0Y25IcTJSUExRTFE4cmUycmFINlU2bmdyOW9jX1pOT1c2Q1VHaz1zOTYtYyIsInByb3ZpZGVyX2lkIjoiMTExNTI3MzkyOTY4MTE4OTE5NzkzIiwic3ViIjoiMTExNTI3MzkyOTY4MTE4OTE5NzkzIn0sInJvbGUiOiJhdXRoZW50aWNhdGVkIiwiYWFsIjoiYWFsMSIsImFtciI6W3sibWV0aG9kIjoib2F1dGgiLCJ0aW1lc3RhbXAiOjE2ODU5NjE1MTd9XSwic2Vzc2lvbl9pZCI6IjE4MzU2NWQ2LWM0OWUtNDhlYS04OTk5LTVmOTEzOGM1YTBhNiJ9.5O3YLRN6Q7xVgpTtVJf76rKzSmop4qpbDmdm9qCXmsc&expires_in=3600&provider_token=ya29.a0AWY7Ckni9oFIO6VtqoIzRCCfJz_gWdCqhcLdAu8vyyZcHN0pCOgkgMFO8Vyv2nEIAFEIonkCnTpqH9DSM4MNv9NHpWL-sGKXcIRQpFfu2uZSXagaRgRKf4NJHaNngBU3YyT7X1MM71UXAGxjnQl5ycHCSa2ZVQaCgYKAcYSARMSFQG1tDrp71NAs7_B5oIz3iiCvsHvcA0165";
            var userSupabase = await _supabaseClient.Auth.SignInWithIdToken(Provider.Google, accessToken);
            return View();
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

    }
}
