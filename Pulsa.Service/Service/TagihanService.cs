using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.Domain.Entities;
using Pulsa.Service.Interface;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;
using System.Security.Cryptography;

namespace Pulsa.Service.Service
{
    public class TagihanService : ITagihanService
    {

        Pulsa.DataAccess.Interface.IUnitOfWork _unitOfWork;
        Pulsa.DataAccess.Interface.ITagihanMasterRepository _tagihanMaster;
        //Pulsa.DataAccess.Interface.ITagihanDetailRepository _tagihanDetail;
        private readonly PulsaDataContext _context;
        private IMapper _mapper;

        public TagihanService(
            IUnitOfWork unitOfWork,
            ITagihanMasterRepository tagihanMasterRepository,
            IMapper mapper,
            PulsaDataContext context
            //ITagihanDetailRepository tagihanDetailRepository
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tagihanMaster = tagihanMasterRepository;
            _context = context;
            //_tagihanDetail= tagihanDetailRepository;
        }

        
        public List<GroupTagihanDTO> getGroupTagihan()
        {
            var results = (from tm in _context.tagihan_masters.GroupBy(n => new { n.group_tagihan})
                          select new GroupTagihanDTO {
                              group_tagihan = tm.Key.group_tagihan
                          }).ToList();
            return results;
        }
        public bool actionTagihanMaster(InputTagihan data)
        {
            Tagihan_master tm = _mapper.Map<Tagihan_master>(data);
            var result = false;
            if (data.id == Guid.Empty)
            {
                _tagihanMaster.Add(tm);
            }
            else
            {
                _tagihanMaster.Update(tm);
                _tagihanMaster.Save();
            }

            result = _unitOfWork.Complete();
            return result;

        }

        public List<TagihanMasterDTO> getListAll() {

            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
            var gr = (from tm in _context.tagihan_masters
                      join detail in _context.tagihan_details.Where(a => a.tanggal_cek >= awalBulan) 
                        on tm.id equals detail.id_tagihan_master 
                        into td
                      from detail in td.DefaultIfEmpty()
                      where 
                        tm.is_active == true
                        && detail.tanggal_cek == null 
                      select new TagihanMasterDTO
                      {
                          id = tm.id,
                          type_tagihan = tm.type_tagihan,
                          id_tagihan = tm.id_tagihan,
                          nama_pelanggan = tm.nama_pelanggan
                      }).ToList();
            return gr;
        }

        public List<TagihanMasterDTO> GetListBayarAll(Guid id)
        {

            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
            var gr = (from tm in _context.tagihan_masters
                      join detail in _context.tagihan_details on tm.id equals detail.id_tagihan_master 
                      where
                        tm.is_active == true
                        && (tm.id == id || id == Guid.Empty) 
                        && detail.harus_dibayar != false
                        && detail.tanggal_cek >= awalBulan
                        && detail.request_bayar != true
                        
                      select new TagihanMasterDTO
                      {
                          id = tm.id,
                          type_tagihan = tm.type_tagihan,
                          id_tagihan = tm.id_tagihan,
                          nama_pelanggan = tm.nama_pelanggan,
                          jumlah_tagihan = detail.jumlah_tagihan
                      }).OrderBy( a => a.jumlah_tagihan).ToList();
            return gr;
        }

        public Domain.Entities.Tagihan_master detailMaster(Guid id) { 
            return _tagihanMaster.GetById(id);
        }

        public List<VMTagihanListrik> getTagihanBulanIni(String _group) {
            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");

            var dt = (from tm in _context.tagihan_masters
                      join td in _context.tagihan_details on tm.id equals td.id_tagihan_master
                      where
                        td.tanggal_cek >= awalBulan
                        //(_periode == "" || _periode.ToLower() == td.periode_tagihan)
                        && (_group == "" || _group.ToLower() == tm.group_tagihan)

                      select new VMTagihanListrik
                      {
                          id = td.id,
                          type_tagihan = tm.type_tagihan,
                          group_tagihan = tm.group_tagihan,
                          nama_pelanggan = tm.nama_pelanggan,
                          id_tagihan = tm.id_tagihan,
                          jumlah_tagihan = td.jumlah_tagihan,
                          admin_tagihan = tm.admin_notta,
                          admin_notta = tm.admin,
                          total = td.jumlah_tagihan+ tm.admin,
                          status_bayar = td.status_bayar,
                          harus_dibayar = td.harus_dibayar
                      });
            return dt.ToList();
        }

        public List<TagihanMasterDTO> GetListBayarAutoPay()
        {

            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");

            int dayToday = DateTime.Now.Day;
            int hourToday = DateTime.Now.Hour;
            
            var gr = (from tm in _context.tagihan_masters
                      join detail in _context.tagihan_details on tm.id equals detail.id_tagihan_master
                      where
                        tm.is_active == true
                        && detail.harus_dibayar != false
                        && detail.tanggal_cek >= awalBulan
                        && detail.request_bayar != true
                        && tm.autopay == 1
                        && (tm.autopay_day == 0 || tm.autopay_day <= dayToday)
                        && (tm.autopay_hour == 0 || (tm.autopay_hour >= hourToday && tm.autopay_day <= dayToday))
                      select new TagihanMasterDTO
                      {
                          id = tm.id,
                          type_tagihan = tm.type_tagihan,
                          id_tagihan = tm.id_tagihan,
                          nama_pelanggan = tm.nama_pelanggan,
                          jumlah_tagihan = detail.jumlah_tagihan
                      }).OrderBy(a => a.jumlah_tagihan).ToList();
            return gr;
        }

    }
}
