using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulsa.Data;
using Pulsa.Domain.Entities;
using Pulsa.Service.Interface;
using Pulsa.Service.Service;
using Pulsa.ViewModel;
using System.Security.Claims;

namespace Pulsa.Web.Controllers
{
    [Authorize]
    public class TopUpController : Controller
    {
        private readonly PulsaDataContext context;
        private ITopUpService _topUpService;
        private Guid userLogin;
        public TopUpController(PulsaDataContext context, ITopUpService topUpService)
        {
            this.context = context;
            this._topUpService = topUpService;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult History()
        {
            return View();
        }

        [HttpPost]
        public IActionResult action([FromBody] InputTopUpDTO data)
        {
            var userClaim = this.User.Claims.ToList();
            Guid id = Guid.NewGuid();
            for (var a = 0; a < userClaim.Count(); a++)
            {
                if (userClaim[a].Type == "Id")
                {
                    id = Guid.Parse(userClaim[a].Value);
                }
            }
            bool result = false;
            // 1 = add || 2 = approve || 3 reject
            if (data.status == 1)
            {
                data.idpengguna= id;
                result = _topUpService.add(data, id);
            }
            else
            {
                Guid idAction = Guid.Parse(data.id.ToString());
                result = _topUpService.action("approve", id, idAction);
                result = true;
            }

            return new JsonResult(new
            {
                status = result,
                data = data
            });
        }

        public JsonResult getRequestTopup()
        {

            var data = _topUpService.listRequestTopup();
            return new JsonResult(new
            {
                status = true,
                data = data.ToList()
            });
        }

        public JsonResult getHistoryTopup()
        {
            // todo add filter date backend

            var data = _topUpService.listRequestTopupHistory();
            return new JsonResult(new
            {
                status = true,
                data = data.ToList()
            });
        }
        

    }
}
