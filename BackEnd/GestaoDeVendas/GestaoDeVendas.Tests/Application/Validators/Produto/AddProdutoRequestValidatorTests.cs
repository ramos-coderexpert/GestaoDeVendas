using FluentValidation.TestHelper;
using GestaoDeVendas.Application.DTO.Produto;
using GestaoDeVendas.Application.Validators.Produto;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Validators.Produto
{
    public class AddProdutoRequestValidatorTests
    {
        private readonly AddProdutoRequestValidator _validator;

        public AddProdutoRequestValidatorTests()
        {
            _validator = new AddProdutoRequestValidator();
        }

        #region Nome Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeIsValid()
        {
            // Arrange
            var request = new AddProdutoRequest("Notebook Dell", 2500m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsEmpty()
        {
            // Arrange
            var request = new AddProdutoRequest("", 100m, 5);

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
            var request = new AddProdutoRequest(null!, 100m, 5);

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
            var request = new AddProdutoRequest("   ", 100m, 5);

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
            var request = new AddProdutoRequest("PC", 100m, 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome deve ter no mínimo 3 caracteres");
        }

        [Theory]
        [InlineData("SSD")]
        [InlineData("Mouse Gamer")]
        [InlineData("Teclado Mecânico RGB")]
        public void Validate_ShouldNotHaveError_WhenNomeHasMinimumLength(string nome)
        {
            // Arrange
            var request = new AddProdutoRequest(nome, 100m, 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsTooLong()
        {
            // Arrange
            var nomeLongo = new string('A', 101); // 101 caracteres
            var request = new AddProdutoRequest(nomeLongo, 100m, 5);

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
            var nome100 = new string('A', 100); // Exatamente 100 caracteres
            var request = new AddProdutoRequest(nome100, 100m, 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeHasExactly3Characters()
        {
            // Arrange
            var request = new AddProdutoRequest("GPU", 1500m, 3);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeHas2Characters()
        {
            // Arrange
            var request = new AddProdutoRequest("HD", 300m, 10);

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
            var request = new AddProdutoRequest("Mouse", 50m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPrecoIsZero()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Grátis", 0m, 10);

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
            var request = new AddProdutoRequest("Produto Negativo", -10m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco)
                .WithErrorMessage("O preço deve ser maior que zero");
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(1)]
        [InlineData(100.50)]
        [InlineData(5000)]
        [InlineData(999999.98)]
        public void Validate_ShouldNotHaveError_WhenPrecoIsInValidRange(decimal preco)
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Teste", preco, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPrecoIsEqualToMaxLimit()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Caro", 999999.99m, 10);

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
            var request = new AddProdutoRequest("Produto Muito Caro", 1000000m, 10);

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
            var request = new AddProdutoRequest("Produto Quase no Limite", 999999.98m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenPrecoIsMinimumValid()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Barato", 0.01m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAcceptPrecoWithManyDecimalPlaces()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Decimal", 123.456789m, 10);

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
            var request = new AddProdutoRequest("Mouse", 50m, 100);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenEstoqueIsZero()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Sem Estoque", 50m, 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEstoqueIsNegative()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Estoque Negativo", 50m, -1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque)
                .WithErrorMessage("O estoque não pode ser negativo");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(1000)]
        [InlineData(999998)]
        public void Validate_ShouldNotHaveError_WhenEstoqueIsInValidRange(int estoque)
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Teste", 100m, estoque);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEstoqueIsEqualToMaxLimit()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Estoque Máximo", 50m, 999999);

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
            var request = new AddProdutoRequest("Produto Estoque Excessivo", 50m, 1000000);

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
            var request = new AddProdutoRequest("Produto Grande Estoque", 50m, 999998);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-100)]
        public void Validate_ShouldHaveError_WhenEstoqueIsNegativeValues(int estoque)
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Teste", 100m, estoque);

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
            var request = new AddProdutoRequest("", -10m, -5);

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
            var request = new AddProdutoRequest(
                nome: "Notebook Dell Inspiron 15",
                preco: 3500.50m,
                estoque: 25
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_ForSingleField_WhenMultipleRulesViolated()
        {
            // Arrange - Nome vazio viola NotEmpty e MinimumLength
            var request = new AddProdutoRequest("", 100m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenPriceAndStockExceedLimits()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Teste", 1000000m, 1000000);

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
            var request = new AddProdutoRequest("Mouse Gamer RGB+ (Premium)", 150m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithNumbers()
        {
            // Arrange
            var request = new AddProdutoRequest("SSD 500GB M.2 NVMe", 400m, 15);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithAccents()
        {
            // Arrange
            var request = new AddProdutoRequest("Teclado Mecânico Óptico", 350m, 20);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithMultipleSpaces()
        {
            // Arrange
            var request = new AddProdutoRequest("Notebook     Dell     Inspiron", 3000m, 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptVeryLowPrice()
        {
            // Arrange
            var request = new AddProdutoRequest("Cabo USB", 0.50m, 1000);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAcceptVeryHighStock()
        {
            // Arrange
            var request = new AddProdutoRequest("Parafuso M3", 0.10m, 500000);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        #endregion

        #region Specific AddProdutoRequest Tests

        [Fact]
        public void Validate_ShouldAllowProductWithZeroStock_ForPreOrder()
        {
            // Arrange - Produto em pré-venda pode ter estoque zero
            var request = new AddProdutoRequest("PlayStation 5 Pro", 4500m, 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldAllowVeryExpensiveProduct()
        {
            // Arrange - Produto de luxo com preço alto mas válido
            var request = new AddProdutoRequest("Workstation Dell Precision", 50000m, 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAllowProductWithMinimalInfo()
        {
            // Arrange - Produto com informações mínimas mas válidas
            var request = new AddProdutoRequest("ABC", 0.01m, 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowProductWithMaximalValidInfo()
        {
            // Arrange - Produto com informações máximas mas válidas
            var nome = new string('A', 100);
            var request = new AddProdutoRequest(nome, 999999.98m, 999998);

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
            var request = new AddProdutoRequest("Produto Limite Inferior", 0.01m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldRejectPrecoJustBelowLowerBoundary()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Abaixo Limite", 0m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAcceptPrecoJustBelowUpperBoundary()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Limite Superior", 999999.98m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldRejectPrecoAtUpperBoundary()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto No Limite", 999999.99m, 10);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.preco);
        }

        [Fact]
        public void Validate_ShouldAcceptEstoqueAtLowerBoundary()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Estoque Zero", 100m, 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldRejectEstoqueBelowLowerBoundary()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Estoque Negativo", 100m, -1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldAcceptEstoqueJustBelowUpperBoundary()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Grande Estoque", 100m, 999998);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.estoque);
        }

        [Fact]
        public void Validate_ShouldRejectEstoqueAtUpperBoundary()
        {
            // Arrange
            var request = new AddProdutoRequest("Produto Estoque Limite", 100m, 999999);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.estoque);
        }

        #endregion
    }
}