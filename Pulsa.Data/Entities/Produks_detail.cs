using System.ComponentModel.DataAnnotations;

namespace Pulsa.Domain.Entities
{
    public class Produks_detail
    {
        [Key]
        public string product_id { get; set; }
        public string suppliyer { get; set; }
        public string? suppliyer_product_id { get; set; }
        public bool status { get; set; }
    }
}
