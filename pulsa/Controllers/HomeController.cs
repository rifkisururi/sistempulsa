﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using pulsa.Models;
using Pulsa.Data;
using System.Diagnostics;
using Pulsa.Service.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace pulsa.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly PulsaDataContext context;
        private readonly IWebHostEnvironment _env;
        private  ITopUpService _topUpService;
        private ISerpulService _serpul;
        public Guid IdLogin { get; private set; }
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
            }
        }

        public IActionResult Index()
        {
            
            var saldo = _serpul.getSaldo();
            var saldoPengguna = _topUpService.saldo(IdLogin);
            ViewBag.saldo = saldo;
            ViewBag.saldoPengguna = saldoPengguna;
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
    }
}