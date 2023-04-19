using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class ProdukRepository : GenericRepository<Domain.Entities.Produk>, IProdukRepository
    {
        public ProdukRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
