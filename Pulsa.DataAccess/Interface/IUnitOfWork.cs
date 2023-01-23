using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Pulsa.DataAccess.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        ITagihanMasterRepository TagihanMasterRepository { get;}
        bool Complete();
    }
}
