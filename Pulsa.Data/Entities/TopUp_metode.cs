using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulsa.Domain.Entities
{
    public class TopUp_metode
    {
        [Key]
        public Guid id { get; set; }
        public string name { get; set; }
        public string unik_key { get; set; }
        public bool isActive { get; set; }

    }
}
