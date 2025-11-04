using GestaoDeVendas.Application.DTO.Auth;
using GestaoDeVendas.Application.DTO.Cliente;

namespace GestaoDeVendas.Application.IServices
{
    public interface IClienteService
    {
        LoginResponseDTO Login(LoginRequestDTO loginRequest);
        Task<List<GetClienteRequest>> ObterTodosAsync();
        Task<List<GetClienteRequest>> ObterTodosAtivosAsync();
        GetClienteRequest ObterPorId(Guid id);
        GetClienteRequest ObterPorNome(string nome);
        Guid AddCliente(AddClienteRequest clienteRequest);
        void UpdateCliente(Guid id, UpdateClienteRequest clienteRequest);
        void DeleteCliente(Guid id);
    }
}