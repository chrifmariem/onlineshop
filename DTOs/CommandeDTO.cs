using System;
using System.Collections.Generic;
using System.Linq;
namespace onlineShop.DTOs
{
    public class CommandeDTO
    {
        public int Idcmd { get; set; }
        public DateTime DateCmd { get; set; }
        public string Statut { get; set; }
        public decimal? TotalHT { get; set; }
        public decimal? TotalTVA { get; set; }
        public decimal? TotalTTC { get; set; }
        public int Idclient { get; set; }
        public string NomClient { get; set; }

        // ✅ initialisation
        public List<LignecmdDTO> Lignes { get; set; } = new List<LignecmdDTO>();
        public bool AFacture { get; set; }

        public int NombreArticles => Lignes?.Sum(l => l.Qte) ?? 0;

        public string StatutCouleur => Statut?.ToLower() switch
        {
            "en cours" => "warning",
            "validée" => "success",
            "annulée" => "danger",
            "livrée" => "info",
            "en attente" => "secondary",
            _ => "light"
        };
    }
}