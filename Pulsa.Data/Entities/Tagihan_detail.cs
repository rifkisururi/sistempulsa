using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulsa.Domain.Entities
{
    public class Tagihan_detail
    {
        [Key]
        public Guid id { get; set; } = new Guid();
        public Guid id_tagihan_master { get; set; }
        [ForeignKey("id_tagihan_master")]
        public Tagihan_master master { get; set; }
        public string ref_id { get; set; }
        public string periode_tagihan { get; set; }
        public DateTime? tanggal_cek { get; set; } = DateTime.Now.Date;
        public DateTime? tanggal_bayar { get; set; }
        public int jumlah_tagihan { get; set; }
        public int? admin_tagihan { get; set; }
        public int? admin_notta { get; set; }
        public bool? status_bayar { get; set; } = false;
        public bool harus_dibayar { get; set; } = true;
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;

    }
}
