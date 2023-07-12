using Microsoft.VisualBasic;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.DataAccess.Repository;
using Pulsa.Domain.Entities;
using Serilog;
namespace Pulsa.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PulsaDataContext _context;
        public UnitOfWork(PulsaDataContext context)
        {
            _context = context;
            TagihanMasterRepository = new TagihanMasterRepository(_context);
            TagihanDetailRepository = new TagihanDetailRepository(_context);
            TopupRepository = new TopupRepository(_context);
            TopupMetodeRepository = new TopupMetodeRepository(_context);
            PenggunaRepository = new PenggunaRepository(_context);
            UserSaldoHistoryRepository = new UserSaldoHistoryRepository(_context);
            Provider_H2HRepository = new Provider_h2hRepository(_context);
            Supplier_ProdukRepository = new Supplier_produkRepository (_context);
            ProdukRepository = new ProdukRepository(_context);
            ProdukDetailRepository = new ProdukDetailRepository(_context);
            PenggunaTransaksiRepository = new PenggunaTransaksiRepository(_context);
        }
        public ITagihanMasterRepository TagihanMasterRepository { get; private set; }
        public ITagihanDetailRepository TagihanDetailRepository { get; private set; }
        public ITopupRepository TopupRepository { get; private set; }
        public ITopupMetodeRepository TopupMetodeRepository { get; private set; }
        public IPenggunaRepository PenggunaRepository { get; private set; }
        public IProvider_h2hRepository Provider_H2HRepository { get; private set; }
        public IUserSaldoHistoryRepository UserSaldoHistoryRepository { get; private set; }
        public ISupplier_produkRepository Supplier_ProdukRepository { get; private set; }
        public IProdukRepository ProdukRepository { get; private set; }
        public IProdukDetailRepository ProdukDetailRepository { get; private set; }
        public IPenggunaTransaksiRepository PenggunaTransaksiRepository { get; private set; }
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