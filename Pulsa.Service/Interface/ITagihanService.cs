using Pulsa.Domain.Entities;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;

namespace Pulsa.Service.Interface
{
    public interface ITagihanService
    {
        public bool actionTagihanMaster(InputTagihan tm);
        public List<TagihanMasterDTO> getListAll();
    }
}
