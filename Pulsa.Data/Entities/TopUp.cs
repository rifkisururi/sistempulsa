using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulsa.Domain.Entities
{
    public class TopUp
    {
        [Key]
        public Guid id { get; set; }
        public Guid idPengguna { get; set; }
        [ForeignKey("idPengguna")]
        public Pengguna pengguna { get; set; }
        public string waktu { get; set; }
        public int jumlah { get; set; }
        public Guid idMetode { get; set; }
        [ForeignKey("idMetode")]
        public TopUp_metode metode {get; set;}
        public int saldo_awal { get; set; }
        public int saldo_akhir { get; set; }
        public int status { get; set; }
        // 0 -> reques || 1 -> approve || 2 -> reject

    }
}
