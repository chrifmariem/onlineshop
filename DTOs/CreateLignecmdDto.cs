using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class CreateLignecmdDto
    {
        [Required(ErrorMessage = "Le produit est requis")]
        [Range(1, int.MaxValue, ErrorMessage = "ID produit invalide")]
        [Display(Name = "Produit")]
        public int Idproduit { get; set; }

        [Required(ErrorMessage = "La quantité est requise")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être au moins 1")]
        [Display(Name = "Quantité")]
        public int Qte { get; set; }

        [Range(0, 100, ErrorMessage = "La TVA doit être entre 0 et 100%")]
        [Display(Name = "TVA (%)")]
        public decimal Tva { get; set; } = 19;

        [Display(Name = "Prix unitaire")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
        public decimal PrixUni { get; set; }
    }
}