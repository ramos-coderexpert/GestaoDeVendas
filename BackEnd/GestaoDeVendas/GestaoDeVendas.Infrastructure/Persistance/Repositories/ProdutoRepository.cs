using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeVendas.Infrastructure.Persistance.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context) { }

        public async Task<List<Produto>> ObterTodosAsync()
        {
            return await GetAll();
        }

        public async Task<List<Produto>> ObterTodosAtivosAsync()
        {
            return await _context.Produtos.AsNoTracking().Where(p => p.Ativo).ToListAsync();
        }

        public Produto ObterPorId(Guid id)
        {
            return _context.Produtos.SingleOrDefault(p => p.Id == id && p.Ativo);
        }

        public Produto ObterPorNome(string nome)
        {
            return _context.Produtos.FirstOrDefault(p => p.Nome == nome && p.Ativo);
        }
    }
}