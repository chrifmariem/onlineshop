using System;
namespace onlineShop.DTOs
{
    public class FactureDTO
    {
        public int Idfacture { get; set; }
        public string NumFac { get; set; }
        public DateTime DateFac { get; set; }
        public decimal? TotalHT { get; set; }
        public decimal? TotalTVA { get; set; }
        public decimal? TotalTTC { get; set; }
        public string Statut { get; set; }
        public int Idcmd { get; set; }
        public CommandeDTO Commande { get; set; }

        // Propriétés calculées
        public bool EstPayee => Statut?.ToLower() == "payée";
        public string StatutCouleur => Statut?.ToLower() switch
        {
            "payée" => "success",
            "non payée" => "danger",
            "en attente" => "warning",
            "annulée" => "secondary",
            _ => "light"
        };
    }
}