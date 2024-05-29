
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class CategoryEvents
    {
        [Key]
        public int CategoryEventId { get; set; }
        public string? Name { get; set; }
       
        public virtual ICollection<EventsContract>? Events { get; set; }

      
    }

  
}
