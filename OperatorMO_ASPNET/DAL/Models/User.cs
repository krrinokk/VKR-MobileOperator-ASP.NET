
using Microsoft.EntityFrameworkCore;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class User
    {

        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public int RoleId_FK { get; set; }
        public virtual RoleUser? RoleUser { get; set; } = null!;
        public virtual ICollection<Chat>? Chat { get; set; }

        public virtual ICollection<Contract>? Contract { get; set; }
    }

  
}
