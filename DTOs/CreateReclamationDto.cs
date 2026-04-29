using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class CreateReclamationDto
    {
        [Required(ErrorMessage = "Le sujet est obligatoire")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Le sujet doit contenir entre 5 et 150 caractères")]
        [Display(Name = "Sujet")]
        public string Sujet { get; set; }

        [Required(ErrorMessage = "La description est obligatoire")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "La description doit contenir entre 10 et 2000 caractères")]
        [Display(Name = "Description")]
        public string Descrip { get; set; }

        [Required]
        public int Idclient { get; set; }

        [Display(Name = "Numéro de commande concernée")]
        public int? Idcmd { get; set; }
    }
}
