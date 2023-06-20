using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class ProdukDetailRepository : GenericRepository<Domain.Entities.Produks_detail>, IProdukDetailRepository
    {
        public ProdukDetailRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
