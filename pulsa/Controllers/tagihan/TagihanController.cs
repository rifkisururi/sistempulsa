using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulsa.Data;

namespace pulsa.Controllers.tagihan
{
    [Authorize]
    public class TagihanController : Controller
    {
        private readonly PulsaDataContext context;
        public TagihanController(PulsaDataContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            //var gr = (from tm in context.tagihan_masters
            //          select new {
            //              groupTagihan = tm.group_tagihan
            //          }).GroupBy(a => a.groupTagihan);
            //ViewBag.Group = gr.ToList();
            return View();
        }

        public IActionResult Listrik()
        {
            return View();
        }
    }
}
