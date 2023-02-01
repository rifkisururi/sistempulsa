using System.ComponentModel.DataAnnotations.Schema;

namespace Pulsa.ViewModel
{
    public class InputTopUpDTO
    {
        public Guid id { get; set; } = Guid.Empty;
        public Guid idpengguna { get; set; }
        public string? idmetode { get; set; }
        public string? nama_pengirim { get; set; }
        //public string? action { get; set; }
        public Int32 jumlah { get; set; }
        public Int32 status { get; set; }


    }
}
