using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pulsa.ViewModel;
using Pulsa.Data;
using Pulsa.Domain.Entities;
using Pulsa.Helper;
using Pulsa.Service.Interface;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;

namespace pulsa.Controllers.tagihan
{
    [Authorize]
    public class TagihanAjaxController : Controller
    {
        private readonly PulsaDataContext context;
        private ITagihanService _tagihan;
        private ISerpulService _serpul;
        public TagihanAjaxController(PulsaDataContext context, ITagihanService tagihan, ISerpulService serpul)
        {
            this.context = context;
            _tagihan = tagihan;
            _serpul = serpul;
        }
        public IActionResult index()
        {
            var _group = Request.Query["group"].ToString();
            var dt = context.tagihan_masters
                .Where(a => a.group_tagihan == _group.ToLower() || _group == "");

            return new JsonResult(new
            {
                draw = 1,
                recordsTotal = dt.Count(),
                recordsFiltered = dt.Count(),
                data = dt
            });
        }
        public IActionResult listrik()
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

            //if (_periode == "")
            //{
            //    DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
            //}
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
                          id = tm.id,
                          type_tagihan = tm.type_tagihan,
                          group_tagihan = tm.group_tagihan,
                          nama_pelanggan = tm.nama_pelanggan,
                          id_tagihan = tm.id_tagihan,
                          jumlah_tagihan = td.jumlah_tagihan,
                          admin_tagihan = td.admin_tagihan,
                          admin_notta = td.admin_notta,
                          status_bayar = td.status_bayar
                      });

            return new JsonResult(new
            {
                draw = 1,
                recordsTotal = dt.Count(),
                recordsFiltered = dt.Count(),
                data = dt
            });
        }

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
            //int saldo = _serpul.getSaldo();
            //var tagihan = await _serpul.getTagihan("PLN", "521510718785");
            return null;
        }
    }
}
