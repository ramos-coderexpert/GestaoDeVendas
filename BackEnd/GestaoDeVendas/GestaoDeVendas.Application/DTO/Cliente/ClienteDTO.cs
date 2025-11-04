namespace GestaoDeVendas.Application.DTO.Cliente
{
    public record ClienteDTO(string nome, string email,string password, string role, decimal saldo);
}