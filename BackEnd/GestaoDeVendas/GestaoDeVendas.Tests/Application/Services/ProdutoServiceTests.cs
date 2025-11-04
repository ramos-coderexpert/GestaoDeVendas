using FluentValidation;
using FluentValidation.Results;
using GestaoDeVendas.Application.DTO.Produto;
using GestaoDeVendas.Application.Services;
using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.IRepositories;
using Moq;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Services
{
    public class ProdutoServiceTests
    {
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly Mock<IValidator<AddProdutoRequest>> _addProdutoValidatorMock;
        private readonly Mock<IValidator<UpdateProdutoRequest>> _updateProdutoValidatorMock;
        private readonly ProdutoService _produtoService;

        public ProdutoServiceTests()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _addProdutoValidatorMock = new Mock<IValidator<AddProdutoRequest>>();
            _updateProdutoValidatorMock = new Mock<IValidator<UpdateProdutoRequest>>();

            _produtoService = new ProdutoService(
                _produtoRepositoryMock.Object,
                _addProdutoValidatorMock.Object,
                _updateProdutoValidatorMock.Object);
        }

        #region ObterTodosAsync Tests

        [Fact]
        public async Task ObterTodosAsync_ShouldReturnListOfProdutos()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new Produto("Notebook", 2500m, 10),
                new Produto("Mouse", 50m, 100),
                new Produto("Teclado", 150m, 50)
            };

            _produtoRepositoryMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(produtos);

            // Act
            var result = await _produtoService.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Notebook", result[0].nome);
            Assert.Equal(2500m, result[0].preco);
            Assert.Equal(10, result[0].estoque);
            Assert.Equal("Mouse", result[1].nome);
            Assert.Equal("Teclado", result[2].nome);
            _produtoRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_ShouldReturnEmptyList_WhenNoProdutosExist()
        {
            // Arrange
            _produtoRepositoryMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(new List<Produto>());

            // Act
            var result = await _produtoService.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region ObterTodosAtivosAsync Tests

        [Fact]
        public async Task ObterTodosAtivosAsync_ShouldReturnOnlyActiveProdutos()
        {
            // Arrange
            var produtosAtivos = new List<Produto>
            {
                new Produto("Monitor", 800m, 20),
                new Produto("Webcam", 300m, 15)
            };

            _produtoRepositoryMock.Setup(r => r.ObterTodosAtivosAsync())
                .ReturnsAsync(produtosAtivos);

            // Act
            var result = await _produtoService.ObterTodosAtivosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Monitor", result[0].nome);
            Assert.Equal(800m, result[0].preco);
            Assert.Equal("Webcam", result[1].nome);
            _produtoRepositoryMock.Verify(r => r.ObterTodosAtivosAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAtivosAsync_ShouldReturnEmptyList_WhenNoActiveProdutosExist()
        {
            // Arrange
            _produtoRepositoryMock.Setup(r => r.ObterTodosAtivosAsync())
                .ReturnsAsync(new List<Produto>());

            // Act
            var result = await _produtoService.ObterTodosAtivosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region ObterPorId Tests

        [Fact]
        public void ObterPorId_ShouldReturnProduto_WhenProdutoExists()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Headset", 250m, 30);

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            // Act
            var result = _produtoService.ObterPorId(produtoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Headset", result.nome);
            Assert.Equal(250m, result.preco);
            Assert.Equal(30, result.estoque);
            _produtoRepositoryMock.Verify(r => r.ObterPorId(produtoId), Times.Once);
        }

        [Fact]
        public void ObterPorId_ShouldThrowProdutoNaoEncontradoException_WhenProdutoDoesNotExist()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns((Produto)null!);

            // Act & Assert
            var exception = Assert.Throws<ProdutoNaoEncontradoException>(() => _produtoService.ObterPorId(produtoId));
            Assert.Contains(produtoId.ToString(), exception.Message);
        }

        #endregion

        #region ObterPorNome Tests

        [Fact]
        public void ObterPorNome_ShouldReturnProduto_WhenProdutoExists()
        {
            // Arrange
            var nomeProduto = "SSD 500GB";
            var produto = new Produto(nomeProduto, 350m, 25);

            _produtoRepositoryMock.Setup(r => r.ObterPorNome(nomeProduto))
                .Returns(produto);

            // Act
            var result = _produtoService.ObterPorNome(nomeProduto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nomeProduto, result.nome);
            Assert.Equal(350m, result.preco);
            Assert.Equal(25, result.estoque);
        }

        [Fact]
        public void ObterPorNome_ShouldThrowProdutoNaoEncontradoException_WhenProdutoDoesNotExist()
        {
            // Arrange
            var nomeProduto = "Produto Inexistente";
            _produtoRepositoryMock.Setup(r => r.ObterPorNome(nomeProduto))
                .Returns((Produto)null!);

            // Act & Assert
            var exception = Assert.Throws<ProdutoNaoEncontradoException>(() => _produtoService.ObterPorNome(nomeProduto));
            Assert.Contains(nomeProduto, exception.Message);
        }

        #endregion

        #region AddProduto Tests

        [Fact]
        public void AddProduto_ShouldCreateProdutoSuccessfully()
        {
            // Arrange
            var produtoRequest = new AddProdutoRequest("Novo Produto", 100m, 50);

            _addProdutoValidatorMock.Setup(v => v.Validate(produtoRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.Add(It.IsAny<Produto>()));
            _produtoRepositoryMock.Setup(r => r.SaveChanges());

            // Act
            var result = _produtoService.AddProduto(produtoRequest);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _produtoRepositoryMock.Verify(r => r.Add(It.Is<Produto>(p =>
                p.Nome == produtoRequest.nome &&
                p.Preco == produtoRequest.preco &&
                p.Estoque == produtoRequest.estoque
            )), Times.Once);
            _produtoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void AddProduto_ShouldThrowProdutoException_WithProdutoInvalidoMessage_WhenValidationFails()
        {
            // Arrange
            var produtoRequest = new AddProdutoRequest("", -10m, -5);
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("nome", "Nome é obrigatório"),
                new ValidationFailure("preco", "Preço deve ser maior que zero"),
                new ValidationFailure("estoque", "Estoque não pode ser negativo")
            };

            _addProdutoValidatorMock.Setup(v => v.Validate(produtoRequest))
                .Returns(new ValidationResult(validationFailures));

            // Act & Assert
            var exception = Assert.Throws<ProdutoException>(() => _produtoService.AddProduto(produtoRequest));
            Assert.Contains("Nome é obrigatório", exception.Message);
        }

        [Fact]
        public void AddProduto_ShouldThrowProdutoException_WhenRepositoryThrowsException()
        {
            // Arrange
            var produtoRequest = new AddProdutoRequest("Produto Erro", 100m, 10);

            _addProdutoValidatorMock.Setup(v => v.Validate(produtoRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.Add(It.IsAny<Produto>()))
                .Throws(new Exception("Database error"));

            // Act & Assert
            var exception = Assert.Throws<ProdutoException>(() => _produtoService.AddProduto(produtoRequest));
            Assert.Contains("Erro ao adicionar produto", exception.Message);
            Assert.Contains("Database error", exception.Message);
        }

        [Fact]
        public void AddProduto_ShouldCreateProdutoWithCorrectValues()
        {
            // Arrange
            var produtoRequest = new AddProdutoRequest("Produto Teste", 299.99m, 100);
            Produto capturedProduto = null!;

            _addProdutoValidatorMock.Setup(v => v.Validate(produtoRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.Add(It.IsAny<Produto>()))
                .Callback<Produto>(p => capturedProduto = p);

            // Act
            _produtoService.AddProduto(produtoRequest);

            // Assert
            Assert.NotNull(capturedProduto);
            Assert.Equal("Produto Teste", capturedProduto.Nome);
            Assert.Equal(299.99m, capturedProduto.Preco);
            Assert.Equal(100, capturedProduto.Estoque);
        }

        [Fact]
        public void AddProduto_ShouldCallSaveChanges_AfterAddingProduto()
        {
            // Arrange
            var produtoRequest = new AddProdutoRequest("Produto Save", 50m, 20);

            _addProdutoValidatorMock.Setup(v => v.Validate(produtoRequest))
                .Returns(new ValidationResult());

            // Act
            _produtoService.AddProduto(produtoRequest);

            // Assert
            _produtoRepositoryMock.Verify(r => r.Add(It.IsAny<Produto>()), Times.Once);
            _produtoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        #endregion

        #region UpdateProduto Tests

        [Fact]
        public void UpdateProduto_ShouldUpdateProdutoSuccessfully()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Nome Original", 100m, 50);
            var updateRequest = new UpdateProdutoRequest("Nome Atualizado", 150m, 75);

            _updateProdutoValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            _produtoRepositoryMock.Setup(r => r.Update(It.IsAny<Produto>()));
            _produtoRepositoryMock.Setup(r => r.SaveChanges());

            // Act
            _produtoService.UpdateProduto(produtoId, updateRequest);

            // Assert
            Assert.Equal("Nome Atualizado", produto.Nome);
            Assert.Equal(150m, produto.Preco);
            Assert.Equal(75, produto.Estoque);
            _produtoRepositoryMock.Verify(r => r.Update(produto), Times.Once);
            _produtoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateProduto_ShouldThrowProdutoException_WithProdutoInvalidoMessage_WhenValidationFails()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var updateRequest = new UpdateProdutoRequest("", -50m, -10);
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("nome", "Nome é obrigatório"),
                new ValidationFailure("preco", "Preço inválido")
            };

            _updateProdutoValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult(validationFailures));

            // Act & Assert
            var exception = Assert.Throws<ProdutoException>(() => _produtoService.UpdateProduto(produtoId, updateRequest));
            Assert.Contains("Nome é obrigatório", exception.Message);
        }

        [Fact]
        public void UpdateProduto_ShouldThrowProdutoException_WhenProdutoDoesNotExist()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var updateRequest = new UpdateProdutoRequest("Produto Update", 100m, 50);

            _updateProdutoValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns((Produto)null!);

            // Act & Assert
            var exception = Assert.Throws<ProdutoException>(() => _produtoService.UpdateProduto(produtoId, updateRequest));
            Assert.Contains("Erro ao atualizar produto", exception.Message);
        }

        [Fact]
        public void UpdateProduto_ShouldThrowProdutoException_WhenRepositoryThrowsException()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Produto Erro", 100m, 10);
            var updateRequest = new UpdateProdutoRequest("Novo Nome", 200m, 20);

            _updateProdutoValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            _produtoRepositoryMock.Setup(r => r.Update(It.IsAny<Produto>()))
                .Throws(new Exception("Update error"));

            // Act & Assert
            var exception = Assert.Throws<ProdutoException>(() => _produtoService.UpdateProduto(produtoId, updateRequest));
            Assert.Contains("Erro ao atualizar produto", exception.Message);
            Assert.Contains("Update error", exception.Message);
        }

        [Fact]
        public void UpdateProduto_ShouldUpdateAllFields()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Original", 50m, 10);
            var updateRequest = new UpdateProdutoRequest("Atualizado", 100m, 25);

            _updateProdutoValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            // Act
            _produtoService.UpdateProduto(produtoId, updateRequest);

            // Assert
            Assert.Equal("Atualizado", produto.Nome);
            Assert.Equal(100m, produto.Preco);
            Assert.Equal(25, produto.Estoque);
        }

        [Fact]
        public void UpdateProduto_ShouldCallUpdateAndSaveChanges()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Produto", 100m, 10);
            var updateRequest = new UpdateProdutoRequest("Updated", 150m, 15);

            _updateProdutoValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            // Act
            _produtoService.UpdateProduto(produtoId, updateRequest);

            // Assert
            _produtoRepositoryMock.Verify(r => r.Update(It.IsAny<Produto>()), Times.Once);
            _produtoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        #endregion

        #region DeleteProduto Tests

        [Fact]
        public void DeleteProduto_ShouldDeactivateProdutoSuccessfully()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Produto a Deletar", 100m, 10);

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            _produtoRepositoryMock.Setup(r => r.Update(It.IsAny<Produto>()));
            _produtoRepositoryMock.Setup(r => r.SaveChanges());

            // Act
            _produtoService.DeleteProduto(produtoId);

            // Assert
            Assert.False(produto.Ativo);
            _produtoRepositoryMock.Verify(r => r.Update(produto), Times.Once);
            _produtoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteProduto_ShouldThrowProdutoException_WithProdutoNaoEncontradoMessage_WhenProdutoDoesNotExist()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns((Produto)null!);

            // Act & Assert
            var exception = Assert.Throws<ProdutoException>(() => _produtoService.DeleteProduto(produtoId));
            Assert.Contains("Produto não encontrado com o Id:", exception.Message);
            Assert.Contains(produtoId.ToString(), exception.Message);
        }

        [Fact]
        public void DeleteProduto_ShouldThrowProdutoException_WhenRepositoryThrowsException()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Produto Erro", 100m, 10);

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            _produtoRepositoryMock.Setup(r => r.Update(It.IsAny<Produto>()))
                .Throws(new Exception("Delete error"));

            // Act & Assert
            var exception = Assert.Throws<ProdutoException>(() => _produtoService.DeleteProduto(produtoId));
            Assert.Contains("Erro ao deletar produto", exception.Message);
            Assert.Contains("Delete error", exception.Message);
        }

        [Fact]
        public void DeleteProduto_ShouldNotDeletePhysically_ButDeactivate()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Produto Soft Delete", 100m, 10);

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            // Act
            _produtoService.DeleteProduto(produtoId);

            // Assert
            // Verifica que Update foi chamado, não Delete
            _produtoRepositoryMock.Verify(r => r.Update(It.IsAny<Produto>()), Times.Once);
            _produtoRepositoryMock.Verify(r => r.Delete(It.IsAny<Produto>()), Times.Never);
            Assert.False(produto.Ativo);
        }

        [Fact]
        public void DeleteProduto_ShouldCallSaveChanges_AfterDeactivation()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Produto Delete", 100m, 10);

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            // Act
            _produtoService.DeleteProduto(produtoId);

            // Assert
            _produtoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        #endregion

        #region Edge Cases and Additional Tests

        [Fact]
        public async Task ObterTodosAsync_ShouldMapAllFieldsCorrectly()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new Produto("Produto Completo", 999.99m, 123)
            };

            _produtoRepositoryMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(produtos);

            // Act
            var result = await _produtoService.ObterTodosAsync();

            // Assert
            var produto = result.First();
            Assert.Equal("Produto Completo", produto.nome);
            Assert.Equal(999.99m, produto.preco);
            Assert.Equal(123, produto.estoque);
            Assert.NotEqual(Guid.Empty, produto.id);
        }

        [Fact]
        public void AddProduto_ShouldGenerateUniqueId_ForNewProduto()
        {
            // Arrange
            var produtoRequest = new AddProdutoRequest("Produto ID Test", 50m, 10);
            var ids = new List<Guid>();

            _addProdutoValidatorMock.Setup(v => v.Validate(produtoRequest))
                .Returns(new ValidationResult());

            // Act
            for (int i = 0; i < 3; i++)
            {
                var id = _produtoService.AddProduto(produtoRequest);
                ids.Add(id);
            }

            // Assert
            Assert.Equal(3, ids.Distinct().Count()); // Todos IDs devem ser únicos
        }

        [Fact]
        public void UpdateProduto_ShouldPreserveId_WhenUpdating()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produto = new Produto("Produto Original", 100m, 10);
            var originalId = produto.Id;
            var updateRequest = new UpdateProdutoRequest("Produto Atualizado", 200m, 20);

            _updateProdutoValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _produtoRepositoryMock.Setup(r => r.ObterPorId(produtoId))
                .Returns(produto);

            // Act
            _produtoService.UpdateProduto(produtoId, updateRequest);

            // Assert
            Assert.Equal(originalId, produto.Id); // ID não deve mudar
        }

        #endregion
    }
}