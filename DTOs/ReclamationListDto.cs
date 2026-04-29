namespace onlineShop.DTOs
{
    public class ReclamationListDto
    {
        public List<ReclamationDTO> Reclamations { get; set; }
        public string StatutFiltre { get; set; }
        public int? ClientId { get; set; }
        public int TotalReclamations { get; set; }
        public int ReclamationsEnCours { get; set; }
        public int ReclamationsTraitees { get; set; }
    }
}
