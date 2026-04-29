using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineShop.Models
{
    [Table("panier")]
    public class Panier
    {
        [Key]
        [Column("Idpanier")]
        public int Idpanier { get; set; }

        [Column("Idclient")]
        public int Idclient { get; set; }

        [Column("Idproduit")]
        public int Idproduit { get; set; }

        [Column("quantite")]
        public int Quantite { get; set; }

        [Column("dateajout")]
        public DateTime DateAjout { get; set; } = DateTime.Now;

        [Column("datemodification")]
        public DateTime? DateModification { get; set; }

        // Navigation properties
        [ForeignKey("Idclient")]
        public virtual Client Client { get; set; }

        [ForeignKey("Idproduit")]
        public virtual Produit Produit { get; set; }
    }
}