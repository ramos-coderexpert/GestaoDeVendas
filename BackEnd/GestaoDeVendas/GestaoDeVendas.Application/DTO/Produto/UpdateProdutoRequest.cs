namespace GestaoDeVendas.Application.DTO.Produto
{
    public record UpdateProdutoRequest(string nome, decimal preco, int estoque);
}