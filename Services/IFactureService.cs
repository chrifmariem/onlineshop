using onlineShop.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace onlineShop.Services
{
    public interface IFactureService
    {
        Task<FactureDTO> GetFactureByIdAsync(int id);
        Task<FactureDTO> GetFactureByCommandeAsync(int idCommande);
        Task<IEnumerable<FactureDTO>> GetAllFacturesAsync();
        Task<IEnumerable<FactureDTO>> GetFacturesByStatutAsync(string statut);
        Task<OperationResultDto> CreateFactureAsync(int idCommande);
        Task<OperationResultDto> CreateFactureAsync(CreateFactureDto dto);
        Task<OperationResultDto> UpdateStatutFactureAsync(int id, string nouveauStatut);
        Task<string> GetNextNumeroFactureAsync();
    }
}