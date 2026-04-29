using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Data;
using onlineShop.DTOs;
using onlineShop.Models;
using onlineShop.Services;
using System.Security.Claims;

namespace onlineShop.Controllers
{
    [Authorize]
    public class CommandeController : Controller
    {
        private readonly IPanierService _panierService;
        private readonly ICommandeService _commandeService;
        private readonly IClientService _clientService;
        private readonly IProduitService _produitService;
        private readonly ApplicationDbContext _context;

        public CommandeController(
            IPanierService panierService,
            ICommandeService commandeService,
            IClientService clientService,
            IProduitService produitService,
            ApplicationDbContext context)
        {
            _panierService = panierService;
            _commandeService = commandeService;
            _clientService = clientService;
            _produitService = produitService;
            _context = context;
        }

        private int GetCurrentClientId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        // GET: Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var panier = _panierService.GetPanier();

            if (panier.EstVide)
            {
                TempData["ErrorMessage"] = "Votre panier est vide";
                return RedirectToAction("Index", "Panier");
            }

            var clientId = GetCurrentClientId();
            var client = await _clientService.GetClientByIdAsync(clientId);

            ViewBag.Panier = panier;
            ViewBag.Client = client;

            return View();
        }

        // POST: Créer la commande
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nomComplet, string telephone, string adresse, string notes)
        {
            try
            {
                var clientId = GetCurrentClientId();
                var panier = _panierService.GetPanier();

                if (panier.EstVide)
                {
                    TempData["ErrorMessage"] = "Votre panier est vide";
                    return RedirectToAction("Index", "Panier");
                }

                // Vérifier les stocks
                foreach (var item in panier.Items)
                {
                    var produit = await _produitService.GetProduitByIdAsync(item.ProduitId);
                    if (produit == null || produit.QteStock < item.Quantite)
                    {
                        TempData["ErrorMessage"] = $"Stock insuffisant pour {item.NomProduit}";
                        return RedirectToAction("Index", "Panier");
                    }
                }

                // Créer la commande
                var commande = new Commande
                {
                    Idclient = clientId,
                    datecmd = DateTime.Now,
                    statut = "en cours",
                    totalHT = panier.TotalHT,
                    totalTVA = panier.TotalTVA,
                    totalTTC = panier.TotalTTC
                };

                _context.Commandes.Add(commande);
                await _context.SaveChangesAsync();

                // Créer les lignes de commande
                foreach (var item in panier.Items)
                {
                    var produit = await _produitService.GetProduitByIdAsync(item.ProduitId);

                    var ligne = new Lignecmd
                    {
                        Idcmd = commande.Idcmd,
                        Idproduit = item.ProduitId,
                        qte = item.Quantite,
                        prixuni = item.Prix,
                        tva = item.Tva,
                        prixtotal = item.SousTotal
                    };

                    _context.Lignecmds.Add(ligne);

                    // Mettre à jour le stock
                    await _produitService.AjusterStockAsync(item.ProduitId, item.Quantite, false);
                }

                await _context.SaveChangesAsync();

                // Vider le panier
                _panierService.ViderPanier();

                TempData["SuccessMessage"] = "Commande créée avec succès !";
                return RedirectToAction(nameof(Confirmation), new { id = commande.Idcmd });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur: {ex.Message}";
                return RedirectToAction(nameof(Checkout));
            }
        }

        // GET: Confirmation
        public async Task<IActionResult> Confirmation(int id)
        {
            var commande = await _commandeService.GetCommandeWithDetailsAsync(id);

            if (commande == null)
            {
                return NotFound();
            }

            var commandeDto = new CommandeDTO
            {
                Idcmd = commande.Idcmd,
                DateCmd = commande.datecmd,
                Statut = commande.statut,
                TotalHT = commande.totalHT,
                TotalTVA = commande.totalTVA,
                TotalTTC = commande.totalTTC,
                Lignes = commande.Lignecmds?.Select(l => new LignecmdDTO
                {
                    Idligne = l.Idligne,
                    NomProduit = l.Produit?.design ?? "",
                    Qte = l.qte,
                    PrixUni = l.prixuni,
                    PrixTotal = l.prixtotal
                }).ToList()
            };

            return View(commandeDto);
        }

        // GET: Détails commande
        public async Task<IActionResult> Details(int id)
        {
            var commande = await _commandeService.GetCommandeWithDetailsAsync(id);

            if (commande == null)
            {
                return NotFound();
            }

            // Vérifier que c'est bien la commande du client connecté
            var clientId = GetCurrentClientId();
            if (commande.Idclient != clientId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            var commandeDto = new CommandeDTO
            {
                Idcmd = commande.Idcmd,
                DateCmd = commande.datecmd,
                Statut = commande.statut,
                TotalHT = commande.totalHT,
                TotalTVA = commande.totalTVA,
                TotalTTC = commande.totalTTC,
                Lignes = commande.Lignecmds?.Select(l => new LignecmdDTO
                {
                    Idligne = l.Idligne,
                    NomProduit = l.Produit?.design ?? "",
                    ImageProduit = l.Produit?.image ?? "",
                    Qte = l.qte,




                    PrixUni = l.prixuni,
                    Tva = l.tva,
                    PrixTotal = l.prixtotal
                }).ToList()
            };

            return View(commandeDto);
        }
    }
}