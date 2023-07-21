using System.ComponentModel.DataAnnotations;

namespace Pulsa.Domain.Entities
{
    public class Pengguna_mutasi
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid pengguna_id { get; set; }
        public int saldo_sebelum { get; set; }
        public int mutasi { get; set; }
        public int saldo_sesudah { get; set; }
        public string type_transaksi { get; set; }
        public Guid id_transaksi { get; set; }
        public string createdAt { get; set; } = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}
