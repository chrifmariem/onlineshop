using Microsoft.AspNetCore.Mvc;
using onlineShop.DTOs;
using onlineShop.Services;

namespace onlineShop.Controllers
{
    public class PanierController : Controller
    {
        private readonly IPanierService _panierService;
        private readonly IProduitService _produitService;

        public PanierController(IPanierService panierService, IProduitService produitService)
        {
            _panierService = panierService;
            _produitService = produitService;
        }

        // GET: Voir le panier
        public async Task<IActionResult> Index()
        {
            var panier = await _panierService.GetPanierAsync();
            return View(panier);
        }

        // POST: Ajouter au panier (from product page)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter(int produitId, int quantite = 1)
        {
            var produit = await _produitService.GetProduitByIdAsync(produitId);
            if (produit == null)
            {
                TempData["ErrorMessage"] = "Produit non trouvé";
                return RedirectToAction("Index", "Produits");
            }

            if (produit.QteStock < quantite)
            {
                TempData["ErrorMessage"] = $"Stock insuffisant. Seulement {produit.QteStock} disponible(s).";
                return RedirectToAction("Details", "Produits", new { id = produitId });
            }

            var panierItem = new PanierItemDTO
            {
                ProduitId = produit.Idproduit,
                NomProduit = produit.Design,
                Prix = produit.VenteHT,
                Quantite = quantite,
                Tva = produit.Tva,
                ImageUrl = produit.Image,
                QteDisponible = produit.QteStock
            };

            await _panierService.AjouterAuPanierAsync(panierItem);
            TempData["SuccessMessage"] = "Produit ajouté au panier !";

            return RedirectToAction("Index");
        }

        // POST: Modifier la quantité
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierQuantite(int produitId, int quantite)
        {
            await _panierService.ModifierQuantiteAsync(produitId, quantite);
            return RedirectToAction(nameof(Index));
        }

        // POST: Supprimer un produit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Supprimer(int produitId)
        {
            await _panierService.SupprimerProduitAsync(produitId);
            TempData["SuccessMessage"] = "Produit retiré du panier";
            return RedirectToAction(nameof(Index));
        }

        // POST: Vider le panier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vider()
        {
            await _panierService.ViderPanierAsync();
            TempData["SuccessMessage"] = "Panier vidé";
            return RedirectToAction(nameof(Index));
        }

        // GET: Nombre d'articles dans le panier (for layout)
        public async Task<IActionResult> GetPanierCount()
        {
            var count = await _panierService.GetPanierCountAsync();
            return Json(new { count });
        }
    }
}