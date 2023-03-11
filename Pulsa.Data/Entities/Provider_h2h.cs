namespace Pulsa.Domain.Entities
{
    public class Provider_h2h
    {
        public int id { get; set; }
        public string nama { get; set; }
        public string username { get; set; }
        public string api_key { get; set; }
        public bool is_active { get; set; } = true;
    }
}
