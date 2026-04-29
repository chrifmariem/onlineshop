using System;
using System.Collections.Generic;

namespace onlineShop.DTOs
{
    public class CommandeListDto
    {
        public List<CommandeDTO> Commandes { get; set; } = new List<CommandeDTO>();
        public string StatutFiltre { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int TotalCommandes { get; set; }
        public decimal MontantTotal { get; set; }
    }
}