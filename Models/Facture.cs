
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace onlineShop.Models
{
    public class Facture
    {
        [Key]
        public int Idfacture { get; set; }

        [Required]
        public string numfac { get; set; }

        public DateTime dateFac { get; set; } = DateTime.Now;

        public decimal totalHT { get; set; }
        public decimal? totalTVA { get; set; }
        public decimal? totalTTC { get; set; }

        public string statut { get; set; } = "payée";

        // Foreign Key
        public int Idcmd { get; set; }

        [ForeignKey("Idcmd")]
        public Commande Commande { get; set; }
    }
}
