using Pulsa.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Service.Interface
{
    public interface IPenggunaService
    {
        public Domain.Entities.Pengguna cekPengguna(Guid id, string fullname, string email);
    }
}
