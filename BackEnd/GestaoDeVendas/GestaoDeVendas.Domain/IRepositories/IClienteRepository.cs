using GestaoDeVendas.Domain.Entity;

namespace GestaoDeVendas.Domain.IRepositories
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Task<List<Cliente>> ObterTodosAsync();
        Task<List<Cliente>> ObterTodosAtivosAsync();
        Cliente ObterPorId(Guid id);
        Cliente ObterPorNome(string nome);
        Cliente ObterPorEmailPassword(string email, string passwordHash);
    }
}