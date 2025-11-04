using FluentAssertions;
using GestaoDeVendas.Domain.Entity;
using Xunit;

namespace GestaoDeVendas.Tests.Domain.Entity
{
    public class ProdutoTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            string nome = "Produto Teste";
            decimal preco = 10.99m;
            int estoque = 100;

            // Act
            var produto = new Produto(nome, preco, estoque);

            // Assert
            produto.Nome.Should().Be(nome);
            produto.Preco.Should().Be(preco);
            produto.Estoque.Should().Be(estoque);
            produto.Ativo.Should().BeTrue();
            produto.Id.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineData(1.99)]
        [InlineData(0)]
        [InlineData(999999.99)]
        public void AtualizarPreco_WithValidPrice_ShouldUpdatePrice(decimal novoPreco)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            produto.AtualizarPreco(novoPreco);

            // Assert
            produto.Preco.Should().Be(novoPreco);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-100)]
        public void AtualizarPreco_WithNegativePrice_ShouldThrowArgumentException(decimal novoPreco)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            var action = () => produto.AtualizarPreco(novoPreco);

            // Assert
            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("O preço não pode ser negativo.*")
                .WithParameterName("novoPreco");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(1000)]
        public void AtualizarEstoqueTotal_WithValidAmount_ShouldUpdateStock(int novoEstoque)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            produto.AtualizarEstoqueTotal(novoEstoque);

            // Assert
            produto.Estoque.Should().Be(novoEstoque);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void AtualizarEstoqueTotal_WithNegativeAmount_ShouldThrowArgumentException(int novoEstoque)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            var action = () => produto.AtualizarEstoqueTotal(novoEstoque);

            // Assert
            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("O estoque não pode ser negativo.*")
                .WithParameterName("novoEstoque");
        }

        [Theory]
        [InlineData(1, 101)]
        [InlineData(50, 150)]
        [InlineData(1000, 1100)]
        public void AumentarEstoque_WithValidAmount_ShouldIncreaseStock(int quantidade, int expectedEstoque)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            produto.AumentarEstoque(quantidade);

            // Assert
            produto.Estoque.Should().Be(expectedEstoque);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AumentarEstoque_WithInvalidAmount_ShouldThrowArgumentException(int quantidade)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            var action = () => produto.AumentarEstoque(quantidade);

            // Assert
            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("A quantidade a ser adicionada deve ser maior que zero.*")
                .WithParameterName("quantidade");
        }

        [Theory]
        [InlineData(50, 50)]
        [InlineData(1, 99)]
        [InlineData(100, 0)]
        public void ReduzirEstoque_WithValidAmount_ShouldDecreaseStock(int quantidade, int expectedEstoque)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            produto.ReduzirEstoque(quantidade);

            // Assert
            produto.Estoque.Should().Be(expectedEstoque);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReduzirEstoque_WithInvalidAmount_ShouldThrowArgumentException(int quantidade)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            var action = () => produto.ReduzirEstoque(quantidade);

            // Assert
            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("A quantidade a ser reduzida deve ser maior que zero.*")
                .WithParameterName("quantidade");
        }

        [Fact]
        public void ReduzirEstoque_WithAmountGreaterThanStock_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);
            int quantidade = 101;

            // Act
            var action = () => produto.ReduzirEstoque(quantidade);

            // Assert
            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Estoque insuficiente para a redução solicitada.");
        }

        [Theory]
        [InlineData(100, true)]  // Quantidade igual ao estoque
        [InlineData(50, true)]   // Quantidade menor que o estoque
        [InlineData(101, false)] // Quantidade maior que o estoque
        [InlineData(0, true)]    // Quantidade zero
        public void ValidarEstoque_ShouldReturnExpectedResult(int qtdSolicitada, bool expectedResult)
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            var result = produto.ValidarEstoque(qtdSolicitada);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Desativar_ShouldSetAtivoToFalse()
        {
            // Arrange
            var produto = new Produto("Produto", 10.99m, 100);

            // Act
            produto.Desativar();

            // Assert
            produto.Ativo.Should().BeFalse();
        }

        [Theory]
        [InlineData("Novo Nome")]
        [InlineData("Outro Produto")]
        public void AtualizarNome_WithValidName_ShouldUpdateName(string novoNome)
        {
            // Arrange
            var produto = new Produto("Produto Original", 10.99m, 100);

            // Act
            produto.AtualizarNome(novoNome);

            // Assert
            produto.Nome.Should().Be(novoNome);
        }
    }
}