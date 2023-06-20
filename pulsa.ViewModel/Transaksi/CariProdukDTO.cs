using System.ComponentModel.DataAnnotations.Schema;

namespace Pulsa.ViewModel
{
    public class CariProdukDTO
    {
        public string? product_id { get; set; }
        public string? product_name { get; set; }
        public string? product_detail{ get; set; }
        public string? product_syarat { get; set; }
        public string? product_zona { get; set; }
        public string? suppliyer { get; set; }
        public Int32 price_suggest { get; set; }
        public Int32 price { get; set; }

    }
}
