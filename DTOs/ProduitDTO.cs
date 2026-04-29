namespace onlineShop.DTOs
{
    public class ProduitDTO
    {
        public int Idproduit { get; set; }
        public string Aref { get; set; }
        public string Design { get; set; }
        public decimal VenteHT { get; set; }
        public decimal Tva { get; set; }
        public decimal Ttc { get; set; }
        public int QteStock { get; set; }
        public decimal? PromoWeb { get; set; }
        public string Image { get; set; }
        public string Labo { get; set; }
        public DateTime? Dlc { get; set; }
        public string CodeBar { get; set; }
        public DateTime? DernMiseAJour { get; set; }
        public DateTime? DateCreation { get; set; }  // Added this property

        // Propriétés calculées
        public bool EstDisponible => QteStock > 0;
        public bool APromotion => PromoWeb.HasValue && PromoWeb.Value > 0;
        public decimal? PrixApresPromo => APromotion ? Ttc * (1 - PromoWeb.Value / 100) : null;

        // Propriétés pour l'affichage formaté
        public string DernMiseAJourFormattee => DernMiseAJour?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
        public string DateCreationFormattee => DateCreation?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
        public string DlcFormattee => Dlc?.ToString("dd/MM/yyyy") ?? "N/A";

        // Propriétés de statut
        public bool EstNouveau => DateCreation.HasValue &&
                                  (DateTime.Now - DateCreation.Value).TotalDays <= 30; // Produit créé il y a moins de 30 jours
    }
}