using System.ComponentModel.DataAnnotations;

namespace onlineShop.DTOs
{
    public class CreateProduitDto
    {
        [Required(ErrorMessage = "La désignation est requise")]
        [StringLength(255, ErrorMessage = "La désignation ne peut pas dépasser 255 caractères")]
        [Display(Name = "Désignation")]
        public string Design { get; set; }

        [Required(ErrorMessage = "La référence est requise")]
        [StringLength(50, ErrorMessage = "La référence ne peut pas dépasser 50 caractères")]
        [Display(Name = "Référence")]
        public string Aref { get; set; }

        [Display(Name = "Laboratoire")]
        [StringLength(100)]
        public string Labo { get; set; }

        [Display(Name = "Code-barres")]
        [StringLength(50)]
        public string CodeBar { get; set; }

        // For form input (accepts string with comma or period)
        [Display(Name = "Prix de vente HT")]
        public string VenteHTString { get; set; }

        // Actual decimal value (set by controller or conversion)
        public decimal VenteHT { get; set; }

        [Required(ErrorMessage = "La TVA est requise")]
        [Range(0, 100, ErrorMessage = "La TVA doit être entre 0 et 100%")]
        [Display(Name = "TVA")]
        public decimal Tva { get; set; } = 19;

        [Required(ErrorMessage = "La quantité en stock est requise")]
        [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être positive")]
        [Display(Name = "Quantité en stock")]
        public int QteStock { get; set; }

        [Display(Name = "Promotion web")]
        [Range(0, 100, ErrorMessage = "La promotion doit être entre 0 et 100%")]
        public decimal? PromoWeb { get; set; }

        [Display(Name = "Colisage")]
        [StringLength(50)]
        public string Coli { get; set; }

        [Display(Name = "Image")]
        [StringLength(500)]
        public string Image { get; set; }

        /// <summary>
        /// Parses VenteHTString to VenteHT decimal
        /// Handles both comma and period as decimal separator
        /// </summary>
        public bool TryParseVenteHT(out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(VenteHTString))
            {
                errorMessage = "Le prix HT est requis";
                return false;
            }

            // Clean and normalize the string
            string cleanValue = VenteHTString.Trim().Replace(',', '.');

            if (decimal.TryParse(cleanValue,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal parsedValue))
            {
                if (parsedValue <= 0)
                {
                    errorMessage = "Le prix HT doit être supérieur à 0";
                    return false;
                }

                VenteHT = parsedValue;
                return true;
            }

            errorMessage = $"Format de prix invalide: '{VenteHTString}'. Utilisez le format: 37.836 ou 37,836";
            return false;
        }

        /// <summary>
        /// Validates the entire DTO
        /// </summary>
        public (bool IsValid, List<string> Errors) Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Design))
                errors.Add("La désignation est requise");

            if (string.IsNullOrWhiteSpace(Aref))
                errors.Add("La référence est requise");

            if (!TryParseVenteHT(out string priceError))
                errors.Add(priceError);

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