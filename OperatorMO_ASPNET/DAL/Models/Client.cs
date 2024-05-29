using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string LastName { get; set; }

        public decimal Balance { get; set; }

        public string Address { get; set; }
        public string Passport { get; set; }
        public string? Mail { get; set; }

        public virtual ICollection<Contract>? Contract { get; set; }
    }
}
