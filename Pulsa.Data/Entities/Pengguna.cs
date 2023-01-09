using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Domain.Entities
{
    public class Pengguna
    {
        public Guid id { get; set; }
        public string nama { get; set; }
        public string email { get; set; }
        public string pin { get; set; }
        public int gagal { get; set; }
        public bool isActive { get; set; }
    }
}
