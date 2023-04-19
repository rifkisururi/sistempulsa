using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulsa.Data;
using Pulsa.Service.Interface;
using Pulsa.Service.Service;
using System.Security.Claims;

namespace Pulsa.Web.Controllers
{
    [Authorize]
    public class ProdukController : Controller
    {
        private Guid IdLogin { get; set; }
        private readonly PulsaDataContext context;
        private IProdukService _produk;
        private Guid userLogin;

        public ProdukController(
            IHttpContextAccessor httpContextAccessor,
            PulsaDataContext context, IProdukService produk
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
            };
            _produk = produk;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult listProduk()
        {

            var data = _produk.getAllProduk();
            return new JsonResult(new
            {
                status = true,
                data = data.ToList()
            });
        }

        public IActionResult Pulsa()
        {
            return View();
        }
    }
}
