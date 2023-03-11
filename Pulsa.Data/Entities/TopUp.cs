using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulsa.Domain.Entities
{
    public class TopUp
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid? idpengguna { get; set; }
        [ForeignKey("idpengguna")]
        public Pengguna? pengguna { get; set; }
        public string waktu { get; set; } = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        public Int32 jumlah { get; set; }
        public string? nama_pengirim { get; set; }
        public string? idmetode { get; set; }
        public Int32? saldo_awal { get; set; }
        public Int32? saldo_akhir { get; set; }
        public Int32 status { get; set; } = 1;
        // 1 -> reques || 2 -> approve || 3 -> reject
        public Guid? action_by { get; set; }
        [ForeignKey("action_by")]
        public Pengguna action { get; set; }
        
        public string? waktu_action { get; set; }

    }
}
