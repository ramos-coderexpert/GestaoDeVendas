using FluentValidation.TestHelper;
using GestaoDeVendas.Application.DTO.Venda;
using GestaoDeVendas.Application.Validators.Venda;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Validators.Venda
{
    public class UpdateVendaRequestValidatorTests
    {
        private readonly UpdateVendaRequestValidator _validator;

        public UpdateVendaRequestValidatorTests()
        {
            _validator = new UpdateVendaRequestValidator();
        }

        #region QtdProduto Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenQtdProdutoIsValid()
        {
            // Arrange
            var request = new UpdateVendaRequest(5, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenQtdProdutoIsZero()
        {
            // Arrange
            var request = new UpdateVendaRequest(0, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto)
                .WithErrorMessage("A quantidade deve ser maior que zero");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenQtdProdutoIsNegative()
        {
            // Arrange
            var request = new UpdateVendaRequest(-5, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto)
                .WithErrorMessage("A quantidade deve ser maior que zero");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(999998)]
        public void Validate_ShouldNotHaveError_WhenQtdProdutoIsInValidRange(int qtdProduto)
        {
            // Arrange
            var request = new UpdateVendaRequest(qtdProduto, 50m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenQtdProdutoIsEqualToMaxLimit()
        {
            // Arrange
            var request = new UpdateVendaRequest(999999, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto)
                .WithErrorMessage("A quantidade deve ser menor que 999.999");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenQtdProdutoExceedsMaxLimit()
        {
            // Arrange
            var request = new UpdateVendaRequest(1000000, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto)
                .WithErrorMessage("A quantidade deve ser menor que 999.999");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenQtdProdutoIsJustBelowMaxLimit()
        {
            // Arrange
            var request = new UpdateVendaRequest(999998, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenQtdProdutoIsMinimumValid()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-100)]
        public void Validate_ShouldHaveError_WhenQtdProdutoIsNegativeValues(int qtdProduto)
        {
            // Arrange
            var request = new UpdateVendaRequest(qtdProduto, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto)
                .WithErrorMessage("A quantidade deve ser maior que zero");
        }

        #endregion

        #region ValorUnitario Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenValorUnitarioIsValid()
        {
            // Arrange
            var request = new UpdateVendaRequest(2, 150.50m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenValorUnitarioIsZero()
        {
            // Arrange
            var request = new UpdateVendaRequest(2, 0m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario)
                .WithErrorMessage("O valor unitário deve ser maior que zero");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenValorUnitarioIsNegative()
        {
            // Arrange
            var request = new UpdateVendaRequest(2, -50m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario)
                .WithErrorMessage("O valor unitário deve ser maior que zero");
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(1)]
        [InlineData(10.50)]
        [InlineData(500)]
        [InlineData(999999.98)]
        public void Validate_ShouldNotHaveError_WhenValorUnitarioIsInValidRange(decimal valorUnitario)
        {
            // Arrange
            var request = new UpdateVendaRequest(5, valorUnitario);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenValorUnitarioIsEqualToMaxLimit()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 999999.99m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario)
                .WithErrorMessage("O valor unitário deve ser menor que 999.999,99");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenValorUnitarioExceedsMaxLimit()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 1000000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario)
                .WithErrorMessage("O valor unitário deve ser menor que 999.999,99");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenValorUnitarioIsJustBelowMaxLimit()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 999999.98m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenValorUnitarioIsMinimumValid()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 0.01m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldAcceptValorUnitarioWithManyDecimalPlaces()
        {
            // Arrange
            var request = new UpdateVendaRequest(5, 123.456789m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-10)]
        [InlineData(-100.50)]
        public void Validate_ShouldHaveError_WhenValorUnitarioIsNegativeValues(decimal valorUnitario)
        {
            // Arrange
            var request = new UpdateVendaRequest(5, valorUnitario);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario)
                .WithErrorMessage("O valor unitário deve ser maior que zero");
        }

        #endregion

        #region Integration Tests - Multiple Errors

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenAllFieldsAreInvalid()
        {
            // Arrange
            var request = new UpdateVendaRequest(-5, -100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto);
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenAllFieldsAreValid()
        {
            // Arrange
            var request = new UpdateVendaRequest(
                qtdProduto: 10,
                valorUnitario: 250.75m
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenBothFieldsAreZero()
        {
            // Arrange
            var request = new UpdateVendaRequest(0, 0m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto);
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenBothFieldsExceedLimits()
        {
            // Arrange
            var request = new UpdateVendaRequest(1000000, 1000000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto);
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Validate_ShouldAcceptVeryLowPrice()
        {
            // Arrange
            var request = new UpdateVendaRequest(1000, 0.05m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldAcceptVeryHighQuantity()
        {
            // Arrange
            var request = new UpdateVendaRequest(500000, 1.50m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldAcceptHighPriceWithLowQuantity()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 50000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAcceptLowPriceWithHighQuantity()
        {
            // Arrange
            var request = new UpdateVendaRequest(100000, 0.50m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        #endregion

        #region Specific UpdateVendaRequest Tests

        [Fact]
        public void Validate_ShouldAllowPriceIncrease()
        {
            // Arrange - Simulando aumento de preço
            var request = new UpdateVendaRequest(5, 350m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldAllowPriceDecrease()
        {
            // Arrange - Simulando desconto/promoção
            var request = new UpdateVendaRequest(5, 80m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldAllowQuantityIncrease()
        {
            // Arrange - Cliente aumentando quantidade
            var request = new UpdateVendaRequest(10, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldAllowQuantityDecrease()
        {
            // Arrange - Cliente reduzindo quantidade
            var request = new UpdateVendaRequest(2, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldAllowUpdateToSingleItem()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 500m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowUpdateWithMinimalValidValues()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 0.01m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowUpdateWithMaximalValidValues()
        {
            // Arrange
            var request = new UpdateVendaRequest(999998, 999999.98m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowTypicalBusinessScenario()
        {
            // Arrange - Cenário comum: ajuste de quantidade e preço
            var request = new UpdateVendaRequest(
                qtdProduto: 3,
                valorUnitario: 149.90m
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Validate_ShouldAcceptQtdProdutoAtLowerBoundary()
        {
            // Arrange
            var request = new UpdateVendaRequest(1, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldRejectQtdProdutoJustBelowLowerBoundary()
        {
            // Arrange
            var request = new UpdateVendaRequest(0, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldAcceptQtdProdutoJustBelowUpperBoundary()
        {
            // Arrange
            var request = new UpdateVendaRequest(999998, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldRejectQtdProdutoAtUpperBoundary()
        {
            // Arrange
            var request = new UpdateVendaRequest(999999, 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldAcceptValorUnitarioAtLowerBoundary()
        {
            // Arrange
            var request = new UpdateVendaRequest(5, 0.01m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldRejectValorUnitarioJustBelowLowerBoundary()
        {
            // Arrange
            var request = new UpdateVendaRequest(5, 0m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldAcceptValorUnitarioJustBelowUpperBoundary()
        {
            // Arrange
            var request = new UpdateVendaRequest(5, 999999.98m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.valorUnitario);
        }

        [Fact]
        public void Validate_ShouldRejectValorUnitarioAtUpperBoundary()
        {
            // Arrange
            var request = new UpdateVendaRequest(5, 999999.99m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.valorUnitario);
        }

        #endregion

        #region Real World Scenarios

        [Fact]
        public void Validate_ShouldAllowRetailPurchase()
        {
            // Arrange - Venda no varejo
            var request = new UpdateVendaRequest(2, 299.90m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowWholesalePurchase()
        {
            // Arrange - Venda no atacado
            var request = new UpdateVendaRequest(5000, 15.50m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowBulkDiscountScenario()
        {
            // Arrange - Grande quantidade com desconto
            var request = new UpdateVendaRequest(10000, 8.75m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowPremiumProductScenario()
        {
            // Arrange - Produto premium com preço alto
            var request = new UpdateVendaRequest(1, 25000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        #endregion
    }
}