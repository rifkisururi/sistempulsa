using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using pulsa.Models;
using Pulsa.Data;
using System.Diagnostics;
using Pulsa.Service.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System;
using System.Globalization;

namespace pulsa.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly PulsaDataContext context;
        private readonly IWebHostEnvironment _env;
        private  ITopUpService _topUpService;
        private ISerpulService _serpul;
        private Guid IdLogin { get; set; }
        private string fullname { get; set; }
        private string picture { get; set; }
        public HomeController(
            PulsaDataContext context, 
            IWebHostEnvironment env, 
            ISerpulService serpul, 
            ITopUpService topUpService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            _env = env;
            _serpul = serpul;
            _topUpService = topUpService;

            var claimsIdentity = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;

            if (claimsIdentity != null)
            {
                var idClaim = claimsIdentity.FindFirst("Id");

                if (idClaim != null)
                {
                    IdLogin = Guid.Parse(idClaim.Value);
                }
                fullname = claimsIdentity.FindFirst("Nama").Value;
                picture = claimsIdentity.FindFirst("picture").Value;

                
            }
        }
        public async Task<IActionResult> Index()
        {
            //var saldo = await _serpul.getSaldo();
            var saldoPengguna = _topUpService.saldo(IdLogin);
            //ViewBag.saldo = saldo;
            ViewBag.nama = fullname+"<br>"+"saldo "+  saldoPengguna.ToString("N0");
            ViewBag.picture = picture;
            return View();
        }

        public async Task<IActionResult> IndexAdmin()
        {
            var saldo = await _serpul.getSaldo();
            var saldoPengguna = _topUpService.saldo(IdLogin);
            ViewBag.saldo = saldo.ToString("N0");
            ViewBag.nama = fullname + "<br>" + "saldo " + saldoPengguna.ToString("N0");
            ViewBag.picture = picture;
            return View();
        }
        public IActionResult env()
        {
            var environment = _env.EnvironmentName;
            return Content("environment " + environment);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult refressProduk() { 
            var dtProduk = _serpul.refressProduk().Result;
            _serpul.saveProduk(dtProduk);
            return Content("refreess produk");
        }
    }
}