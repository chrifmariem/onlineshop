using onlineShop.Models;


namespace onlineShop.Respositories
{
    
   public interface IClientRepository
    {
        Task<Client> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<Client>> GetClientsByRoleAsync(string role);
        Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm);
        Task<int> GetTotalClientsCountAsync();
        Task<IEnumerable<Client>> GetRecentClientsAsync(int count);
    }
}
