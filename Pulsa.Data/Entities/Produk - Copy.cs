using System.ComponentModel.DataAnnotations;

namespace Pulsa.Domain.Entities
{
    public class Produk
    {
        [Key]
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string? product_detail { get; set; }
        public string? product_syarat { get; set; }
        public string? product_zona { get; set; }
        public string category { get; set; }
        public string type_produk { get; set; }
        public string? brand { get; set; }
        public int? margin { get; set; }
        public int? price_suggest { get; set; }
        public int? bagihasil1 { get; set; }
        public int? bagihasil2 { get; set; }
        public bool status { get; set; } = true;
    }
}
