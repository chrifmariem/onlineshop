using onlineShop.Models;


namespace onlineShop.Respositories
{
    public interface IProduitRepository
    {
        Task<Produit> GetByCodeBarAsync(string codeBar);
        Task<IEnumerable<Produit>> GetProduitsDisponiblesAsync();
        Task<IEnumerable<Produit>> GetProduitsAvecPromoAsync();
        Task<IEnumerable<Produit>> SearchProduitsAsync(string searchTerm);
        Task<IEnumerable<Produit>> GetProduitsStockFaibleAsync(int seuilStock = 10);
        Task<IEnumerable<Produit>> GetProduitsByLaboAsync(string labo);
        Task<IEnumerable<Produit>> GetProduitsPageAsync(int pageNumber, int pageSize);
        Task<int> GetTotalProduitsCountAsync();
        Task<bool> CodeBarExistsAsync(string codeBar);
        Task UpdateStockAsync(int idProduit, int nouvelleQuantite);
    }
}
