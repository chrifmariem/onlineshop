namespace onlineShop.DTOs
{
    public class ProduitListDto
    {
        public List<ProduitDTO> Produits { get; set; }
        public int PageActuelle { get; set; }
        public int TotalPages { get; set; }
        public int TotalProduits { get; set; }
        public string Recherche { get; set; }
        public string Categorie { get; set; }
    }
}
