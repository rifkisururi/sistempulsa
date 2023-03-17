using System.ComponentModel.DataAnnotations;

namespace Pulsa.Domain.Entities
{
    public class Supplier_produk
    {
        public string supplier { get; set; }
        public string category_id { get; set; }
        public string category_name { get; set; }
        public string operator_id { get; set; }
        public string operator_name { get; set; }
        [Key]
        public string product_id { get; set; }
        public string product_name { get; set; }
        public string product_detail { get; set; }
        public string product_syarat { get; set; }
        public string product_zona { get; set; }
        public int product_price { get; set; } = 0;
        public string product_multi { get; set; }
        public string status { get; set; } = string.Empty;
        //public virtual bool status_bool
        //{
        //    get
        //    {
        //        if (status == "INACTIVE")
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        };
        //    }
        //}
        public string updated_at { get; set; } = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}
