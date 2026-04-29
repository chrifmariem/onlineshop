using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
namespace onlineShop.Models
{

    public class Client
    {
        [Key]
        public int Idclient { get; set; }

        [Required, MaxLength(100)]
        public string nom { get; set; }

        [MaxLength(100)]
        public string prenom { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string email { get; set; }

        [Required, MaxLength(250)]
        public string motpasse { get; set; }

        [MaxLength(50)]
        public string telephone { get; set; }

        [MaxLength(250)]
        public string adresse { get; set; }

        public DateTime dateinscrip { get; set; } = DateTime.Now;

        [Required]
        public string role { get; set; } = "client";

        // Navigation
        public ICollection<Commande> Commandes { get; set; }
        public ICollection<Reclamation> Reclamations { get; set; }
    }
}

