using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class Supplier_produkRepository : GenericRepository<Domain.Entities.Supplier_produk>, ISupplier_produkRepository
    {
        public Supplier_produkRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
