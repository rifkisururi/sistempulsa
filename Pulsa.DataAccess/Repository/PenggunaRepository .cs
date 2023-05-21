using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class PenggunaRepository : GenericRepository<Domain.Entities.Pengguna>, IPenggunaRepository
    {
        public PenggunaRepository(PulsaDataContext context) : base(context)
        {
        }

        public Int32 getSaldo(Guid Id)
        {
            return this.Find(p => p.id == Id).Select(a => a.saldo).FirstOrDefault();
        }
    }
}
