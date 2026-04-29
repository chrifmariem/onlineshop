using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Services;
using onlineShop.DTOs;
using onlineShop.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace onlineShop.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IProduitService _produitService;
        private readonly ICommandeService _commandeService;
        private readonly IFactureService _factureService;
        private readonly ApplicationDbContext _context;

        public AdminController(
            IClientService clientService,
            IProduitService produitService,
            ICommandeService commandeService,
            IFactureService factureService,
            ApplicationDbContext context)
        {
            _clientService = clientService;
            _produitService = produitService;
            _commandeService = commandeService;
            _factureService = factureService;
            _context = context;
        }

        // =========================
        // 📊 DASHBOARD
        // =========================
        public async Task<IActionResult> Dashboard()
        {
            var dashboard = new DashBoardDto
            {
                TotalClients = await _clientService.GetTotalClientsCountAsync(),
                TotalProduits = await _produitService.GetTotalProduitsCountAsync(),
                TotalCommandes = await _commandeService.GetTotalCommandesCountAsync(),
                CommandesEnCours = await _commandeService.GetCommandesEnCoursCountAsync(),
                ChiffreAffairesTotal = await _commandeService.GetChiffreAffairesTotalAsync(),
                ChiffreAffairesMois = await _commandeService.GetChiffreAffairesMoisAsync(DateTime.Now.Month, DateTime.Now.Year),
                DernieresCommandes = (await _commandeService.GetRecentCommandesAsync(10))
                    .Select(c => new CommandeDTO
                    {
                        Idcmd = c.Idcmd,
                        DateCmd = c.datecmd,
                        Statut = c.statut,
                        TotalHT = c.totalHT,
                        TotalTVA = c.totalTVA,
                        TotalTTC = c.totalTTC,
                        Idclient = c.Idclient,
                        NomClient = c.Client != null ? $"{c.Client.nom} {c.Client.prenom}" : ""
                    }).ToList(),
                ProduitsPlusVendus = (await _produitService.GetProduitsStockFaibleAsync(10)).ToList()
            };

            return View(dashboard);
        }

        // =========================
        // 👥 CLIENTS
        // =========================
        [HttpGet]
        public async Task<IActionResult> Clients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return View(clients);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerClient(int id)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    TempData["ErrorMessage"] = "Client non trouvé";
                    return RedirectToAction(nameof(Clients));
                }

                // Vérifier si le client a des commandes
                var hasCommandes = await _context.Commandes.AnyAsync(c => c.Idclient == id);
                if (hasCommandes)
                {
                    TempData["ErrorMessage"] = "Impossible de supprimer ce client car il a des commandes associées";
                    return RedirectToAction(nameof(Clients));
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Client {client.nom} {client.prenom} supprimé avec succès";
                return RedirectToAction(nameof(Clients));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur lors de la suppression: {ex.Message}";
                return RedirectToAction(nameof(Clients));
            }
        }

        // =========================
        // 📦 PRODUITS
        // =========================
        [HttpGet]
        public async Task<IActionResult> Produits()
        {
            var produits = await _produitService.GetAllProduitsAsync();
            return View(produits);
        }

        // =========================
        // 🛒 COMMANDES
        // =========================
        [HttpGet]
        public async Task<IActionResult> Commandes(string statut = null)
        {
            ViewBag.StatutFiltre = statut;

            // Get all commandes with client info
            var commandes = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Facture)
                .OrderByDescending(c => c.datecmd)
                .ToListAsync();

            if (!string.IsNullOrEmpty(statut))
            {
                commandes = commandes.Where(c => c.statut == statut).ToList();
            }

            var commandesDTO = commandes.Select(c => new CommandeDTO
            {
                Idcmd = c.Idcmd,
                DateCmd = c.datecmd,
                Statut = c.statut,
                TotalHT = c.totalHT,
                TotalTVA = c.totalTVA,
                TotalTTC = c.totalTTC,
                Idclient = c.Idclient,
                NomClient = c.Client != null ? $"{c.Client.nom} {c.Client.prenom}" : "",
                AFacture = c.Facture != null
            }).ToList();

            return View(commandesDTO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerCommande(int id)
        {
            try
            {
                var commande = await _context.Commandes
                    .Include(c => c.Lignecmds)
                    .FirstOrDefaultAsync(c => c.Idcmd == id);

                if (commande == null)
                {
                    TempData["ErrorMessage"] = "Commande non trouvée";
                    return RedirectToAction(nameof(Commandes));
                }

                // Vérifier si une facture est associée
                var facture = await _context.Factures.FirstOrDefaultAsync(f => f.Idcmd == id);
                if (facture != null)
                {
                    TempData["ErrorMessage"] = "Impossible de supprimer cette commande car une facture y est associée";
                    return RedirectToAction(nameof(Commandes));
                }

                // Supprimer d'abord les lignes de commande
                _context.Lignecmds.RemoveRange(commande.Lignecmds);

                // Puis supprimer la commande
                _context.Commandes.Remove(commande);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Commande #{id} supprimée avec succès";
                return RedirectToAction(nameof(Commandes));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur lors de la suppression: {ex.Message}";
                return RedirectToAction(nameof(Commandes));
            }
        }

        // =========================
        // 🔄 CHANGER STATUT COMMANDE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangerStatutCommande(int idCommande, string nouveauStatut)
        {
            try
            {
                var commande = await _context.Commandes.FindAsync(idCommande);
                if (commande == null)
                {
                    TempData["ErrorMessage"] = "Commande non trouvée";
                    return RedirectToAction("Commandes");
                }

                commande.statut = nouveauStatut;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Statut de la commande #{idCommande} changé à '{nouveauStatut}'";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur: {ex.Message}";
            }

            return RedirectToAction("Commandes");
        }

        // =========================
        // 📄 FACTURES
        // =========================
        [HttpGet]
        public async Task<IActionResult> Factures()
        {
            var factures = await _factureService.GetAllFacturesAsync();
            return View(factures);
        }

        // =========================
        // 🆕 CRÉER FACTURE
        // =========================
      
        [HttpGet]
        public async Task<IActionResult> CreateFacture()
        {
            try
            {
                // Get commandes that don't have a facture yet
                // Include commandes with status: "en cours", "validée", "livrée"
                var commandesEnAttente = await _context.Commandes
                    .Include(c => c.Client)
                    .Where(c => !_context.Factures.Any(f => f.Idcmd == c.Idcmd) &&
                               (c.statut.ToLower() == "en cours" ||
                                c.statut.ToLower() == "validée" ||
                                c.statut.ToLower() == "livrée"))
                    .OrderByDescending(c => c.datecmd)
                    .Select(c => new
                    {
                        Idcmd = c.Idcmd,
                        NomClient = c.Client != null ? $"{c.Client.nom} {c.Client.prenom}" : "Client inconnu",
                        TotalTTC = c.totalTTC ?? 0,
                        Statut = c.statut
                    })
                    .ToListAsync();

                var nextNumero = await _factureService.GetNextNumeroFactureAsync();

                ViewBag.CommandesEnAttente = commandesEnAttente;
                ViewBag.NextNumeroFacture = nextNumero;

                // Create empty model
                var model = new CreateFactureDto();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur lors du chargement: {ex.Message}";
                return RedirectToAction("Dashboard");
            }
        }

        // 📄 CRÉER FACTURE - POST (FIXED)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFacture(CreateFactureDto model)
        {
            if (!ModelState.IsValid)
            {
                // Reload commandes for the view
                var commandesEnAttente = await _context.Commandes
                    .Include(c => c.Client)
                    .Where(c => !_context.Factures.Any(f => f.Idcmd == c.Idcmd) &&
                               (c.statut.ToLower() == "en cours" ||
                                c.statut.ToLower() == "validée" ||
                                c.statut.ToLower() == "livrée"))
                    .OrderByDescending(c => c.datecmd)
                    .Select(c => new
                    {
                        Idcmd = c.Idcmd,
                        NomClient = c.Client != null ? $"{c.Client.nom} {c.Client.prenom}" : "Client inconnu",
                        TotalTTC = c.totalTTC ?? 0,
                        Statut = c.statut
                    })
                    .ToListAsync();

                var nextNumero = await _factureService.GetNextNumeroFactureAsync();

                ViewBag.CommandesEnAttente = commandesEnAttente;
                ViewBag.NextNumeroFacture = nextNumero;

                return View(model);
            }

            var result = await _factureService.CreateFactureAsync(model.Idcmd);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Factures));
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(CreateFacture));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerFacture(int id)
        {
            try
            {
                var facture = await _context.Factures
                    .Include(f => f.Commande)
                    .FirstOrDefaultAsync(f => f.Idfacture == id);

                if (facture == null)
                {
                    TempData["ErrorMessage"] = "Facture non trouvée";
                    return RedirectToAction(nameof(Factures));
                }

                // Récupérer le numéro de facture avant suppression
                var numFacture = facture.numfac;

                // Vérifier si la facture est payée (optionnel - selon votre logique métier)
                if (facture.statut.ToLower() == "payée")
                {
                    TempData["ErrorMessage"] = "Impossible de supprimer une facture déjà payée";
                    return RedirectToAction(nameof(Factures));
                }

                // Réactiver la commande (remettre son statut précédent)
                if (facture.Commande != null)
                {
                    // Mettre la commande en statut "validée" ou "livrée" selon votre logique
                    facture.Commande.statut = "validée";
                }

                // Supprimer la facture
                _context.Factures.Remove(facture);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Facture {numFacture} supprimée avec succès";
                return RedirectToAction(nameof(Factures));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur lors de la suppression: {ex.Message}";
                return RedirectToAction(nameof(Factures));
            }
        }
        // 📄 VIEW FACTURE AS PDF - Generate HTML invoice
        [HttpGet]
        public async Task<IActionResult> ViewFacture(int id)
        {
            var facture = await _factureService.GetFactureByIdAsync(id);

            if (facture == null)
            {
                TempData["ErrorMessage"] = "Facture non trouvée";
                return RedirectToAction(nameof(Factures));
            }

            return View(facture);
        }

        // 🔄 UPDATE STATUT FACTURE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatutFacture(int id, string nouveauStatut)
        {
            try
            {
                var result = await _factureService.UpdateStatutFactureAsync(id, nouveauStatut);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur: {ex.Message}";
            }

            return RedirectToAction(nameof(Factures));
        }

        // =========================
        // 📊 GESTION STOCK
        // =========================
        [HttpGet]
        public async Task<IActionResult> GestionStock()
        {
            var produitsStockFaible = await _produitService.GetProduitsStockFaibleAsync(20);
            return View(produitsStockFaible);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int idProduit, int nouvelleQuantite)
        {
            try
            {
                var produit = await _context.Produits.FindAsync(idProduit);
                if (produit == null)
                {
                    TempData["ErrorMessage"] = "Produit non trouvé";
                    return RedirectToAction("GestionStock");
                }

                produit.qtestock = nouvelleQuantite;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Stock de '{produit.design}' mis à jour à {nouvelleQuantite} unités";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur: {ex.Message}";
            }

            return RedirectToAction("GestionStock");
        }

        // =========================
        // 🚪 LOGOUT
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            HttpContext.Session.Clear();
            Response.Cookies.Delete("OnlineShop.Auth");

            TempData["SuccessMessage"] = "Déconnexion réussie.";
            return RedirectToAction("Index", "Home");
        }

        // GET version of logout for easier testing
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            HttpContext.Session.Clear();
            Response.Cookies.Delete("OnlineShop.Auth");

            TempData["SuccessMessage"] = "Déconnexion réussie.";
            return RedirectToAction("Index", "Home");
        }
    }
}