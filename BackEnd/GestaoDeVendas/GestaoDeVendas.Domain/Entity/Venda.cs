namespace GestaoDeVendas.Domain.Entity
{
    public class Venda : BaseEntity
    {
        public int QtdProduto { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public decimal ValorTotal { get; private set; }
        public DateTime DataVenda { get; private set; }

        public Guid ClienteId { get; private set; }
        public virtual Cliente Cliente { get; private set; }

        public Guid ProdutoId { get; private set; }
        public virtual Produto Produto { get; private set; }

        public Venda(int qtdProduto, decimal valorUnitario, Guid clienteId, Guid produtoId) : base()
        {
            QtdProduto = qtdProduto;
            ValorUnitario = valorUnitario;
            ValorTotal = qtdProduto * valorUnitario;
            DataVenda = DateTime.Now;
            ClienteId = clienteId;
            ProdutoId = produtoId;
        }

        public void AtualizarVenda(int qtdProduto, decimal valorUnitario)
        {
            QtdProduto = qtdProduto;
            ValorUnitario = valorUnitario;
            ValorTotal = qtdProduto * valorUnitario;
        }
    }
}