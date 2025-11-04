namespace GestaoDeVendas.Application.DTO.Venda
{
    public record AddVendaRequest(string nomeCliente, string nomeProduto, int qtdProduto);
}