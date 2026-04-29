using onlineShop.DTOs;

namespace onlineShop.Services
{
    public interface IClientService
    {
        // Authentication & Registration
        Task<OperationResultDto> RegisterAsync(ClientRegisterDto dto);
        Task<OperationResultDto> LoginAsync(ClientLoginDto dto);

        // Client Management
        Task<ClientDTO> GetClientByIdAsync(int id);
        Task<ClientDTO> GetClientByEmailAsync(string email);
        Task<IEnumerable<ClientDTO>> GetAllClientsAsync();
        Task<OperationResultDto> UpdateClientAsync(int id, UpdateClientDto dto);
        Task<OperationResultDto> ChangePasswordAsync(int clientId, ChangerMotpasseDto dto);

        // Dashboard & Statistics
        Task<ClientDashBoardDto> GetClientDashboardAsync(int clientId);
        Task<int> GetTotalClientsCountAsync();
    }
}