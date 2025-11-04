using System.Linq.Expressions;

namespace GestaoDeVendas.Domain.IRepositories
{
    public interface IBaseRepository<T>
    {
        Task<List<T>> GetAll();
        T GetById(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
    }
}