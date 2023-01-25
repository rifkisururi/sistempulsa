using Pulsa.Core.Interface;

namespace Pulsa.DataAccess.Interface
{
    public interface ITopupRepository : IGenericRepository<Domain.Entities.TopUp>
    {
        // dibuat kosong jika tidak menambahkan spesial query yang digunakan terus menerus
    }
}
