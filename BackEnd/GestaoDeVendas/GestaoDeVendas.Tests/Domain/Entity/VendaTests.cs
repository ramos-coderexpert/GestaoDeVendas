using FluentAssertions;
using GestaoDeVendas.Domain.Entity;
using Xunit;

namespace GestaoDeVendas.Tests.Domain.Entity
{
    public class VendaTests
    {
        [Theory]
        [InlineData(1, 100.00)]
        [InlineData(5, 19.99)]
        [InlineData(10, 9.99)]
        public void Constructor_ShouldInitializePropertiesCorrectly(int qtdProduto, decimal valorUnitario)
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();
            var valorTotalEsperado = qtdProduto * valorUnitario;

            // Act
            var venda = new Venda(qtdProduto, valorUnitario, clienteId, produtoId);

            // Assert
            venda.Id.Should().NotBe(Guid.Empty);
            venda.QtdProduto.Should().Be(qtdProduto);
            venda.ValorUnitario.Should().Be(valorUnitario);
            venda.ValorTotal.Should().Be(valorTotalEsperado);
            venda.DataVenda.Date.Should().Be(DateTime.Now.Date);
            venda.ClienteId.Should().Be(clienteId);
            venda.ProdutoId.Should().Be(produtoId);
        }

        [Fact]
        public void Constructor_ShouldCalculateValorTotalCorrectly()
        {
            // Arrange
            int qtdProduto = 3;
            decimal valorUnitario = 50.00m;
            decimal valorTotalEsperado = 150.00m;

            // Act
            var venda = new Venda(
                qtdProduto,
                valorUnitario,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            // Assert
            venda.ValorTotal.Should().Be(valorTotalEsperado);
        }

        [Theory]
        [InlineData(2, 75.00, 150.00)]    // Valores inteiros
        [InlineData(1, 19.99, 19.99)]     // Valores decimais
        [InlineData(10, 9.99, 99.90)]     // Múltiplos itens com decimal
        [InlineData(100, 0.99, 99.00)]    // Grande quantidade com valor baixo
        public void AtualizarVenda_ShouldUpdatePropertiesAndRecalculateTotal(
            int novaQtd,
            decimal novoValorUnitario,
            decimal valorTotalEsperado)
        {
            // Arrange
            var venda = new Venda(1, 100.00m, Guid.NewGuid(), Guid.NewGuid());

            // Act
            venda.AtualizarVenda(novaQtd, novoValorUnitario);

            // Assert
            venda.QtdProduto.Should().Be(novaQtd);
            venda.ValorUnitario.Should().Be(novoValorUnitario);
            venda.ValorTotal.Should().Be(valorTotalEsperado);
        }

        [Fact]
        public void AtualizarVenda_ShouldNotChangeOtherProperties()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();
            var venda = new Venda(1, 100.00m, clienteId, produtoId);
            var dataVendaOriginal = venda.DataVenda;

            // Act
            venda.AtualizarVenda(2, 150.00m);

            // Assert
            venda.ClienteId.Should().Be(clienteId);
            venda.ProdutoId.Should().Be(produtoId);
            venda.DataVenda.Should().Be(dataVendaOriginal);
        }

        [Theory]
        [InlineData(0, 100.00)]       // Quantidade zero
        [InlineData(-1, 100.00)]      // Quantidade negativa
        [InlineData(1, 0)]            // Valor unitário zero
        [InlineData(1, -10.00)]       // Valor unitário negativo
        [InlineData(-5, -10.00)]      // Ambos negativos
        public void Constructor_WithInvalidValues_ShouldAllowCreation(int qtdProduto, decimal valorUnitario)
        {
            // Arrange & Act
            var venda = new Venda(
                qtdProduto,
                valorUnitario,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            // Assert
            venda.QtdProduto.Should().Be(qtdProduto);
            venda.ValorUnitario.Should().Be(valorUnitario);
            venda.ValorTotal.Should().Be(qtdProduto * valorUnitario);
        }

        [Fact]
        public void Constructor_WithEmptyGuids_ShouldAllowCreation()
        {
            // Arrange & Act
            var venda = new Venda(1, 100.00m, Guid.Empty, Guid.Empty);

            // Assert
            venda.ClienteId.Should().Be(Guid.Empty);
            venda.ProdutoId.Should().Be(Guid.Empty);
        }

        [Theory]
        [InlineData(1, 100.00, 100.00)]           // Valor exato
        [InlineData(2, 19.99, 39.98)]            // Valor com decimais
        [InlineData(3, 33.33, 99.99)]            // Valor com arredondamento
        public void ValorTotal_ShouldBeCalculatedWithCorrectPrecision(
            int qtdProduto,
            decimal valorUnitario,
            decimal valorTotalEsperado)
        {
            // Arrange & Act
            var venda = new Venda(
                qtdProduto,
                valorUnitario,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            // Assert
            venda.ValorTotal.Should().Be(valorTotalEsperado);
        }
    }
}