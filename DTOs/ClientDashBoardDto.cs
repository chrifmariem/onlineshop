namespace onlineShop.DTOs
{
    public class ClientDashBoardDto
    {
        public ClientDTO Client { get; set; }
        public int NombreCommandes { get; set; }
        public decimal MontantTotalAchats { get; set; }
        public List<CommandeDTO> DernieresCommandes { get; set; }
        public List<ReclamationDTO> ReclamationsEnCours { get; set; }

    }

}
