using FluentValidation;
using GestaoDeVendas.Application.DTO.Produto;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace GestaoDeVendas.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IValidator<AddProdutoRequest> _addProdutoValidator;
        private readonly IValidator<UpdateProdutoRequest> _updateProdutoValidator;
        private readonly ILogger<ProdutoService> _logger;

        public ProdutoService(
            IProdutoRepository produtoRepository,
            IValidator<AddProdutoRequest> addProdutoValidator,
            IValidator<UpdateProdutoRequest> updateProdutoValidator,
            ILogger<ProdutoService> logger)
        {
            _produtoRepository = produtoRepository;
            _addProdutoValidator = addProdutoValidator;
            _updateProdutoValidator = updateProdutoValidator;
            _logger = logger;
        }

        public async Task<List<GetProdutoRequest>> ObterTodosAsync()
        {
            _logger.LogInformation("Obtendo todos os produtos");

            var produtos = await _produtoRepository.ObterTodosAsync();
            var produtoDTOs = produtos
                .Select(p => new GetProdutoRequest(p.Id, p.Nome, p.Preco, p.Estoque))
                .ToList();

            _logger.LogInformation("Total de produtos retornados: {Count}", produtoDTOs.Count);

            return produtoDTOs;
        }

        public async Task<List<GetProdutoRequest>> ObterTodosAtivosAsync()
        {
            _logger.LogInformation("Obtendo todos os produtos ativos");

            var produtos = await _produtoRepository.ObterTodosAtivosAsync();
            var produtoDTOs = produtos
                .Select(p => new GetProdutoRequest(p.Id, p.Nome, p.Preco, p.Estoque))
                .ToList();

            _logger.LogInformation("Total de produtos ativos retornados: {Count}", produtoDTOs.Count);

            return produtoDTOs;
        }

        public GetProdutoRequest ObterPorId(Guid id)
        {
            _logger.LogInformation("Buscando produto por ID: {ProdutoId}", id);

            var produto = _produtoRepository.ObterPorId(id);

            if (produto is null)
            {
                _logger.LogWarning("Produto não encontrado com ID: {ProdutoId}", id);
                throw new ProdutoNaoEncontradoException(id);
            }

            var produtoDTO = new GetProdutoRequest(produto.Id, produto.Nome, produto.Preco, produto.Estoque);

            _logger.LogInformation("Produto encontrado: {ProdutoNome}", produto.Nome);

            return produtoDTO;
        }

        public GetProdutoRequest ObterPorNome(string nome)
        {
            _logger.LogInformation("Buscando produto por nome: {ProdutoNome}", nome);

            var produto = _produtoRepository.ObterPorNome(nome);

            if (produto is null)
            {
                _logger.LogWarning("Produto não encontrado com nome: {ProdutoNome}", nome);
                throw new ProdutoNaoEncontradoException(nome);
            }

            var produtoDTO = new GetProdutoRequest(produto.Id, produto.Nome, produto.Preco, produto.Estoque);

            _logger.LogInformation("Produto encontrado: {ProdutoId}", produto.Id);

            return produtoDTO;
        }

        public Guid AddProduto(AddProdutoRequest produtoRequest)
        {
            _logger.LogInformation("Iniciando adição de produto: {ProdutoNome}", produtoRequest.nome);

            try
            {
                var validationResult = _addProdutoValidator.Validate(produtoRequest);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validação falhou ao adicionar produto: {ProdutoNome}. Erros: {Errors}",
                        produtoRequest.nome, string.Join("; ", validationResult.Errors));
                    throw new ProdutoInvalidoException(string.Join("; ", validationResult.Errors));
                }

                var produto = new Produto(
                    produtoRequest.nome,
                    produtoRequest.preco,
                    produtoRequest.estoque);

                _produtoRepository.Add(produto);
                _produtoRepository.SaveChanges();

                _logger.LogInformation("Produto adicionado com sucesso. ID: {ProdutoId}, Nome: {ProdutoNome}, Estoque: {Estoque}",
                    produto.Id, produto.Nome, produto.Estoque);

                return produto.Id;
            }
            catch (Exception ex) when (ex is not ProdutoInvalidoException)
            {
                _logger.LogError(ex, "Erro ao adicionar produto: {ProdutoNome}", produtoRequest.nome);
                throw new ProdutoException($"Erro ao adicionar produto: {ex.Message}");
            }
        }

        public void UpdateProduto(Guid id, UpdateProdutoRequest produtoRequest)
        {
            _logger.LogInformation("Iniciando atualização de produto. ID: {ProdutoId}", id);

            try
            {
                var validationResult = _updateProdutoValidator.Validate(produtoRequest);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validação falhou ao atualizar produto ID: {ProdutoId}. Erros: {Errors}",
                        id, string.Join("; ", validationResult.Errors));
                    throw new ProdutoInvalidoException(string.Join("; ", validationResult.Errors));
                }

                var produto = _produtoRepository.ObterPorId(id);

                produto.AtualizarNome(produtoRequest.nome);
                produto.AtualizarPreco(produtoRequest.preco);
                produto.AtualizarEstoqueTotal(produtoRequest.estoque);

                _produtoRepository.Update(produto);
                _produtoRepository.SaveChanges();

                _logger.LogInformation("Produto atualizado com sucesso. ID: {ProdutoId}, Nome: {ProdutoNome}, Estoque: {Estoque}",
                    produto.Id, produto.Nome, produto.Estoque);
            }
            catch (Exception ex) when (ex is not ProdutoInvalidoException)
            {
                _logger.LogError(ex, "Erro ao atualizar produto ID: {ProdutoId}", id);
                throw new ProdutoException($"Erro ao atualizar produto: {ex.Message}");
            }
        }

        public void DeleteProduto(Guid id)
        {
            _logger.LogInformation("Iniciando desativação de produto. ID: {ProdutoId}", id);

            try
            {
                var produto = _produtoRepository.ObterPorId(id);

                if (produto is null)
                {
                    _logger.LogWarning("Produto não encontrado para desativação. ID: {ProdutoId}", id);
                    throw new ProdutoNaoEncontradoException(id);
                }

                produto.Desativar();
                _produtoRepository.Update(produto);
                _produtoRepository.SaveChanges();

                _logger.LogInformation("Produto desativado com sucesso. ID: {ProdutoId}, Nome: {ProdutoNome}",
                    produto.Id, produto.Nome);
            }
            catch (Exception ex) when (ex is not ProdutoNaoEncontradoException)
            {
                _logger.LogError(ex, "Erro ao desativar produto ID: {ProdutoId}", id);
                throw new ProdutoException($"Erro ao deletar produto: {ex.Message}");
            }
        }
    }
}