using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class CategoryTransaction
    {
        [Key]
        public int CategoryTransactionId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Transactions>? Transactions { get; set; }
        public virtual ICollection<CostDetails>? CostDetails { get; set; }
    }
}
