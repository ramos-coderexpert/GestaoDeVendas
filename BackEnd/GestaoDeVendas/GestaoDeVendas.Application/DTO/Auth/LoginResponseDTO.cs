namespace GestaoDeVendas.Application.DTO.Auth
{
    public record LoginResponseDTO(string token, string email, string role);
}