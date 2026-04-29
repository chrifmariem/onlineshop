using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using onlineShop.Respositories;

namespace onlineShop.Repositories
{
    public class CommandeRepository : ICommandeRepository
    {
        private readonly ApplicationDbContext _context;

        public CommandeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📦 Commande avec client + lignes + produits
        public async Task<Commande> GetCommandeWithDetailsAsync(int id)
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Lignecmds)
                    .ThenInclude(l => l.Produit)
                .FirstOrDefaultAsync(c => c.Idcmd == id);
        }

        // 👤 Commandes d’un client
        public async Task<IEnumerable<Commande>> GetCommandesByClientAsync(int clientId)
        {
            return await _context.Commandes
                .Where(c => c.Idclient == clientId)
                .OrderByDescending(c => c.datecmd)
                .ToListAsync();
        }

        // 📌 Commandes par statut
        public async Task<IEnumerable<Commande>> GetCommandesByStatutAsync(string statut)
        {
            return await _context.Commandes
                .Where(c => c.statut == statut)
                .OrderByDescending(c => c.datecmd)
                .ToListAsync();
        }

        // 📅 Commandes par période
        public async Task<IEnumerable<Commande>> GetCommandesByDateRangeAsync(DateTime dateDebut, DateTime dateFin)
        {
            return await _context.Commandes
                .Where(c => c.datecmd >= dateDebut && c.datecmd <= dateFin)
                .OrderByDescending(c => c.datecmd)
                .ToListAsync();
        }

        // 🆕 Dernières commandes
        public async Task<IEnumerable<Commande>> GetRecentCommandesAsync(int count)
        {
            return await _context.Commandes
                .OrderByDescending(c => c.datecmd)
                .Take(count)
                .ToListAsync();
        }

        // 💰 Chiffre d’affaires total
        public async Task<decimal> GetChiffreAffairesTotalAsync()
        {
            return await _context.Commandes
                .Where(c => c.statut == "Validée")
                .SumAsync(c => c.totalTTC) ?? 0m;
        }


        // 📆 Chiffre d’affaires par mois
        public async Task<decimal> GetChiffreAffairesMoisAsync(int mois, int annee)
        {
            return await _context.Commandes
                .Where(c =>
                    c.statut == "Validée" &&
                    c.datecmd.Month == mois &&
                    c.datecmd.Year == annee)
                .SumAsync(c => c.totalTTC) ?? 0m;
        }


        // 📊 Nombre total de commandes
        public async Task<int> GetTotalCommandesCountAsync()
        {
            return await _context.Commandes.CountAsync();
        }

        // ⏳ Commandes en cours
        public async Task<int> GetCommandesEnCoursCountAsync()
        {
            return await _context.Commandes
                .CountAsync(c => c.statut == "En cours");
        }

        // 🧾 Commandes sans facture
        public async Task<IEnumerable<Commande>> GetCommandesPendingFactureAsync()
        {
            return await _context.Commandes
                .Where(c => !_context.Factures.Any(f => f.Idcmd == c.Idcmd))
                .OrderByDescending(c => c.datecmd)
                .ToListAsync();
        }

        // 🔄 Mise à jour du statut
        public async Task UpdateStatutAsync(int idCommande, string nouveauStatut)
        {
            var commande = await _context.Commandes
                .FirstOrDefaultAsync(c => c.Idcmd == idCommande);

            if (commande == null)
                throw new Exception("Commande introuvable");

            commande.statut = nouveauStatut;
            await _context.SaveChangesAsync();
        }
    }
}
