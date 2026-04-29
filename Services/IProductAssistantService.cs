using onlineShop.Models;

namespace onlineShop.Services
{
    public interface IProductAssistantService
    {
        Task<string> GetProductGuidanceAsync(Produit product, string userQuestion);
    }
}
