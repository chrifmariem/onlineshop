using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class AjouterAuPanierDto
    {
        [Required(ErrorMessage = "Le produit est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "ID produit invalide")]
        public int ProduitId { get; set; }

        [Required(ErrorMessage = "La quantité est obligatoire")]
        [Range(1, 10000, ErrorMessage = "La quantité doit être entre 1 et 10000")]
        [Display(Name = "Quantité")]
        public int Quantite { get; set; } = 1;
    }
}
