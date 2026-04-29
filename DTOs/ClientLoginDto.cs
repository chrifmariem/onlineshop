using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class ClientLoginDto
    {
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotPasse { get; set; }

        // IMPORTANT: Role is now optional - no [Required] attribute
        [Display(Name = "Rôle")]
        public string? Role { get; set; }

        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }
    }
}