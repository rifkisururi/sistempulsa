using Pulsa.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.DataAccess.Interface
{
    public interface ITagihanDetailRepository : IGenericRepository<Domain.Entities.Tagihan_detail>
    {
        // dibuat kosong jika tidak menambahkan spesial query yang digunakan terus menerus
    }
}
