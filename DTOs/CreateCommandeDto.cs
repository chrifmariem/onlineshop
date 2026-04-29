using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class CreateCommandeDto
    {
        [Required(ErrorMessage = "L'ID client est requis")]
        [Range(1, int.MaxValue, ErrorMessage = "ID client invalide")]
        [Display(Name = "Client")]
        public int Idclient { get; set; }

        [Required(ErrorMessage = "L'adresse de livraison est requise")]
        [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères")]
        [Display(Name = "Adresse de livraison")]
        public string AdresseLivraison { get; set; }

        [Required(ErrorMessage = "Le téléphone de livraison est requis")]
        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        [StringLength(20, ErrorMessage = "Le téléphone ne peut pas dépasser 20 caractères")]
        [Display(Name = "Téléphone de livraison")]
        public string TelephoneLivraison { get; set; }

        [Required(ErrorMessage = "Le mode de paiement est requis")]
        [Display(Name = "Mode de paiement")]
        public string ModePaiement { get; set; } = "Carte bancaire";

        [Display(Name = "Statut")]
        public string Statut { get; set; } = "En attente";

        [Required(ErrorMessage = "La commande doit contenir au moins un produit")]
        [MinLength(1, ErrorMessage = "La commande doit contenir au moins un produit")]
        [Display(Name = "Produits")]
        public List<CreateLignecmdDto> Lignes { get; set; } = new List<CreateLignecmdDto>();

        [StringLength(500, ErrorMessage = "Les notes ne peuvent pas dépasser 500 caractères")]
        [Display(Name = "Notes de commande")]
        public string Notes { get; set; }
    }
}