using System.ComponentModel.DataAnnotations;

namespace Pulsa.Domain.Entities
{
    public class Pengguna_Traksaksi
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid pengguna { get; set; }
        public string? ref_id { get; set; }
        public string product_id { get; set; }
        public string suppliyer { get; set; }
        public string tujuan { get; set; }
        public int harga { get; set; }
        public int harga_jual { get; set; }
        public int? harga_jual_agen { get; set; }
        public int? bagihasil1 { get; set; } = 0;
        public int? bagihasil2 { get; set; } = 0;
        public Guid? customer_id { get; set; }
        public int? status_transaksi { get; set; }
        public string? sn { get; set; }
        public string? hit_start { get; set; }
        public string? hit_result { get; set; }
        public string? createdAt { get; set; } 
        public string? updatedAt { get; set; } = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        public Guid? pasca_id { get; set; }

    }
}
