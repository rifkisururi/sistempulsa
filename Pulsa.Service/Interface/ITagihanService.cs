using Pulsa.Domain.Entities;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;

namespace Pulsa.Service.Interface
{
    public interface ITagihanService
    {
        public bool actionTagihanMaster(InputTagihan tm);
        public List<TagihanMasterDTO> getListAll();
        public List<TagihanMasterDTO> GetListBayarAll();
        public Domain.Entities.Tagihan_master detailMaster(Guid id);
        public List<VMTagihanListrik> getTagihanBulanIni(String _group);

    }
}
