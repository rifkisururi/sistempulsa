using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using pulsa.Models;
using Pulsa.Data;
using System.Diagnostics;
using Pulsa.Service.Interface;

namespace pulsa.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly PulsaDataContext context;
        private readonly IWebHostEnvironment _env;

        private ISerpulService _serpul;
        public HomeController(PulsaDataContext context, IWebHostEnvironment env, ISerpulService serpul)
        {
            this.context = context;
            _env = env;
            _serpul = serpul;
        }

        public IActionResult Index()
        {
            var saldo = _serpul.getSaldo();
            ViewBag.saldo = saldo;
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