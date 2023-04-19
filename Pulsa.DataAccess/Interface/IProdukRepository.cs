using Pulsa.Core.Interface;

namespace Pulsa.DataAccess.Interface
{
    public interface IProdukRepository : IGenericRepository<Domain.Entities.Produk>
    {
        // dibuat kosong jika tidak menambahkan spesial query yang digunakan terus menerus
    }
}
