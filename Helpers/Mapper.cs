using onlineShop.Models;
using onlineShop.DTOs;
using System.Linq;

namespace onlineShop.Helpers
{
    public static class Mapper
    {
        // ============ Client Mappings ============
        public static ClientDTO ToDto(this Client client)
        {
            return new ClientDTO
            {
                Idclient = client.Idclient,
                Nom = client.nom,
                Prenom = client.prenom,
                Email = client.email,
                Telephone = client.telephone,
                Adresse = client.adresse,
                DateInscrip = client.dateinscrip,
                Role = client.role
            };
        }

        public static Client ToEntity(this ClientRegisterDto dto)
        {
            return new Client
            {
                nom = dto.Nom,
                prenom = dto.Prenom,
                email = dto.Email,
                motpasse = dto.MotPasse, // Hash before saving!
                telephone = dto.Telephone,
                adresse = dto.Adresse
            };
        }

        // ============ Produit Mappings ============
        public static ProduitDTO ToDto(this Produit produit)
        {
            return new ProduitDTO
            {
                Idproduit = produit.Idproduit,
                Aref = produit.aref,
                Design = produit.design,
                VenteHT = produit.venteHT,
                Tva = produit.tva,
                Ttc = produit.ttc,
                QteStock = produit.qtestock,
                PromoWeb = produit.promoweb,
                Image = produit.image,
                Labo = produit.labo,
                Dlc = produit.dlc
            };
        }

        public static Produit ToEntity(this CreateProduitDto dto)
        {
            // Calculate TTC
            var ttc = dto.VenteHT + (dto.VenteHT * dto.Tva / 100);

            return new Produit
            {
                aref = dto.Aref?.Trim(),
                design = dto.Design?.Trim(),
                venteHT = dto.VenteHT,
                tva = dto.Tva,
                ttc = ttc,
                qtestock = dto.QteStock,
                promoweb = dto.PromoWeb,
                coli = dto.Coli,
                labo = dto.Labo,
                image = string.IsNullOrEmpty(dto.Image) ? "/images/default-product.png" : dto.Image,
                codebar = dto.CodeBar,
                dernmiseajour = System.DateTime.Now,
                prixpub = 0,
                qtelast = 0,
                tauxtvav = dto.Tva
            };
        }

        public static Produit ToEntity(this UpdateProduitDto dto, int idProduit)
        {
            // Calculate TTC
            var ttc = dto.VenteHT + (dto.VenteHT * dto.Tva / 100);

            return new Produit
            {
                Idproduit = idProduit,
                aref = dto.Aref?.Trim(),
                design = dto.Design?.Trim(),
                venteHT = dto.VenteHT,
                tva = dto.Tva,
                ttc = ttc,
                qtestock = dto.QteStock,
                promoweb = dto.PromoWeb,
                labo = dto.Labo,
                image = dto.Image,
                dernmiseajour = System.DateTime.Now,
                tauxtvav = dto.Tva
            };
        }

        // ============ Commande Mappings ============
        public static CommandeDTO ToDto(this Commande commande)
        {
            return new CommandeDTO
            {
                Idcmd = commande.Idcmd,
                DateCmd = commande.datecmd,
                Statut = commande.statut,
                TotalHT = commande.totalHT,
                TotalTVA = commande.totalTVA,
                TotalTTC = commande.totalTTC,
                Idclient = commande.Idclient,
                NomClient = commande.Client != null
                    ? $"{commande.Client.nom} {commande.Client.prenom}"
                    : string.Empty,
                Lignes = commande.Lignecmds?.Select(l => l.ToDto()).ToList()
            };
        }

        // ============ LigneCmd Mappings ============
        public static LignecmdDTO ToDto(this Lignecmd ligne)
        {
            return new LignecmdDTO
            {
                Idligne = ligne.Idligne,
                Idproduit = ligne.Idproduit,
                NomProduit = ligne.Produit?.design ?? string.Empty,
                Qte = ligne.qte,
                PrixUni = ligne.prixuni,
                Tva = ligne.tva,
                PrixTotal = ligne.prixtotal
            };
        }

        public static Lignecmd ToEntity(this CreateLignecmdDto dto, Produit produit)
        {
            var prixHT = produit.venteHT * dto.Qte;
            var montantTVA = prixHT * (produit.tva / 100);
            var prixTotal = prixHT + montantTVA;

            return new Lignecmd
            {
                Idproduit = dto.Idproduit,
                qte = dto.Qte,
                prixuni = produit.venteHT,
                tva = produit.tva,
                prixtotal = prixTotal
            };
        }

        // ============ Facture Mappings ============
        public static FactureDTO ToDto(this Facture facture)
        {
            return new FactureDTO
            {
                Idfacture = facture.Idfacture,
                NumFac = facture.numfac,
                DateFac = facture.dateFac,
                TotalHT = facture.totalHT,
                TotalTVA = facture.totalTVA,
                TotalTTC = facture.totalTTC,
                Statut = facture.statut,
                Idcmd = facture.Idcmd,
                Commande = facture.Commande?.ToDto()
            };
        }

        // ============ Reclamation Mappings ============
        public static ReclamationDTO ToDto(this Reclamation reclamation)
        {
            return new ReclamationDTO
            {
                Idrec = reclamation.Idrec,
                Sujet = reclamation.sujet,
                Descrip = reclamation.descrip,
                DateRec = reclamation.daterec,
                Statut = reclamation.statut,
                ReponseAdmin = reclamation.reponseAdmin,
                DateResp = reclamation.dateresp,
                Idclient = reclamation.Idclient,
                NomClient = reclamation.Client != null
                    ? $"{reclamation.Client.nom} {reclamation.Client.prenom}"
                    : string.Empty,
                Idcmd = reclamation.Idcmd
            };
        }

        public static Reclamation ToEntity(this CreateReclamationDto dto)
        {
            return new Reclamation
            {
                sujet = dto.Sujet,
                descrip = dto.Descrip,
                Idclient = dto.Idclient,
                Idcmd = dto.Idcmd
            };
        }

        // ============ Panier Mappings ============
        public static PanierItemDTO ToPanierItemDto(this Produit produit, int quantite)
        {
            var montantHT = produit.venteHT * quantite;
            var montantTVA = montantHT * (produit.tva / 100);

            return new PanierItemDTO
            {
                ProduitId = produit.Idproduit,
                NomProduit = produit.design,
                Prix = produit.venteHT,
                Quantite = quantite,
                Tva = produit.tva,
                ImageUrl = produit.image,
                QteDisponible = produit.qtestock
            };
        }
    }
}