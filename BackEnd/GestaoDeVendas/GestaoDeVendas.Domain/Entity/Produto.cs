namespace GestaoDeVendas.Domain.Entity
{
    public class Produto : DeactivatableEntity
    {
        public decimal Preco { get; private set; }
        public int Estoque { get; private set; }

        public Produto(string nome, decimal preco, int estoque) : base(nome)
        {
            Preco = preco;
            Estoque = estoque;
        }

        public void AtualizarPreco(decimal novoPreco)
        {
            if (novoPreco < 0)
                throw new ArgumentException("O preço não pode ser negativo.", nameof(novoPreco));

            Preco = novoPreco;
        }

        public void AtualizarEstoqueTotal(int novoEstoque)
        {
            if (novoEstoque < 0)
                throw new ArgumentException("O estoque não pode ser negativo.", nameof(novoEstoque));

            Estoque = novoEstoque;
        }

        public void AumentarEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade a ser adicionada deve ser maior que zero.", nameof(quantidade));

            Estoque += quantidade;
        }

        public void ReduzirEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade a ser reduzida deve ser maior que zero.", nameof(quantidade));
            if (quantidade > Estoque)
                throw new InvalidOperationException("Estoque insuficiente para a redução solicitada.");

            Estoque -= quantidade;
        }

        public bool ValidarEstoque(int qtdSolicitada)
        {
            return Estoque >= qtdSolicitada;
        }
    }
}