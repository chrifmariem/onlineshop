namespace onlineShop.DTOs
{
    using System;

    public class ReclamationDTO
    {
        public int Idrec { get; set; }
        public string Sujet { get; set; }
        public string Descrip { get; set; }
        public DateTime DateRec { get; set; }
        public string Statut { get; set; }
        public string ReponseAdmin { get; set; }
        public DateTime? DateResp { get; set; }
        public int Idclient { get; set; }
        public string NomClient { get; set; }
        public string EmailClient { get; set; }
        public int? Idcmd { get; set; }
        public string NumCommande { get; set; }

        // Propriétés calculées
        public bool AReponse => !string.IsNullOrEmpty(ReponseAdmin);
        public string StatutCouleur => Statut?.ToLower() switch
        {
            "en cours" => "warning",
            "traitée" => "success",
            "fermée" => "secondary",
            _ => "info"
        };
        public int JoursDepuisCreation => (DateTime.Now - DateRec).Days;
    }

}
