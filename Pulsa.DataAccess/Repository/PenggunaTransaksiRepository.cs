using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class PenggunaTransaksiRepository : GenericRepository<Domain.Entities.Pengguna_Traksaksi >, IPenggunaTransaksiRepository
    {
        public PenggunaTransaksiRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
