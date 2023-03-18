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
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Pulsa()
        {
            return View();
        }
    }
}
