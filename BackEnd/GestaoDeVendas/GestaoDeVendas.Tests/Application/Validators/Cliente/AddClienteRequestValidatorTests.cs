using FluentValidation.TestHelper;
using GestaoDeVendas.Application.DTO.Cliente;
using GestaoDeVendas.Application.Validators.Cliente;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Validators.Cliente
{
    public class AddClienteRequestValidatorTests
    {
        private readonly AddClienteRequestValidator _validator;

        public AddClienteRequestValidatorTests()
        {
            _validator = new AddClienteRequestValidator();
        }

        #region Nome Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeIsValid()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeIsEmpty()
        {
            // Arrange
            var request = new AddClienteRequest("", "joao@email.com", "password123", "User", 1000m);

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
            var request = new AddClienteRequest(null!, "joao@email.com", "password123", "User", 1000m);

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
            var request = new AddClienteRequest("   ", "joao@email.com", "password123", "User", 1000m);

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
            var request = new AddClienteRequest("Jo", "joao@email.com", "password123", "User", 1000m);

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
            var request = new AddClienteRequest(nome, "email@test.com", "password123", "User", 500m);

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
            var request = new AddClienteRequest(nomeLongo, "joao@email.com", "password123", "User", 1000m);

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
            var request = new AddClienteRequest(nome100, "email@test.com", "password123", "User", 500m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        #endregion

        #region Email Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenEmailIsValid()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsEmpty()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "", "password123", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email)
                .WithErrorMessage("O email é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsNull()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", null!, "password123", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email)
                .WithErrorMessage("O email é obrigatório");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsWhitespace()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "   ", "password123", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email)
                .WithErrorMessage("O email é obrigatório");
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test.user@domain.com")]
        [InlineData("user+tag@example.co.uk")]
        public void Validate_ShouldNotHaveError_WhenEmailHasValidFormats(string email)
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", email, "password123", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        #endregion

        #region Password Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenPasswordIsValid()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordIsEmpty()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password)
                .WithErrorMessage("A senha é obrigatória");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordIsNull()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", null!, "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password)
                .WithErrorMessage("A senha é obrigatória");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordIsWhitespace()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "   ", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password)
                .WithErrorMessage("A senha é obrigatória");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordIsTooShort()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "12345", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password)
                .WithErrorMessage("A senha deve ter no mínimo 6 caracteres");
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("password")]
        [InlineData("SecurePass123!@#")]
        public void Validate_ShouldNotHaveError_WhenPasswordHasMinimumLength(string password)
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", password, "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
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
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", role, 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.role);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenRoleIsEmpty()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "", 1000m);

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
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", null!, 1000m);

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
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "   ", 1000m);

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
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", role, 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.role)
                .WithErrorMessage("O papel deve ser 'admin' ou 'user'");
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
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "User", saldo);

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
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "User", saldo);

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
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "User", 0m);

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
            var request = new AddClienteRequest("", "", "", "", -100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
            result.ShouldHaveValidationErrorFor(r => r.email);
            result.ShouldHaveValidationErrorFor(r => r.password);
            result.ShouldHaveValidationErrorFor(r => r.role);
            result.ShouldHaveValidationErrorFor(r => r.saldo);
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenAllFieldsAreValid()
        {
            // Arrange
            var request = new AddClienteRequest(
                nome: "João Pedro Silva",
                email: "joao.silva@example.com",
                password: "SecurePassword123",
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
            var request = new AddClienteRequest("", "email@test.com", "password123", "User", 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
            // Deve ter pelo menos um erro, mas NotEmpty é avaliado primeiro
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Validate_ShouldAcceptRoleWithMixedCase()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "AdMiN", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.role);
        }

        [Fact]
        public void Validate_ShouldAcceptNomeWithSpecialCharacters()
        {
            // Arrange
            var request = new AddClienteRequest("João D'Ávila-Santos", "joao@email.com", "password123", "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldAcceptLargeSaldoValue()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "password123", "User", 999999999.99m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.saldo);
        }

        [Fact]
        public void Validate_ShouldAcceptVeryLongPasswordWithinReason()
        {
            // Arrange
            var longPassword = new string('a', 100);
            var request = new AddClienteRequest("João Silva", "joao@email.com", longPassword, "User", 1000m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenNomeHasExactly3Characters()
        {
            // Arrange
            var request = new AddClienteRequest("Ana", "ana@email.com", "password123", "User", 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNomeHas2Characters()
        {
            // Arrange
            var request = new AddClienteRequest("An", "ana@email.com", "password123", "User", 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.nome);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenPasswordHasExactly6Characters()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "pass12", "User", 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordHas5Characters()
        {
            // Arrange
            var request = new AddClienteRequest("João Silva", "joao@email.com", "pass1", "User", 100m);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password);
        }

        #endregion
    }
}