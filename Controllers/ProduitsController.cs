using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Data;
using onlineShop.DTOs;
using onlineShop.Services;

namespace onlineShop.Controllers
{
    public class ProduitsController : Controller
    {
        private readonly IProduitService _produitService;
        private readonly IPanierService _panierService;
        private readonly IProductAssistantService _assistantService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProduitsController> _logger;

        public ProduitsController(
            IProduitService produitService,
            IPanierService panierService,
            IProductAssistantService assistantService,
            ApplicationDbContext context,
            ILogger<ProduitsController> logger)
        {
            _produitService = produitService;
            _panierService = panierService;
            _assistantService = assistantService;
            _context = context;
            _logger = logger;
        }

        // GET: Liste des produits with pagination
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1)
        {
            const int pageSize = 8;

            IEnumerable<ProduitDTO> allProduits;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allProduits = await _produitService.SearchProduitsAsync(searchTerm);
            }
            else
            {
                allProduits = await _produitService.GetProduitsDisponiblesAsync();
            }

            var totalProduits = allProduits.Count();
            var totalPages = (int)Math.Ceiling(totalProduits / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var produits = allProduits
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new ProduitListDto
            {
                Produits = produits,
                PageActuelle = page,
                TotalPages = totalPages,
                TotalProduits = totalProduits,
                Recherche = searchTerm
            };

            ViewBag.SearchTerm = searchTerm;
            return View(model);
        }

        // GET: Détails d'un produit
        public async Task<IActionResult> Details(int id)
        {
            var produit = await _produitService.GetProduitByIdAsync(id);

            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // POST: API endpoint pour le chatbot IA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskAssistant([FromBody] ChatRequest request)
        {
            try
            {
                _logger.LogInformation($"AskAssistant called for product {request.ProductId}");

                if (string.IsNullOrWhiteSpace(request.Question) || request.Question.Length > 500)
                {
                    _logger.LogWarning("Question is empty or too long");
                    return BadRequest(new { error = "La question est invalide" });
                }

                if (request.ProductId <= 0)
                {
                    _logger.LogWarning($"Invalid product ID: {request.ProductId}");
                    return BadRequest(new { error = "ID produit invalide" });
                }

                // Récupérer le produit complet depuis la base de données
                var produit = await _context.Produits.FindAsync(request.ProductId);

                if (produit == null)
                {
                    _logger.LogWarning($"Product {request.ProductId} not found");
                    return NotFound(new { error = "Produit introuvable" });
                }

                _logger.LogInformation($"Product found: {produit.design}. Generating response...");

                // Obtenir la réponse de l'assistant IA
                string response = await _assistantService.GetProductGuidanceAsync(produit, request.Question);

                _logger.LogInformation($"Response generated successfully");

                return Ok(new ChatResponse
                {
                    Answer = response,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in AskAssistant for product {request?.ProductId}");

                return StatusCode(500, new ChatResponse
                {
                    Answer = "Désolé, je rencontre des difficultés techniques. Veuillez réessayer dans quelques instants.",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // POST: Ajouter au panier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterAuPanier(int id, int quantite = 1)
        {
            if (quantite <= 0)
            {
                TempData["ErrorMessage"] = "Quantité invalide";
                return RedirectToAction(nameof(Details), new { id });
            }

            var produit = await _produitService.GetProduitByIdAsync(id);

            if (produit == null)
            {
                TempData["ErrorMessage"] = "Produit non trouvé";
                return RedirectToAction(nameof(Index));
            }

            if (produit.QteStock < quantite)
            {
                TempData["ErrorMessage"] = "Stock insuffisant";
                return RedirectToAction(nameof(Details), new { id });
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

            _panierService.AjouterAuPanier(panierItem);

            TempData["SuccessMessage"] = "Produit ajouté au panier !";
            return RedirectToAction(nameof(Index));
        }

        // GET: Produits en promotion
        public async Task<IActionResult> Promotions()
        {
            var produits = await _produitService.GetProduitsAvecPromoAsync();
            return View("Index", produits);
        }

        // GET: Gestion du Stock (Admin uniquement) - NOUVELLE MÉTHODE
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GestionStock()
        {
            try
            {
                // Appeler la méthode qui retourne TOUS les produits
                var produits = await _produitService.GetAllProduitsAsync();

                return View(produits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GestionStock");
                TempData["ErrorMessage"] = "Erreur lors du chargement des produits";
                return View(new List<ProduitDTO>());
            }
        }

        // POST: Mettre à jour le stock (Admin uniquement) - NOUVELLE MÉTHODE
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int idProduit, int nouvelleQuantite)
        {
            try
            {
                if (nouvelleQuantite < 0)
                {
                    TempData["ErrorMessage"] = "La quantité ne peut pas être négative";
                    return RedirectToAction(nameof(GestionStock));
                }

                var result = await _produitService.UpdateStockAsync(idProduit, nouvelleQuantite);

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
                TempData["ErrorMessage"] = "Erreur lors de la mise à jour du stock";
                _logger.LogError(ex, $"Erreur UpdateStock pour produit {idProduit}");
            }

            return RedirectToAction(nameof(GestionStock));
        }

        // GET: Créer un produit (Admin uniquement)
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Créer un produit (Admin uniquement)
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProduitDto dto)
        {
            try
            {
                Console.WriteLine("=== CREATE PRODUIT START ===");

                if (!dto.TryParseVenteHT(out string priceError))
                {
                    ModelState.AddModelError("VenteHTString", priceError);
                }

                var (isValid, errors) = dto.Validate();

                if (!isValid)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(dto);
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid");
                    return View(dto);
                }

                var result = await _produitService.CreateProduitAsync(dto);

                if (!result.Success)
                {
                    foreach (var error in result.Errors ?? new List<string>())
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(dto);
                }

                TempData["SuccessMessage"] = "Produit créé avec succès !";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== EXCEPTION IN CREATE ===");
                Console.WriteLine($"Exception: {ex.Message}");
                TempData["ErrorMessage"] = $"Erreur: {ex.Message}";
                return View(dto);
            }
        }

        // GET: Modifier un produit (Admin uniquement)
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var produit = await _produitService.GetProduitByIdAsync(id);

            if (produit == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateProduitDto
            {
                Aref = produit.Aref,
                Design = produit.Design,
                VenteHT = produit.VenteHT,
                Tva = produit.Tva,
                QteStock = produit.QteStock,
                PromoWeb = produit.PromoWeb,
                Labo = produit.Labo,
                Image = produit.Image
            };

            ViewBag.ProduitId = id;
            return View(updateDto);
        }

        // POST: Modifier un produit (Admin uniquement)
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProduitDto dto, string VenteHTString, string TvaString)
        {
            try
            {
                ModelState.Remove("VenteHT");
                ModelState.Remove("Tva");

                if (!string.IsNullOrEmpty(VenteHTString))
                {
                    string cleanVenteHT = VenteHTString.Trim().Replace(',', '.');
                    if (decimal.TryParse(cleanVenteHT,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out decimal parsedVenteHT))
                    {
                        dto.VenteHT = parsedVenteHT;
                    }
                    else
                    {
                        ModelState.AddModelError("VenteHT", "Format de prix HT invalide");
                    }
                }

                if (!string.IsNullOrEmpty(TvaString))
                {
                    string cleanTva = TvaString.Trim().Replace(',', '.');
                    if (decimal.TryParse(cleanTva,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out decimal parsedTva))
                    {
                        dto.Tva = parsedTva;
                    }
                    else
                    {
                        ModelState.AddModelError("Tva", "Format de TVA invalide");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var result = await _produitService.UpdateProduitAsync(id, dto);

                if (!result.Success)
                {
                    foreach (var error in result.Errors ?? new List<string>())
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(dto);
                }

                TempData["SuccessMessage"] = "Produit modifié avec succès !";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur: {ex.Message}";
                return View(dto);
            }
        }

        // POST: Supprimer un produit (Admin uniquement)
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _produitService.DeleteProduitAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Produit supprimé avec succès !";
            }

            return RedirectToAction(nameof(Index));
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashBoardDto
            {
                ProduitsStockFaible = (await _produitService.GetProduitsStockFaibleAsync(20)).ToList()
            };
            return View(model);
        }

        // Diagnostic pour debug
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DiagnoseStock()
        {
            try
            {
                var result = await _produitService.GetAllProduitsAsync();
                return Content($@"
                    Type: {result.GetType().FullName}
                    Count: {result?.Count ?? 0}
                    Is List<ProduitDTO>: {result is List<ProduitDTO>}
                    Is IEnumerable<ProduitDTO>: {result is IEnumerable<ProduitDTO>}
                ");
            }
            catch (Exception ex)
            {
                return Content($"ERROR: {ex.Message}\n\n{ex.StackTrace}");
            }
        }
    }

    public class ChatRequest
    {
        public int ProductId { get; set; }
        public string Question { get; set; }
    }

    public class ChatResponse
    {
        public string Answer { get; set; }
        public DateTime Timestamp { get; set; }
    }
}