using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class Chat
    {
        [Key]
        public int MessageId { get; set; }
        public string Message { get; set; }
        public int UserId_FK { get; set; }
        public DateTime Date { get; set; }
       
        public virtual User? User { get; set; } = null!;
    }
}
