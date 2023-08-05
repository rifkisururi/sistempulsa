namespace Pulsa.ViewModel
{
    public class MutasiDTO
    {
        public Guid id { get; set; } // id mutasi
        public string type { get; set; }
        public string produk { get; set; }
        public string saldo_sebelum { get; set; }
        public string jumlah_mutasi { get; set; }
        public string saldo_sesudah { get; set; }
        public string created_at { get; set; }
        public string note { get; set; }
    }
}