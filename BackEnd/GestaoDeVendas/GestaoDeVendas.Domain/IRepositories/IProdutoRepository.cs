using GestaoDeVendas.Domain.Entity;

namespace GestaoDeVendas.Domain.IRepositories
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Task<List<Produto>> ObterTodosAsync();
        Task<List<Produto>> ObterTodosAtivosAsync();
        Produto ObterPorId(Guid id);
        Produto ObterPorNome(string nome);
    }
}