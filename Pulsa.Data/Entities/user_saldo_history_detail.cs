using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Domain.Entities
{
    public class user_saldo_history_detail
    {
        [Key]
        public Guid id { get; set; }
        public Guid? idpengguna { get; set; }
        public Guid? id_transaksi { get; set; }
    }
}
