using onlineShop.Models;


namespace onlineShop.Respositories
{
    public interface IFactureRepository
    {
        Task<Facture> GetFactureByCommandeAsync(int idCommande);
        Task<Facture> GetFactureWithDetailsAsync(int id);
        Task<Facture> GetByNumeroFactureAsync(string numFac);
        Task<IEnumerable<Facture>> GetFacturesByStatutAsync(string statut);
        Task<IEnumerable<Facture>> GetFacturesByDateRangeAsync(DateTime dateDebut, DateTime dateFin);
        Task<string> GenerateNumeroFactureAsync();
        Task<bool> NumeroFactureExistsAsync(string numFac);
        Task UpdateStatutAsync(int idFacture, string nouveauStatut);
        Task<decimal> GetMontantTotalFacturesAsync();
    }
}
