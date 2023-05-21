using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulsa.Data;
using Pulsa.Service.Interface;
using System.Security.Claims;

namespace Pulsa.Web.Controllers
{
    [Authorize]
    public class TransaksiController : Controller
    {
        private Guid IdLogin { get; set; }
        public TransaksiController(
            IHttpContextAccessor httpContextAccessor)
        {
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
        public IActionResult Index(String produk)
        {
            var title = "";
            if (produk == "pulsa")
            {
                title = "pulsa";
            }
            else if(produk == "data")
            {
                title = "data";
            }
            else if(produk == "tokenlistrik")
            {
                title = "token listrik";

            }
            else if(produk == "emonay")
            {
                title = "emonay";
                // redirect new page
            }
            else if(produk == "game")
            {
                title = "game";
                // redirect new page
            }
            ViewBag.title = title;
            ViewBag.produk = produk;
            return View();
        }
        public IActionResult choseproduk(string produk, string dest)
        {

            return View();
        }
    }
}
