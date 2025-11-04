using GestaoDeVendas.Domain.IRepositories;

namespace GestaoDeVendas.Application.IServices
{
    public interface IUnitOfWork : IDisposable
    {
        IClienteRepository Clientes { get; }
        IProdutoRepository Produtos { get; }
        IVendaRepository Vendas { get; }

        void Commit();
    }
}