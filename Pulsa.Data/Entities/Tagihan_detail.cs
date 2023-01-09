using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Domain.Entities
{
    public class Tagihan_detail
    {
        [Key]
        public Guid id { get; set; }
        public Guid id_tagihan_master { get; set; }
        [ForeignKey("id_tagihan_master")]
        public Tagihan_master master { get; set; }
        public string ref_id { get; set; }
        public string periode_tagihan { get; set; }
        public DateOnly? tanggal_cek { get; set; }
        public DateOnly? tanggal_bayar { get; set; }
        public int jumlah_tagihan { get; set; }
        public int? admin_tagihan { get; set; }
        public int? admin_notta { get; set; }
        public bool? status_bayar { get; set; }
        public bool harus_dibayar { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;
        
    }
}
