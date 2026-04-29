using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineShop.Respositories
{
    public class ProduitRepository : IProduitRepository
    {
        private readonly ApplicationDbContext _context;

        public ProduitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Produit> GetByCodeBarAsync(string codeBar)
        {
            return await _context.Produits
                .FirstOrDefaultAsync(p => p.codebar == codeBar);
        }

        public async Task<IEnumerable<Produit>> GetProduitsDisponiblesAsync()
        {
            return await _context.Produits
                .Where(p => p.qtestock > 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produit>> GetProduitsAvecPromoAsync()
        {
            return await _context.Produits
                .Where(p => p.promoweb > 0)
                .ToListAsync();
        }
        public async Task<IEnumerable<Produit>> SearchProduitsAsync(string searchTerm)
        {
            return await _context.Produits
                .Where(p => p.design.Contains(searchTerm))
                .ToListAsync();
        }


        public async Task<IEnumerable<Produit>> GetProduitsStockFaibleAsync(int seuilStock = 10)
        {
            return await _context.Produits
                .Where(p => p.qtestock <= seuilStock)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produit>> GetProduitsByLaboAsync(string labo)
        {
            return await _context.Produits
                .Where(p => p.labo == labo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produit>> GetProduitsPageAsync(int pageNumber, int pageSize)
        {
            return await _context.Produits
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalProduitsCountAsync()
        {
            return await _context.Produits.CountAsync();
        }

        public async Task<bool> CodeBarExistsAsync(string codeBar)
        {
            return await _context.Produits
                .AnyAsync(p => p.codebar == codeBar);
        }

        public async Task UpdateStockAsync(int idProduit, int nouvelleQuantite)
        {
            var produit = await _context.Produits.FindAsync(idProduit);
            if (produit != null)
            {
                produit.qtestock= nouvelleQuantite;
                await _context.SaveChangesAsync();
            }
        }
    }
}
