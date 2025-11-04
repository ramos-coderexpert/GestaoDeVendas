using FluentValidation;
using GestaoDeVendas.Application.DTO.Produto;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Domain.Entity;

namespace GestaoDeVendas.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IValidator<AddProdutoRequest> _addProdutoValidator;
        private readonly IValidator<UpdateProdutoRequest> _updateProdutoValidator;

        public ProdutoService(
            IProdutoRepository produtoRepository,
            IValidator<AddProdutoRequest> addProdutoValidator,
            IValidator<UpdateProdutoRequest> updateProdutoValidator)
        {
            _produtoRepository = produtoRepository;
            _addProdutoValidator = addProdutoValidator;
            _updateProdutoValidator = updateProdutoValidator;
        }

        public async Task<List<GetProdutoRequest>> ObterTodosAsync()
        {
            var produtos = await _produtoRepository.ObterTodosAsync();
            var produtoDTOs = produtos
                .Select(p => new GetProdutoRequest(p.Id, p.Nome, p.Preco, p.Estoque))
                .ToList();

            return produtoDTOs;
        }

        public async Task<List<GetProdutoRequest>> ObterTodosAtivosAsync()
        {
            var produtos = await _produtoRepository.ObterTodosAtivosAsync();
            var produtoDTOs = produtos
                .Select(p => new GetProdutoRequest(p.Id, p.Nome, p.Preco, p.Estoque))
                .ToList();

            return produtoDTOs;
        }

        public GetProdutoRequest ObterPorId(Guid id)
        {
            var produto = _produtoRepository.ObterPorId(id);

            if (produto is null)
                throw new ProdutoNaoEncontradoException(id);

            var produtoDTO = new GetProdutoRequest(produto.Id, produto.Nome, produto.Preco, produto.Estoque);

            return produtoDTO;
        }

        public GetProdutoRequest ObterPorNome(string nome)
        {
            var produto = _produtoRepository.ObterPorNome(nome);

            if (produto is null)
                throw new ProdutoNaoEncontradoException(nome);

            var produtoDTO = new GetProdutoRequest(produto.Id, produto.Nome, produto.Preco, produto.Estoque);

            return produtoDTO;
        }

        public Guid AddProduto(AddProdutoRequest produtoRequest)
        {
            try
            {
                var validationResult = _addProdutoValidator.Validate(produtoRequest);

                if (!validationResult.IsValid)
                    throw new ProdutoInvalidoException(string.Join("; ", validationResult.Errors));

                var produto = new Produto(
                    produtoRequest.nome,
                    produtoRequest.preco,
                    produtoRequest.estoque);

                _produtoRepository.Add(produto);
                _produtoRepository.SaveChanges();

                return produto.Id;
            }
            catch (Exception ex)
            {
                throw new ProdutoException($"Erro ao adicionar produto: {ex.Message}");
            }
        }

        public void UpdateProduto(Guid id, UpdateProdutoRequest produtoRequest)
        {
            try
            {
                var validationResult = _updateProdutoValidator.Validate(produtoRequest);

                if (!validationResult.IsValid)
                    throw new ProdutoInvalidoException(string.Join("; ", validationResult.Errors));

                var produto = _produtoRepository.ObterPorId(id);

                produto.AtualizarNome(produtoRequest.nome);
                produto.AtualizarPreco(produtoRequest.preco);
                produto.AtualizarEstoqueTotal(produtoRequest.estoque);

                _produtoRepository.Update(produto);
                _produtoRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ProdutoException($"Erro ao atualizar produto: {ex.Message}");
            }
        }

        public void DeleteProduto(Guid id)
        {
            try
            {
                var produto = _produtoRepository.ObterPorId(id);

                if (produto is null)
                    throw new ProdutoNaoEncontradoException(id);

                produto.Desativar();
                _produtoRepository.Update(produto);
                _produtoRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ProdutoException($"Erro ao deletar produto: {ex.Message}");
            }
        }
    }
}