using GestaoDeVendas.Domain.Entity.Interfaces;

namespace GestaoDeVendas.Domain.Entity
{
    public abstract class DeactivatableEntity : BaseEntity, IDeactivatable
    {
        public string Nome { get; protected set; }
        public bool Ativo { get; protected set; }

        protected DeactivatableEntity(string nome) : base()
        {
            Nome = nome;
            Ativo = true;
        }

        public virtual void AtualizarNome(string novoNome)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new ArgumentException("Nome não pode ser vazio", nameof(novoNome));

            Nome = novoNome;
        }

        public virtual void Desativar()
        {
            Ativo = false;
        }
    }
}