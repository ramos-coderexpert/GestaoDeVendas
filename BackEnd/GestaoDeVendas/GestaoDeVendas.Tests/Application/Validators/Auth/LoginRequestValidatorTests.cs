using FluentValidation.TestHelper;
using GestaoDeVendas.Application.DTO.Auth;
using GestaoDeVendas.Application.Validators.Auth;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Validators.Auth
{
    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator;

        public LoginRequestValidatorTests()
        {
            _validator = new LoginRequestValidator();
        }

        #region Email Tests

        [Fact]
        public void Validate_ShouldNotHaveError_WhenEmailIsValid()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsEmpty()
        {
            // Arrange
            var request = new LoginRequestDTO("", "senha123");

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
            var request = new LoginRequestDTO(null!, "senha123");

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
            var request = new LoginRequestDTO("   ", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email)
                .WithErrorMessage("O email é obrigatório");
        }

        [Theory]
        [InlineData("usuario@example.com")]
        [InlineData("user.name@example.com")]
        [InlineData("user+tag@example.co.uk")]
        [InlineData("user_123@test-domain.com")]
        [InlineData("a@b.co")]
        public void Validate_ShouldNotHaveError_WhenEmailHasValidFormats(string email)
        {
            // Arrange
            var request = new LoginRequestDTO(email, "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Theory]
        [InlineData("usuario")]
        [InlineData("usuario@")]
        [InlineData("@example.com")]
        [InlineData("usuario example.com")]
        [InlineData("usuario@@example.com")]
        public void Validate_ShouldHaveError_WhenEmailHasInvalidFormat(string email)
        {
            // Arrange
            var request = new LoginRequestDTO(email, "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email)
                .WithErrorMessage("Email inválido");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsMissingAtSymbol()
        {
            // Arrange
            var request = new LoginRequestDTO("usuarioexample.com", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email)
                .WithErrorMessage("Email inválido");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsMissingDomain()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email)
                .WithErrorMessage("Email inválido");
        }

        [Fact]
        public void Validate_ShouldAcceptEmailWithMultipleDots()
        {
            // Arrange
            var request = new LoginRequestDTO("user.name.test@example.co.uk", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldAcceptEmailWithNumbers()
        {
            // Arrange
            var request = new LoginRequestDTO("user123@example456.com", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldAcceptEmailWithPlusSign()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario+teste@example.com", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldAcceptEmailWithSubdomain()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@mail.example.com", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldAcceptEmailWithHyphenInDomain()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@my-domain.com", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldAcceptEmailWithUnderscoreInUsername()
        {
            // Arrange
            var request = new LoginRequestDTO("user_name@example.com", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldAcceptShortestValidEmail()
        {
            // Arrange
            var request = new LoginRequestDTO("a@b.c", "senha123");

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
            var request = new LoginRequestDTO("usuario@example.com", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordIsEmpty()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", "");

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
            var request = new LoginRequestDTO("usuario@example.com", null!);

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
            var request = new LoginRequestDTO("usuario@example.com", "   ");

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
            var request = new LoginRequestDTO("usuario@example.com", "12345");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password)
                .WithErrorMessage("A senha deve ter no mínimo 6 caracteres");
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("senha1")]
        [InlineData("Password")]
        [InlineData("SecurePass123")]
        [InlineData("VeryLongPasswordWith50CharactersOrMore!!!!!!!!!")]
        public void Validate_ShouldNotHaveError_WhenPasswordHasMinimumLength(string password)
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", password);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        [InlineData("1234")]
        [InlineData("12345")]
        public void Validate_ShouldHaveError_WhenPasswordHasLessThan6Characters(string password)
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", password);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password)
                .WithErrorMessage("A senha deve ter no mínimo 6 caracteres");
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenPasswordHasExactly6Characters()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", "pass12");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldAcceptPasswordWithSpecialCharacters()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", "P@ssw0rd!");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldAcceptPasswordWithOnlyNumbers()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", "123456");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldAcceptPasswordWithSpaces()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", "my pass word");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldAcceptVeryLongPassword()
        {
            // Arrange
            var longPassword = new string('a', 100);
            var request = new LoginRequestDTO("usuario@example.com", longPassword);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldAcceptPasswordExactlyAtMinimumBoundary()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", "123456");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        #endregion

        #region Integration Tests - Multiple Errors

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenBothFieldsAreEmpty()
        {
            // Arrange
            var request = new LoginRequestDTO("", "");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email);
            result.ShouldHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenBothFieldsAreNull()
        {
            // Arrange
            var request = new LoginRequestDTO(null!, null!);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email);
            result.ShouldHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenAllFieldsAreValid()
        {
            // Arrange
            var request = new LoginRequestDTO(
                email: "usuario@example.com",
                password: "SenhaSegura123"
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenEmailIsInvalidAndPasswordIsTooShort()
        {
            // Arrange
            var request = new LoginRequestDTO("invalidemail", "12345");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email);
            result.ShouldHaveValidationErrorFor(r => r.password);
        }

        #endregion

        #region Specific Login Scenarios

        [Fact]
        public void Validate_ShouldAllowTypicalLoginScenario()
        {
            // Arrange - Cenário comum de login
            var request = new LoginRequestDTO(
                email: "joao.silva@empresa.com.br",
                password: "MinhaSenha123!"
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldAllowLoginWithMinimalCredentials()
        {
            // Arrange
            var request = new LoginRequestDTO("a@b.co", "pass12");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldRejectLoginWithIncompleteEmail()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@", "senha123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.email);
        }

        [Fact]
        public void Validate_ShouldRejectLoginWithWeakPassword()
        {
            // Arrange
            var request = new LoginRequestDTO("usuario@example.com", "123");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password);
        }

        #endregion

        #region Boundary Tests

        [Fact]
        public void Validate_ShouldAcceptPasswordAtLowerBoundary()
        {
            // Arrange
            var request = new LoginRequestDTO("user@test.com", "abcdef");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.password);
        }

        [Fact]
        public void Validate_ShouldRejectPasswordJustBelowLowerBoundary()
        {
            // Arrange
            var request = new LoginRequestDTO("user@test.com", "abcde");

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.password);
        }

        #endregion
    }
}