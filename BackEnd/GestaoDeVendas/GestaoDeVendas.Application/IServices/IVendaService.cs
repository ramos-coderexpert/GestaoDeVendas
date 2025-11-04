using GestaoDeVendas.Application.DTO.Venda;

namespace GestaoDeVendas.Application.IServices
{
    public interface IVendaService
    {
        Task<List<GetVendaRequest>> ObterTodosAsync();
        GetVendaRequest ObterPorId(Guid id);
        List<GetVendaRequest> ObterPorNomeCliente(string nomeCliente);
        List<GetVendaRequest> ObterPorNomeProduto(string nomeProduto);
        Guid AddVenda(AddVendaRequest vendaRequest);
        void UpdateVenda(Guid id, UpdateVendaRequest VendaRequest);
    }
}