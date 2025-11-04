namespace GestaoDeVendas.Domain.Exceptions
{
    public class VendaException : Exception
    {
        public VendaException(string message) : base(message) { }
    }

    public class EstoqueInsuficienteException : VendaException
    {
        public int EstoqueDisponivel { get; }
        public int QuantidadeSolicitada { get; }

        public EstoqueInsuficienteException(int estoqueDisponivel, int quantidadeSolicitada)
            : base($"Estoque insuficiente. Disponível: {estoqueDisponivel}, Solicitado: {quantidadeSolicitada}")
        {
            EstoqueDisponivel = estoqueDisponivel;
            QuantidadeSolicitada = quantidadeSolicitada;
        }
    }

    public class SaldoInsuficienteException : VendaException
    {
        public decimal SaldoDisponivel { get; }
        public decimal ValorNecessario { get; }

        public SaldoInsuficienteException(decimal saldoDisponivel, decimal valorNecessario)
            : base($"Saldo insuficiente. Disponível: R${saldoDisponivel}, Necessário: R${valorNecessario}")
        {
            SaldoDisponivel = saldoDisponivel;
            ValorNecessario = valorNecessario;
        }
    }

    public class VendaNaoEncontradaException : VendaException
    {
        public Guid? VendaId { get; }

        public VendaNaoEncontradaException(Guid id)
            : base($"Venda não encontrado com o Id: {id}")
        {
            VendaId = id;
        }

        public VendaNaoEncontradaException() : base("Venda não encontrado") { }
    }

    public class VendaInvalidaException : VendaException
    {
        public VendaInvalidaException(string message) : base(message) { }
    }
}