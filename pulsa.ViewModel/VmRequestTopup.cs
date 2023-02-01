namespace pulsa.ViewModel
{
    public class VmRequestTopup
    {
        public Guid? id { get; set; }
        public string? idmetode { get; set; }
        public string? nama_pengirim { get; set; }
        public int jumlah { get; set; }
        public int saldo_awal { get; set; }
        public int saldo_akhir { get; set; } 
        public string? waktu { get; set; }
        public string? waktu_action { get; set; }
        public string? penggunaNama { get; set; }
        public string? actionNama { get; set; }
        public int status { get; set; }
        public string statusTxt
        {
            get
            {
                if (status == 1)
                {
                    return $"Request";
                }
                else if (status == 2)
                {
                    return $"Approve";
                }
                else if (status == 3)
                {
                    return $"Reject";
                }
                else {
                    return "";   
                }
            }
        }
    }
}