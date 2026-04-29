using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class UpdateRecalamtionDto
    {
        [Required]
        public int Idrec { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire")]
        [Display(Name = "Statut")]
        public string Statut { get; set; }

        [StringLength(2000, ErrorMessage = "La réponse ne peut pas dépasser 2000 caractères")]
        [Display(Name = "Réponse")]
        public string ReponseAdmin { get; set; }
    }
}
