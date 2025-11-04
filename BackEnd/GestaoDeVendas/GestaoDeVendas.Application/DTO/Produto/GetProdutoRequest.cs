namespace GestaoDeVendas.Application.DTO.Produto
{
    public record GetProdutoRequest(Guid id, string nome, decimal preco, int estoque);
}