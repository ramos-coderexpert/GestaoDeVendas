namespace GestaoDeVendas.Domain.Entity
{
    public class Cliente : DeactivatableEntity
    {
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }
        public decimal Saldo { get; private set; }
        public DateTime DataRegistro { get; private set; }

        public Cliente(string nome, string email, string password, string role, decimal saldo) : base(nome)
        {
            Email = email;
            Password = password;
            Role = role;
            Saldo = saldo;
            DataRegistro = DateTime.Now;
        }

        public void AtualizarRole(string role)
        {
            Role = role;
        }

        public void AtualizarSaldo(decimal novoSaldo)
        {
            Saldo = novoSaldo;
        }

        public void AumentarSaldo(decimal valor)
        {
            if (valor < 0)
                throw new ArgumentException("O valor a ser adicionado não pode ser negativo.", nameof(valor));

            Saldo += valor;
        }

        public void ReduzirSaldo(decimal valor)
        {
            if (valor < 0)
                throw new ArgumentException("O valor a ser reduzido não pode ser negativo.", nameof(valor));
            if (valor > Saldo)
                throw new InvalidOperationException("Saldo insuficiente para realizar a operação.");

            Saldo -= valor;
        }

        public bool ValidarSaldo(decimal valorNecessario)
        {
            return Saldo >= valorNecessario;
        }
    }
}