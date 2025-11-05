using FluentValidation;
using GestaoDeVendas.Application.DTO.Venda;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace GestaoDeVendas.Application.Services
{
    public class VendaService : IVendaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AddVendaRequest> _addVendaValidator;
        private readonly IValidator<UpdateVendaRequest> _updateVendaValidator;
        private readonly ILogger<VendaService> _logger;

        public VendaService(
            IUnitOfWork unitOfWork,
            IValidator<AddVendaRequest> addVendaValidator,
            IValidator<UpdateVendaRequest> updateVendaValidator,
            ILogger<VendaService> logger)
        {
            _unitOfWork = unitOfWork;
            _addVendaValidator = addVendaValidator;
            _updateVendaValidator = updateVendaValidator;
            _logger = logger;
        }

        public async Task<List<GetVendaRequest>> ObterTodosAsync()
        {
            _logger.LogInformation("Obtendo todas as vendas");

            var vendas = await _unitOfWork.Vendas.ObterTodosAsync();
            var vendasDTOs = vendas
                .Select(v => new GetVendaRequest(v.Id, v.QtdProduto, v.ValorUnitario, v.ValorTotal, v.DataVenda, v.Cliente.Nome, v.Produto.Nome))
                .ToList();

            _logger.LogInformation("Total de vendas retornadas: {Count}", vendasDTOs.Count);

            return vendasDTOs;
        }

        public GetVendaRequest ObterPorId(Guid id)
        {
            _logger.LogInformation("Buscando venda por ID: {VendaId}", id);

            var venda = _unitOfWork.Vendas.ObterPorId(id);

            if (venda is null)
            {
                _logger.LogWarning("Venda não encontrada com ID: {VendaId}", id);
                throw new KeyNotFoundException("Venda não encontrada.");
            }

            var vendaDTO = new GetVendaRequest(venda.Id, venda.QtdProduto, venda.ValorUnitario, venda.ValorTotal, venda.DataVenda, venda.Cliente.Nome, venda.Produto.Nome);

            _logger.LogInformation("Venda encontrada. Cliente: {ClienteNome}, Produto: {ProdutoNome}",
                venda.Cliente.Nome, venda.Produto.Nome);

            return vendaDTO;
        }

        public List<GetVendaRequest> ObterPorNomeCliente(string nomeCliente)
        {
            _logger.LogInformation("Buscando vendas por nome do cliente: {ClienteNome}", nomeCliente);

            var vendas = _unitOfWork.Vendas.ObterPorNomeCliente(nomeCliente);

            if (vendas is null)
            {
                _logger.LogWarning("Nenhuma venda encontrada para o cliente: {ClienteNome}", nomeCliente);
                throw new KeyNotFoundException("Venda não encontrada.");
            }

            var vendasDTOs = vendas
                .Select(v => new GetVendaRequest(v.Id, v.QtdProduto, v.ValorUnitario, v.ValorTotal, v.DataVenda, v.Cliente.Nome, v.Produto.Nome))
                .ToList();

            _logger.LogInformation("Total de vendas encontradas para o cliente {ClienteNome}: {Count}",
                nomeCliente, vendasDTOs.Count);

            return vendasDTOs;
        }

        public List<GetVendaRequest> ObterPorNomeProduto(string nomeProduto)
        {
            _logger.LogInformation("Buscando vendas por nome do produto: {ProdutoNome}", nomeProduto);

            var vendas = _unitOfWork.Vendas.ObterPorNomeProduto(nomeProduto);

            if (vendas is null)
            {
                _logger.LogWarning("Nenhuma venda encontrada para o produto: {ProdutoNome}", nomeProduto);
                throw new KeyNotFoundException("Venda não encontrada.");
            }

            var vendasDTOs = vendas
                .Select(v => new GetVendaRequest(v.Id, v.QtdProduto, v.ValorUnitario, v.ValorTotal, v.DataVenda, v.Cliente.Nome, v.Produto.Nome))
                .ToList();

            _logger.LogInformation("Total de vendas encontradas para o produto {ProdutoNome}: {Count}",
                nomeProduto, vendasDTOs.Count);

            return vendasDTOs;
        }

        public Guid AddVenda(AddVendaRequest vendaRequest)
        {
            _logger.LogInformation("Iniciando processo de venda. Cliente: {ClienteNome}, Produto: {ProdutoNome}, Quantidade: {Quantidade}",
                vendaRequest.nomeCliente, vendaRequest.nomeProduto, vendaRequest.qtdProduto);

            #region Validação Venda
            var validationResult = _addVendaValidator.Validate(vendaRequest);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao adicionar venda. Erros: {Errors}",
                    string.Join("; ", validationResult.Errors));
                throw new VendaInvalidaException(string.Join("; ", validationResult.Errors));
            }

            var cliente = _unitOfWork.Clientes.ObterPorNome(vendaRequest.nomeCliente);
            var produto = _unitOfWork.Produtos.ObterPorNome(vendaRequest.nomeProduto);

            if (cliente is null)
            {
                _logger.LogWarning("Cliente não encontrado: {ClienteNome}", vendaRequest.nomeCliente);
                throw new ClienteNaoEncontradoException(vendaRequest.nomeCliente);
            }

            if (produto is null)
            {
                _logger.LogWarning("Produto não encontrado: {ProdutoNome}", vendaRequest.nomeProduto);
                throw new ProdutoNaoEncontradoException(vendaRequest.nomeProduto);
            }

            //Validar estoque disponível
            if (!produto.ValidarEstoque(vendaRequest.qtdProduto))
            {
                _logger.LogWarning("Estoque insuficiente. Produto: {ProdutoNome}, Disponível: {EstoqueDisponivel}, Solicitado: {QtdSolicitada}",
                    produto.Nome, produto.Estoque, vendaRequest.qtdProduto);
                throw new EstoqueInsuficienteException(produto.Estoque, vendaRequest.qtdProduto);
            }

            //Calcular valor total da venda
            decimal valorTotal = produto.Preco * vendaRequest.qtdProduto;

            //Validar saldo do cliente
            if (!cliente.ValidarSaldo(valorTotal))
            {
                _logger.LogWarning("Saldo insuficiente. Cliente: {ClienteNome}, Disponível: {SaldoDisponivel:C}, Necessário: {ValorNecessario:C}",
                    cliente.Nome, cliente.Saldo, valorTotal);
                throw new SaldoInsuficienteException(cliente.Saldo, valorTotal);
            }
            #endregion


            try
            {
                var venda = new Venda(
                    qtdProduto: vendaRequest.qtdProduto,
                    valorUnitario: produto.Preco,
                    clienteId: cliente.Id,
                    produtoId: produto.Id);

                // Atualizar saldo do cliente
                cliente.ReduzirSaldo(valorTotal);
                _unitOfWork.Clientes.Update(cliente);

                // Atualizar estoque do produto
                produto.ReduzirEstoque(vendaRequest.qtdProduto);
                _unitOfWork.Produtos.Update(produto);

                _unitOfWork.Vendas.Add(venda);
                _unitOfWork.Commit();

                _logger.LogInformation("Venda processada com sucesso. ID: {VendaId}, Cliente: {ClienteNome}, Produto: {ProdutoNome}, Valor Total: {ValorTotal:C}",
                    venda.Id, cliente.Nome, produto.Nome, valorTotal);

                return venda.Id;
            }
            catch (Exception ex) when (ex is not VendaException and not ClienteNaoEncontradoException and not ProdutoNaoEncontradoException)
            {
                _logger.LogError(ex, "Erro ao processar venda. Cliente: {ClienteNome}, Produto: {ProdutoNome}",
                    vendaRequest.nomeCliente, vendaRequest.nomeProduto);
                throw new VendaException($"Erro ao processar a venda: {ex.Message}");
            }
        }

        public void UpdateVenda(Guid id, UpdateVendaRequest vendaRequest)
        {
            _logger.LogInformation("Iniciando atualização de venda. ID: {VendaId}, Nova Quantidade: {Quantidade}, Novo Valor: {Valor:C}",
                id, vendaRequest.qtdProduto, vendaRequest.valorUnitario);

            #region Validação Venda
            var validationResult = _updateVendaValidator.Validate(vendaRequest);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao atualizar venda ID: {VendaId}. Erros: {Errors}",
                    id, string.Join("; ", validationResult.Errors));
                throw new VendaInvalidaException(string.Join("; ", validationResult.Errors));
            }

            var venda = _unitOfWork.Vendas.ObterPorId(id);

            if (venda is null)
            {
                _logger.LogWarning("Venda não encontrada para atualização. ID: {VendaId}", id);
                throw new VendaNaoEncontradaException(id);
            }

            var cliente = _unitOfWork.Clientes.ObterPorNome(venda.Cliente.Nome);
            var produto = _unitOfWork.Produtos.ObterPorNome(venda.Produto.Nome);

            if (cliente is null)
            {
                _logger.LogWarning("Cliente não encontrado: {ClienteNome}", venda.Cliente.Nome);
                throw new ClienteNaoEncontradoException(venda.Cliente.Nome);
            }

            if (produto is null)
            {
                _logger.LogWarning("Produto não encontrado: {ProdutoNome}", venda.Produto.Nome);
                throw new ProdutoNaoEncontradoException(venda.Produto.Nome);
            }


            // Restaura o estoque original
            int estoqueDisponivel = produto.Estoque + venda.QtdProduto;

            // Validar se há estoque suficiente para a nova quantidade
            if (estoqueDisponivel < vendaRequest.qtdProduto)
            {
                _logger.LogWarning("Estoque insuficiente para atualização. Produto: {ProdutoNome}, Disponível: {EstoqueDisponivel}, Solicitado: {QtdSolicitada}",
                    produto.Nome, estoqueDisponivel, vendaRequest.qtdProduto);
                throw new EstoqueInsuficienteException(estoqueDisponivel, vendaRequest.qtdProduto);
            }

            // Calcular valores
            decimal valorTotalAntigo = venda.ValorTotal;
            decimal valorTotalNovo = vendaRequest.qtdProduto * vendaRequest.valorUnitario;

            // Calcular diferença que será cobrada/devolvida ao cliente
            decimal diferencaValor = valorTotalNovo - valorTotalAntigo;

            decimal saldoAntigo = cliente.Saldo + venda.ValorTotal;
            // Se a diferença for positiva, validar se o cliente tem saldo suficiente
            if (diferencaValor > 0 && !cliente.ValidarSaldo(diferencaValor))
            {
                _logger.LogWarning("Saldo insuficiente para atualização. Cliente: {ClienteNome}, Disponível: {SaldoDisponivel:C}, Necessário: {ValorNecessario:C}",
                    cliente.Nome, saldoAntigo, valorTotalNovo);
                throw new SaldoInsuficienteException(saldoAntigo, valorTotalNovo);
            }
            #endregion


            try
            {
                //Atualizar saldo do cliente
                if (diferencaValor > 0)
                    cliente.ReduzirSaldo(diferencaValor);
                else if (diferencaValor < 0)
                    cliente.AumentarSaldo(Math.Abs(diferencaValor));

                _unitOfWork.Clientes.Update(cliente);

                //Atualizar estoque e preco do produto
                int diferencaQuantidade = vendaRequest.qtdProduto - venda.QtdProduto;

                if (diferencaQuantidade > 0)
                    produto.ReduzirEstoque(diferencaQuantidade);
                else if (diferencaQuantidade < 0)
                    produto.AumentarEstoque(Math.Abs(diferencaQuantidade));

                produto.AtualizarPreco(vendaRequest.valorUnitario);
                _unitOfWork.Produtos.Update(produto);

                venda.AtualizarVenda(vendaRequest.qtdProduto, vendaRequest.valorUnitario);

                _unitOfWork.Vendas.Update(venda);
                _unitOfWork.Commit();

                _logger.LogInformation("Venda atualizada com sucesso. ID: {VendaId}, Diferença de Valor: {DiferencaValor:C}, Novo Total: {ValorTotal:C}",
                    venda.Id, diferencaValor, valorTotalNovo);
            }
            catch (Exception ex) when (ex is not VendaException and not ClienteNaoEncontradoException and not ProdutoNaoEncontradoException)
            {
                _logger.LogError(ex, "Erro ao atualizar venda ID: {VendaId}", id);
                throw new VendaException($"Erro ao processar a venda: {ex.Message}");
            }
        }
    }
}