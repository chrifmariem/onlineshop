namespace onlineShop.DTOs
{
    using System;

    public class PanierItemDTO
    {
        internal decimal sousTotal;

        public int ProduitId { get; set; }
        public string NomProduit { get; set; }
        public decimal Prix { get; set; }
        public int Quantite { get; set; }
        public decimal Tva { get; set; }
        public string ImageUrl { get; set; }
        public int QteDisponible { get; set; }

        // Propriétés calculées
        public decimal MontantHT => Prix * Quantite;
        public decimal MontantTVA => MontantHT * (Tva / 100);
        public decimal SousTotal => MontantHT + MontantTVA;
        public bool QuantiteValide => Quantite <= QteDisponible && Quantite > 0;
        public string AlerteStock => Quantite > QteDisponible
            ? $"Seulement {QteDisponible} en stock"
            : null;
    }

}
