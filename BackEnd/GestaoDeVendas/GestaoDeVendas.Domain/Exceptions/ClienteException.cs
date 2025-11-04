namespace GestaoDeVendas.Domain.Exceptions
{
    public class ClienteException : Exception
    {
        public ClienteException(string message) : base(message) { }
    }

    public class ClienteNaoEncontradoException : ClienteException
    {
        public Guid? ClienteId { get; }
        public string? Nome { get; }

        public ClienteNaoEncontradoException(Guid id)
            : base($"Cliente não encontrado com o Id: {id}")
        {
            ClienteId = id;
        }

        public ClienteNaoEncontradoException(string nome)
            : base($"Cliente não encontrado com o nome: {nome}")
        {
            Nome = nome;
        }

        public ClienteNaoEncontradoException() : base("Cliente não encontrado") { }
    }

    public class CredenciaisInvalidasException : ClienteException
    {
        public string? Email { get; }

        public CredenciaisInvalidasException(string email)
             : base($"Cliente não encontrado com o email: {email}")
        {
            Email = email;
        }
    }

    public class ClienteInvalidoException : ClienteException
    {
        public ClienteInvalidoException(string message) : base(message) { }
    }
}