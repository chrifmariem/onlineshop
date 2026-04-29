using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.Models
{
    public class Produit
    {
        [Key]
        public int Idproduit { get; set; }

        [Required]
        [StringLength(50)]
        public string aref { get; set; }

        [Required]
        [StringLength(255)]
        public string design { get; set; }

        [Required]
        [Range(0.001, double.MaxValue)]
        public decimal venteHT { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal tva { get; set; }

        [Required]
        public decimal ttc { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int qtestock { get; set; }

        [Range(0, 100)]
        public decimal? promoweb { get; set; }

        public DateTime? cmdLast { get; set; }

        [Range(0, int.MaxValue)]
        public int? qtelast { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? prixpub { get; set; }

        [StringLength(50)]
        public string coli { get; set; }

        [StringLength(100)]
        public string labo { get; set; }

        public DateTime? dernmiseajour { get; set; }

        [StringLength(500)]
        [Url]
        public string image { get; set; }

        [Range(0, 100)]
        public decimal? tauxtvav { get; set; }

        public DateTime? dlc { get; set; }

        [StringLength(50)]
        public string codebar { get; set; }

        // Navigation
        public ICollection<Lignecmd> Lignecmds { get; set; }
    }
}