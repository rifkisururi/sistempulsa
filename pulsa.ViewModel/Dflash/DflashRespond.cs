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

    public class DflashRespondGetProduk
    {
        List<DflashProviderData> data { get; set; }
    }
    public class DflashProviderData
    {
        public string provider { get; set; }
        public string kategori { get; set; }
        public List<DflashProduk> data { get; set; }
    }

    public class DflashProduk
    {
        public string kode { get; set; }
        public string nama { get; set; }
        public int harga { get; set; }
        public int status { get; set; }
    }
    public class DflashCekTransaksi
    {
        public bool Check { get; set; }
        public string Refid { get; set; }
        public string KodeProduk { get; set; }
        public string Tujuan { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public string refid { get; set; }
        public bool check { get; set; }
        public bool @double { get; set; }
        public DateTime tgl_entri { get; set; }
        public DateTime tgl_status { get; set; }
        public string kode_produk { get; set; }
        public string tujuan { get; set; }
        public int? counter { get; set; }
        public int status { get; set; }
        public string status_text { get; set; }
        public string message { get; set; }
        public string sn { get; set; }
        public string keterangan { get; set; }
        public int? harga { get; set; }
        public long? saldo { get; set; }
    }



}
