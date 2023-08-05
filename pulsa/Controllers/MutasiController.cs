using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulsa.Service.Interface;
using System.Security.Claims;

namespace Pulsa.Web.Controllers
{
    [Authorize]
    public class MutasiController : Controller
    {
        private Guid IdLogin { get; set; }
        private ITransaksiService _transaksi;
        public MutasiController(IHttpContextAccessor httpContextAccessor, ITransaksiService transaksi)
        {
            var claimsIdentity = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            var idClaim = claimsIdentity.FindFirst("Id");
            _transaksi = transaksi;
            if (idClaim != null)
            {
                IdLogin = Guid.Parse(idClaim.Value);
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ajaxIndexMutasi(int from = 0, int take = 10)
        {
            var data = _transaksi.listMutasi(IdLogin, from, take);
            return new JsonResult(new
            {
                status = true,
                data = data
            });
        }

        public async Task<IActionResult> detail(Guid id)
        {
            
            var dataMutasi = await _transaksi.detailTransaksi(id);
            ViewBag.data = dataMutasi;
            return View();
        }
    }
}
