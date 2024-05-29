using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class CostDetails
    {
        [Key]
        public int CostDetailId { get; set; }
        public int ContractId_FK { get; set; }
        public DateTime Date { get; set; }
        public decimal? Cost { get; set; }
        public int CategoryTransaction_FK { get; set; }
        public virtual Contract? Contract { get; set; } = null!;
        public virtual CategoryTransaction? CategoryTransaction { get; set; } = null!;

    }
}
