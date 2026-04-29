using Microsoft.EntityFrameworkCore;
using onlineShop.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace onlineShop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Client> Clients { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<Lignecmd> Lignecmds { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<Reclamation> Reclamations { get; set; }
          public DbSet<Panier> Paniers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client Configuration
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("client");
                entity.HasKey(e => e.Idclient);
                entity.Property(e => e.Idclient).HasColumnName("Idclient");
                entity.Property(e => e.nom).HasMaxLength(100).IsRequired();
                entity.Property(e => e.prenom).HasMaxLength(100);
                entity.Property(e => e.email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.motpasse).HasMaxLength(250).IsRequired();
                entity.Property(e => e.telephone).HasMaxLength(50);
                entity.Property(e => e.adresse).HasMaxLength(250);
                entity.Property(e => e.dateinscrip).HasColumnType("datetime").HasDefaultValueSql("getdate()");
                entity.Property(e => e.role).HasMaxLength(50).HasDefaultValue("client").IsRequired();
                entity.HasIndex(e => e.email).IsUnique();
            });

            // Produit Configuration
            modelBuilder.Entity<Produit>(entity =>
            {
                entity.ToTable("produit");
                entity.HasKey(e => e.Idproduit);
                entity.Property(e => e.Idproduit).HasColumnName("Idproduit");
                entity.Property(e => e.aref).HasMaxLength(50).IsRequired();
                entity.Property(e => e.design).HasMaxLength(255).IsRequired();
                entity.Property(e => e.venteHT).HasColumnType("decimal(18,3)").IsRequired();
                entity.Property(e => e.tva).HasColumnType("decimal(18,3)").IsRequired();
                entity.Property(e => e.ttc).HasColumnType("decimal(18,3)").IsRequired();
                entity.Property(e => e.qtestock).IsRequired();
                entity.Property(e => e.promoweb).HasColumnType("decimal(5,2)");
                entity.Property(e => e.cmdLast).HasColumnType("date");
                entity.Property(e => e.prixpub).HasColumnType("decimal(18,3)");
                entity.Property(e => e.coli).HasMaxLength(50);
                entity.Property(e => e.labo).HasMaxLength(100);
                entity.Property(e => e.dernmiseajour).HasColumnType("date");
                entity.Property(e => e.image).HasMaxLength(500);
                entity.Property(e => e.tauxtvav).HasColumnType("decimal(5,2)");
                entity.Property(e => e.dlc).HasColumnType("date");
                entity.Property(e => e.codebar).HasMaxLength(50);
                entity.HasIndex(e => e.codebar).IsUnique();
            });

            // Commande Configuration
            modelBuilder.Entity<Commande>(entity =>
            {
                entity.ToTable("commande");
                entity.HasKey(e => e.Idcmd);
                entity.Property(e => e.Idcmd).HasColumnName("Idcmd");
                entity.Property(e => e.datecmd).HasColumnType("datetime").HasDefaultValueSql("getdate()");
                entity.Property(e => e.statut).HasMaxLength(50).HasDefaultValue("en cours");
                entity.Property(e => e.totalHT).HasColumnType("decimal(18,3)");
                entity.Property(e => e.totalTVA).HasColumnType("decimal(18,3)");
                entity.Property(e => e.totalTTC).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Idclient).HasColumnName("Idclient");

                entity.HasOne(e => e.Client)
                      .WithMany(c => c.Commandes)
                      .HasForeignKey(e => e.Idclient)
                      .HasConstraintName("FK_commande_ToClient")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Lignecmd Configuration
            modelBuilder.Entity<Lignecmd>(entity =>
            {
                entity.ToTable("Lignecmd");
                entity.HasKey(e => e.Idligne);
                entity.Property(e => e.Idligne).HasColumnName("Idligne");
                entity.Property(e => e.qte).IsRequired();
                entity.Property(e => e.prixuni).HasColumnType("decimal(18,3)").IsRequired();
                entity.Property(e => e.tva).HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.prixtotal).HasColumnType("decimal(18,3)").IsRequired();
                entity.Property(e => e.Idcmd).HasColumnName("Idcmd");
                entity.Property(e => e.Idproduit).HasColumnName("Idproduit");

                entity.HasOne(e => e.Commande)
                      .WithMany(c => c.Lignecmds)
                      .HasForeignKey(e => e.Idcmd)
                      .HasConstraintName("FK_Lignecmd_Tocommande")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Produit)
                      .WithMany(p => p.Lignecmds)
                      .HasForeignKey(e => e.Idproduit)
                      .HasConstraintName("FK_Lignecmd_ToProduit")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Facture Configuration
            modelBuilder.Entity<Facture>(entity =>
            {
                entity.ToTable("Facture");
                entity.HasKey(e => e.Idfacture);
                entity.Property(e => e.Idfacture).HasColumnName("Idfacture");
                entity.Property(e => e.numfac).HasMaxLength(50).IsRequired();
                entity.Property(e => e.dateFac).HasColumnType("datetime").HasDefaultValueSql("getdate()");
                entity.Property(e => e.totalHT).HasColumnType("decimal(18,3)").IsRequired();
                entity.Property(e => e.totalTVA).HasColumnType("decimal(18,3)");
                entity.Property(e => e.totalTTC).HasColumnType("decimal(18,3)");
                entity.Property(e => e.statut).HasMaxLength(50).HasDefaultValue("payée");
                entity.Property(e => e.Idcmd).HasColumnName("Idcmd");
                entity.HasIndex(e => e.numfac).IsUnique();

                entity.HasOne(e => e.Commande)
                      .WithOne(c => c.Facture)
                      .HasForeignKey<Facture>(e => e.Idcmd)
                      .HasConstraintName("FK_Facture_Tocommande")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Panier>(entity =>
            {
                entity.ToTable("panier");
                entity.HasKey(e => e.Idpanier);
                entity.Property(e => e.Idpanier).HasColumnName("Idpanier").UseIdentityColumn();
                entity.Property(e => e.Idclient).HasColumnName("Idclient").IsRequired();
                entity.Property(e => e.Idproduit).HasColumnName("Idproduit").IsRequired();
                entity.Property(e => e.Quantite).HasColumnName("quantite").IsRequired();
                entity.Property(e => e.DateAjout).HasColumnName("dateajout").HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.DateModification).HasColumnName("datemodification").HasColumnType("datetime");

                // Unique constraint to prevent duplicate products in cart for same client
                entity.HasIndex(e => new { e.Idclient, e.Idproduit })
                      .IsUnique()
                      .HasDatabaseName("IX_Panier_UniqueClientProduct");

                // Foreign keys
                entity.HasOne(e => e.Client)
                      .WithMany()
                      .HasForeignKey(e => e.Idclient)
                      .HasConstraintName("FK_Panier_Client")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Produit)
                      .WithMany()
                      .HasForeignKey(e => e.Idproduit)
                      .HasConstraintName("FK_Panier_Produit")
                      .OnDelete(DeleteBehavior.Cascade);
            });
        


// Reclamation Configuration
modelBuilder.Entity<Reclamation>(entity =>
            {
                entity.ToTable("Reclamation");
                entity.HasKey(e => e.Idrec);
                entity.Property(e => e.Idrec).HasColumnName("Idrec");
                entity.Property(e => e.sujet).HasMaxLength(150).IsRequired();
                entity.Property(e => e.descrip).HasColumnType("text").IsRequired();
                entity.Property(e => e.daterec).HasColumnType("datetime").HasDefaultValueSql("getdate()");
                entity.Property(e => e.statut).HasMaxLength(50).HasDefaultValue("en cours");
                entity.Property(e => e.reponseAdmin).HasColumnType("text");
                entity.Property(e => e.dateresp).HasColumnType("datetime");
                entity.Property(e => e.Idclient).HasColumnName("Idclient");
                entity.Property(e => e.Idcmd).HasColumnName("Idcmd");

                entity.HasOne(e => e.Client)
                      .WithMany(c => c.Reclamations)
                      .HasForeignKey(e => e.Idclient)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Commande)
                      .WithMany()
                      .HasForeignKey(e => e.Idcmd)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}