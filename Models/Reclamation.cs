using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace onlineShop.Models
{
    public class Reclamation
    {
        [Key]
        public int Idrec { get; set; }

        [Required]
        public string sujet { get; set; }

        [Required]
        public string descrip { get; set; }

        public DateTime daterec { get; set; } = DateTime.Now;

        public string statut { get; set; } = "en cours";

        public string reponseAdmin { get; set; }
        public DateTime? dateresp { get; set; }

        // Foreign Keys
        public int Idclient { get; set; }
        public int? Idcmd { get; set; }

        [ForeignKey("Idclient")]
        public Client Client { get; set; }

        [ForeignKey("Idcmd")]
        public Commande Commande { get; set; }
    }
}
