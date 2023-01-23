using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly PulsaDataContext _context;
        public UnitOfWork(PulsaDataContext context)
        {
            _context = context;
            TagihanMasterRepository = new TagihanMasterRepository(_context);
            //TagihanDetailRepository = new TagihanDetailRepository(_context);

        }
        public ITagihanMasterRepository TagihanMasterRepository { get; private set; }
        //public ITagihanDetailRepository TagihanDetailRepository { get; private set; }

        public bool Complete()
        {
            bool isSuccess = true;
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    //Log.Error("Error UoW: {0} {1} {2} {3}", ex.LineNumber(), ex.Detail(), ex.Message, ((ex.InnerException != null) ? ex.InnerException.Message : ""));
                    Log.Error("Error UoW: {0} {1} ", ex.Message, ((ex.InnerException != null) ? ex.InnerException.Message : ""));
                    isSuccess = false;
                    dbContextTransaction.Rollback();
                }
            }
            return isSuccess;
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
