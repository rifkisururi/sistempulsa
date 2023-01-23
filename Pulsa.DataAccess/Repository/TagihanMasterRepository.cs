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
    public class TagihanMasterRepository : GenericRepository<Domain.Entities.Tagihan_master>, ITagihanMasterRepository
    {
        public TagihanMasterRepository(PulsaDataContext context) : base(context)
        {
        }
    }
}
