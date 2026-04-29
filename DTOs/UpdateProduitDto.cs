using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class UpdateProduitDto
    {
        [Display(Name = "Référence")]
        public string Aref { get; set; }

        [Display(Name = "Désignation")]
        public string Design { get; set; }

        [Display(Name = "Prix de vente HT")]
        public decimal VenteHT { get; set; }

        [Display(Name = "TVA")]
        public decimal Tva { get; set; }

        [Display(Name = "Quantité en stock")]
        public int QteStock { get; set; }

        [Display(Name = "Promotion web")]
        public decimal? PromoWeb { get; set; }

        [Display(Name = "Laboratoire")]
        public string Labo { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }

        /// <summary>
        /// Validates the DTO
        /// </summary>
        public (bool IsValid, List<string> Errors) Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Design))
                errors.Add("La désignation est requise");

            if (string.IsNullOrWhiteSpace(Aref))
                errors.Add("La référence est requise");

            if (VenteHT <= 0)
                errors.Add("Le prix HT doit être supérieur à 0");

            if (Tva < 0 || Tva > 100)
                errors.Add("La TVA doit être entre 0 et 100%");

            if (QteStock < 0)
                errors.Add("La quantité en stock ne peut pas être négative");

            if (PromoWeb.HasValue && (PromoWeb < 0 || PromoWeb > 100))
                errors.Add("La promotion doit être entre 0 et 100%");

            return (errors.Count == 0, errors);
        }
    }
}