namespace onlineShop.DTOs
{
    public class PanierDto
    {
        public List<PanierItemDTO> Items { get; set; } = new List<PanierItemDTO>();
        public decimal TotalHT { get; set; }
        public decimal TotalTVA { get; set; }
        public decimal TotalTTC { get; set; }
        public int NombreArticles { get; set; }
        public int NombreProduits => Items?.Count ?? 0;
        public bool EstVide => !Items?.Any() ?? true;

        // Calculer les totaux
        public void CalculerTotaux()
        {
            if (Items == null || !Items.Any())
            {
                TotalHT = 0;
                TotalTVA = 0;
                TotalTTC = 0;
                NombreArticles = 0;
                return;
            }

            TotalHT = Items.Sum(i => i.MontantHT);
            TotalTVA = Items.Sum(i => i.MontantTVA);
            TotalTTC = Items.Sum(i => i.SousTotal);
            NombreArticles = Items.Sum(i => i.Quantite);
        }
    }
}
