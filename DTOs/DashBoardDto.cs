namespace onlineShop.DTOs
{
    public class DashBoardDto
    {
        public int TotalClients { get; set; }
        public int TotalProduits { get; set; }
        public int TotalCommandes { get; set; }
        public int CommandesEnCours { get; set; }
        public int CommandesValidees { get; set; }
        public decimal ChiffreAffairesTotal { get; set; }
        public decimal ChiffreAffairesMois { get; set; }
        public int ReclamationsEnAttente { get; set; }
        public List<CommandeDTO> DernieresCommandes { get; set; } = new List<CommandeDTO>();

        // DOIT ÊTRE ICI : List<ProduitDTO> au lieu de IEnumerable<ProduitDTO>
        public List<ProduitDTO> ProduitsStockFaible { get; set; } = new List<ProduitDTO>();


        public List<ProduitDTO> ProduitsPlusVendus { get; set; }
    }
}
