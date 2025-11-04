using FluentAssertions;
using GestaoDeVendas.Domain.Entity;
using Xunit;

namespace GestaoDeVendas.Tests.Domain.Entity
{
    public class ClienteTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            string nome = "Cliente Teste";
            string email = "teste@email.com";
            string password = "senha123";
            string role = "User";
            decimal saldo = 100.00m;

            // Act
            var cliente = new Cliente(nome, email, password, role, saldo);

            // Assert
            cliente.Nome.Should().Be(nome);
            cliente.Email.Should().Be(email);
            cliente.Password.Should().Be(password);
            cliente.Role.Should().Be(role);
            cliente.Saldo.Should().Be(saldo);
            cliente.DataRegistro.Date.Should().Be(DateTime.Now.Date);
            cliente.Ativo.Should().BeTrue();
            cliente.Id.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Manager")]
        public void AtualizarRole_ShouldUpdateRole(string novaRole)
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);

            // Act
            cliente.AtualizarRole(novaRole);

            // Assert
            cliente.Role.Should().Be(novaRole);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(500.50)]
        [InlineData(1000)]
        public void AtualizarSaldo_ShouldUpdateBalance(decimal novoSaldo)
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);

            // Act
            cliente.AtualizarSaldo(novoSaldo);

            // Assert
            cliente.Saldo.Should().Be(novoSaldo);
        }

        [Theory]
        [InlineData(100, 200)]
        [InlineData(0.01, 100.01)]
        [InlineData(1000, 1100)]
        public void AumentarSaldo_WithValidAmount_ShouldIncreaseBalance(decimal valorAumento, decimal expectedSaldo)
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);

            // Act
            cliente.AumentarSaldo(valorAumento);

            // Assert
            cliente.Saldo.Should().Be(expectedSaldo);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-100)]
        public void AumentarSaldo_WithNegativeAmount_ShouldThrowArgumentException(decimal valorInvalido)
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);

            // Act
            var action = () => cliente.AumentarSaldo(valorInvalido);

            // Assert
            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("O valor a ser adicionado não pode ser negativo.*")
                .WithParameterName("valor");
        }

        [Theory]
        [InlineData(50, 50)]
        [InlineData(0.01, 99.99)]
        [InlineData(100, 0)]
        public void ReduzirSaldo_WithValidAmount_ShouldDecreaseBalance(decimal valorReducao, decimal expectedSaldo)
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);

            // Act
            cliente.ReduzirSaldo(valorReducao);

            // Assert
            cliente.Saldo.Should().Be(expectedSaldo);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-100)]
        public void ReduzirSaldo_WithNegativeAmount_ShouldThrowArgumentException(decimal valorInvalido)
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);

            // Act
            var action = () => cliente.ReduzirSaldo(valorInvalido);

            // Assert
            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("O valor a ser reduzido não pode ser negativo.*")
                .WithParameterName("valor");
        }

        [Fact]
        public void ReduzirSaldo_WithAmountGreaterThanBalance_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);
            decimal valorMaiorQueSaldo = 100.01m;

            // Act
            var action = () => cliente.ReduzirSaldo(valorMaiorQueSaldo);

            // Assert
            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Saldo insuficiente para realizar a operação.");
        }

        [Theory]
        [InlineData(100, true)]   // Valor igual ao saldo
        [InlineData(50, true)]    // Valor menor que o saldo
        [InlineData(100.01, false)] // Valor maior que o saldo
        [InlineData(0, true)]     // Valor zero
        public void ValidarSaldo_ShouldReturnExpectedResult(decimal valorNecessario, bool expectedResult)
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);

            // Act
            var result = cliente.ValidarSaldo(valorNecessario);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Desativar_ShouldSetAtivoToFalse()
        {
            // Arrange
            var cliente = new Cliente("Test", "test@email.com", "pwd123", "User", 100);

            // Act
            cliente.Desativar();

            // Assert
            cliente.Ativo.Should().BeFalse();
        }

        [Theory]
        [InlineData("Novo Nome")]
        [InlineData("Cliente Atualizado")]
        public void AtualizarNome_WithValidName_ShouldUpdateName(string novoNome)
        {
            // Arrange
            var cliente = new Cliente("Nome Original", "test@email.com", "pwd123", "User", 100);

            // Act
            cliente.AtualizarNome(novoNome);

            // Assert
            cliente.Nome.Should().Be(novoNome);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void AtualizarNome_WithInvalidName_ShouldThrowArgumentException(string nomeInvalido)
        {
            // Arrange
            var cliente = new Cliente("Nome Original", "test@email.com", "pwd123", "User", 100);

            // Act
            var action = () => cliente.AtualizarNome(nomeInvalido);

            // Assert
            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("Nome não pode ser vazio*")
                .WithParameterName("novoNome");
        }
    }
}