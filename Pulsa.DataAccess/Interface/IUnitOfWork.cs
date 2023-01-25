namespace Pulsa.DataAccess.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        ITagihanMasterRepository TagihanMasterRepository { get; }
        bool Complete();
    }
}
