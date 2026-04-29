using onlineShop.Models;


namespace onlineShop.Respositories
{
    public interface IReclamationRepository
    {
        Task<Reclamation> GetReclamationWithDetailsAsync(int id);
        Task<IEnumerable<Reclamation>> GetReclamationsByClientAsync(int clientId);
        Task<IEnumerable<Reclamation>> GetReclamationsByStatutAsync(string statut);
        Task<IEnumerable<Reclamation>> GetReclamationsEnAttenteAsync();
        Task<IEnumerable<Reclamation>> GetRecentReclamationsAsync(int count);
        Task<int> GetReclamationsEnCoursCountAsync();
        Task UpdateStatutAsync(int idReclamation, string nouveauStatut, string reponseAdmin);
        Task<IEnumerable<Reclamation>> GetReclamationsByCommandeAsync(int idCommande);
    }
}
