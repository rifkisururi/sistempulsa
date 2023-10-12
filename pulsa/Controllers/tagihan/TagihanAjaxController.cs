using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Pulsa.Data;
using Pulsa.Domain.Entities;
using Pulsa.Helper;
using Pulsa.Service.Interface;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;

namespace pulsa.Controllers.tagihan
{
    public class TagihanAjaxController : Controller
    {
        private readonly PulsaDataContext context;
        private ITagihanService _tagihan;
        private ISerpulService _serpul; 
        private IMapper _mapper;
        public TagihanAjaxController(
            PulsaDataContext context, 
            ITagihanService tagihan, 
            ISerpulService serpul,
            IMapper mapper)
        {
            this.context = context;
            _tagihan = tagihan;
            _serpul = serpul;
            _mapper = mapper;
        }

        [Authorize]
        public IActionResult index()
        {
            var _group = Request.Query["group"].ToString();
            var dt = context.tagihan_masters
                .Where(a => a.group_tagihan == _group.ToLower() || _group == "").AsNoTracking();

            return new JsonResult(new
            {
                draw = 1,
                recordsTotal = dt.Count(),
                recordsFiltered = dt.Count(),
                data = dt
            });
        }

        [Authorize]
        public async Task<IActionResult> listrik()
        {
            var _periode = Request.Query["periode"].ToString();
            var _typeTagihan = Request.Query["type"].ToString();
            var _group = Request.Query["group"].ToString();
            var status = Request.Query["status"].ToString();
            bool _status = false;
            if (status == "1")
            {
                _status = true;
            }

            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");

            var dt = (from tm in context.tagihan_masters
                      join td in context.tagihan_details on tm.id equals td.id_tagihan_master
                      where
                        td.tanggal_cek >= awalBulan
                        //(_periode == "" || _periode.ToLower() == td.periode_tagihan)
                        && (_typeTagihan == "" || _typeTagihan.ToLower() == tm.type_tagihan)
                        && (_group == "" || _group.ToLower() == tm.group_tagihan)
                        && (status == "" || _status == td.status_bayar)

                      select new VMTagihanListrik
                      {
                          id = td.id,
                          idMaster = td.id_tagihan_master,
                          type_tagihan = tm.type_tagihan,
                          group_tagihan = tm.group_tagihan,
                          nama_pelanggan = tm.nama_pelanggan,
                          id_tagihan = tm.id_tagihan,
                          jumlah_tagihan = td.jumlah_tagihan,
                          admin_tagihan = td.admin_tagihan,
                          admin_notta = td.admin_notta,
                          status_bayar = td.status_bayar,
                          harus_dibayar = td.harus_dibayar,
                          request_bayar = td.request_bayar
                      });

            return new JsonResult(new
            {
                draw = 1,
                recordsTotal = dt.Count(),
                recordsFiltered = dt.Count(),
                data = dt
            }); 
        }

        [Authorize]
        [HttpPost]
        public IActionResult action([FromBody] InputTagihan inputTagihan)
        {
            var result = _tagihan.actionTagihanMaster(inputTagihan);
            return new JsonResult(new
            {
                status = true,
                data = inputTagihan
            });
        }

        public async Task<IActionResult> getTagihan()
        {
            var tagihanMaster = _tagihan.getListAll();
            
            foreach (var td in tagihanMaster) {
                 await _serpul.getTagihan(td);
            }
            return Ok("cek data sukses");
        }

        [Authorize]
        public async Task<IActionResult> bayarTagihan()
        {
            var tagihanMaster = _tagihan.GetListBayarAll(Guid.Empty);

            foreach (var td in tagihanMaster)
            {
                 await _serpul.PayTagihan(td);
            }
            return null;
        }

        [Authorize]
        public IActionResult getDetailMaster(Guid idMaster)
        {
            var tagihanMaster = _tagihan.detailMaster(idMaster);
            
            return new JsonResult(new 
            {
                status = true,
                data = tagihanMaster
            });
        }
        [Authorize]
        public async Task<IActionResult> bayarTagihanIni(Guid idMaster) {
            var tagihan = _tagihan.GetListBayarAll(idMaster);
            var returnBayar = await _serpul.PayTagihan(tagihan[0]);
            return Ok(returnBayar);
        }

        public async Task<IActionResult> cekTransaksiPascaPending(Guid idMaster)
        {
            var tagihan = _serpul.cekTransaksiPascaPending();
            foreach (var td in tagihan)
            {
                await _serpul.cekTransaksiPendingPasca(td.ref_id);
            }
            return Ok("tidak ada pending transaksi");
        }

        public async Task<IActionResult> cekTransaksiPrabayarPending()
        {
            var tagihan = _serpul.cekTransaksiPrabayarPending();
            foreach (var td in tagihan)
            {
                await _serpul.cekTransaksiPendingPrabayar(td.id.ToString());
            }
            return Ok("tidak ada pending transaksi");
        }
        public async Task<IActionResult> autoPayTagihan()
        {
            var dateNow = DateTime.Now;
            var tagihan = _tagihan.GetListBayarAutoPay();
            foreach (var td in tagihan)
            {
                await _serpul.PayTagihan(td);
            }
            
            return Ok("tidak ada pending autopay " + dateNow.ToLongDateString() + " " + dateNow.ToLongTimeString()); ;
        }



    }
}
