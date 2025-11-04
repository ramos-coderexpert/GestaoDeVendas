namespace GestaoDeVendas.Domain.Exceptions
{
    public class ProdutoException : Exception
    {
        public ProdutoException(string message) : base(message) { }
    }

    public class ProdutoNaoEncontradoException : ProdutoException
    {
        public Guid? ProdutoId { get; }
        public string? Nome { get; }

        public ProdutoNaoEncontradoException(Guid id)
            : base($"Produto não encontrado com o Id: {id}")
        {
            ProdutoId = id;
        }

        public ProdutoNaoEncontradoException(string nome)
            : base($"Produto não encontrado com o nome: {nome}")
        {
            Nome = nome;
        }
    }

    public class ProdutoInvalidoException : ProdutoException
    {
        public ProdutoInvalidoException(string message) : base(message) { }
    }
}