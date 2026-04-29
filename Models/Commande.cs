
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
namespace onlineShop.Models
{
    public class Commande
    {
        [Key]
        public int Idcmd { get; set; }

        public DateTime datecmd { get; set; } = DateTime.Now;

        public string statut { get; set; } = "en cours";

        public decimal? totalHT { get; set; }
        public decimal? totalTVA { get; set; }
        public decimal? totalTTC { get; set; }

        // Foreign Key
        public int Idclient { get; set; }

        [ForeignKey("Idclient")]
        public Client Client { get; set; }

        // Navigation
        public ICollection<Lignecmd> Lignecmds { get; set; }
        public Facture Facture { get; set; }
    }
}