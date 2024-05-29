﻿using System.ComponentModel.DataAnnotations;

namespace OperatorMO_ASPNET.DAL.Models
{
    public class Depositing
    {
        [Key]
        public int Id { get; set; }
        public int ContractId_FK { get; set; }
        public DateTime Date { get; set; }
        public decimal Sum { get; set; }
        public virtual Contract? Contract { get; set; } = null!;

    }
}