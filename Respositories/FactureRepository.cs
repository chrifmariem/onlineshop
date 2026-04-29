using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;

namespace onlineShop.Respositories
{
    public class FactureRepository : IFactureRepository
    {
        private readonly ApplicationDbContext _context;

        public FactureRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔎 Facture par commande
        public async Task<Facture> GetFactureByCommandeAsync(int idCommande)
        {
            return await _context.Factures
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(f => f.Idcmd == idCommande);
        }

        // 🔎 Facture avec tous les détails
        public async Task<Facture> GetFactureWithDetailsAsync(int id)
        {
            return await _context.Factures
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Lignecmds)
                        .ThenInclude(l => l.Produit)
                .Include(f => f.Commande.Client)
                .FirstOrDefaultAsync(f => f.Idfacture == id);
        }

        // 🔎 Recherche par numéro de facture
        public async Task<Facture> GetByNumeroFactureAsync(string numFac)
        {
            return await _context.Factures
                .FirstOrDefaultAsync(f => f.numfac == numFac);
        }

        // 📌 Factures par statut
        public async Task<IEnumerable<Facture>> GetFacturesByStatutAsync(string statut)
        {
            return await _context.Factures
                .Where(f => f.statut == statut)
                .OrderByDescending(f => f.dateFac)
                .ToListAsync();
        }

        // 📆 Factures par période
        public async Task<IEnumerable<Facture>> GetFacturesByDateRangeAsync(DateTime dateDebut, DateTime dateFin)
        {
            return await _context.Factures
                .Where(f => f.dateFac >= dateDebut && f.dateFac <= dateFin)
                .OrderByDescending(f => f.dateFac)
                .ToListAsync();
        }

        // 🧾 Génération automatique du numéro de facture
        public async Task<string> GenerateNumeroFactureAsync()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");

            var lastFacture = await _context.Factures
                .Where(f => f.numfac.StartsWith($"FAC-{date}"))
                .OrderByDescending(f => f.numfac)
                .FirstOrDefaultAsync();

            int compteur = 1;

            if (lastFacture != null)
            {
                var lastNumber = lastFacture.numfac.Split('-').Last();
                int.TryParse(lastNumber, out compteur);
                compteur++;
            }

            return $"FAC-{date}-{compteur:D4}";
        }

        // ✅ Vérifier si le numéro existe
        public async Task<bool> NumeroFactureExistsAsync(string numFac)
        {
            return await _context.Factures.AnyAsync(f => f.numfac == numFac);
        }

        // 🔄 Mise à jour du statut
        public async Task UpdateStatutAsync(int idFacture, string nouveauStatut)
        {
            var facture = await _context.Factures.FindAsync(idFacture);

            if (facture == null)
                throw new KeyNotFoundException("Facture introuvable");

            facture.statut = nouveauStatut;
            await _context.SaveChangesAsync();
        }

        // 💰 Montant total des factures payées
        public async Task<decimal> GetMontantTotalFacturesAsync()
        {
            return await _context.Factures
                .Where(f => f.statut == "Payée")
                .SumAsync(f => f.totalTTC) ?? 0m;
        }
    }
}
