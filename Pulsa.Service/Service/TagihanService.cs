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

            if (data.id == Guid.Empty)
            {
                _tagihanMaster.Add(tm);
                _tagihanMaster.Save();
                var result = _unitOfWork.Complete();
                return result;
            }
            else
            {
                return false;

            }
        }

        public List<TagihanMasterDTO> getListAll() {

            DateTime awalBulan = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1");
            var gr = (from tm in _context.tagihan_masters
                      join detail in _context.tagihan_details on tm.id equals detail.id_tagihan_master into td
                      from detail in td.DefaultIfEmpty()
                      where 
                        tm.is_active == true
                        //&& tm.id_tagihan == "521511308486" || 
                        //&& tm.id_tagihan == "521510460244"
                      // detail.tanggal_cek >= awalBulan
                      //  m.jumlah_tagihan == null && detail.tanggal_cek >= awalBulan
                      select new TagihanMasterDTO
                      {
                          id = tm.id,
                          type_tagihan = tm.type_tagihan,
                          id_tagihan = tm.id_tagihan,
                          nama_pelanggan = tm.nama_pelanggan
                      }).ToList();
            return gr;
        }
        //public List<Tagihan_detail> getAllTagihanActive() {
        //    var data = _tagihanDetail.Find( a => a.harus_dibayar == true && a.status_bayar == false).ToList();
        //    return data;
        //}

    }
}
