using Pulsa.Domain.Entities;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;

namespace Pulsa.Service.Interface
{
    public interface IDflashService
    {
        Task<int> getSaldo();
        Task<string> getTagihan(TagihanMasterDTO tm);
    }
}
