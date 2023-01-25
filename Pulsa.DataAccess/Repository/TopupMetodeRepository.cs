using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class TopupMetodeRepository : GenericRepository<Domain.Entities.TopUp_metode>, ITopupMetodeRepository
    {
        public TopupMetodeRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
