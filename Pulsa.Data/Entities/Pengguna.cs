namespace Pulsa.Domain.Entities
{
    public class Pengguna
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string nama { get; set; }
        public string email { get; set; }
        public string pin { get; set; } = "1234";
        public int gagal { get; set; } = 0;
        public int saldo { get; set; } = 0;
        public bool is_active { get; set; } = true;
        public bool isBlokir { get; set; } = false;
        
    }
}
