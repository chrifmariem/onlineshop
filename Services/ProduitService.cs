using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.DTOs;
using onlineShop.Helpers;
using onlineShop.Models;
using onlineShop.Respositories;
using System.Collections.Generic;

namespace onlineShop.Services
{
    public class ProduitService : IProduitService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProduitRepository _produitRepository;

        public ProduitService(ApplicationDbContext context, IProduitRepository produitRepository)
        {
            _context = context;
            _produitRepository = produitRepository;
        }

        // 🔎 Récupérer un produit par ID
        public async Task<ProduitDTO> GetProduitByIdAsync(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            return produit?.ToDto();
        }

        // 🔎 Récupérer un produit par code-barres
        public async Task<ProduitDTO> GetProduitByCodeBarAsync(string codeBar)
        {
            var produit = await _produitRepository.GetByCodeBarAsync(codeBar);
            return produit?.ToDto();
        }

        // 📋 Liste de tous les produits - CORRIGÉ : Retourne List<ProduitDTO>
        public async Task<List<ProduitDTO>> GetAllProduitsAsync()
        {
            var produits = await _context.Produits
                .OrderBy(p => p.design)
                .ToListAsync();

            // Convertir en List<ProduitDTO> avec ToList()
            return produits.Select(p => p.ToDto()).ToList();
        }

        // ✅ Produits disponibles (en stock)
        public async Task<IEnumerable<ProduitDTO>> GetProduitsDisponiblesAsync()
        {
            var produits = await _produitRepository.GetProduitsDisponiblesAsync();
            return produits.Select(p => p.ToDto());
        }

        // 🎁 Produits avec promotion
        public async Task<IEnumerable<ProduitDTO>> GetProduitsAvecPromoAsync()
        {
            var produits = await _produitRepository.GetProduitsAvecPromoAsync();
            return produits.Select(p => p.ToDto());
        }

        // ⚠️ Produits à stock faible
        public async Task<IEnumerable<ProduitDTO>> GetProduitsStockFaibleAsync(int seuilStock = 10)
        {
            var produits = await _produitRepository.GetProduitsStockFaibleAsync(seuilStock);
            return produits.Select(p => p.ToDto());
        }

        // 🏭 Produits par laboratoire
        public async Task<IEnumerable<ProduitDTO>> GetProduitsByLaboAsync(string labo)
        {
            if (string.IsNullOrWhiteSpace(labo))
                return Enumerable.Empty<ProduitDTO>();

            var produits = await _produitRepository.GetProduitsByLaboAsync(labo);
            return produits.Select(p => p.ToDto());
        }

        // 🔍 Recherche de produits
        public async Task<IEnumerable<ProduitDTO>> SearchProduitsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllProduitsAsync();

            var produits = await _produitRepository.SearchProduitsAsync(searchTerm);
            return produits.Select(p => p.ToDto());
        }

        // 📄 Pagination
        public async Task<IEnumerable<ProduitDTO>> GetProduitsPageAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var produits = await _produitRepository.GetProduitsPageAsync(pageNumber, pageSize);
            return produits.Select(p => p.ToDto());
        }

        // 📊 Nombre total de produits
        public async Task<int> GetTotalProduitsCountAsync()
        {
            return await _produitRepository.GetTotalProduitsCountAsync();
        }

        // ➕ CRÉER un nouveau produit
        public async Task<OperationResultDto> CreateProduitAsync(CreateProduitDto dto)
        {
            try
            {
                Console.WriteLine("=== SERVICE: CreateProduitAsync START ===");

                var produit = dto.ToEntity();
                Console.WriteLine($"Created entity - VenteHT: {produit.venteHT}, TTC: {produit.ttc}");

                _context.Produits.Add(produit);
                int rowsAffected = await _context.SaveChangesAsync();

                Console.WriteLine($"SaveChangesAsync completed. Rows affected: {rowsAffected}");

                if (rowsAffected > 0)
                {
                    Console.WriteLine("=== SERVICE: CreateProduitAsync SUCCESS ===");
                    return OperationResultDto.SuccessResult("Produit créé avec succès");
                }
                else
                {
                    Console.WriteLine("=== SERVICE: No rows affected ===");
                    return OperationResultDto.ErrorResult("Aucune donnée n'a été sauvegardée");
                }
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"=== DB UPDATE EXCEPTION ===");
                Console.WriteLine($"Message: {dbEx.Message}");

                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");

                    if (dbEx.InnerException.Message.Contains("aref") ||
                        dbEx.InnerException.Message.Contains("duplicate"))
                    {
                        return OperationResultDto.ErrorResult(
                            "Cette référence existe déjà",
                            new List<string> { "La référence doit être unique" }
                        );
                    }

                    if (dbEx.InnerException.Message.Contains("codebar"))
                    {
                        return OperationResultDto.ErrorResult(
                            "Ce code-barres existe déjà",
                            new List<string> { "Le code-barres doit être unique" }
                        );
                    }
                }

                return OperationResultDto.ErrorResult(
                    "Erreur de base de données",
                    new List<string> { dbEx.Message }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== GENERAL EXCEPTION ===");
                Console.WriteLine($"Message: {ex.Message}");

                return OperationResultDto.ErrorResult(
                    "Erreur inattendue",
                    new List<string> { ex.Message }
                );
            }
        }

        // ✏️ METTRE À JOUR un produit
        public async Task<OperationResultDto> UpdateProduitAsync(int id, UpdateProduitDto dto)
        {
            try
            {
                Console.WriteLine($"=== SERVICE: UpdateProduitAsync START ===");
                Console.WriteLine($"Updating produit ID: {id}");

                var produit = await _context.Produits.FindAsync(id);
                if (produit == null)
                {
                    Console.WriteLine($"Produit with ID {id} not found");
                    return OperationResultDto.ErrorResult("Produit non trouvé");
                }

                // Validation
                var (isValid, errors) = dto.Validate();
                if (!isValid)
                {
                    Console.WriteLine($"Validation failed: {string.Join(", ", errors)}");
                    return OperationResultDto.ErrorResult("Validation échouée", errors);
                }

                // Calculer le TTC
                var ttc = dto.VenteHT + (dto.VenteHT * dto.Tva / 100);

                // Mettre à jour les propriétés
                produit.aref = dto.Aref?.Trim();
                produit.design = dto.Design?.Trim();
                produit.venteHT = dto.VenteHT;
                produit.tva = dto.Tva;
                produit.ttc = ttc;
                produit.qtestock = dto.QteStock;
                produit.promoweb = dto.PromoWeb;
                produit.labo = dto.Labo;
                produit.image = string.IsNullOrEmpty(dto.Image) ? produit.image : dto.Image;
                produit.dernmiseajour = DateTime.Now;
                produit.tauxtvav = dto.Tva;

                _context.Produits.Update(produit);
                var rowsAffected = await _context.SaveChangesAsync();

                Console.WriteLine($"SaveChanges completed. Rows affected: {rowsAffected}");

                if (rowsAffected == 0)
                {
                    Console.WriteLine("WARNING: No rows were affected by SaveChanges!");
                    return OperationResultDto.ErrorResult("Aucune modification n'a été enregistrée");
                }

                Console.WriteLine($"=== SERVICE: UpdateProduitAsync SUCCESS ===");

                var produitDto = produit.ToDto();
                return OperationResultDto.SuccessResult(
                    "Produit mis à jour avec succès",
                    produitDto
                );
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"=== DB UPDATE EXCEPTION in UpdateProduitAsync ===");
                Console.WriteLine($"Message: {dbEx.Message}");

                if (dbEx.InnerException?.Message.Contains("aref") == true ||
                    dbEx.InnerException?.Message.Contains("duplicate") == true)
                {
                    return OperationResultDto.ErrorResult(
                        "Cette référence existe déjà",
                        new List<string> { "La référence doit être unique" }
                    );
                }

                return OperationResultDto.ErrorResult(
                    "Erreur de base de données lors de la mise à jour",
                    new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== GENERAL EXCEPTION in UpdateProduitAsync ===");
                Console.WriteLine($"Message: {ex.Message}");

                return OperationResultDto.ErrorResult(
                    "Erreur inattendue lors de la mise à jour",
                    new List<string> { ex.Message }
                );
            }
        }

        // 🗑️ SUPPRIMER un produit
        public async Task<OperationResultDto> DeleteProduitAsync(int id)
        {
            try
            {
                var produit = await _context.Produits.FindAsync(id);
                if (produit == null)
                {
                    return OperationResultDto.ErrorResult("Produit non trouvé");
                }

                var hasCommandes = await _context.Lignecmds
                    .AnyAsync(l => l.Idproduit == id);

                if (hasCommandes)
                {
                    return OperationResultDto.ErrorResult(
                        "Impossible de supprimer ce produit",
                        new List<string> { "Ce produit est référencé dans des commandes existantes" }
                    );
                }

                _context.Produits.Remove(produit);
                await _context.SaveChangesAsync();

                return OperationResultDto.SuccessResult("Produit supprimé avec succès");
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors de la suppression du produit",
                    new List<string> { ex.Message }
                );
            }
        }

        // 📦 Mettre à jour le stock
        public async Task<OperationResultDto> UpdateStockAsync(int idProduit, int nouvelleQuantite)
        {
            try
            {
                if (nouvelleQuantite < 0)
                {
                    return OperationResultDto.ErrorResult(
                        "Quantité invalide",
                        new List<string> { "La quantité ne peut pas être négative" }
                    );
                }

                var produit = await _context.Produits.FindAsync(idProduit);
                if (produit == null)
                {
                    return OperationResultDto.ErrorResult("Produit non trouvé");
                }

                await _produitRepository.UpdateStockAsync(idProduit, nouvelleQuantite);

                return OperationResultDto.SuccessResult(
                    "Stock mis à jour avec succès",
                    new { AncienStock = produit.qtestock, NouveauStock = nouvelleQuantite }
                );
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors de la mise à jour du stock",
                    new List<string> { ex.Message }
                );
            }
        }

        // ➕➖ Ajuster le stock
        public async Task<OperationResultDto> AjusterStockAsync(int idProduit, int quantite, bool isAddition)
        {
            try
            {
                var produit = await _context.Produits.FindAsync(idProduit);
                if (produit == null)
                {
                    return OperationResultDto.ErrorResult("Produit non trouvé");
                }

                int nouvelleQuantite;
                if (isAddition)
                {
                    nouvelleQuantite = produit.qtestock + quantite;
                }
                else
                {
                    nouvelleQuantite = produit.qtestock - quantite;

                    if (nouvelleQuantite < 0)
                    {
                        return OperationResultDto.ErrorResult(
                            "Stock insuffisant",
                            new List<string> { $"Stock actuel: {produit.qtestock}, Demandé: {quantite}" }
                        );
                    }
                }

                await _produitRepository.UpdateStockAsync(idProduit, nouvelleQuantite);

                return OperationResultDto.SuccessResult(
                    "Stock ajusté avec succès",
                    new { AncienStock = produit.qtestock, NouveauStock = nouvelleQuantite }
                );
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors de l'ajustement du stock",
                    new List<string> { ex.Message }
                );
            }
        }

        // ✅ Vérifier si un code-barres existe
        public async Task<bool> CodeBarExistsAsync(string codeBar)
        {
            if (string.IsNullOrWhiteSpace(codeBar))
                return false;

            return await _produitRepository.CodeBarExistsAsync(codeBar);
        }

        // ✅ Vérifier si un produit existe
        public async Task<bool> ProduitExistsAsync(int id)
        {
            return await _context.Produits.AnyAsync(p => p.Idproduit == id);
        }
    }
}