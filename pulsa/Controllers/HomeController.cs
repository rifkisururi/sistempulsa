using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pulsa.Models;
using pulsa.ViewModel;
using Pulsa.Data;
using System.Diagnostics;

namespace pulsa.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        private readonly PulsaDataContext context;

        public HomeController(PulsaDataContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            //var data = this.context.penggunas
            //    .Select( m => new penggunaModel { 
            //        nama = m.nama,
            //        email= m.email,
            //    });
            return View();
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