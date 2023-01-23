using Pulsa.Core.Repositories;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.DataAccess.Repository
{
    public class TagihanDetailRepository : GenericRepository<Domain.Entities.Tagihan_detail>, ITagihanDetailRepository
    {
        public TagihanDetailRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
