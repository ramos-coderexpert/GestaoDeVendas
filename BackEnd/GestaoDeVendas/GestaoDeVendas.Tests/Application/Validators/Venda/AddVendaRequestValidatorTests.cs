using FluentValidation.TestHelper;
using GestaoDeVendas.Application.DTO.Venda;
using GestaoDeVendas.Application.Validators.Venda;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Validators.Venda
{
    public class AddVendaRequestValidatorTests
    {
        private readonly AddVendaRequestValidator _validator;

        public AddVendaRequestValidatorTests()
        {
            _validator = new AddVendaRequestValidator();
        }

        #region NomeCliente Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeClienteIsValid()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "Mouse Gamer", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeCliente);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeClienteIsEmpty()
        {
            // Arrange
            var request = new AddVendaRequest("", "Mouse Gamer", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente)
                .WithErrorMessage("O nome do cliente é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeClienteIsNull()
        {
            // Arrange
            var request = new AddVendaRequest(null!, "Mouse Gamer", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente)
                .WithErrorMessage("O nome do cliente é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeClienteIsWhitespace()
        {
            // Arrange
            var request = new AddVendaRequest("   ", "Mouse Gamer", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente)
                .WithErrorMessage("O nome do cliente é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeClienteIsTooShort()
        {
            // Arrange
            var request = new AddVendaRequest("AB", "Mouse Gamer", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente)
                .WithErrorMessage("O nome do cliente deve ter no mínimo 3 caracteres");
        }

        [Theory]
        [InlineData("Ana")]
        [InlineData("João Silva")]
        [InlineData("Maria Aparecida Santos")]
        public void Validate_ShouldNotHaveError_WhenNomeClienteHasValidLength(string nomeCliente)
        {
            // Arrange
            var request = new AddVendaRequest(nomeCliente, "Produto Teste", 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeCliente);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeClienteIsTooLong()
        {
            // Arrange
            var nomeLongo = new string('A', 101);
            var request = new AddVendaRequest(nomeLongo, "Mouse", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente)
                .WithErrorMessage("O nome do cliente deve ter no máximo 100 caracteres");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeClienteHasExactly100Characters()
        {
            // Arrange
            var nome100 = new string('A', 100);
            var request = new AddVendaRequest(nome100, "Mouse", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeCliente);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeClienteHasExactly3Characters()
        {
            // Arrange
            var request = new AddVendaRequest("Ana", "Teclado", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeCliente);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeClienteHas2Characters()
        {
            // Arrange
            var request = new AddVendaRequest("Jo", "Mouse", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente);
        }

        #endregion

        #region NomeProduto Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeProdutoIsValid()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "Notebook Dell", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeProdutoIsEmpty()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto)
                .WithErrorMessage("O nome do produto é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeProdutoIsNull()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", null!, 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto)
                .WithErrorMessage("O nome do produto é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeProdutoIsWhitespace()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "   ", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto)
                .WithErrorMessage("O nome do produto é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeProdutoIsTooShort()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "PC", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto)
                .WithErrorMessage("O nome do produto deve ter no mínimo 3 caracteres");
        }

        [Theory]
        [InlineData("SSD")]
        [InlineData("Mouse RGB")]
        [InlineData("Teclado Mecânico Gamer")]
        public void Validate_ShouldNotHaveError_WhenNomeProdutoHasValidLength(string nomeProduto)
        {
            // Arrange
            var request = new AddVendaRequest("Cliente Teste", nomeProduto, 3);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeProdutoIsTooLong()
        {
            // Arrange
            var nomeLongo = new string('B', 101);
            var request = new AddVendaRequest("João Silva", nomeLongo, 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto)
                .WithErrorMessage("O nome do produto deve ter no máximo 100 caracteres");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeProdutoHasExactly100Characters()
        {
            // Arrange
            var nome100 = new string('B', 100);
            var request = new AddVendaRequest("João Silva", nome100, 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeProdutoHasExactly3Characters()
        {
            // Arrange
            var request = new AddVendaRequest("Cliente", "RAM", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeProdutoHas2Characters()
        {
            // Arrange
            var request = new AddVendaRequest("Cliente", "HD", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto);
        }

        #endregion

        #region QtdProduto Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenQtdProdutoIsValid()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "Mouse", 5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenQtdProdutoIsZero()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "Mouse", 0);

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
            var request = new AddVendaRequest("João Silva", "Mouse", -5);

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
            var request = new AddVendaRequest("Cliente Teste", "Produto Teste", qtdProduto);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenQtdProdutoIsEqualToMaxLimit()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "Mouse", 999999);

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
            var request = new AddVendaRequest("João Silva", "Mouse", 1000000);

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
            var request = new AddVendaRequest("João Silva", "Mouse", 999998);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenQtdProdutoIsMinimumValid()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "Mouse", 1);

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
            var request = new AddVendaRequest("Cliente Teste", "Produto Teste", qtdProduto);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto)
                .WithErrorMessage("A quantidade deve ser maior que zero");
        }

        #endregion

        #region Integration Tests - Multiple Errors

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenAllFieldsAreInvalid()
        {
            // Arrange
            var request = new AddVendaRequest("", "", -5);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente);
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto);
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenAllFieldsAreValid()
        {
            // Arrange
            var request = new AddVendaRequest(
                nomeCliente: "Carlos Eduardo Silva",
                nomeProduto: "Notebook Dell Inspiron 15",
                qtdProduto: 2
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenNamesAreTooShort()
        {
            // Arrange
            var request = new AddVendaRequest("AB", "CD", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente);
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto);
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenNamesAreTooLong()
        {
            // Arrange
            var nomeLongo = new string('X', 101);
            var request = new AddVendaRequest(nomeLongo, nomeLongo, 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nomeCliente);
            result.ShouldHaveValidationErrorFor(r => r.nomeProduto);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Validate_ShouldAcceptNomeClienteWithSpecialCharacters()
        {
            // Arrange
            var request = new AddVendaRequest("João D'Ávila-Santos", "Mouse", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeCliente);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeProdutoWithNumbers()
        {
            // Arrange
            var request = new AddVendaRequest("Cliente Teste", "RTX 4090 24GB", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeProdutoWithAccents()
        {
            // Arrange
            var request = new AddVendaRequest("João Silva", "Câmera Fotográfica", 2);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
        }

        [Fact]
        public void Validate_ShouldAcceptNamesWithMultipleSpaces()
        {
            // Arrange
            var request = new AddVendaRequest("João     Silva", "Mouse     Gamer", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nomeCliente);
            result.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
        }

        [Fact]
        public void Validate_ShouldAcceptVeryHighQuantity()
        {
            // Arrange
            var request = new AddVendaRequest("Empresa Atacado", "Parafuso M3", 500000);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        #endregion

        #region Specific AddVendaRequest Tests

        [Fact]
        public void Validate_ShouldAllowSingleItemPurchase()
        {
            // Arrange
            var request = new AddVendaRequest("Cliente Individual", "Notebook", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldAllowBulkPurchase()
        {
            // Arrange
            var request = new AddVendaRequest("Empresa Distribuidor", "Mouse USB", 10000);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowVendaWithMinimalValidInfo()
        {
            // Arrange
            var request = new AddVendaRequest("Ana", "SSD", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowVendaWithMaximalValidInfo()
        {
            // Arrange
            var nomeCliente = new string('A', 100);
            var nomeProduto = new string('B', 100);
            var request = new AddVendaRequest(nomeCliente, nomeProduto, 999998);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowCommonBusinessScenario()
        {
            // Arrange - Cenário comum de venda
            var request = new AddVendaRequest(
                nomeCliente: "Maria Aparecida dos Santos",
                nomeProduto: "Mouse Logitech MX Master 3",
                qtdProduto: 2
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
            var request = new AddVendaRequest("Cliente", "Produto", 1);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldRejectQtdProdutoJustBelowLowerBoundary()
        {
            // Arrange
            var request = new AddVendaRequest("Cliente", "Produto", 0);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldAcceptQtdProdutoJustBelowUpperBoundary()
        {
            // Arrange
            var request = new AddVendaRequest("Cliente", "Produto", 999998);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldRejectQtdProdutoAtUpperBoundary()
        {
            // Arrange
            var request = new AddVendaRequest("Cliente", "Produto", 999999);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.qtdProduto);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeClienteAtBoundaries()
        {
            // Arrange - 3 caracteres (mínimo)
            var request1 = new AddVendaRequest("Ana", "Produto", 1);
            // 100 caracteres (máximo)
            var nome100 = new string('X', 100);
            var request2 = new AddVendaRequest(nome100, "Produto", 1);

            // Act
            var result1 = _validator.TestValidate(request1);
            var result2 = _validator.TestValidate(request2);

            // Assert
            result1.ShouldNotHaveValidationErrorFor(r => r.nomeCliente);
            result2.ShouldNotHaveValidationErrorFor(r => r.nomeCliente);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeProdutoAtBoundaries()
        {
            // Arrange - 3 caracteres (mínimo)
            var request1 = new AddVendaRequest("Cliente", "SSD", 1);
            // 100 caracteres (máximo)
            var nome100 = new string('Y', 100);
            var request2 = new AddVendaRequest("Cliente", nome100, 1);

            // Act
            var result1 = _validator.TestValidate(request1);
            var result2 = _validator.TestValidate(request2);

            // Assert
            result1.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
            result2.ShouldNotHaveValidationErrorFor(r => r.nomeProduto);
        }

        #endregion
    }
}