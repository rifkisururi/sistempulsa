using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class PenggunaMutasiRepository : GenericRepository<Domain.Entities.Pengguna_mutasi >, IPenggunaMutasiRepository
    {
        public PenggunaMutasiRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
