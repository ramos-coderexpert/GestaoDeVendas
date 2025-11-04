using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeVendas.Infrastructure.Persistance.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(AppDbContext context) : base(context) { }

        public async Task<List<Cliente>> ObterTodosAsync()
        {
            return await GetAll();
        }

        public async Task<List<Cliente>> ObterTodosAtivosAsync()
        {
            return await _context.Clientes.AsNoTracking().Where(c => c.Ativo).ToListAsync();
        }

        public Cliente ObterPorId(Guid id)
        {
            return _context.Clientes.SingleOrDefault(c => c.Id == id && c.Ativo);
        }

        public Cliente ObterPorNome(string nome)
        {
            return _context.Clientes.FirstOrDefault(c => c.Nome == nome && c.Ativo);
        }

        public Cliente ObterPorEmailPassword(string email, string passwordHash)
        {
            return _context.Clientes.FirstOrDefault(c => c.Email == email && c.Password == passwordHash && c.Ativo);
        }
    }
}