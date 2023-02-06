using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class TagihanMasterRepository : GenericRepository<Domain.Entities.Tagihan_master>, ITagihanMasterRepository
    {
        public TagihanMasterRepository(PulsaDataContext context) : base(context)
        {
        }
        //public IQueryable<Domain.Entities.Tagihan_master> getHeader()
        //{
        //    return this.Find(p => p.FinancialReportId == Id);
        //}
    }
}
