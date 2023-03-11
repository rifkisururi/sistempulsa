using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class Provider_h2hRepository : GenericRepository<Domain.Entities.Provider_h2h>, IProvider_h2hRepository
    {
        public Provider_h2hRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
