namespace Pulsa.ViewModel
{
    public class InputTagihan
    {
        public Guid? id { get; set; }
        public string? type_tagihan { get; set; }
        public string? id_tagihan { get; set; }
        public string? nama_pelanggan { get; set; }
        public string? group_tagihan { get; set; }
        public int? admin_notta { get; set; }
        public int? admin { get; set; }
        public bool? is_active { get; set; }
        public string? action { get; set; }
    }
}
