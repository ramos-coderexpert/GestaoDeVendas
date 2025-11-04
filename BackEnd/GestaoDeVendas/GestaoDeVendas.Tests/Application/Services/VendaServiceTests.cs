using FluentValidation;
using FluentValidation.Results;
using GestaoDeVendas.Application.DTO.Venda;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Application.Services;
using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.IRepositories;
using Moq;
using Xunit;

namespace GestaoDeVendas.Tests.Application.Services
{
    public class VendaServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IValidator<AddVendaRequest>> _addVendaValidatorMock;
        private readonly Mock<IValidator<UpdateVendaRequest>> _updateVendaValidatorMock;
        private readonly Mock<IVendaRepository> _vendaRepositoryMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly VendaService _vendaService;

        public VendaServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _addVendaValidatorMock = new Mock<IValidator<AddVendaRequest>>();
            _updateVendaValidatorMock = new Mock<IValidator<UpdateVendaRequest>>();
            _vendaRepositoryMock = new Mock<IVendaRepository>();
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _produtoRepositoryMock = new Mock<IProdutoRepository>();

            _unitOfWorkMock.Setup(u => u.Vendas).Returns(_vendaRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Clientes).Returns(_clienteRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Produtos).Returns(_produtoRepositoryMock.Object);

            _vendaService = new VendaService(
                _unitOfWorkMock.Object,
                _addVendaValidatorMock.Object,
                _updateVendaValidatorMock.Object);
        }

        #region ObterTodosAsync Tests

        [Fact]
        public async Task ObterTodosAsync_ShouldReturnListOfVendas()
        {
            // Arrange
            var cliente = new Cliente("João Silva", "joao@email.com", "hash123", "User", 1000m);
            var produto = new Produto("Notebook", 2500m, 10);
            var vendas = new List<Venda>
            {
                new Venda(2, 2500m, cliente.Id, produto.Id)
            };

            // Simular as propriedades de navegação
            typeof(Venda).GetProperty("Cliente")!.SetValue(vendas[0], cliente);
            typeof(Venda).GetProperty("Produto")!.SetValue(vendas[0], produto);

            _vendaRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(vendas);

            // Act
            var result = await _vendaService.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("João Silva", result[0].nomeCliente);
            Assert.Equal("Notebook", result[0].nomeProduto);
            _vendaRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_ShouldReturnEmptyList_WhenNoVendasExist()
        {
            // Arrange
            _vendaRepositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(new List<Venda>());

            // Act
            var result = await _vendaService.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region ObterPorId Tests

        [Fact]
        public void ObterPorId_ShouldReturnVenda_WhenVendaExists()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var cliente = new Cliente("Maria Santos", "maria@email.com", "hash456", "User", 500m);
            var produto = new Produto("Mouse", 50m, 20);
            var venda = new Venda(3, 50m, cliente.Id, produto.Id);

            typeof(Venda).GetProperty("Cliente")!.SetValue(venda, cliente);
            typeof(Venda).GetProperty("Produto")!.SetValue(venda, produto);

            _vendaRepositoryMock.Setup(r => r.ObterPorId(vendaId)).Returns(venda);

            // Act
            var result = _vendaService.ObterPorId(vendaId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Maria Santos", result.nomeCliente);
            Assert.Equal("Mouse", result.nomeProduto);
            Assert.Equal(3, result.qtdProduto);
        }

        [Fact]
        public void ObterPorId_ShouldThrowKeyNotFoundException_WhenVendaDoesNotExist()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            _vendaRepositoryMock.Setup(r => r.ObterPorId(vendaId)).Returns((Venda)null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() => _vendaService.ObterPorId(vendaId));
            Assert.Equal("Venda não encontrada.", exception.Message);
        }

        #endregion

        #region ObterPorNomeCliente Tests

        [Fact]
        public void ObterPorNomeCliente_ShouldReturnVendas_WhenVendasExist()
        {
            // Arrange
            var nomeCliente = "Carlos Oliveira";
            var cliente = new Cliente(nomeCliente, "carlos@email.com", "hash789", "User", 2000m);
            var produto = new Produto("Teclado", 150m, 15);
            var vendas = new List<Venda>
            {
                new Venda(1, 150m, cliente.Id, produto.Id),
                new Venda(2, 150m, cliente.Id, produto.Id)
            };

            foreach (var venda in vendas)
            {
                typeof(Venda).GetProperty("Cliente")!.SetValue(venda, cliente);
                typeof(Venda).GetProperty("Produto")!.SetValue(venda, produto);
            }

            _vendaRepositoryMock.Setup(r => r.ObterPorNomeCliente(nomeCliente)).Returns(vendas);

            // Act
            var result = _vendaService.ObterPorNomeCliente(nomeCliente);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, v => Assert.Equal(nomeCliente, v.nomeCliente));
        }

        [Fact]
        public void ObterPorNomeCliente_ShouldThrowKeyNotFoundException_WhenNoVendasExist()
        {
            // Arrange
            var nomeCliente = "Cliente Inexistente";
            _vendaRepositoryMock.Setup(r => r.ObterPorNomeCliente(nomeCliente)).Returns((List<Venda>)null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() => _vendaService.ObterPorNomeCliente(nomeCliente));
            Assert.Equal("Venda não encontrada.", exception.Message);
        }

        #endregion

        #region ObterPorNomeProduto Tests

        [Fact]
        public void ObterPorNomeProduto_ShouldReturnVendas_WhenVendasExist()
        {
            // Arrange
            var nomeProduto = "Monitor";
            var cliente = new Cliente("Ana Costa", "ana@email.com", "hash321", "User", 3000m);
            var produto = new Produto(nomeProduto, 800m, 5);
            var vendas = new List<Venda>
            {
                new Venda(1, 800m, cliente.Id, produto.Id)
            };

            typeof(Venda).GetProperty("Cliente")!.SetValue(vendas[0], cliente);
            typeof(Venda).GetProperty("Produto")!.SetValue(vendas[0], produto);

            _vendaRepositoryMock.Setup(r => r.ObterPorNomeProduto(nomeProduto)).Returns(vendas);

            // Act
            var result = _vendaService.ObterPorNomeProduto(nomeProduto);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(nomeProduto, result[0].nomeProduto);
        }

        [Fact]
        public void ObterPorNomeProduto_ShouldThrowKeyNotFoundException_WhenNoVendasExist()
        {
            // Arrange
            var nomeProduto = "Produto Inexistente";
            _vendaRepositoryMock.Setup(r => r.ObterPorNomeProduto(nomeProduto)).Returns((List<Venda>)null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() => _vendaService.ObterPorNomeProduto(nomeProduto));
            Assert.Equal("Venda não encontrada.", exception.Message);
        }

        #endregion

        #region AddVenda Tests

        [Fact]
        public void AddVenda_ShouldCreateVendaSuccessfully()
        {
            // Arrange
            var cliente = new Cliente("Pedro Souza", "pedro@email.com", "hash654", "User", 5000m);
            var produto = new Produto("Webcam", 300m, 25);
            var vendaRequest = new AddVendaRequest("Pedro Souza", "Webcam", 2);

            _addVendaValidatorMock.Setup(v => v.Validate(vendaRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorNome("Pedro Souza")).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome("Webcam")).Returns(produto);

            // Act
            var result = _vendaService.AddVenda(vendaRequest);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _vendaRepositoryMock.Verify(r => r.Add(It.IsAny<Venda>()), Times.Once);
            _clienteRepositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Once);
            _produtoRepositoryMock.Verify(r => r.Update(It.IsAny<Produto>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public void AddVenda_ShouldThrowVendaInvalidaException_WhenValidationFails()
        {
            // Arrange
            var vendaRequest = new AddVendaRequest("", "Produto", -1);
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("nomeCliente", "Nome do cliente é obrigatório"),
                new ValidationFailure("qtdProduto", "Quantidade deve ser maior que zero")
            };

            _addVendaValidatorMock.Setup(v => v.Validate(vendaRequest))
                .Returns(new ValidationResult(validationFailures));

            // Act & Assert
            var exception = Assert.Throws<VendaInvalidaException>(() => _vendaService.AddVenda(vendaRequest));
            Assert.Contains("Nome do cliente é obrigatório", exception.Message);
        }

        [Fact]
        public void AddVenda_ShouldThrowClienteNaoEncontradoException_WhenClienteDoesNotExist()
        {
            // Arrange
            var vendaRequest = new AddVendaRequest("Cliente Inexistente", "Produto", 1);

            _addVendaValidatorMock.Setup(v => v.Validate(vendaRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorNome("Cliente Inexistente")).Returns((Cliente)null!);

            // Act & Assert
            var exception = Assert.Throws<ClienteNaoEncontradoException>(() => _vendaService.AddVenda(vendaRequest));
            Assert.Contains("Cliente Inexistente", exception.Message);
        }

        [Fact]
        public void AddVenda_ShouldThrowProdutoNaoEncontradoException_WhenProdutoDoesNotExist()
        {
            // Arrange
            var cliente = new Cliente("Cliente Teste", "teste@email.com", "hash", "User", 1000m);
            var vendaRequest = new AddVendaRequest("Cliente Teste", "Produto Inexistente", 1);

            _addVendaValidatorMock.Setup(v => v.Validate(vendaRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorNome("Cliente Teste")).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome("Produto Inexistente")).Returns((Produto)null!);

            // Act & Assert
            var exception = Assert.Throws<ProdutoNaoEncontradoException>(() => _vendaService.AddVenda(vendaRequest));
            Assert.Contains("Produto Inexistente", exception.Message);
        }

        [Fact]
        public void AddVenda_ShouldThrowEstoqueInsuficienteException_WhenNotEnoughStock()
        {
            // Arrange
            var cliente = new Cliente("Cliente Teste", "teste@email.com", "hash", "User", 10000m);
            var produto = new Produto("Produto Limitado", 100m, 5);
            var vendaRequest = new AddVendaRequest("Cliente Teste", "Produto Limitado", 10);

            _addVendaValidatorMock.Setup(v => v.Validate(vendaRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorNome("Cliente Teste")).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome("Produto Limitado")).Returns(produto);

            // Act & Assert
            var exception = Assert.Throws<EstoqueInsuficienteException>(() => _vendaService.AddVenda(vendaRequest));
            Assert.Contains("5", exception.Message);
            Assert.Contains("10", exception.Message);
        }

        [Fact]
        public void AddVenda_ShouldThrowSaldoInsuficienteException_WhenClienteHasInsufficientBalance()
        {
            // Arrange
            var cliente = new Cliente("Cliente Pobre", "pobre@email.com", "hash", "User", 100m);
            var produto = new Produto("Produto Caro", 500m, 10);
            var vendaRequest = new AddVendaRequest("Cliente Pobre", "Produto Caro", 2);

            _addVendaValidatorMock.Setup(v => v.Validate(vendaRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorNome("Cliente Pobre")).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome("Produto Caro")).Returns(produto);

            // Act & Assert
            var exception = Assert.Throws<SaldoInsuficienteException>(() => _vendaService.AddVenda(vendaRequest));
            Assert.Contains("100", exception.Message);
            Assert.Contains("1000", exception.Message);
        }

        [Fact]
        public void AddVenda_ShouldUpdateClienteSaldoAndProdutoEstoque()
        {
            // Arrange
            var cliente = new Cliente("Cliente Rico", "rico@email.com", "hash", "User", 5000m);
            var produto = new Produto("Produto Disponível", 200m, 50);
            var vendaRequest = new AddVendaRequest("Cliente Rico", "Produto Disponível", 5);

            _addVendaValidatorMock.Setup(v => v.Validate(vendaRequest))
                .Returns(new ValidationResult());

            _clienteRepositoryMock.Setup(r => r.ObterPorNome("Cliente Rico")).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome("Produto Disponível")).Returns(produto);

            // Act
            _vendaService.AddVenda(vendaRequest);

            // Assert
            Assert.Equal(4000m, cliente.Saldo); // 5000 - (200 * 5)
            Assert.Equal(45, produto.Estoque); // 50 - 5
        }

        #endregion

        #region UpdateVenda Tests

        [Fact]
        public void UpdateVenda_ShouldUpdateVendaSuccessfully()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var cliente = new Cliente("Cliente Atualização", "atualizacao@email.com", "hash", "User", 3000m);
            var produto = new Produto("Produto Update", 100m, 20);
            var venda = new Venda(5, 100m, cliente.Id, produto.Id);

            typeof(Venda).GetProperty("Cliente")!.SetValue(venda, cliente);
            typeof(Venda).GetProperty("Produto")!.SetValue(venda, produto);

            var updateRequest = new UpdateVendaRequest(3, 120m);

            _updateVendaValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _vendaRepositoryMock.Setup(r => r.ObterPorId(vendaId)).Returns(venda);
            _clienteRepositoryMock.Setup(r => r.ObterPorNome(cliente.Nome)).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome(produto.Nome)).Returns(produto);

            // Act
            _vendaService.UpdateVenda(vendaId, updateRequest);

            // Assert
            _vendaRepositoryMock.Verify(r => r.Update(It.IsAny<Venda>()), Times.Once);
            _clienteRepositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Once);
            _produtoRepositoryMock.Verify(r => r.Update(It.IsAny<Produto>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public void UpdateVenda_ShouldThrowVendaInvalidaException_WhenValidationFails()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var updateRequest = new UpdateVendaRequest(-1, -50m);
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("qtdProduto", "Quantidade inválida")
            };

            _updateVendaValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult(validationFailures));

            // Act & Assert
            var exception = Assert.Throws<VendaInvalidaException>(() => _vendaService.UpdateVenda(vendaId, updateRequest));
            Assert.Contains("Quantidade inválida", exception.Message);
        }

        [Fact]
        public void UpdateVenda_ShouldThrowVendaNaoEncontradaException_WhenVendaDoesNotExist()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var updateRequest = new UpdateVendaRequest(2, 100m);

            _updateVendaValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _vendaRepositoryMock.Setup(r => r.ObterPorId(vendaId)).Returns((Venda)null!);

            // Act & Assert
            var exception = Assert.Throws<VendaNaoEncontradaException>(() => _vendaService.UpdateVenda(vendaId, updateRequest));
            Assert.Contains(vendaId.ToString(), exception.Message);
        }

        [Fact]
        public void UpdateVenda_ShouldThrowEstoqueInsuficienteException_WhenNotEnoughStockForUpdate()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var cliente = new Cliente("Cliente Update", "update@email.com", "hash", "User", 5000m);
            var produto = new Produto("Produto Estoque Limitado", 100m, 10);
            var venda = new Venda(5, 100m, cliente.Id, produto.Id);

            typeof(Venda).GetProperty("Cliente")!.SetValue(venda, cliente);
            typeof(Venda).GetProperty("Produto")!.SetValue(venda, produto);

            var updateRequest = new UpdateVendaRequest(20, 100m); // Tentando aumentar de 5 para 20

            _updateVendaValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _vendaRepositoryMock.Setup(r => r.ObterPorId(vendaId)).Returns(venda);
            _clienteRepositoryMock.Setup(r => r.ObterPorNome(cliente.Nome)).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome(produto.Nome)).Returns(produto);

            // Act & Assert
            var exception = Assert.Throws<EstoqueInsuficienteException>(() => _vendaService.UpdateVenda(vendaId, updateRequest));
            Assert.Contains("15", exception.Message); // Estoque disponível: 10 + 5 (da venda original)
            Assert.Contains("20", exception.Message);
        }

        [Fact]
        public void UpdateVenda_ShouldThrowSaldoInsuficienteException_WhenClienteHasInsufficientBalanceForUpdate()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var cliente = new Cliente("Cliente Saldo Baixo", "baixo@email.com", "hash", "User", 1000m);
            var produto = new Produto("Produto Caro Update", 500m, 50);
            var venda = new Venda(2, 500m, cliente.Id, produto.Id); // Venda atual: 1000

            typeof(Venda).GetProperty("Cliente")!.SetValue(venda, cliente);
            typeof(Venda).GetProperty("Produto")!.SetValue(venda, produto);

            var updateRequest = new UpdateVendaRequest(5, 500m); // Nova venda: 2500

            _updateVendaValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _vendaRepositoryMock.Setup(r => r.ObterPorId(vendaId)).Returns(venda);
            _clienteRepositoryMock.Setup(r => r.ObterPorNome(cliente.Nome)).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome(produto.Nome)).Returns(produto);

            // Act & Assert
            var exception = Assert.Throws<SaldoInsuficienteException>(() => _vendaService.UpdateVenda(vendaId, updateRequest));
        }

        [Fact]
        public void UpdateVenda_ShouldIncreaseClienteSaldo_WhenReducingQuantity()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var cliente = new Cliente("Cliente Redução", "reducao@email.com", "hash", "User", 1000m);
            var produto = new Produto("Produto Redução", 200m, 30);
            var venda = new Venda(5, 200m, cliente.Id, produto.Id); // Valor total: 1000

            typeof(Venda).GetProperty("Cliente")!.SetValue(venda, cliente);
            typeof(Venda).GetProperty("Produto")!.SetValue(venda, produto);

            var updateRequest = new UpdateVendaRequest(3, 200m); // Novo valor total: 600

            _updateVendaValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _vendaRepositoryMock.Setup(r => r.ObterPorId(vendaId)).Returns(venda);
            _clienteRepositoryMock.Setup(r => r.ObterPorNome(cliente.Nome)).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome(produto.Nome)).Returns(produto);

            var saldoInicial = cliente.Saldo;

            // Act
            _vendaService.UpdateVenda(vendaId, updateRequest);

            // Assert
            Assert.True(cliente.Saldo > saldoInicial); // Saldo deve aumentar
            Assert.Equal(32, produto.Estoque); // Estoque deve aumentar de 30 para 32 (devolvendo 2 unidades)
        }

        [Fact]
        public void UpdateVenda_ShouldUpdateProdutoPreco()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var cliente = new Cliente("Cliente Preço", "preco@email.com", "hash", "User", 5000m);
            var produto = new Produto("Produto Preço", 100m, 50);
            var venda = new Venda(2, 100m, cliente.Id, produto.Id);

            typeof(Venda).GetProperty("Cliente")!.SetValue(venda, cliente);
            typeof(Venda).GetProperty("Produto")!.SetValue(venda, produto);

            var updateRequest = new UpdateVendaRequest(2, 150m); // Mesmo qtd, preço diferente

            _updateVendaValidatorMock.Setup(v => v.Validate(updateRequest))
                .Returns(new ValidationResult());

            _vendaRepositoryMock.Setup(r => r.ObterPorId(vendaId)).Returns(venda);
            _clienteRepositoryMock.Setup(r => r.ObterPorNome(cliente.Nome)).Returns(cliente);
            _produtoRepositoryMock.Setup(r => r.ObterPorNome(produto.Nome)).Returns(produto);

            // Act
            _vendaService.UpdateVenda(vendaId, updateRequest);

            // Assert
            Assert.Equal(150m, produto.Preco);
        }

        #endregion
    }
}