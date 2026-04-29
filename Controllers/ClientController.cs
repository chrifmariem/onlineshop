using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.DTOs;
using onlineShop.Services;
using System.Security.Claims;

namespace onlineShop.Controllers
{
    [Authorize(Roles = "client")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientController(IClientService clientService, IHttpContextAccessor httpContextAccessor)
        {
            _clientService = clientService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            HttpContext.Session.Clear();
            Response.Cookies.Delete("OnlineShop.Auth");
            TempData["SuccessMessage"] = "Vous avez été déconnecté avec succès.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            HttpContext.Session.Clear();
            Response.Cookies.Delete("OnlineShop.Auth");
            TempData["SuccessMessage"] = "Vous avez été déconnecté avec succès.";
            return RedirectToAction("Index", "Home");
        }

        // 👤 PROFILE - Dashboard du client
        [HttpGet]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int clientId))
            {
                return RedirectToAction("Login", "Account");
            }

            var dashboard = await _clientService.GetClientDashboardAsync(clientId);
            if (dashboard == null)
            {
                TempData["ErrorMessage"] = "Client non trouvé";
                return RedirectToAction("Index", "Home");
            }

            return View(dashboard);
        }

        // ✏️ EDIT PROFILE - Afficher le formulaire
        [HttpGet]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> EditProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int clientId))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = await _clientService.GetClientByIdAsync(clientId);
            if (client == null)
            {
                TempData["ErrorMessage"] = "Client non trouvé";
                return RedirectToAction("Profile");
            }

            var model = new UpdateClientDto
            {
                Nom = client.Nom,
                Prenom = client.Prenom,
                Telephone = client.Telephone,
                Adresse = client.Adresse
            };

            return View(model);
        }

        // ✏️ EDIT PROFILE - Traiter la mise à jour
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UpdateClientDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int clientId))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _clientService.UpdateClientAsync(clientId, model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Profile");
            }

            TempData["ErrorMessage"] = result.Message;
            return View(model);
        }

        // 🔑 CHANGE PASSWORD - Afficher le formulaire
        [HttpGet]
        public IActionResult ChangerPassword()
        {
            return View(new ChangerMotpasseDto());
        }

        // 🔑 CHANGE PASSWORD - Traiter le changement
        // In ClientController - UPDATE THESE TWO METHODS:

        // 🔑 CHANGE PASSWORD - Afficher le formulaire
        [HttpGet]
        public IActionResult ChangePassword()  // CHANGED FROM ChangerPassword
        {
            return View(new ChangerMotpasseDto());
        }

       

        // 📦 MES COMMANDES
        [HttpGet]
        public async Task<IActionResult> MesCommandes()
        {
            var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var dashboard = await _clientService.GetClientDashboardAsync(clientId);

            dashboard ??= new ClientDashBoardDto
            {
                DernieresCommandes = new List<CommandeDTO>()
            };

            return View(dashboard);
        }

    }
}