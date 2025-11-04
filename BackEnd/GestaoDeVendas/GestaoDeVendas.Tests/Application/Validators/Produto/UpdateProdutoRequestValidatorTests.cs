using FluentValidation.TestHelper;
using GestaoDeVendas.Application.DTO.Produto;
using GestaoDeVendas.Application.Validators.Produto;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Validators.Produto
{
    public class UpdateProdutoRequestValidatorTests
    {
        private readonly UpdateProdutoRequestValidator _validator;

        public UpdateProdutoRequestValidatorTests()
        {
            _validator = new UpdateProdutoRequestValidator();
        }

        #region Nome Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeIsValid()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Mouse Gamer", 150m, 50);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsEmpty()
        {
            // Arrange
            var request = new UpdateProdutoRequest("", 100m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome do produto é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsNull()
        {
            // Arrange
            var request = new UpdateProdutoRequest(null!, 100m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome do produto é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsWhitespace()
        {
            // Arrange
            var request = new UpdateProdutoRequest("   ", 100m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome do produto é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsTooShort()
        {
            // Arrange
            var request = new UpdateProdutoRequest("AB", 100m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome deve ter no mínimo 3 caracteres");
        }

        [Theory]
        [InlineData("RAM")]
        [InlineData("Teclado RGB")]
        [InlineData("Monitor LG UltraWide")]
        public void Validate_ShouldNotHaveError_WhenNomeHasValidLength(string nome)
        {
            // Arrange
            var request = new UpdateProdutoRequest(nome, 100m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsTooLong()
        {
            // Arrange
            var nomeLongo = new string('X', 101);
            var request = new UpdateProdutoRequest(nomeLongo, 100m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome deve ter no máximo 100 caracteres");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeHasExactly100Characters()
        {
            // Arrange
            var nome100 = new string('X', 100);
            var request = new UpdateProdutoRequest(nome100, 100m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeHasExactly3Characters()
        {
            // Arrange
            var request = new UpdateProdutoRequest("CPU", 2000m, 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeHas2Characters()
        {
            // Arrange
            var request = new UpdateProdutoRequest("PC", 1000m, 3);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
        }

        #endregion

        #region Preco Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenPrecoIsValid()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Teclado", 200m, 15);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPrecoIsZero()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Grátis", 0m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco)
                .WithErrorMessage("O preço deve ser maior que zero");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPrecoIsNegative()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Negativo", -50m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco)
                .WithErrorMessage("O preço deve ser maior que zero");
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(10)]
        [InlineData(250.50)]
        [InlineData(10000)]
        [InlineData(999999.98)]
        public void Validate_ShouldNotHaveError_WhenPrecoIsInValidRange(decimal preco)
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Válido", preco, 20);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPrecoIsEqualToMaxLimit()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Limite", 999999.99m, 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco)
                .WithErrorMessage("O preço deve ser menor que 999.999,99");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPrecoExceedsMaxLimit()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Caro Demais", 1000000m, 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco)
                .WithErrorMessage("O preço deve ser menor que 999.999,99");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenPrecoIsJustBelowMaxLimit()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Quase Limite", 999999.98m, 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenPrecoIsMinimumValid()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Barato", 0.01m, 100);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAcceptPrecoWithManyDecimalPlaces()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Decimal", 199.999999m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        #endregion

        #region Estoque Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenEstoqueIsValid()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Webcam", 300m, 25);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenEstoqueIsZero()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Esgotado", 150m, 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEstoqueIsNegative()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Estoque Negativo", 100m, -5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque)
                .WithErrorMessage("O estoque não pode ser negativo");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(999998)]
        public void Validate_ShouldNotHaveError_WhenEstoqueIsInValidRange(int estoque)
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Teste", 100m, estoque);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEstoqueIsEqualToMaxLimit()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Estoque Máximo", 50m, 999999);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque)
                .WithErrorMessage("O estoque deve ser menor que 999.999");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEstoqueExceedsMaxLimit()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Estoque Excedente", 50m, 1000000);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque)
                .WithErrorMessage("O estoque deve ser menor que 999.999");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenEstoqueIsJustBelowMaxLimit()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Grande Estoque", 50m, 999998);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-50)]
        [InlineData(-1000)]
        public void Validate_ShouldHaveError_WhenEstoqueIsNegativeValues(int estoque)
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Teste", 100m, estoque);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque)
                .WithErrorMessage("O estoque não pode ser negativo");
        }

        #endregion

        #region Integration Tests - Multiple Errors

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenAllFieldsAreInvalid()
        {
            // Arrange
            var request = new UpdateProdutoRequest("", -100m, -10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
            result.ShouldHaveValidationErrorFor(r => r.preco);
            result.ShouldHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenAllFieldsAreValid()
        {
            // Arrange
            var request = new UpdateProdutoRequest(
                nome: "MacBook Pro M3",
                preco: 15000m,
                estoque: 10
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenPriceAndStockExceedLimits()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Teste", 2000000m, 2000000);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco);
            result.ShouldHaveValidationErrorFor(r => r.estoque);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Validate_ShouldAcceptNomeWithSpecialCharacters()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Mouse RGB+ (2024)", 180m, 30);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithNumbers()
        {
            // Arrange
            var request = new UpdateProdutoRequest("RTX 4090 24GB", 12000m, 3);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithAccents()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Câmera Fotográfica", 4500m, 8);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithMultipleSpaces()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto     Com     Espaços", 500m, 15);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptVeryLowPrice()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Chiclete", 0.25m, 500);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAcceptVeryHighStock()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Clips de Papel", 0.05m, 800000);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        #endregion

        #region Specific UpdateProdutoRequest Tests

        [Fact]
        public void Validate_ShouldAllowPriceUpdate_ToHigherValue()
        {
            // Arrange - Simulando aumento de preço
            var request = new UpdateProdutoRequest("Produto Inflação", 500m, 20);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAllowPriceUpdate_ToLowerValue()
        {
            // Arrange - Simulando promoção
            var request = new UpdateProdutoRequest("Produto Promoção", 49.90m, 50);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAllowStockUpdate_ToZero()
        {
            // Arrange - Produto esgotado
            var request = new UpdateProdutoRequest("Produto Esgotado", 300m, 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldAllowStockIncrease()
        {
            // Arrange - Reposição de estoque
            var request = new UpdateProdutoRequest("Produto Reposto", 150m, 200);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldAllowNameChange()
        {
            // Arrange - Renomeação de produto
            var request = new UpdateProdutoRequest("Produto Novo Nome", 100m, 25);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAllowProductWithMinimalValidInfo()
        {
            // Arrange
            var request = new UpdateProdutoRequest("ABC", 0.01m, 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowProductWithMaximalValidInfo()
        {
            // Arrange
            var nome = new string('Z', 100);
            var request = new UpdateProdutoRequest(nome, 999999.98m, 999998);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Validate_ShouldAcceptPrecoAtLowerBoundary()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Mínimo", 0.01m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldRejectPrecoJustBelowLowerBoundary()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Abaixo", 0m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAcceptPrecoJustBelowUpperBoundary()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Quase Máximo", 999999.98m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldRejectPrecoAtUpperBoundary()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Máximo", 999999.99m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAcceptEstoqueAtLowerBoundary()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Zero", 100m, 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldRejectEstoqueBelowLowerBoundary()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Negativo", 100m, -1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldAcceptEstoqueJustBelowUpperBoundary()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Alto Estoque", 100m, 999998);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldRejectEstoqueAtUpperBoundary()
        {
            // Arrange
            var request = new UpdateProdutoRequest("Produto Estoque Limite", 100m, 999999);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque);
        }

        #endregion
    }
}