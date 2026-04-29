using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineShop.Respositories
{
    public class ReclamationRepository : IReclamationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReclamationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get a single reclamation with related Client and Commande details
        public async Task<Reclamation> GetReclamationWithDetailsAsync(int id)
        {
            return await _context.Reclamations
                .Include(r => r.Client)
                .Include(r => r.Commande)
                .FirstOrDefaultAsync(r => r.Idrec == id);
        }

        // Get all reclamations for a specific client
        public async Task<IEnumerable<Reclamation>> GetReclamationsByClientAsync(int clientId)
        {
            return await _context.Reclamations
                .Where(r => r.Idclient == clientId)
                .ToListAsync();
        }

        // Get reclamations by their statut
        public async Task<IEnumerable<Reclamation>> GetReclamationsByStatutAsync(string statut)
        {
            return await _context.Reclamations
                .Where(r => r.statut == statut)
                .ToListAsync();
        }

        // Get reclamations that are "en cours" (waiting)
        public async Task<IEnumerable<Reclamation>> GetReclamationsEnAttenteAsync()
        {
            return await _context.Reclamations
                .Where(r => r.statut == "en cours")
                .ToListAsync();
        }

        // Get the most recent reclamations (by date)
        public async Task<IEnumerable<Reclamation>> GetRecentReclamationsAsync(int count)
        {
            return await _context.Reclamations
                .OrderByDescending(r => r.daterec)
                .Take(count)
                .ToListAsync();
        }

        // Count reclamations that are still in progress
        public async Task<int> GetReclamationsEnCoursCountAsync()
        {
            return await _context.Reclamations
                .CountAsync(r => r.statut == "en cours");
        }

        // Update the status and admin response for a reclamation
        public async Task UpdateStatutAsync(int idReclamation, string nouveauStatut, string reponseAdmin)
        {
            var reclamation = await _context.Reclamations.FindAsync(idReclamation);
            if (reclamation != null)
            {
                reclamation.statut = nouveauStatut;
                reclamation.reponseAdmin = reponseAdmin;
                reclamation.dateresp = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Get reclamations linked to a specific commande
        public async Task<IEnumerable<Reclamation>> GetReclamationsByCommandeAsync(int idCommande)
        {
            return await _context.Reclamations
                .Where(r => r.Idcmd == idCommande)
                .ToListAsync();
        }
    }
}
