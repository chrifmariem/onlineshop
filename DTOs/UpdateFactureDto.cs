using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class UpdateFactureDto
    {
        [Required]
        public int Idfacture { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire")]
        [Display(Name = "Statut")]
        public string Statut { get; set; }
    }
}
