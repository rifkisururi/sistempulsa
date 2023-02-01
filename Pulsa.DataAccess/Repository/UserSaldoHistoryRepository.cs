using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;

namespace Pulsa.DataAccess.Repository
{
    public class UserSaldoHistoryRepository : GenericRepository<Domain.Entities.user_saldo_history_detail>, IIUserSaldoHistoryRepository
    {
        public UserSaldoHistoryRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
