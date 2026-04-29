using onlineShop.DTOs;

namespace onlineShop.Services
{
    public interface IProduitService
    {
        // Basic CRUD operations
        Task<ProduitDTO> GetProduitByIdAsync(int id);
        Task<ProduitDTO> GetProduitByCodeBarAsync(string codeBar);

        // ✅ CORRECTION: Retourne List<ProduitDTO> au lieu de IEnumerable
        Task<List<ProduitDTO>> GetAllProduitsAsync();

        Task<OperationResultDto> CreateProduitAsync(CreateProduitDto dto);
        Task<OperationResultDto> UpdateProduitAsync(int id, UpdateProduitDto dto);
        Task<OperationResultDto> DeleteProduitAsync(int id);

        // Query operations - Ces méthodes peuvent rester IEnumerable
        Task<IEnumerable<ProduitDTO>> GetProduitsDisponiblesAsync();
        Task<IEnumerable<ProduitDTO>> GetProduitsAvecPromoAsync();
        Task<IEnumerable<ProduitDTO>> GetProduitsStockFaibleAsync(int seuilStock = 10);
        Task<IEnumerable<ProduitDTO>> GetProduitsByLaboAsync(string labo);
        Task<IEnumerable<ProduitDTO>> SearchProduitsAsync(string searchTerm);

        // Pagination
        Task<IEnumerable<ProduitDTO>> GetProduitsPageAsync(int pageNumber, int pageSize);
        Task<int> GetTotalProduitsCountAsync();

        // Stock management
        Task<OperationResultDto> UpdateStockAsync(int idProduit, int nouvelleQuantite);
        Task<OperationResultDto> AjusterStockAsync(int idProduit, int quantite, bool isAddition);

        // Validation
        Task<bool> CodeBarExistsAsync(string codeBar);
        Task<bool> ProduitExistsAsync(int id);
    }
}