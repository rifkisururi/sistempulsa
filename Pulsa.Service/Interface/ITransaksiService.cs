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
        public Guid transaksi(string product_id, string suppliyer, string dest, Guid penggunan);
        public Pengguna_Traksaksi getDetailTransaksi(Guid id);
        public bool verifikasiPin(Guid id, string pin);
        public void fixorder(Guid id);

    }
}
