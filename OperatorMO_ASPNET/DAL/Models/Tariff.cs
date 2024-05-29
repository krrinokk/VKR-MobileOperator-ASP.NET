using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class Tariff
    {
        [Key]
        public int TariffId { get; set; }
        public DateTime DateOpening { get; set; }
        public string Name { get; set; }
        public int Minutes { get; set; }
        public int GB { get; set; }
        public int SMS { get; set; }
        public decimal Cost { get; set; }
        public virtual ICollection<Transactions>? Transactions { get; set; }
        public virtual ICollection<Contract>? Contract { get; set; }
    }
}
