using GestaoDeVendas.Application.DTO.Produto;

namespace GestaoDeVendas.Application.IServices
{
    public interface IProdutoService
    {
        Task<List<GetProdutoRequest>> ObterTodosAsync();
        Task<List<GetProdutoRequest>> ObterTodosAtivosAsync();
        GetProdutoRequest ObterPorId(Guid id);
        GetProdutoRequest ObterPorNome(string nome);
        Guid AddProduto(AddProdutoRequest ProdutoDTO);
        void UpdateProduto(Guid id, UpdateProdutoRequest ProdutoRequest);
        void DeleteProduto(Guid id);
    }
}