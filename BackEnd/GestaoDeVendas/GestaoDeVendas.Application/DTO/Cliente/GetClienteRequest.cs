namespace GestaoDeVendas.Application.DTO.Cliente
{
    public record GetClienteRequest(Guid id, string nome, string email, string role, decimal saldo);
}