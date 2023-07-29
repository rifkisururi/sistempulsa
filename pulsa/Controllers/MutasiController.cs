using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Pulsa.Web.Controllers
{
    [Authorize]
    public class MutasiController : Controller
    {
        private Guid IdLogin { get; set; }
        public MutasiController(IHttpContextAccessor httpContextAccessor)
        {
            var claimsIdentity = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            var idClaim = claimsIdentity.FindFirst("Id");

            if (idClaim != null)
            {
                IdLogin = Guid.Parse(idClaim.Value);
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
