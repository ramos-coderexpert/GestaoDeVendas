namespace GestaoDeVendas.Application.DTO.Produto
{
    public record AddProdutoRequest(string nome, decimal preco, int estoque);
}