namespace Pulsa.ViewModel
{
    public class VMTagihanListrik
    {
        public Guid id { get; set; } // id tagihan detail
        public string type_tagihan { get; set; }
        public string id_tagihan { get; set; }
        public string nama_pelanggan { get; set; }
        public string group_tagihan { get; set; }
        public string ref_id { get; set; }
        public string periode_tagihan { get; set; }
        public DateOnly? tanggal_cek { get; set; }
        public DateOnly? tanggal_bayar { get; set; }
        public int? jumlah_tagihan { get; set; }
        public int? admin_tagihan { get; set; }
        public int? admin_notta { get; set; }
        public int? admin { get; set; }
        public int? total { get; set; }
        public bool? status_bayar { get; set; }
        public bool? harus_dibayar { get; set; }
    }
}