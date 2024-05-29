using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class Contract
    {
        [Key]
        public int ContractId { get; set; }
        public DateTime DateConclusion { get; set; }
        public DateTime DateConnectionTariff { get; set; }
        public string NumberPhone { get; set; }
        public string? Status { get; set; }
        public int ClientId_FK { get; set; }

        public int UserId_FK { get; set; }
        public int TariffId { get; set; }

        public int? SMSRemaining { get; set; }
        public int? MinutesRemaining { get; set; }
        public int? GBRemaining { get; set; }
        public virtual Tariff? Tariff { get; set; } = null!;
        public virtual Client? Client { get; set; } = null!;
        public virtual User? User { get; set; } = null!;
        public virtual ICollection<ServicesConnected>? ServicesConnected { get; set; }
        public virtual ICollection<CostDetails>? CostDetails { get; set; }
        public virtual ICollection<WriteOffs>? WriteOffs { get; set; }
        public virtual ICollection<Depositing>? Depositing { get; set; }

        public virtual ICollection<EventsContract>? Events { get; set; }

    }
}
