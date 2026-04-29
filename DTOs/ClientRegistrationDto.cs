using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class ClientRegisterDto
    {
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [MaxLength(100)]
        public string Nom { get; set; }

        [MaxLength(100)]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        [DataType(DataType.Password)]
        public string MotPasse { get; set; }

        [Required(ErrorMessage = "Veuillez confirmer le mot de passe")]
        [Compare("MotPasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [DataType(DataType.Password)]
        public string ConfirmMotPasse { get; set; }

        [Phone]
        [MaxLength(50)]
        public string Telephone { get; set; }

        [MaxLength(250)]
        public string Adresse { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un rôle")]
        [MaxLength(20)]
        public string Role { get; set; } = "client";

        [MaxLength(50)]
        public string AdminSecretCode { get; set; }
    }
}