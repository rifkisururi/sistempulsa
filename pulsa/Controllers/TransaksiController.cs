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
        private ISerpulService _serpul;
        public TransaksiController(
            IHttpContextAccessor httpContextAccessor,
            IProdukService produk,
            ITransaksiService transaksi,
            ISerpulService serpul
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
            _serpul = serpul;
        }
        public IActionResult Index(String produk)
        {
            var title = "";
            var label = "";
            if (produk == "pulsa")
            {
                title = "pulsa";
                label = "No HP";
            }
            else if(produk == "data")
            {
                title = "data";
                label = "No HP";
            }
            else if(produk == "token")
            {
                title = "token listrik";
                label = "No PLN";

            }
            else if(produk == "ud")
            {
                label = "Tujuan";
                title = "Uang digital";
                var listTypeProduk = _produk.listTypeProduk(produk);
                ViewBag.TypeProduk = listTypeProduk.ToList();
            }
            else if(produk == "game")
            {
                label = "game";
                // redirect new page
            }
            ViewBag.label = label;
            ViewBag.title = title;
            ViewBag.produk = produk;
            return View();
        }
        public IActionResult cariproduk(string produk, string dest, string typeProduk = "")
        {
            var brand = _produk.cekOperator(dest);
            var dtProduk = _produk.getProdukByType(produk, brand, typeProduk);

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
            if (!cekPin)
            {
                return new JsonResult(new
                {
                    status = false,
                    message = "pin salah !"
                });
            }
            // save nama pembeli

            // action ke serpul
            var result = _transaksi.fixorder(idTransaksi);
            if (result == "0")
            {
                return new JsonResult(new
                {
                    status = false,
                    message = "Saldo tidak cukup"
                });
            }
            else {
                return new JsonResult(new
                {
                    status = true,
                    message = "Transaksi sedang di proses"
                });
            }
            
            
        }

        public async Task<IActionResult> cekPln(string no)
        {
            var result = await _serpul.cekPln(no);

            return new JsonResult(new
            {
                status = true,
                message = result
            });
        }

         

    }
}
