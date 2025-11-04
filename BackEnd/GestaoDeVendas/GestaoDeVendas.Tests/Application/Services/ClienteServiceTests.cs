using FluentValidation;
using FluentValidation.Results;
using GestaoDeVendas.Application.DTO.Auth;
using GestaoDeVendas.Application.DTO.Cliente;
using GestaoDeVendas.Application.Services;
using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Domain.Services;
using Moq;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Services
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IValidator<LoginRequestDTO>> _loginValidatorMock;
        private readonly Mock<IValidator<AddClienteRequest>> _addClienteValidatorMock;
        private readonly Mock<IValidator<UpdateClienteRequest>> _updateClienteValidatorMock;
        private readonly ClienteService _clienteService;

        public ClienteServiceTests()
        {
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _authServiceMock = new Mock<IAuthService>();
            _loginValidatorMock = new Mock<IValidator<LoginRequestDTO>>();
            _addClienteValidatorMock = new Mock<IValidator<AddClienteRequest>>();
            _updateClienteValidatorMock = new Mock<IValidator<UpdateClienteRequest>>();

            _clienteService = new ClienteService(
                _clienteRepositoryMock.Object,
                _authServiceMock.Object,
                _loginValidatorMock.Object,
                _addClienteValidatorMock.Object,
                _updateClienteValidatorMock.Object);
        }

        #region Login Tests

        [Fact]
        public void Login_ShouldReturnLoginResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO("user@email.com", "password123");
            var passwordHash = "hashedPassword";
            var cliente = new Cliente("João Silva", "user@email.com", passwordHash, "User", 1000m);
            var token = "jwt-token-123";

            _loginValidatorMock.Setup(v => v.Validate(loginRequest))
                .Returns(new ValidationResult());

            _authServiceMock.Setup(a => a.ComputeSha256Hash(loginRequest.password))
                .Returns(passwordHash);

            _clienteRepositoryMock.Setup(r => r.ObterPorEmailPassword(loginRequest.email, passwordHash))
                .Returns(cliente);

            _authServiceMock.Setup(a => a.GenerateJwtToken(cliente.Email, cliente.Role))
                .Returns(token);

            // Act
            var result = _clienteService.Login(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token, result.token);
            Assert.Equal(cliente.Email, result.email);
            Assert.Equal(cliente.Role, result.role);
            _authServiceMock.Verify(a => a.ComputeSha256Hash(loginRequest.password), Times.Once);
            _authServiceMock.Verify(a => a.GenerateJwtToken(cliente.Email, cliente.Role), Times.Once);
        }

        [Fact]
        public void Login_ShouldThrowCredenciaisInvalidasException_WhenValidationFails()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO("", "");
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("email", "Email é obrigatório")
            };

            _loginValidatorMock.Setup(v => v.Validate(loginRequest))
                .Returns(new ValidationResult(validationFailures));

            // Act & Assert
            var exception = Assert.Throws<CredenciaisInvalidasException>(() => _clienteService.Login(loginRequest));
            Assert.Contains(loginRequest.email, exception.Message);
        }

        [Fact]
        public void Login_ShouldThrowClienteException_WhenClienteNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO("notfound@email.com", "password123");
            var passwordHash = "hashedPassword";

            _loginValidatorMock.Setup(v => v.Validate(loginRequest))
                .Returns(new ValidationResult());

            _authServiceMock.Setup(a => a.ComputeSha256Hash(loginRequest.password))
                .Returns(passwordHash);

            _clienteRepositoryMock.Setup(r => r.ObterPorEmailPassword(loginRequest.email, passwordHash))
                .Returns((Cliente)null!);

            // Act & Assert
            var exception = Assert.Throws<ClienteException>(() => _clienteService.Login(loginRequest));
            Assert.Contains("Login não encontrado com email: notfound@email.com", exception.Message);
        }

        [Fact]
        public void Login_ShouldThrowClienteException_WhenRepositoryThrowsException()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO("error@email.com", "password123");

            _loginValidatorMock.Setup(v => v.Validate(loginRequest))
                .Returns(new ValidationResult());

            _authServiceMock.Setup(a => a.ComputeSha256Hash(loginRequest.password))
                .Throws(new Exception("Database error"));

            // Act & Assert
            var exception = Assert.Throws<ClienteException>(() => _clienteService.Login(loginRequest));
            Assert.Contains("Erro ao realizar login", exception.Message);
        }

        #endregion

        #region ObterTodosAsync Tests

        [Fact]
        public async Task ObterTodosAsync_ShouldReturnListOfClientes()
        {
            // Arrange
            var clientes = new List<Cliente>
            {
                new Cliente("João Silva", "joao@email.com", "hash1", "User", 1000m),
                new Cliente("Maria Santos", "maria@email.com", "hash2", "Admin", 2000m)
            };

            _clienteRepositoryMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(clientes);

            // Act
            var result = await _clienteService.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("João Silva", result[0].nome);
            Assert.Equal("Maria Santos", result[1].nome);
            _clienteRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_ShouldReturnEmptyList_WhenNoClientesExist()
        {
            // Arrange
            _clienteRepositoryMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(new List<Cliente>());

            // Act
            var result = await _clienteService.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region ObterTodosAtivosAsync Tests

        [Fact]
        public async Task ObterTodosAtivosAsync_ShouldReturnOnlyActiveClientes()
        {
            // Arrange
            var clientesAtivos = new List<Cliente>
            {
                new Cliente("Carlos Oliveira", "carlos@email.com", "hash3", "User", 1500m),
                new Cliente("Ana Costa", "ana@email.com", "hash4", "User", 2500m)
            };

            _clienteRepositoryMock.Setup(r => r.ObterTodosAtivosAsync())
                .ReturnsAsync(clientesAtivos);

            // Act
            var result = await _clienteService.ObterTodosAtivosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.NotNull(c.nome));
            _clienteRepositoryMock.Verify(r => r.ObterTodosAtivosAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAtivosAsync_ShouldReturnEmptyList_WhenNoActiveClientesExist()
        {
            // Arrange
            _clienteRepositoryMock.Setup(r => r.ObterTodosAtivosAsync())
                .ReturnsAsync(new List<Cliente>());

            // Act
            var result = await _clienteService.ObterTodosAtivosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region ObterPorId Tests

        [Fact]
        public void ObterPorId_ShouldReturnCliente_WhenClienteExists()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente("Pedro Souza", "pedro@email.com", "hash5", "User", 3000m);

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns(cliente);

            // Act
            var result = _clienteService.ObterPorId(clienteId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Pedro Souza", result.nome);
            Assert.Equal("pedro@email.com", result.email);
            Assert.Equal(3000m, result.saldo);
        }

        [Fact]
        public void ObterPorId_ShouldThrowKeyNotFoundException_WhenClienteDoesNotExist()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns((Cliente)null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() => _clienteService.ObterPorId(clienteId));
            Assert.Equal("Cliente não encontrado.", exception.Message);
        }

        #endregion

        #region ObterPorNome Tests

        [Fact]
        public void ObterPorNome_ShouldReturnCliente_WhenClienteExists()
        {
            // Arrange
            var nome = "Fernanda Lima";
            var cliente = new Cliente(nome, "fernanda@email.com", "hash6", "User", 4000m);

            _clienteRepositoryMock.Setup(r => r.ObterPorNome(nome))
                .Returns(cliente);

            // Act
            var result = _clienteService.ObterPorNome(nome);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nome, result.nome);
            Assert.Equal("fernanda@email.com", result.email);
        }

        [Fact]
        public void ObterPorNome_ShouldThrowClienteNaoEncontradoException_WhenClienteDoesNotExist()
        {
            // Arrange
            var nome = "Cliente Inexistente";
            _clienteRepositoryMock.Setup(r => r.ObterPorNome(nome))
                .Returns((Cliente)null!);

            // Act & Assert
            var exception = Assert.Throws<ClienteNaoEncontradoException>(() => _clienteService.ObterPorNome(nome));
            Assert.Contains(nome, exception.Message);
        }

        #endregion

        #region AddCliente Tests

        [Fact]
        public void AddCliente_ShouldCreateClienteSuccessfully()
        {
            // Arrange
            var clienteRequest = new AddClienteRequest("Novo Cliente", "novo@email.com", "password123", "User", 1000m);
            var passwordHash = "hashedPassword123";

            _addClienteValidatorMock.Setup(v => v.Validate(clienteRequest))
                .Returns(new ValidationResult());

            _authServiceMock.Setup(a => a.ComputeSha256Hash(clienteRequest.password))
                .Returns(passwordHash);

            _clienteRepositoryMock.Setup(r => r.Add(It.IsAny<Cliente>()));
            _clienteRepositoryMock.Setup(r => r.SaveChanges());

            // Act
            var result = _clienteService.AddCliente(clienteRequest);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _clienteRepositoryMock.Verify(r => r.Add(It.Is<Cliente>(c =>
                c.Nome == clienteRequest.nome &&
                c.Email == clienteRequest.email &&
                c.Password == passwordHash &&
                c.Role == clienteRequest.role &&
                c.Saldo == clienteRequest.saldo
            )), Times.Once);
            _clienteRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
            _authServiceMock.Verify(a => a.ComputeSha256Hash(clienteRequest.password), Times.Once);
        }

        [Fact]
        public void AddCliente_ShouldThrowClienteInvalidoException_WhenValidationFails()
        {
            // Arrange
            var clienteRequest = new AddClienteRequest("", "invalid-email", "", "InvalidRole", -100m);
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("nome", "Nome é obrigatório"),
                new ValidationFailure("email", "Email inválido"),
                new ValidationFailure("saldo", "Saldo não pode ser negativo")
            };

            _addClienteValidatorMock.Setup(v => v.Validate(clienteRequest))
                .Returns(new ValidationResult(validationFailures));

            // Act & Assert
            var exception = Assert.Throws<ClienteInvalidoException>(() => _clienteService.AddCliente(clienteRequest));
            Assert.Contains("Nome é obrigatório", exception.Message);
        }

        [Fact]
        public void AddCliente_ShouldThrowClienteException_WhenRepositoryThrowsException()
        {
            // Arrange
            var clienteRequest = new AddClienteRequest("Cliente Erro", "erro@email.com", "password", "User", 100m);

            _addClienteValidatorMock.Setup(v => v.Validate(clienteRequest))
                .Returns(new ValidationResult());

            _authServiceMock.Setup(a => a.ComputeSha256Hash(clienteRequest.password))
                .Throws(new Exception("Hash error"));

            // Act & Assert
            var exception = Assert.Throws<ClienteException>(() => _clienteService.AddCliente(clienteRequest));
            Assert.Contains("Erro ao adicionar cliente", exception.Message);
        }

        [Fact]
        public void AddCliente_ShouldHashPassword_BeforeStoringCliente()
        {
            // Arrange
            var plainPassword = "mySecretPassword";
            var hashedPassword = "hashed_mySecretPassword";
            var clienteRequest = new AddClienteRequest("Test User", "test@email.com", plainPassword, "User", 500m);

            _addClienteValidatorMock.Setup(v => v.Validate(clienteRequest))
                .Returns(new ValidationResult());

            _authServiceMock.Setup(a => a.ComputeSha256Hash(plainPassword))
                .Returns(hashedPassword);

            // Act
            _clienteService.AddCliente(clienteRequest);

            // Assert
            _clienteRepositoryMock.Verify(r => r.Add(It.Is<Cliente>(c => c.Password == hashedPassword)), Times.Once);
            _authServiceMock.Verify(a => a.ComputeSha256Hash(plainPassword), Times.Once);
        }

        #endregion

        #region UpdateCliente Tests

        [Fact]
        public void UpdateCliente_ShouldUpdateClienteSuccessfully()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente("Nome Original", "original@email.com", "hash", "User", 1000m);
            var updateRequest = new UpdateClienteRequest("Nome Atualizado", "Admin", 2000m);

            _updateClienteValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns(cliente);

            _clienteRepositoryMock.Setup(r => r.Update(It.IsAny<Cliente>()));
            _clienteRepositoryMock.Setup(r => r.SaveChanges());

            // Act
            _clienteService.UpdateCliente(clienteId, updateRequest);

            // Assert
            Assert.Equal("Nome Atualizado", cliente.Nome);
            Assert.Equal("Admin", cliente.Role);
            Assert.Equal(2000m, cliente.Saldo);
            _clienteRepositoryMock.Verify(r => r.Update(cliente), Times.Once);
            _clienteRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateCliente_ShouldThrowClienteInvalidoException_WhenValidationFails()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var updateRequest = new UpdateClienteRequest("", "InvalidRole", -500m);
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("nome", "Nome é obrigatório"),
                new ValidationFailure("saldo", "Saldo inválido")
            };

            _updateClienteValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult(validationFailures));

            // Act & Assert
            var exception = Assert.Throws<ClienteInvalidoException>(() => _clienteService.UpdateCliente(clienteId, updateRequest));
            Assert.Contains("Nome é obrigatório", exception.Message);
        }

        [Fact]
        public void UpdateCliente_ShouldThrowClienteNaoEncontradoException_WhenClienteDoesNotExist()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var updateRequest = new UpdateClienteRequest("Nome", "User", 1000m);

            _updateClienteValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns((Cliente)null!);

            // Act & Assert
            var exception = Assert.Throws<ClienteNaoEncontradoException>(() => _clienteService.UpdateCliente(clienteId, updateRequest));
            Assert.Contains(clienteId.ToString(), exception.Message);
        }

        [Fact]
        public void UpdateCliente_ShouldThrowClienteException_WhenRepositoryThrowsException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente("Cliente Erro", "erro@email.com", "hash", "User", 100m);
            var updateRequest = new UpdateClienteRequest("Novo Nome", "User", 200m);

            _updateClienteValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns(cliente);

            _clienteRepositoryMock.Setup(r => r.Update(It.IsAny<Cliente>()))
                .Throws(new Exception("Database error"));

            // Act & Assert
            var exception = Assert.Throws<ClienteException>(() => _clienteService.UpdateCliente(clienteId, updateRequest));
            Assert.Contains("Erro ao atualizar cliente", exception.Message);
        }

        [Fact]
        public void UpdateCliente_ShouldOnlyUpdateAllowedFields()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var originalEmail = "original@email.com";
            var originalPassword = "originalHash";
            var cliente = new Cliente("Nome Original", originalEmail, originalPassword, "User", 1000m);
            var updateRequest = new UpdateClienteRequest("Novo Nome", "Admin", 3000m);

            _updateClienteValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns(cliente);

            // Act
            _clienteService.UpdateCliente(clienteId, updateRequest);

            // Assert
            Assert.Equal("Novo Nome", cliente.Nome);
            Assert.Equal("Admin", cliente.Role);
            Assert.Equal(3000m, cliente.Saldo);
            // Email e Password não devem ser alterados
            Assert.Equal(originalEmail, cliente.Email);
            Assert.Equal(originalPassword, cliente.Password);
        }

        #endregion

        #region DeleteCliente Tests

        [Fact]
        public void DeleteCliente_ShouldDeactivateClienteSuccessfully()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente("Cliente a Deletar", "delete@email.com", "hash", "User", 500m);

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns(cliente);

            _clienteRepositoryMock.Setup(r => r.Update(It.IsAny<Cliente>()));
            _clienteRepositoryMock.Setup(r => r.SaveChanges());

            // Act
            _clienteService.DeleteCliente(clienteId);

            // Assert
            Assert.False(cliente.Ativo);
            _clienteRepositoryMock.Verify(r => r.Update(cliente), Times.Once);
            _clienteRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteCliente_ShouldThrowClienteNaoEncontradoException_WhenClienteDoesNotExist()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns((Cliente)null!);

            // Act & Assert
            var exception = Assert.Throws<ClienteNaoEncontradoException>(() => _clienteService.DeleteCliente(clienteId));
            Assert.Contains(clienteId.ToString(), exception.Message);
        }

        [Fact]
        public void DeleteCliente_ShouldThrowClienteException_WhenRepositoryThrowsException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente("Cliente Erro", "erro@email.com", "hash", "User", 100m);

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns(cliente);

            _clienteRepositoryMock.Setup(r => r.Update(It.IsAny<Cliente>()))
                .Throws(new Exception("Database error"));

            // Act & Assert
            var exception = Assert.Throws<ClienteException>(() => _clienteService.DeleteCliente(clienteId));
            Assert.Contains("Erro ao deletar cliente", exception.Message);
        }

        [Fact]
        public void DeleteCliente_ShouldNotDeletePhysically_ButDeactivate()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente("Cliente Soft Delete", "softdelete@email.com", "hash", "User", 1000m);

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId))
                .Returns(cliente);

            // Act
            _clienteService.DeleteCliente(clienteId);

            // Assert
            // Verifica que Update foi chamado, não Delete
            _clienteRepositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Once);
            _clienteRepositoryMock.Verify(r => r.Delete(It.IsAny<Cliente>()), Times.Never);
            Assert.False(cliente.Ativo);
        }

        #endregion
    }
}