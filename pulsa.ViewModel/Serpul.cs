namespace Pulsa.ViewModel
{

    public class SerpulRespondStatus
    {
        public string? responseStatus { get; set; }
        public string? responseMessage { get; set; }
        public int? responseCode { get; set; }
    }


    public class SerpulRespondAccount : SerpulRespondStatus
    {
        public SerpulAccount? responseData { get; set; }
    }


    public class responseDataCategory : SerpulRespondStatus
    {
        public List<DataCategory> responseData { get; set; }
    }
    public class responseDataProdukOperator : SerpulRespondStatus
    {
        public List<operatorCategory> responseData { get; set; }
    }
    public class responsePrabayarProduk : SerpulRespondStatus
    {
        public List<prabayarProduk> responseData { get; set; }
    }
    public class SerpulRespondTagihanListrik : SerpulRespondStatus
    {
        public SerpulTagihanListrik responseData { get; set; }
    }
    public class SerpulRespondPaymentBill: SerpulRespondStatus
    {
        public SerpulPaymentBill responseData { get; set; }
    }

    public class SerpulRespondCekBill : SerpulRespondStatus
    {
        public DetailPaymentPasca responseData { get; set; }
    }

    public class SerpulRespondPayment : SerpulRespondStatus
    {
        public DetailPayment responseData { get; set; }
    }

    
    public class SerpulAccount
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? company_name { get; set; }
        public string? phone { get; set; }
        public int balance { get; set; }
    }
    public class DataCategory
    {
        public string? product_id { get; set; }
        public string? product_name { get; set; }
        public string? status { get; set; }
        public string? updated_at { get; set; }
    }
    public class operatorCategory
    {
        public string? product_id { get; set; }
        public string? product_name { get; set; }
        public string? prefix { get; set; }
        public string? status { get; set; }
        public string? updated_at { get; set; }
    }

    public class prabayarProduk
    {
        public string? supplier { get; set; } = "serpul";
        public string? category_id { get; set; }
        public string? category_name { get; set; }
        public string? operator_id { get; set; }
        public string? operator_name { get; set; }
        public string? product_id { get; set; }
        public string? product_name { get; set; }
        public string? product_detail { get; set; }
        public string? product_syarat { get; set; }
        public string? product_zona { get; set; }
        public Int32? product_price { get; set; }
        public string? product_multi { get; set; }
        public string? status { get; set; }
        public virtual bool status_bool
        {
            get
            {
                if (status == "INACTIVE")
                {
                    return false;
                }
                else
                {
                    return true;
                };
            }
        }
    }

    public class SerpulTagihanListrik
    {
        public string? ref_id { get; set; }
        public string? no_pelanggan { get; set; }
        public string? nama_pelanggan { get; set; }
        public string? periode { get; set; }
        public string? multiplier { get; set; }
        public string? jumlah_tagihan { get; set; }
        public string? biaya_admin { get; set; }
        public string? total_tagihan { get; set; }
        public string? fee { get; set; }
        public string? total_bayar { get; set; }
        public List<DetailTagihan>? detail { get; set; }
        public string? description { get; set; }
    }

    public class SerpulPaymentBill
    {
        public string? status { get; set; }
        public string? ref_id { get; set; }
        public string? message { get; set; }
    }

    public class DetailTagihan
    {
        public string? key { get; set; }
        public string? value { get; set; }
    }

    public class DetailPaymentPasca
    {
        public int? id { get; set; }
        public string? ref_id { get; set; }
        public string? product_id { get; set; }
        public string? product_name { get; set; }
        public string? no_pelanggan { get; set; }
        public string? nama_pelanggan { get; set; }
        public string? bill_periode { get; set; }
        public int bill_tagihan { get; set; }
        public int bill_admin { get; set; }
        public int bill_total_tagihan { get; set; }
        public int bill_fee { get; set; }
        public int bill_total_bayar { get; set; }
        public string? bill_detail { get; set; }
        public string? message { get; set; }
        public string? serial_number { get; set; }
        public string? status { get; set; }
        public string? via { get; set; }
        public string? sender { get; set; }
        public string? created_at { get; set; }
        public string? updated_at { get; set; }
        public int? bill_multiplier { get; set; }
    }

    public class DetailPayment
    {
        public string? status { get; set; }
        public string? message { get; set; }
    }
}
