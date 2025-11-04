namespace GestaoDeVendas.Domain.Entity.Interfaces
{
    public interface IDeactivatable
    {
        public string Nome { get; }
        public bool Ativo { get; }
        public void AtualizarNome(string novoNome);
        public void Desativar();
    }
}