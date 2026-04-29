using onlineShop.Models;

namespace onlineShop.Services
{
    public interface ICommandeService
    {
        Task<Commande> GetCommandeWithDetailsAsync(int id);

        Task<IEnumerable<Commande>> GetCommandesByClientAsync(int clientId);

        Task<IEnumerable<Commande>> GetCommandesByStatutAsync(string statut);

        Task<IEnumerable<Commande>> GetCommandesByDateRangeAsync(DateTime dateDebut, DateTime dateFin);

        Task<IEnumerable<Commande>> GetRecentCommandesAsync(int count);

        Task<decimal> GetChiffreAffairesTotalAsync();

        Task<decimal> GetChiffreAffairesMoisAsync(int mois, int annee);

        Task<int> GetTotalCommandesCountAsync();

        Task<int> GetCommandesEnCoursCountAsync();

        Task<IEnumerable<Commande>> GetCommandesPendingFactureAsync();

        Task UpdateStatutAsync(int idCommande, string nouveauStatut);

    }
}
