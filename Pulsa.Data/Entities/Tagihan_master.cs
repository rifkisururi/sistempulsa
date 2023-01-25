using System.ComponentModel.DataAnnotations;

namespace Pulsa.Domain.Entities
{
    public class Tagihan_master
    {
        [Key]
        public Guid id { get; set; }
        public string? type_tagihan { get; set; }
        public string? id_tagihan { get; set; }
        public string? nama_pelanggan { get; set; }
        public string? group_tagihan { get; set; }
        public int? admin_notta { get; set; }
        public int? admin { get; set; }
        public bool? is_active { get; set; }
        //public DateTime? createdAt { get; set; } = DateTime.Now;
        //public DateTime? updatedAt { get; set; } = DateTime.Now;
    }
}
