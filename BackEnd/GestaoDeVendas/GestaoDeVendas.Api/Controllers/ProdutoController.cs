using GestaoDeVendas.Application.DTO.Produto;
using GestaoDeVendas.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeVendas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ObterTodos()
        {
            var produtos = await _produtoService.ObterTodosAsync();

            return Ok(produtos);
        }

        [HttpGet("ativos")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ObterTodosAtivos()
        {
            var produtos = await _produtoService.ObterTodosAtivosAsync();

            return Ok(produtos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public IActionResult ObterPorId(Guid id)
        {
            var produto = _produtoService.ObterPorId(id);
            return Ok(produto);
        }

        [HttpPost]
        [Authorize(Roles = "admin, user")]
        public IActionResult AddProduto([FromBody] AddProdutoRequest request)
        {
            if (request is null)
                return BadRequest("Request inválido.");

            var produtoId = _produtoService.AddProduto(request);
            return Ok(produtoId);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateProduto(Guid id, [FromBody] UpdateProdutoRequest request)
        {
            if (request is null)
                return BadRequest("Request inválido.");

            _produtoService.UpdateProduto(id, request);
            return Ok("Produto Atualizado");
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteProduto(Guid id)
        {
            _produtoService.DeleteProduto(id);
            return Ok("Produto Excluido");

        }
    }
}