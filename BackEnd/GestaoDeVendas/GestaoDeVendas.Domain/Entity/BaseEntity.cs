namespace GestaoDeVendas.Domain.Entity
{
    public abstract class BaseEntity
    {
        public Guid Id { get; init; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}