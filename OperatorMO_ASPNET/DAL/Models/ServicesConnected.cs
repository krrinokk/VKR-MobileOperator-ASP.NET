using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class ServicesConnected
    {
        [Key]
        public int CSId { get; set; }
        public int ServiceId_FK { get; set; }
        public int ContractId_FK { get; set; }
        public DateTime DateConnection { get; set; }
        public virtual Services? Services { get; set; } = null!;
        public virtual Contract? Contract { get; set; } = null!;
    }
}
