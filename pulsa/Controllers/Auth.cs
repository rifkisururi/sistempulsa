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

namespace pulsa.Controllers
{
    public class Auth : Controller
    {
        FirebaseAuthProvider auth;

        public Auth()
        {
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCd9Qw1xwXaxB1LIIByISX_c--aclzhaF0"));
        }


        [HttpGet]
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            var id = HttpContext.Session.GetString("_Id");
            
            ClaimsPrincipal claimsUser = HttpContext.User;
            if (claimsUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
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

    }
}
