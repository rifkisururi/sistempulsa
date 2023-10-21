namespace Pulsa.Domain.Entities
{
    public class Saldo_refund
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public Guid idpengguna { get; set; }
        public Guid idtransaksi { get; set; }
        public string note { get; set; }
        public int jumlah { get; set; }
        public int saldo_awal { get; set; }
        public int saldo_akhir { get; set; }

    }
}
