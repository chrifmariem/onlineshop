using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class AjouterQuantitePanierDto
    {
        [Required]
        public int ProduitId { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "La quantité doit être entre 0 et 10000")]
        public int NouvelleQuantite { get; set; }
    }
}
