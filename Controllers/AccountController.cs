using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.DTOs;
using onlineShop.Services;
using System.Security.Claims;

namespace onlineShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IClientService _clientService;

        public AccountController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // 📝 REGISTER - Afficher le formulaire
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new ClientRegisterDto());
        }

        // 📝 REGISTER - Traiter l'inscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ClientRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _clientService.RegisterAsync(dto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;

                // Auto-login après inscription
                var clientDto = result.Data as ClientDTO;
                if (clientDto != null)
                {
                    await SignInUserAsync(clientDto);

                    // Redirection basée sur le rôle
                    if (clientDto.Role == "admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Profile", "Client");
                    }
                }

                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = result.Message;
            if (result.Errors != null && result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(dto);
        }

        // 🔐 LOGIN - Afficher le formulaire
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectBasedOnRole();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new ClientLoginDto());
        }

        // 🔐 LOGIN - Traiter la connexion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ClientLoginDto dto, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // Authentication based on email/password only
            // The service will return the user's actual role from database
            var result = await _clientService.LoginAsync(dto);

            if (result.Success)
            {
                var clientDto = result.Data as ClientDTO;
                if (clientDto != null)
                {
                    await SignInUserAsync(clientDto);

                    TempData["SuccessMessage"] = $"Bienvenue, {clientDto.Prenom} {clientDto.Nom}!";

                    // Vérifier returnUrl
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Redirection basée sur le rôle de l'utilisateur dans la base de données
                    if (clientDto.Role == "admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            TempData["ErrorMessage"] = result.Message;
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
        }

        // =========================
        // 🔑 CHANGE PASSWORD (FOR ALL USERS)
        // =========================
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangerMotpasseDto());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangerMotpasseDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Utilisateur non identifié";
                return RedirectToAction("Login");
            }

            var result = await _clientService.ChangePasswordAsync(userId, dto);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(dto);
            }

            // If admin, force logout after password change
            var isAdmin = User.IsInRole("admin");

            TempData["SuccessMessage"] = isAdmin
                ? "Mot de passe modifié. Veuillez vous reconnecter."
                : "Mot de passe modifié avec succès.";

            if (isAdmin)
            {
                // 🔒 FORCE LOGOUT FOR ADMIN
                await HttpContext.SignOutAsync("CookieAuth");
                HttpContext.Session.Clear();
                Response.Cookies.Delete("OnlineShop.Auth");
                return RedirectToAction("Login");
            }

            // For client, redirect to profile
            return RedirectToAction("Profile", "Client");
        }

        // 🚪 LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Use "CookieAuth" - matches your Program.cs configuration
            await HttpContext.SignOutAsync("CookieAuth");

            // Clear session
            HttpContext.Session.Clear();

            // Clear authentication cookies
            Response.Cookies.Delete("OnlineShop.Auth");

            TempData["SuccessMessage"] = "Vous avez été déconnecté avec succès.";
            return RedirectToAction("Index", "Home");
        }

        // GET version for easier testing
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            HttpContext.Session.Clear();
            Response.Cookies.Delete("OnlineShop.Auth");
            TempData["SuccessMessage"] = "Vous avez été déconnecté avec succès.";
            return RedirectToAction("Index", "Home");
        }

        // 🚫 ACCESS DENIED
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // 🔧 HELPER: Connecter l'utilisateur
        private async Task SignInUserAsync(ClientDTO client)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, client.Idclient.ToString()),
                new Claim(ClaimTypes.Name, $"{client.Prenom} {client.Nom}"),
                new Claim(ClaimTypes.Email, client.Email),
                new Claim(ClaimTypes.Role, client.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                "CookieAuth",
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }

        // 🔧 HELPER: Redirection basée sur le rôle
        private IActionResult RedirectBasedOnRole()
        {
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (User.IsInRole("client"))
            {
                return RedirectToAction("Profile", "Client");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}