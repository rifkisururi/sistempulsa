using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class SaldoRefundRepository : GenericRepository<Domain.Entities.Saldo_refund>, ISaldoRefundRepository
    {
        public SaldoRefundRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
