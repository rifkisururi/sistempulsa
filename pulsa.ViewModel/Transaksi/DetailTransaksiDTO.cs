using System.ComponentModel.DataAnnotations.Schema;

namespace Pulsa.ViewModel
{
    public class DetailTransaksiDTO
    {
        public Guid? id { get; set; }
        public string? dest { get; set; }
        public string? product_id { get; set; }
        public string? product_name { get; set; }
        public string? product_detail{ get; set; }
        public string? product_syarat { get; set; }
        public string? product_zona { get; set; }
        public string? sn { get; set; }
        public string? suppliyer { get; set; }
        public string? nama_pembeli { get; set; }
        public string price_buyer { get; set; }
        public string saldo_sebelum { get; set; }
        public string jumlah_mutasi { get; set; }
        public string saldo_sesudah { get; set; }
        public string created_at { get; set; }
        public int? status { get; set; }

    }
}
