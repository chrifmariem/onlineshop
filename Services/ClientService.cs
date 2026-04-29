using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using onlineShop.Data;
using onlineShop.DTOs;
using onlineShop.Models;
using onlineShop.Respositories;

namespace onlineShop.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IClientRepository _clientRepository;
        private readonly IOptions<AppSettings> _appSettings;

        public ClientService(
            ApplicationDbContext context,
            IClientRepository clientRepository,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _clientRepository = clientRepository;
            _appSettings = appSettings;
        }

        // =========================
        // 📝 REGISTER
        // =========================
        public async Task<OperationResultDto> RegisterAsync(ClientRegisterDto dto)
        {
            try
            {
                if (await _clientRepository.EmailExistsAsync(dto.Email))
                {
                    return OperationResultDto.ErrorResult(
                        "Cet email est déjà utilisé",
                        new List<string> { "L'adresse email existe déjà" }
                    );
                }

                // Admin secret check
                if (dto.Role == "admin")
                {
                    if (string.IsNullOrEmpty(dto.AdminSecretCode) ||
                        dto.AdminSecretCode != _appSettings.Value.AdminSecretCode)
                    {
                        return OperationResultDto.ErrorResult(
                            "Code admin invalide",
                            new List<string> { "Création admin non autorisée" }
                        );
                    }
                }

                var client = new Client
                {
                    nom = dto.Nom,
                    prenom = dto.Prenom,
                    email = dto.Email,
                    motpasse = BCrypt.Net.BCrypt.HashPassword(dto.MotPasse),
                    telephone = dto.Telephone,
                    adresse = dto.Adresse,
                    dateinscrip = DateTime.Now,
                    role = dto.Role ?? "client"
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                return OperationResultDto.SuccessResult(
                    "Inscription réussie",
                    MapClientToDto(client)
                );
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors de l'inscription",
                    new List<string> { ex.Message }
                );
            }
        }

        // =========================
        // 🔐 LOGIN - NO ROLE VERIFICATION
        // =========================
        public async Task<OperationResultDto> LoginAsync(ClientLoginDto dto)
        {
            try
            {
                var client = await _clientRepository.GetByEmailAsync(dto.Email);

                if (client == null)
                {
                    return OperationResultDto.ErrorResult(
                        "Email ou mot de passe incorrect",
                        new List<string> { "Identifiants invalides" }
                    );
                }

                if (!BCrypt.Net.BCrypt.Verify(dto.MotPasse, client.motpasse))
                {
                    return OperationResultDto.ErrorResult(
                        "Email ou mot de passe incorrect",
                        new List<string> { "Identifiants invalides" }
                    );
                }

                // NO ROLE VERIFICATION - System uses role from database automatically
                // The user's actual role from the database will be returned

                return OperationResultDto.SuccessResult(
                    "Connexion réussie",
                    MapClientToDto(client)
                );
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors de la connexion",
                    new List<string> { ex.Message }
                );
            }
        }

        // =========================
        // 👤 GET CLIENT
        // =========================
        public async Task<ClientDTO?> GetClientByIdAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            return client == null ? null : MapClientToDto(client);
        }

        public async Task<ClientDTO?> GetClientByEmailAsync(string email)
        {
            var client = await _clientRepository.GetByEmailAsync(email);
            return client == null ? null : MapClientToDto(client);
        }

        public async Task<IEnumerable<ClientDTO>> GetAllClientsAsync()
        {
            var clients = await _context.Clients
                .OrderByDescending(c => c.dateinscrip)
                .ToListAsync();

            return clients.Select(MapClientToDto);
        }

        // =========================
        // ✏️ UPDATE PROFILE
        // =========================
        public async Task<OperationResultDto> UpdateClientAsync(int id, UpdateClientDto dto)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return OperationResultDto.ErrorResult(
                        "Client introuvable",
                        new List<string> { "Client inexistant" }
                    );
                }

                client.nom = dto.Nom;
                client.prenom = dto.Prenom;
                client.telephone = dto.Telephone;
                client.adresse = dto.Adresse;

                await _context.SaveChangesAsync();

                return OperationResultDto.SuccessResult(
                    "Profil mis à jour avec succès",
                    null
                );
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors de la mise à jour",
                    new List<string> { ex.Message }
                );
            }
        }

        // =========================
        // 🔑 CHANGE PASSWORD (CLIENT + ADMIN)
        // =========================
        public async Task<OperationResultDto> ChangePasswordAsync(int userId, ChangerMotpasseDto dto)
        {
            try
            {
                var user = await _context.Clients.FindAsync(userId);
                if (user == null)
                {
                    return OperationResultDto.ErrorResult(
                        "Utilisateur introuvable",
                        new List<string> { "Compte inexistant" }
                    );
                }

                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.motpasse))
                {
                    return OperationResultDto.ErrorResult(
                        "Ancien mot de passe incorrect",
                        new List<string> { "Mot de passe actuel invalide" }
                    );
                }

                if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.motpasse))
                {
                    return OperationResultDto.ErrorResult(
                        "Mot de passe invalide",
                        new List<string> { "Le nouveau mot de passe doit être différent" }
                    );
                }

                user.motpasse = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();

                return OperationResultDto.SuccessResult(
                    "Mot de passe changé avec succès",
                    null
                );
            }
            catch (Exception ex)
            {
                return OperationResultDto.ErrorResult(
                    "Erreur lors du changement de mot de passe",
                    new List<string> { ex.Message }
                );
            }
        }

        // =========================
        // 📊 CLIENT DASHBOARD
        // =========================
        public async Task<ClientDashBoardDto?> GetClientDashboardAsync(int clientId)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null) return null;

            var commandes = await _context.Commandes
                .Where(c => c.Idclient == clientId)
                .Include(c => c.Lignecmds)
                    .ThenInclude(l => l.Produit)
                .OrderByDescending(c => c.datecmd)
                .ToListAsync();

            var reclamations = await _context.Reclamations
                .Where(r => r.Idclient == clientId && r.statut == "en cours")
                .ToListAsync();

            return new ClientDashBoardDto
            {
                Client = MapClientToDto(client),
                NombreCommandes = commandes.Count,
                MontantTotalAchats = commandes.Sum(c => c.totalTTC ?? 0),

                DernieresCommandes = commandes.Take(5).Select(c => new CommandeDTO
                {
                    Idcmd = c.Idcmd,
                    DateCmd = c.datecmd,
                    Statut = c.statut,
                    TotalHT = c.totalHT,
                    TotalTVA = c.totalTVA,
                    TotalTTC = c.totalTTC,
                    Idclient = c.Idclient,
                    NomClient = $"{client.nom} {client.prenom}",
                    Lignes = c.Lignecmds?.Select(l => new LignecmdDTO
                    {
                        NomProduit = l.Produit?.design ?? "",
                        Qte = l.qte,
                        PrixTotal = l.prixtotal
                    }).ToList() ?? new List<LignecmdDTO>()
                }).ToList(),

                ReclamationsEnCours = reclamations.Select(r => new ReclamationDTO
                {
                    Idrec = r.Idrec,
                    Sujet = r.sujet,
                    Descrip = r.descrip,
                    DateRec = r.daterec,
                    Statut = r.statut,
                    Idclient = r.Idclient,
                    Idcmd = r.Idcmd
                }).ToList()
            };
        }

        // =========================
        // 📈 STATS
        // =========================
        public async Task<int> GetTotalClientsCountAsync()
        {
            return await _context.Clients.CountAsync();
        }

        // =========================
        // 🔁 MAPPER
        // =========================
        private static ClientDTO MapClientToDto(Client client)
        {
            return new ClientDTO
            {
                Idclient = client.Idclient,
                Nom = client.nom,
                Prenom = client.prenom,
                Email = client.email,
                Telephone = client.telephone,
                Adresse = client.adresse,
                DateInscrip = client.dateinscrip,
                Role = client.role
            };
        }
    }
}