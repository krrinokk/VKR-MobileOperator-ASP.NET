using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class RoleUser
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User>? User { get; set; }
    }
}
