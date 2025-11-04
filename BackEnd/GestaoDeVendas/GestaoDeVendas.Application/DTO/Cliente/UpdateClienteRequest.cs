namespace GestaoDeVendas.Application.DTO.Cliente
{
    public record UpdateClienteRequest(string nome, string role, decimal saldo);
}