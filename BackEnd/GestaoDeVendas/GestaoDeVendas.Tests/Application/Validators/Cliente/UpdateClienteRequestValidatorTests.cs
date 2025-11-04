using FluentValidation.TestHelper;
using GestaoDeVendas.Application.DTO.Cliente;
using GestaoDeVendas.Application.Validators.Cliente;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Validators.Cliente
{
    public class UpdateClienteRequestValidatorTests
    {
        private readonly UpdateClienteRequestValidator _validator;

        public UpdateClienteRequestValidatorTests()
        {
            _validator = new UpdateClienteRequestValidator();
        }

        #region Nome Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeIsValid()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsEmpty()
        {
            // Arrange
            var request = new UpdateClienteRequest("", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsNull()
        {
            // Arrange
            var request = new UpdateClienteRequest(null!, "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsWhitespace()
        {
            // Arrange
            var request = new UpdateClienteRequest("   ", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsTooShort()
        {
            // Arrange
            var request = new UpdateClienteRequest("Jo", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome)
                .WithErrorMessage("O nome deve ter no mínimo 3 caracteres");
        }

        [Theory]
        [InlineData("Ana")]
        [InlineData("José Silva")]
        [InlineData("Maria Aparecida dos Santos")]
        public void Validate_ShouldNotHaveError_WhenNomeHasMinimumLength(string nome)
        {
            // Arrange
            var request = new UpdateClienteRequest(nome, "User", 500m);

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
            var request = new UpdateClienteRequest(nomeLongo, "User", 1000m);

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
            var request = new UpdateClienteRequest(nome100, "User", 500m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeHasExactly3Characters()
        {
            // Arrange
            var request = new UpdateClienteRequest("Ana", "User", 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeHas2Characters()
        {
            // Arrange
            var request = new UpdateClienteRequest("An", "User", 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
        }

        #endregion

        #region Role Tests

        [Theory]
        [InlineData("admin")]
        [InlineData("Admin")]
        [InlineData("ADMIN")]
        [InlineData("user")]
        [InlineData("User")]
        [InlineData("USER")]
        public void Validate_ShouldNotHaveError_WhenRoleIsValid(string role)
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", role, 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.role);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenRoleIsEmpty()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.role)
                .WithErrorMessage("O papel é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenRoleIsNull()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", null!, 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.role)
                .WithErrorMessage("O papel é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenRoleIsWhitespace()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "   ", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.role)
                .WithErrorMessage("O papel é obrigatório");
        }

        [Theory]
        [InlineData("SuperAdmin")]
        [InlineData("Manager")]
        [InlineData("Guest")]
        [InlineData("InvalidRole")]
        [InlineData("moderator")]
        public void Validate_ShouldHaveError_WhenRoleIsInvalid(string role)
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", role, 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.role)
                .WithErrorMessage("O papel deve ser 'admin' ou 'user'");
        }

        [Fact]
        public void Validate_ShouldAcceptRoleWithMixedCase()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "AdMiN", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.role);
        }

        #endregion

        #region Saldo Tests

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(1000.50)]
        [InlineData(999999.99)]
        public void Validate_ShouldNotHaveError_WhenSaldoIsValid(decimal saldo)
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", saldo);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.saldo);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(-0.01)]
        public void Validate_ShouldHaveError_WhenSaldoIsNegative(decimal saldo)
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", saldo);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.saldo)
                .WithErrorMessage("O saldo não pode ser negativo");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenSaldoIsZero()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", 0m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.saldo);
        }

        #endregion

        #region Integration Tests - Multiple Errors

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenAllFieldsAreInvalid()
        {
            // Arrange
            var request = new UpdateClienteRequest("", "", -100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
            result.ShouldHaveValidationErrorFor(r => r.role);
            result.ShouldHaveValidationErrorFor(r => r.saldo);
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenAllFieldsAreValid()
        {
            // Arrange
            var request = new UpdateClienteRequest(
                nome: "João Pedro Silva",
                role: "Admin",
                saldo: 5000m
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
            var request = new UpdateClienteRequest("", "User", 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Validate_ShouldAcceptNomeWithSpecialCharacters()
        {
            // Arrange
            var request = new UpdateClienteRequest("João D'Ávila-Santos", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptLargeSaldoValue()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", 999999999.99m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.saldo);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithNumbers()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva 3", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithMultipleSpaces()
        {
            // Arrange
            var request = new UpdateClienteRequest("João     Silva", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        #endregion

        #region Specific UpdateClienteRequest Tests

        [Fact]
        public void Validate_ShouldNotRequireEmail_InUpdateRequest()
        {
            // Arrange - UpdateClienteRequest não possui campo email
            var request = new UpdateClienteRequest("João Silva", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert - Deve validar sem erros pois email não é parte do UpdateRequest
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldNotRequirePassword_InUpdateRequest()
        {
            // Arrange - UpdateClienteRequest não possui campo password
            var request = new UpdateClienteRequest("João Silva", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert - Deve validar sem erros pois password não é parte do UpdateRequest
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowSaldoUpdate_ToZero()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", 0m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.saldo);
        }

        [Fact]
        public void Validate_ShouldAllowRoleChange_FromUserToAdmin()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "Admin", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.role);
        }

        [Fact]
        public void Validate_ShouldAllowRoleChange_FromAdminToUser()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.role);
        }

        [Fact]
        public void Validate_ShouldAllowNomeUpdate_WithDifferentLength()
        {
            // Arrange - Nomes de tamanhos muito diferentes
            var shortName = new UpdateClienteRequest("Ana", "User", 100m);
            var longName = new UpdateClienteRequest(new string('A', 100), "User", 100m);

            // Act
            var resultShort = _validator.TestValidate(shortName);
            var resultLong = _validator.TestValidate(longName);

            // Assert
            resultShort.ShouldNotHaveValidationErrorFor(r => r.nome);
            resultLong.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Validate_ShouldAcceptMinimumValidSaldo()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", 0.01m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.saldo);
        }

        [Fact]
        public void Validate_ShouldRejectJustBelowZeroSaldo()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", -0.01m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.saldo);
        }

        [Fact]
        public void Validate_ShouldAcceptSaldoWithManyDecimalPlaces()
        {
            // Arrange
            var request = new UpdateClienteRequest("João Silva", "User", 1234.56789m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.saldo);
        }

        #endregion
    }
}