using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using onlineShop.Respositories;

namespace onlineShop.Services
{
    public class CommandeService : ICommandeService
    {
        private readonly ICommandeRepository _commandeRepository;
        private readonly ApplicationDbContext _context;

        public CommandeService(ICommandeRepository commandeRepository, ApplicationDbContext context)
        {
            _commandeRepository = commandeRepository;
            _context = context;
        }

        public async Task<Commande> GetCommandeWithDetailsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID commande invalide");

            return await _commandeRepository.GetCommandeWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Commande>> GetCommandesByClientAsync(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("ID client invalide");

            return await _commandeRepository.GetCommandesByClientAsync(clientId);
        }

        public async Task<IEnumerable<Commande>> GetCommandesByStatutAsync(string statut)
        {
            if (string.IsNullOrWhiteSpace(statut))
                return Enumerable.Empty<Commande>();

            return await _commandeRepository.GetCommandesByStatutAsync(statut);
        }

        public async Task<IEnumerable<Commande>> GetCommandesByDateRangeAsync(DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
                throw new ArgumentException("La date de début doit être antérieure à la date de fin");

            return await _commandeRepository.GetCommandesByDateRangeAsync(dateDebut, dateFin);
        }

        public async Task<IEnumerable<Commande>> GetRecentCommandesAsync(int count)
        {
            if (count <= 0)
                count = 5;

            return await _commandeRepository.GetRecentCommandesAsync(count);
        }

        public async Task<decimal> GetChiffreAffairesTotalAsync()
        {
            return await _commandeRepository.GetChiffreAffairesTotalAsync();
        }

        public async Task<decimal> GetChiffreAffairesMoisAsync(int mois, int annee)
        {
            if (mois < 1 || mois > 12)
                throw new ArgumentException("Mois invalide");

            if (annee < 2000)
                throw new ArgumentException("Année invalide");

            return await _commandeRepository.GetChiffreAffairesMoisAsync(mois, annee);
        }

        public async Task<int> GetTotalCommandesCountAsync()
        {
            return await _commandeRepository.GetTotalCommandesCountAsync();
        }

        public async Task<int> GetCommandesEnCoursCountAsync()
        {
            return await _commandeRepository.GetCommandesEnCoursCountAsync();
        }

        public async Task<IEnumerable<Commande>> GetCommandesPendingFactureAsync()
        {
            return await _commandeRepository.GetCommandesPendingFactureAsync();
        }

        public async Task UpdateStatutAsync(int idCommande, string nouveauStatut)
        {
            if (idCommande <= 0)
                throw new ArgumentException("ID commande invalide");

            if (string.IsNullOrWhiteSpace(nouveauStatut))
                throw new ArgumentException("Statut invalide");

            await _commandeRepository.UpdateStatutAsync(idCommande, nouveauStatut);
        }

        // NEW METHODS ADDED:
        public async Task<List<Commande>> GetAllCommandesAsync()
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Facture)
                .OrderByDescending(c => c.datecmd)
                .ToListAsync();
        }

        public async Task<List<Commande>> GetCommandesSansFactureAsync()
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Where(c => c.Facture == null && (c.statut == "validée" || c.statut == "livrée"))
                .OrderByDescending(c => c.datecmd)
                .ToListAsync();
        }

        public async Task<Commande?> GetCommandeByIdAsync(int id)
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Facture)
                .FirstOrDefaultAsync(c => c.Idcmd == id);
        }
    }
}