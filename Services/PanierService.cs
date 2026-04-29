using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.DTOs;
using onlineShop.Models;
using System.Security.Claims;

namespace onlineShop.Services
{
    public class PanierService : IPanierService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PanierService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetCurrentClientId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int clientId))
            {
                return clientId;
            }
            return null;
        }

        public async Task<PanierDto> GetPanierAsync()
        {
            var clientId = GetCurrentClientId();
            if (!clientId.HasValue)
                return new PanierDto();

            var panierItems = await _context.Paniers
                .Include(p => p.Produit)
                .Where(p => p.Idclient == clientId.Value)
                .ToListAsync();

            var items = panierItems.Select(p => new PanierItemDTO
            {
                ProduitId = p.Idproduit,
                NomProduit = p.Produit?.design,
                Prix = p.Produit?.venteHT ?? 0,
                Quantite = p.Quantite,
                Tva = p.Produit?.tva ?? 0,
                ImageUrl = p.Produit?.image,
                QteDisponible = p.Produit?.qtestock ?? 0
            }).ToList();

            var panier = new PanierDto { Items = items };
            panier.CalculerTotaux();
            return panier;
        }

        // Synchronous version for compatibility
        public PanierDto GetPanier()
        {
            return GetPanierAsync().GetAwaiter().GetResult();
        }

        public async Task AjouterAuPanierAsync(PanierItemDTO item)
        {
            var clientId = GetCurrentClientId();
            if (!clientId.HasValue)
                return;

            var existingItem = await _context.Paniers
                .FirstOrDefaultAsync(p => p.Idclient == clientId.Value && p.Idproduit == item.ProduitId);

            if (existingItem != null)
            {
                existingItem.Quantite += item.Quantite;
                existingItem.DateModification = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            else
            {
                var panierItem = new Panier
                {
                    Idclient = clientId.Value,
                    Idproduit = item.ProduitId,
                    Quantite = item.Quantite,
                    DateAjout = DateTime.Now
                };
                await _context.Paniers.AddAsync(panierItem);
                await _context.SaveChangesAsync();
            }
        }

        // Synchronous version for compatibility
        public void AjouterAuPanier(PanierItemDTO item)
        {
            AjouterAuPanierAsync(item).GetAwaiter().GetResult();
        }

        public async Task ModifierQuantiteAsync(int produitId, int nouvelleQuantite)
        {
            var clientId = GetCurrentClientId();
            if (!clientId.HasValue)
                return;

            var item = await _context.Paniers
                .FirstOrDefaultAsync(p => p.Idclient == clientId.Value && p.Idproduit == produitId);

            if (item != null)
            {
                if (nouvelleQuantite <= 0)
                {
                    _context.Paniers.Remove(item);
                }
                else
                {
                    item.Quantite = nouvelleQuantite;
                    item.DateModification = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }
        }

        // Synchronous version for compatibility
        public void ModifierQuantite(int produitId, int nouvelleQuantite)
        {
            ModifierQuantiteAsync(produitId, nouvelleQuantite).GetAwaiter().GetResult();
        }

        public async Task SupprimerProduitAsync(int produitId)
        {
            var clientId = GetCurrentClientId();
            if (!clientId.HasValue)
                return;

            var item = await _context.Paniers
                .FirstOrDefaultAsync(p => p.Idclient == clientId.Value && p.Idproduit == produitId);

            if (item != null)
            {
                _context.Paniers.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        // Synchronous version for compatibility
        public void SupprimerProduit(int produitId)
        {
            SupprimerProduitAsync(produitId).GetAwaiter().GetResult();
        }

        public async Task ViderPanierAsync()
        {
            var clientId = GetCurrentClientId();
            if (!clientId.HasValue)
                return;

            var panierItems = await _context.Paniers
                .Where(p => p.Idclient == clientId.Value)
                .ToListAsync();

            _context.Paniers.RemoveRange(panierItems);
            await _context.SaveChangesAsync();
        }

        // Synchronous version for compatibility
        public void ViderPanier()
        {
            ViderPanierAsync().GetAwaiter().GetResult();
        }

        public async Task<int> GetPanierCountAsync()
        {
            var clientId = GetCurrentClientId();
            if (!clientId.HasValue)
                return 0;

            return await _context.Paniers
                .Where(p => p.Idclient == clientId.Value)
                .SumAsync(p => p.Quantite);
        }

        public async Task<decimal> GetPanierTotalAsync()
        {
            var clientId = GetCurrentClientId();
            if (!clientId.HasValue)
                return 0;

            var panierItems = await _context.Paniers
                .Include(p => p.Produit)
                .Where(p => p.Idclient == clientId.Value)
                .ToListAsync();

            return panierItems.Sum(p => (p.Produit?.venteHT ?? 0) * p.Quantite);
        }
    }
}