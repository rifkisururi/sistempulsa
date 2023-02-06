using Pulsa.ViewModel.tagihan;

namespace Pulsa.Service.Interface
{
    public interface ISerpulService
    {
        //Task<int> getSaldo();
        public int getSaldo();
        //Task<Dictionary<string, object>> GetK2User();
        Task<string> getTagihan(TagihanMasterDTO tm);
    }
}
