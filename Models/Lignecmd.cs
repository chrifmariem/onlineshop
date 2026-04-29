using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineShop.Models
{
    public class Lignecmd
    {
        [Key]
        public int Idligne { get; set; }

        [Required]
        public int qte { get; set; }

        [Required]
        public decimal prixuni { get; set; }

        [Required]
        public decimal tva { get; set; }

        [Required]
        public decimal prixtotal { get; set; }

        // Foreign Keys
        public int Idcmd { get; set; }
        public int Idproduit { get; set; }

        [ForeignKey("Idcmd")]
        public Commande Commande { get; set; }

        [ForeignKey("Idproduit")]
        public Produit Produit { get; set; }
    }
}
