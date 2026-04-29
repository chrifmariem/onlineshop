namespace onlineShop.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateClientDto
    {
        [Required(ErrorMessage = "Le nom est requis")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        public string Prenom { get; set; }

        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        public string Telephone { get; set; }

        public string Adresse { get; set; }
    }
}