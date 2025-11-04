using FluentValidation;
using GestaoDeVendas.Application.DTO.Venda;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.Entity;

namespace GestaoDeVendas.Application.Services
{
    public class VendaService : IVendaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<AddVendaRequest> _addVendaValidator;
        private readonly IValidator<UpdateVendaRequest> _updateVendaValidator;

        public VendaService(
            IUnitOfWork unitOfWork,
            IValidator<AddVendaRequest> addVendaValidator,
            IValidator<UpdateVendaRequest> updateVendaValidator)
        {
            _unitOfWork = unitOfWork;
            _addVendaValidator = addVendaValidator;
            _updateVendaValidator = updateVendaValidator;
        }

        public async Task<List<GetVendaRequest>> ObterTodosAsync()
        {
            var vendas = await _unitOfWork.Vendas.ObterTodosAsync();
            var vendasDTOs = vendas
                .Select(v => new GetVendaRequest(v.Id, v.QtdProduto, v.ValorUnitario, v.ValorTotal, v.DataVenda, v.Cliente.Nome, v.Produto.Nome))
                .ToList();

            return vendasDTOs;
        }

        public GetVendaRequest ObterPorId(Guid id)
        {
            var venda = _unitOfWork.Vendas.ObterPorId(id);

            if (venda is null)
                throw new KeyNotFoundException("Venda não encontrada.");

            var vendaDTO = new GetVendaRequest(venda.Id, venda.QtdProduto, venda.ValorUnitario, venda.ValorTotal, venda.DataVenda, venda.Cliente.Nome, venda.Produto.Nome);

            return vendaDTO;
        }

        public List<GetVendaRequest> ObterPorNomeCliente(string nomeCliente)
        {
            var vendas = _unitOfWork.Vendas.ObterPorNomeCliente(nomeCliente);

            if (vendas is null)
                throw new KeyNotFoundException("Venda não encontrada.");

            var vendasDTOs = vendas
                .Select(v => new GetVendaRequest(v.Id, v.QtdProduto, v.ValorUnitario, v.ValorTotal, v.DataVenda, v.Cliente.Nome, v.Produto.Nome))
                .ToList();

            return vendasDTOs;
        }

        public List<GetVendaRequest> ObterPorNomeProduto(string nomeProduto)
        {
            var vendas = _unitOfWork.Vendas.ObterPorNomeProduto(nomeProduto);

            if (vendas is null)
                throw new KeyNotFoundException("Venda não encontrada.");

            var vendasDTOs = vendas
                .Select(v => new GetVendaRequest(v.Id, v.QtdProduto, v.ValorUnitario, v.ValorTotal, v.DataVenda, v.Cliente.Nome, v.Produto.Nome))
                .ToList();

            return vendasDTOs;
        }

        public Guid AddVenda(AddVendaRequest vendaRequest)
        {
            #region Validação Venda
            var validationResult = _addVendaValidator.Validate(vendaRequest);

            if (!validationResult.IsValid)
                throw new VendaInvalidaException(string.Join("; ", validationResult.Errors));

            var cliente = _unitOfWork.Clientes.ObterPorNome(vendaRequest.nomeCliente);
            var produto = _unitOfWork.Produtos.ObterPorNome(vendaRequest.nomeProduto);

            if (cliente is null)
                throw new ClienteNaoEncontradoException(vendaRequest.nomeCliente);

            if (produto is null)
                throw new ProdutoNaoEncontradoException(vendaRequest.nomeProduto);

            //Validar estoque disponível
            if (!produto.ValidarEstoque(vendaRequest.qtdProduto))
                throw new EstoqueInsuficienteException(produto.Estoque, vendaRequest.qtdProduto);

            //Calcular valor total da venda
            decimal valorTotal = produto.Preco * vendaRequest.qtdProduto;

            //Validar saldo do cliente
            if (!cliente.ValidarSaldo(valorTotal))
                throw new SaldoInsuficienteException(cliente.Saldo, valorTotal);
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

                return venda.Id;
            }
            catch (Exception ex)
            {
                throw new VendaException($"Erro ao processar a venda: {ex.Message}");
            }
        }

        public void UpdateVenda(Guid id, UpdateVendaRequest vendaRequest)
        {
            #region Validação Venda
            var validationResult = _updateVendaValidator.Validate(vendaRequest);

            if (!validationResult.IsValid)
                throw new VendaInvalidaException(string.Join("; ", validationResult.Errors));

            var venda = _unitOfWork.Vendas.ObterPorId(id);

            if (venda is null)
                throw new VendaNaoEncontradaException(id);

            var cliente = _unitOfWork.Clientes.ObterPorNome(venda.Cliente.Nome);
            var produto = _unitOfWork.Produtos.ObterPorNome(venda.Produto.Nome);

            if (cliente is null)
                throw new ClienteNaoEncontradoException(venda.Cliente.Nome);

            if (produto is null)
                throw new ProdutoNaoEncontradoException(venda.Produto.Nome);


            // Restaura o estoque original
            int estoqueDisponivel = produto.Estoque + venda.QtdProduto;

            // Validar se há estoque suficiente para a nova quantidade
            if (estoqueDisponivel < vendaRequest.qtdProduto)
                throw new EstoqueInsuficienteException(estoqueDisponivel, vendaRequest.qtdProduto);

            // Calcular valores
            decimal valorTotalAntigo = venda.ValorTotal;
            decimal valorTotalNovo = vendaRequest.qtdProduto * vendaRequest.valorUnitario;

            // Calcular diferença que será cobrada/devolvida ao cliente
            decimal diferencaValor = valorTotalNovo - valorTotalAntigo;

            decimal saldoAntigo = cliente.Saldo + venda.ValorTotal;
            // Se a diferença for positiva, validar se o cliente tem saldo suficiente
            if (diferencaValor > 0 && !cliente.ValidarSaldo(diferencaValor))
                throw new SaldoInsuficienteException(saldoAntigo, valorTotalNovo);
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
            }
            catch (Exception ex)
            {
                throw new VendaException($"Erro ao processar a venda: {ex.Message}");
            }
        }
    }
}