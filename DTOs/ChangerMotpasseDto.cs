using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class ChangerMotpasseDto
    {
        [Required(ErrorMessage = "Le mot de passe actuel est requis")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Le nouveau mot de passe est requis")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "La confirmation est requise")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmNewPassword { get; set; }
    }
}