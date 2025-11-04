using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Application.Services;
using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Services
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            // Arrange: Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _unitOfWork = new UnitOfWork(_context);
        }

        [Fact]
        public void Constructor_ShouldInitializeAllRepositories()
        {
            // Assert
            Assert.NotNull(_unitOfWork.Clientes);
            Assert.NotNull(_unitOfWork.Produtos);
            Assert.NotNull(_unitOfWork.Vendas);
        }

        [Fact]
        public void Constructor_ShouldInitializeClienteRepository_WithCorrectType()
        {
            // Assert
            Assert.IsAssignableFrom<IClienteRepository>(_unitOfWork.Clientes);
        }

        [Fact]
        public void Constructor_ShouldInitializeProdutoRepository_WithCorrectType()
        {
            // Assert
            Assert.IsAssignableFrom<IProdutoRepository>(_unitOfWork.Produtos);
        }

        [Fact]
        public void Constructor_ShouldInitializeVendaRepository_WithCorrectType()
        {
            // Assert
            Assert.IsAssignableFrom<IVendaRepository>(_unitOfWork.Vendas);
        }

        [Fact]
        public void Commit_ShouldSaveChangesToDatabase()
        {
            // Arrange
            var cliente = new Cliente(
                nome: "Test Cliente",
                email: "test@example.com",
                password: "hashedPassword",
                role: "User",
                saldo: 100m
            );

            _unitOfWork.Clientes.Add(cliente);

            // Act
            _unitOfWork.Commit();

            // Assert
            var savedCliente = _context.Clientes.FirstOrDefault(c => c.Email == "test@example.com");
            Assert.NotNull(savedCliente);
            Assert.Equal("Test Cliente", savedCliente.Nome);
        }

        [Fact]
        public void Commit_ShouldPersistMultipleChanges()
        {
            // Arrange
            var cliente = new Cliente(
                nome: "Cliente 1",
                email: "cliente1@example.com",
                password: "hash1",
                role: "User",
                saldo: 150m
            );

            var produto = new Produto(
                nome: "Produto 1",
                preco: 50m,
                estoque: 10
            );

            _unitOfWork.Clientes.Add(cliente);
            _unitOfWork.Produtos.Add(produto);

            // Act
            _unitOfWork.Commit();

            // Assert
            Assert.Equal(1, _context.Clientes.Count());
            Assert.Equal(1, _context.Produtos.Count());
        }

        [Fact]
        public void Commit_WithoutChanges_ShouldNotThrowException()
        {
            // Act & Assert
            var exception = Record.Exception(() => _unitOfWork.Commit());
            Assert.Null(exception);
        }

        [Fact]
        public void Dispose_ShouldDisposeContext()
        {
            // Act
            _unitOfWork.Dispose();

            // Assert
            Assert.Throws<ObjectDisposedException>(() => _context.Clientes.ToList());
        }

        [Fact]
        public void UnitOfWork_ShouldImplementIUnitOfWorkInterface()
        {
            // Assert
            Assert.IsAssignableFrom<IUnitOfWork>(_unitOfWork);
        }

        [Fact]
        public void UnitOfWork_ShouldImplementIDisposableInterface()
        {
            // Assert
            Assert.IsAssignableFrom<IDisposable>(_unitOfWork);
        }

        [Fact]
        public void Repositories_ShouldShareSameContext()
        {
            // Arrange
            var cliente = new Cliente(
                nome: "Shared Context Test",
                email: "shared@example.com",
                password: "96458524",
                role: "User",
                saldo: 200m
            );

            // Act
            _unitOfWork.Clientes.Add(cliente);
            _unitOfWork.Commit();

            var retrievedCliente = _unitOfWork.Clientes.ObterPorEmailPassword("shared@example.com", "96458524");

            // Assert
            Assert.NotNull(retrievedCliente);
            Assert.Equal("Shared Context Test", retrievedCliente.Nome);
        }

        [Fact]
        public void Commit_AfterUpdate_ShouldPersistChanges()
        {
            // Arrange
            var produto = new Produto(
                nome: "Produto Original",
                preco: 100m,
                estoque: 5
            );

            _unitOfWork.Produtos.Add(produto);
            _unitOfWork.Commit();

            // Act
            produto.AtualizarPreco(150m);
            _unitOfWork.Produtos.Update(produto);
            _unitOfWork.Commit();

            // Assert
            var updatedProduto = _context.Produtos.FirstOrDefault(p => p.Id == produto.Id);
            Assert.NotNull(updatedProduto);
            Assert.Equal(150m, updatedProduto.Preco);
        }

        [Fact]
        public void Commit_AfterDelete_ShouldRemoveEntity()
        {
            // Arrange
            var cliente = new Cliente(
                nome: "Cliente to Delete",
                email: "delete@example.com",
                password: "hash",
                role: "User",
                saldo: 50m
            );

            _unitOfWork.Clientes.Add(cliente);
            _unitOfWork.Commit();

            // Act
            _unitOfWork.Clientes.Delete(cliente);
            _unitOfWork.Commit();

            // Assert
            var deletedCliente = _context.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
            Assert.Null(deletedCliente);
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _context?.Dispose();
        }
    }
}