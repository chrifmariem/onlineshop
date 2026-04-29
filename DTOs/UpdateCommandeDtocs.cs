using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class UpdateCommandeDtocs
    {
        [Required]
        public int Idcmd { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire")]
        [Display(Name = "Statut")]
        public string Statut { get; set; }
    }
}
