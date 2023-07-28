using Pulsa.Domain.Entities;
using Pulsa.ViewModel;
using Pulsa.ViewModel.tagihan;

namespace Pulsa.Service.Interface
{
    public interface ISerpulService
    {
        //Task<int> getSaldo();
        public int getSaldo();
        Task<List<Supplier_produk>> refressProduk();
        public bool saveProduk(List<Supplier_produk> tm);
        Task<string> getTagihan(TagihanMasterDTO tm);
        Task<string> PayTagihan(TagihanMasterDTO tm);
        public List<Tagihan_detail> cekTransaksiPascaPending();
        Task<string> cekTransaksiPending(string refId);
        Task<string> orderPrabayar(string produkId, string dest, string refId);
        Task<string> cekPln(string noPln);
    }
}
