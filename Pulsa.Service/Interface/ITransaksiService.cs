using Pulsa.Domain.Entities;
using Pulsa.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Service.Interface
{
    public interface ITransaksiService
    {
        public Guid transaksi(string product_id, string suppliyer, string dest, Guid pengguna);
        public Pengguna_Traksaksi getDetailTransaksi(Guid id);
        public bool verifikasiPin(Guid id, string pin);
        Task<string> fixorder(Guid id);
        public List<MutasiDTO> listMutasi(Guid idPengguna, int start, int jumlah);
        Task<DetailTransaksiDTO> detailTransaksi(Guid id);
        Task<string> cekTransaksiPending(Pengguna_Traksaksi pt);
        Task<Pengguna_Traksaksi> detailTransaksiById(Guid id);
        public List<Guid> getTransaksiPending(Guid idPengguna);
    }
}
