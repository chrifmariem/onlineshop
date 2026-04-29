using onlineShop.DTOs;

namespace onlineShop.Services
{
    public interface IPanierService
    {
        // Get cart
        PanierDto GetPanier();
        Task<PanierDto> GetPanierAsync();

        // Add to cart
        void AjouterAuPanier(PanierItemDTO item);
        Task AjouterAuPanierAsync(PanierItemDTO item);

        // Update quantity
        void ModifierQuantite(int produitId, int nouvelleQuantite);
        Task ModifierQuantiteAsync(int produitId, int nouvelleQuantite);

        // Remove item
        void SupprimerProduit(int produitId);
        Task SupprimerProduitAsync(int produitId);

        // Clear cart
        void ViderPanier();
        Task ViderPanierAsync();

        // Helper methods
        Task<int> GetPanierCountAsync();
        Task<decimal> GetPanierTotalAsync();
    }
}