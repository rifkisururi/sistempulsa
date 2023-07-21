using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pulsa.Data;
using Pulsa.Domain.Entities;
using Pulsa.Service.Interface;
using System.Security.Claims;

namespace Pulsa.Web.Controllers
{
    [Authorize]
    public class TransaksiController : Controller
    {
        private Guid IdLogin { get; set; }
        private IProdukService _produk;
        private ITransaksiService _transaksi;
        public TransaksiController(
            IHttpContextAccessor httpContextAccessor,
            IProdukService produk,
            ITransaksiService transaksi
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
            _transaksi = transaksi;
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
        public IActionResult order(Guid idTransaksi)
        {
            var data = _transaksi.getDetailTransaksi(idTransaksi);
            var dataProduk = _produk.getProdukSuppliyer(data.product_id, data.suppliyer);
            var rekomendasiHarga = _produk.getAllProduk().Where(a => a.product_id == data.product_id).SingleOrDefault()?.price_suggest;
           
            ViewBag.data = data;
            ViewBag.dataProduk = dataProduk;
            ViewBag.hargaJual = rekomendasiHarga;
            return View();
        }

        public IActionResult generate_order(string produkId, string suppliyer, string dest)
        {
            Guid idTransaksi = _transaksi.transaksi(produkId, suppliyer, dest, IdLogin);
            return RedirectToAction("order", new { idTransaksi = idTransaksi });
        }

        public IActionResult submitorder(Guid idTransaksi, string harga_jual, string pin, string nama_pembeli)
        {
            // cek pin
            bool cekPin = _transaksi.verifikasiPin(IdLogin, pin);
            if (cekPin == false) {
                return new JsonResult(new
                {
                    status = false,
                    message = "pin salah !"
                });
            }
            // save nama pembeli

            // action ke serpul
            _transaksi.fixorder(idTransaksi);



            return null;
        }
        
    }
}
