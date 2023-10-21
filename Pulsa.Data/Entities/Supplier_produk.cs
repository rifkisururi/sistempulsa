using System.ComponentModel.DataAnnotations;

namespace Pulsa.Domain.Entities
{
    public class Supplier_produk
    {
        [Key]
        public string supplierkey { get; set; }
        public string supplier { get; set; }
        public string category { get; set; }
        public string operator_name { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string? product_detail { get; set; }
        public string? product_syarat { get; set; }
        public string? product_zona { get; set; }
        public int product_price { get; set; } = 0;
        public string? product_multi { get; set; }
        public string status { get; set; } = string.Empty;
        public string updated_at { get; set; }
    }
}
