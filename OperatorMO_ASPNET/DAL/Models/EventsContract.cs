
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class EventsContract
    {
        [Key]
        public int Id { get; set; }
        public int ContractId_FK { get; set; }
        public int CategoryEventId_FK { get; set; }
        public DateTime Date { get; set; }
   
        public virtual CategoryEvents? CategoryEvents { get; set; } = null!;

        public virtual Contract? Contract { get; set; } = null!;

    }

  
}
