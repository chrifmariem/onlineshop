using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.DTOs;
using onlineShop.Models;
using onlineShop.Respositories;

namespace onlineShop.Services
{
    public class FactureService : IFactureService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFactureRepository _factureRepository;

        public FactureService(ApplicationDbContext context, IFactureRepository factureRepository)
        {
            _context = context;
            _factureRepository = factureRepository;
        }

        // 🔎 Récupérer une facture par ID avec tous les détails
        public async Task<FactureDTO> GetFactureByIdAsync(int id)
        {
            var facture = await _factureRepository.GetFactureWithDetailsAsync(id);
            if (facture == null) return null;

            return new FactureDTO
            {
                Idfacture = facture.Idfacture,
                NumFac = facture.numfac,
                DateFac = facture.dateFac,
                TotalHT = facture.totalHT,
                TotalTVA = facture.totalTVA,
                TotalTTC = facture.totalTTC,
                Statut = facture.statut,
                Idcmd = facture.Idcmd,
                Commande = facture.Commande != null ? new CommandeDTO
                {
                    Idcmd = facture.Commande.Idcmd,
                    DateCmd = facture.Commande.datecmd,
                    Statut = facture.Commande.statut,
                    TotalHT = facture.Commande.totalHT,
                    TotalTVA = facture.Commande.totalTVA,
                    TotalTTC = facture.Commande.totalTTC,
                    Idclient = facture.Commande.Idclient,
                    NomClient = facture.Commande.Client != null
                        ? $"{facture.Commande.Client.nom} {facture.Commande.Client.prenom}"
                        : "",
                    Lignes = facture.Commande.Lignecmds?.Select(l => new LignecmdDTO
                    {
                        Idligne = l.Idligne,
                        Idproduit = l.Idproduit,
                        NomProduit = l.Produit?.design ?? "",
                        ImageProduit = l.Produit?.image ?? "",
                        Qte = l.qte,
                        PrixUni = l.prixuni,
                        Tva = l.tva,
                        PrixTotal = l.prixtotal
                    }).ToList() ?? new List<LignecmdDTO>()
                } : null
            };
        }

        // 🔎 Récupérer la facture d'une commande
        public async Task<FactureDTO> GetFactureByCommandeAsync(int idCommande)
        {
            var facture = await _factureRepository.GetFactureByCommandeAsync(idCommande);
            if (facture == null) return null;

            return new FactureDTO
            {
                Idfacture = facture.Idfacture,
                NumFac = facture.numfac,
                DateFac = facture.dateFac,
                TotalHT = facture.totalHT,
                TotalTVA = facture.totalTVA,
                TotalTTC = facture.totalTTC,
                Statut = facture.statut,
                Idcmd = facture.Idcmd
            };
        }

        // 📋 Liste de toutes les factures
        public async Task<IEnumerable<FactureDTO>> GetAllFacturesAsync()
        {
            var factures = await _context.Factures
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Client)
                .OrderByDescending(f => f.dateFac)
                .ToListAsync();

            return factures.Select(f => new FactureDTO
            {
                Idfacture = f.Idfacture,
                NumFac = f.numfac,
                DateFac = f.dateFac,
                TotalHT = f.totalHT,
                TotalTVA = f.totalTVA,
                TotalTTC = f.totalTTC,
                Statut = f.statut,
                Idcmd = f.Idcmd,
                Commande = f.Commande != null ? new CommandeDTO
                {
                    Idcmd = f.Commande.Idcmd,
                    NomClient = f.Commande.Client != null
                        ? $"{f.Commande.Client.nom} {f.Commande.Client.prenom}"
                        : ""
                } : null
            });
        }

        // 📌 Factures par statut
        public async Task<IEnumerable<FactureDTO>> GetFacturesByStatutAsync(string statut)
        {
            var factures = await _factureRepository.GetFacturesByStatutAsync(statut);
            return factures.Select(f => new FactureDTO
            {
                Idfacture = f.Idfacture,
                NumFac = f.numfac,
                DateFac = f.dateFac,
                TotalHT = f.totalHT,
                TotalTVA = f.totalTVA,
                TotalTTC = f.totalTTC,
                Statut = f.statut,
                Idcmd = f.Idcmd
            });
        }

        // ➕ CRÉER une facture pour une commande
        public async Task<OperationResultDto> CreateFactureAsync(int idCommande)
        {
            try
            {
                // ✅ Vérifier que la commande existe
                var commande = await _context.Commandes
                    .Include(c => c.Client)
                    .Include(c => c.Lignecmds)
                    .ThenInclude(l => l.Produit)
                    .FirstOrDefaultAsync(c => c.Idcmd == idCommande);

                if (commande == null)
                {
                    return OperationResultDto.ErrorResult("Commande non trouvée");
                }

                // ✅ Vérifier qu'une facture n'existe pas déjà
                var factureExistante = await _context.Factures
                    .FirstOrDefaultAsync(f => f.Idcmd == idCommande);

                if (factureExistante != null)
                {
                    return OperationResultDto.ErrorResult(
                        "Une facture existe déjà pour cette commande",
                        new List<string> { $"Numéro de facture: {factureExistante.numfac}" }
                    );
                }

                // ✅ Vérifier que la commande a un statut valide pour facturation
                var statutsValides = new[] { "en cours", "validée", "livrée" };
                if (!statutsValides.Contains(commande.statut.ToLower()))
                {
                    return OperationResultDto.ErrorResult(
                        "Impossible de créer une facture pour cette commande",
                        new List<string> { "La commande doit être 'En cours', 'Validée' ou 'Livrée'" }
                    );
                }

                // 🔢 Générer le numéro de facture
                var dernierNumero = await _context.Factures
                    .OrderByDescending(f => f.Idfacture)
                    .Select(f => f.numfac)
                    .FirstOrDefaultAsync();

                var numeroFacture = "FACT-" + DateTime.Now.ToString("yyyyMMdd") + "-" +
                    ((string.IsNullOrEmpty(dernierNumero) ? 0 :
                    int.Parse(dernierNumero.Split('-').Last())) + 1).ToString("0000");

                // 📄 Créer la facture
                var facture = new Facture
                {
                    Idcmd = idCommande,
                    numfac = numeroFacture,
                    dateFac = DateTime.Now,
                    totalHT = commande.totalHT ?? 0,
                    totalTVA = commande.totalTVA ?? 0,
                    totalTTC = commande.totalTTC ?? 0,
                    statut = "Non payée"
                };

                _context.Factures.Add(facture);
                await _context.SaveChangesAsync();

                // Mettre à jour le statut de la commande
                commande.statut = "facturée";
                await _context.SaveChangesAsync();

                var factureDto = await GetFactureByIdAsync(facture.Idfacture);
                return OperationResultDto.SuccessResult(
                    $"Facture {numeroFacture} créée avec succès",
                    factureDto
                );
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors de la création de la facture",
                    new List<string> { ex.Message }
                );
            }
        }

        // ➕ CRÉER une facture à partir d'un DTO
        public async Task<OperationResultDto> CreateFactureAsync(CreateFactureDto dto)
        {
            return await CreateFactureAsync(dto.Idcmd);
        }

        // 🔄 Mettre à jour le statut d'une facture
        public async Task<OperationResultDto> UpdateStatutFactureAsync(int id, string nouveauStatut)
        {
            try
            {
                var facture = await _context.Factures.FindAsync(id);
                if (facture == null)
                {
                    return OperationResultDto.ErrorResult("Facture non trouvée");
                }

                // ✅ Valider le statut
                var statutsValides = new[] { "Non payée", "Payée", "Annulée", "En attente" };
                if (!statutsValides.Contains(nouveauStatut))
                {
                    return OperationResultDto.ErrorResult(
                        "Statut invalide",
                        new List<string> { $"Statuts valides: {string.Join(", ", statutsValides)}" }
                    );
                }

                facture.statut = nouveauStatut;
                await _context.SaveChangesAsync();

                return OperationResultDto.SuccessResult("Statut de la facture mis à jour");
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors de la mise à jour du statut",
                    new List<string> { ex.Message }
                );
            }
        }

        // 🔢 Obtenir le prochain numéro de facture
        public async Task<string> GetNextNumeroFactureAsync()
        {
            var dernierNumero = await _context.Factures
                .OrderByDescending(f => f.Idfacture)
                .Select(f => f.numfac)
                .FirstOrDefaultAsync();

            return "FACT-" + DateTime.Now.ToString("yyyyMMdd") + "-" +
                ((string.IsNullOrEmpty(dernierNumero) ? 0 :
                int.Parse(dernierNumero.Split('-').Last())) + 1).ToString("0000");
        }
    }
}