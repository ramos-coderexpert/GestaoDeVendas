using GestaoDeVendas.Domain.Entity;

namespace GestaoDeVendas.Domain.IRepositories
{
    public interface IVendaRepository : IBaseRepository<Venda>
    {
        Task<List<Venda>> ObterTodosAsync();
        Venda ObterPorId(Guid id);
        List<Venda> ObterPorNomeCliente(string nomeCliente);
        List<Venda> ObterPorNomeProduto(string nomeProduto);
    }
}