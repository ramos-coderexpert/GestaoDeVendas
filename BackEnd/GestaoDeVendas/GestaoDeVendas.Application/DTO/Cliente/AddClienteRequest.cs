namespace GestaoDeVendas.Application.DTO.Cliente
{
    public record AddClienteRequest(string nome, string email, string password, string role, decimal saldo);
}