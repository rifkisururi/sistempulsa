using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class TopupRepository : GenericRepository<Domain.Entities.TopUp>, ITopupRepository
    {
        public TopupRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
