using AutoMapper;
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
                      join detail in _context.tagihan_details on tm.id equals detail.id_tagihan_master into td
                      from detail in td.DefaultIfEmpty()
                      where 
                        tm.is_active == true
                        && detail.tanggal_cek >= awalBulan
                      select new TagihanMasterDTO
                      {
                          id = tm.id,
                          type_tagihan = tm.type_tagihan,
                          id_tagihan = tm.id_tagihan,
                          nama_pelanggan = tm.nama_pelanggan
                      }).ToList();
            return gr;
        }

        public List<TagihanMasterDTO> GetListBayarAll()
        {

            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
            var gr = (from tm in _context.tagihan_masters
                      join detail in _context.tagihan_details on tm.id equals detail.id_tagihan_master 
                      where
                        tm.is_active == true
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

    }
}
