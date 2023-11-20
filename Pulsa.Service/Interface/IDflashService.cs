using Pulsa.Domain.Entities;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;

namespace Pulsa.Service.Interface
{
    public interface IDflashService
    {
        Task<int> getSaldo();
        Task<string> getTagihan(TagihanMasterDTO tm);
        Task<string> PayTagihan(TagihanMasterDTO tm);
        Task<List<Supplier_produk>> refressProduk(string dateUpdate);
        public void saveProduk(string dateUpdate, List<Supplier_produk> tm);
        Task<string> order(string produkId, string dest, string refId);
        Task<string> cekTransaksiPending(Pengguna_Traksaksi pengguna_Traksaksi);
    }
}
