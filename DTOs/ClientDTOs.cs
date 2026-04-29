namespace onlineShop.DTOs
{
    public class ClientDTO
    {
        public int Idclient { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string NomComplet => $"{Nom} {Prenom}".Trim();
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Adresse { get; set; }
        public DateTime DateInscrip { get; set; }
        public string Role { get; set; }
    }

}
