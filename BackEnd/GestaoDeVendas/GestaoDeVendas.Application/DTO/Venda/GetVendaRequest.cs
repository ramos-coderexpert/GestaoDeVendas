namespace GestaoDeVendas.Application.DTO.Venda
{
    public record class GetVendaRequest(Guid id, int qtdProduto, decimal valorUnitario, decimal valorTotal, DateTime dataVenda, string nomeCliente, string nomeProduto);
}