using Pulsa.ViewModel.tagihan;

namespace Pulsa.Service.Interface
{
    public interface ISerpulService
    {
        //Task<int> getSaldo();
        public int getSaldo();
        public void refressProduk();
        //Task<Dictionary<string, object>> GetK2User();
        Task<string> getTagihan(TagihanMasterDTO tm);
        Task<string> PayTagihan(TagihanMasterDTO tm);
    }
}
