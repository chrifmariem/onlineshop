using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using onlineShop.Respositories;

namespace onlineShop.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔍 Trouver un client par email
        public async Task<Client> GetByEmailAsync(string email)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.email == email);
        }

        // 📧 Vérifier si un email existe
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Clients
                .AnyAsync(c => c.email == email);
        }

        // 👤 Récupérer clients par rôle (ADMIN / CLIENT)
        public async Task<IEnumerable<Client>> GetClientsByRoleAsync(string role)
        {
            return await _context.Clients
                .Where(c => c.role == role)
                .ToListAsync();
        }

        // 🔎 Recherche client (nom, prénom, email)
        public async Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm)
        {
            return await _context.Clients
                .Where(c =>
                    c.nom.Contains(searchTerm) ||
                    c.prenom.Contains(searchTerm) ||
                    c.email.Contains(searchTerm))
                .ToListAsync();
        }

        // 📊 Nombre total de clients
        public async Task<int> GetTotalClientsCountAsync()
        {
            return await _context.Clients.CountAsync();
        }

        // 🆕 Derniers clients inscrits
        public async Task<IEnumerable<Client>> GetRecentClientsAsync(int count)
        {
            return await _context.Clients
                .OrderByDescending(c => c.dateinscrip)
                .Take(count)
                .ToListAsync();
        }
    }
}
