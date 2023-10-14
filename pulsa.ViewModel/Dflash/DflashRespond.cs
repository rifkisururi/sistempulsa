using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.ViewModel.Dflash
{
    public class DflashRespond
    {
        public string refid { get; set; }
        public bool check { get; set; }
        //public bool [double] { get; set;
        public string tgl_entri { get; set; }
        public string tgl_status { get; set; }
        public string kode_produk { get; set; }
        public string tujuan { get; set; }
        public int status { get; set; }
        public string status_text { get; set; }
        public string message { get; set; }
        public int counter { get; set; }
        public string keterangan { get; set; }
        public string sn { get; set; }
        public int harga { get; set; }
        public int saldo { get; set; }


    }
}
