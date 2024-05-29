using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }
        public int TariffId_FK { get; set; }
        public int CategoryTransaction_FK { get; set; }
        public decimal Cost { get; set; }
        public virtual CategoryTransaction? CategoryTransaction { get; set; } = null!;
        public virtual Tariff? Tariff { get; set; } = null!;
    }
}
