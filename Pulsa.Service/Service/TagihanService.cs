using AutoMapper;
using Pulsa.DataAccess.Interface;
using Pulsa.Domain.Entities;
using Pulsa.Service.Interface;
using Pulsa.ViewModel;

namespace Pulsa.Service.Service
{
    public class TagihanService : ITagihanService
    {

        Pulsa.DataAccess.Interface.IUnitOfWork _unitOfWork;
        Pulsa.DataAccess.Interface.ITagihanMasterRepository _tagihanMaster;
        //Pulsa.DataAccess.Interface.ITagihanDetailRepository _tagihanDetail;
        private IMapper _mapper;

        public TagihanService(
            IUnitOfWork unitOfWork,
            ITagihanMasterRepository tagihanMasterRepository,
            IMapper mapper
            //ITagihanDetailRepository tagihanDetailRepository
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tagihanMaster = tagihanMasterRepository;
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
        //public List<Tagihan_detail> getAllTagihanActive() {
        //    var data = _tagihanDetail.Find( a => a.harus_dibayar == true && a.status_bayar == false).ToList();
        //    return data;
        //}

    }
}
