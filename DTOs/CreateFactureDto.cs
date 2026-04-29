using System;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class CreateFactureDto
    {
        [Required(ErrorMessage = "La commande est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "ID commande invalide")]
        [Display(Name = "Commande")]
        public int Idcmd { get; set; }

        [Display(Name = "Mode de paiement")]
        [Required(ErrorMessage = "Le mode de paiement est requis")]
        public string ModePaiement { get; set; } = "Carte bancaire";

        [Display(Name = "Statut")]
        public string Statut { get; set; } = "Non payée";

        [Display(Name = "Date d'échéance")]
        [DataType(DataType.Date)]
        public DateTime? DateEcheance { get; set; }

        [Display(Name = "Notes")]
        [StringLength(500, ErrorMessage = "Les notes ne peuvent pas dépasser 500 caractères")]
        public string Notes { get; set; }
    }
}