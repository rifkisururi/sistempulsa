using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.ViewModel.Dflash
{
    public class DflashSaldoDTO
    {
        public int status { get; set; }
        public string memberID { get; set; }
        public string nama { get; set; }
        public int trxcount { get; set; }
        public int saldo { get; set; }
        public int pemakaian { get; set; }
    }
}
