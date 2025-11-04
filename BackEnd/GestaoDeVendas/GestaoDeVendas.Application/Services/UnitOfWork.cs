using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Infrastructure.Persistance.Context;
using GestaoDeVendas.Infrastructure.Persistance.Repositories;

namespace GestaoDeVendas.Application.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IClienteRepository Clientes { get; }
        public IProdutoRepository Produtos { get; }
        public IVendaRepository Vendas { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Clientes = new ClienteRepository(_context);
            Produtos = new ProdutoRepository(_context);
            Vendas = new VendaRepository(_context);
        }

        public void Commit() => _context.SaveChanges();

        public void Dispose() => _context.Dispose();
    }
}