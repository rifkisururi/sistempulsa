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
        private IProdukService _produk;
        public TransaksiController(
            IHttpContextAccessor httpContextAccessor,
            IProdukService produk
        )
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
            _produk = produk;
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
        public IActionResult cariproduk(string produk, string dest)
        {
            var brand = _produk.cekOperator(dest);
            var dtProduk = _produk.getProdukByType(produk, brand);
            ViewBag.produk = produk;
            ViewBag.dest = dest;
            ViewBag.listProduk = dtProduk;
            return View();
        }
        public IActionResult order(string produkId, string suppliyer, string dest)
        {
            // insert data transaksi
            var brand = _produk.cekOperator(dest);
            ViewBag.dest = dest;
     
            return View();
        }
        
    }
}
