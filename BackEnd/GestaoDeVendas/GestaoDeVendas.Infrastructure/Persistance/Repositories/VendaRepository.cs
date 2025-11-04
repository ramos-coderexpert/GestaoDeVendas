using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeVendas.Infrastructure.Persistance.Repositories
{
    public class VendaRepository : BaseRepository<Venda>, IVendaRepository
    {
        public VendaRepository(AppDbContext context) : base(context) { }

        public async Task<List<Venda>> ObterTodosAsync()
        {
            return await _context.Vendas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .ToListAsync();
        }

        public Venda ObterPorId(Guid id)
        {
            return _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .SingleOrDefault(v => v.Id == id);
        }

        public List<Venda> ObterPorNomeCliente(string nomeCliente)
        {
            return _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Where(v => v.Cliente.Nome == nomeCliente)
                .ToList();
        }

        public List<Venda> ObterPorNomeProduto(string nomeProduto)
        {
            return _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Where(v => v.Produto.Nome == nomeProduto)
                .ToList();
        }
    }
}