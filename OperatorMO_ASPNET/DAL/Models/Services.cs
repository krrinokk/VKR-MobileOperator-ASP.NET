using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class Services
    {
        [Key]
        public int ServiceId { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ServicesConnected>? ServicesConnected { get; set; }
    }
}
