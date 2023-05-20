using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulsa.Data;
using Pulsa.Service.Interface;

namespace pulsa.Controllers.tagihan
{
    
    public class TagihanController : Controller
    {
        private readonly PulsaDataContext context;
        private ITagihanService _tagihan;
        public TagihanController(PulsaDataContext context, ITagihanService tagihan)
        {
            this.context = context;
            _tagihan = tagihan;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.groupTagihan = _tagihan.getGroupTagihan().ToList();
            return View();
        }

        [Authorize]
        public IActionResult BulanIni()
        {
            ViewBag.groupTagihan = _tagihan.getGroupTagihan().ToList();
            return View();
        }

        public IActionResult Print(String Group)
        {
            var data = _tagihan.getTagihanBulanIni(Group);

            int? jumlahPembayaran = 0;
            int? biayaJasa = 0;
            int? setor = 0;

            foreach (var dt in data) {
                jumlahPembayaran += dt.jumlah_tagihan + dt.admin_notta;
                biayaJasa += dt.admin_tagihan;
                setor += dt.admin_tagihan + dt.jumlah_tagihan;
            }

            ViewBag.data = data;
            ViewBag.jumlahPembayaran = jumlahPembayaran;
            ViewBag.biayaJasa = biayaJasa;
            ViewBag.setor = setor;

            return View();
        }
    }
}
