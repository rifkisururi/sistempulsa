using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pulsa.ViewModel;
using Pulsa.Data;

namespace pulsa.Controllers.tagihan
{
    public class TagihanController : Controller
    {

        private readonly PulsaDataContext context;
        public TagihanController(PulsaDataContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Listrik()
        {
            return View();
        }
    }
}
