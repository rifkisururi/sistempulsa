using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class TagihanDetailRepository : GenericRepository<Domain.Entities.Tagihan_detail>, ITagihanDetailRepository
    {
        public TagihanDetailRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
