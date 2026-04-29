namespace onlineShop.DTOs
{
    public class LignecmdDTO
    {
        public int Idligne { get; set; }
        public int Idproduit { get; set; }
        public string NomProduit { get; set; }
        public string ImageProduit { get; set; }
        public int Qte { get; set; }
        public decimal? PrixUni { get; set; }  // Made nullable
        public decimal? Tva { get; set; }      // Made nullable
        public decimal? PrixTotal { get; set; } // Made nullable

        // Propriétés calculées
        public decimal MontantHT => (PrixUni ?? 0) * Qte;
        public decimal MontantTVA => MontantHT * ((Tva ?? 0) / 100);
        public decimal MontantTTC => MontantHT + MontantTVA;
    }
}