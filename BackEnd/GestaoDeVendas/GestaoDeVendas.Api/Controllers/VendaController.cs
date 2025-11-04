using GestaoDeVendas.Application.DTO.Venda;
using GestaoDeVendas.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeVendas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class VendaController : ControllerBase
    {
        private readonly IVendaService _vendaService;

        public VendaController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var vendas = await _vendaService.ObterTodosAsync();

            return Ok(vendas);
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(Guid id)
        {
            var venda = _vendaService.ObterPorId(id);
            return Ok(venda);
        }

        [HttpGet("nomeCliente/{nomeCliente}")]
        public IActionResult ObterPorNomeCliente(string nomeCliente)
        {
            var vendas = _vendaService.ObterPorNomeCliente(nomeCliente);
            return Ok(vendas);
        }

        [HttpGet("nomeProduto/{nomeProduto}")]
        public IActionResult ObterPorNomeProduto(string nomeProduto)
        {
            var vendas = _vendaService.ObterPorNomeProduto(nomeProduto);
            return Ok(vendas);
        }

        [HttpPost]
        public IActionResult AddVenda([FromBody] AddVendaRequest request)
        {
            if (request is null)
                return BadRequest("Request inválido.");

            var vendaId = _vendaService.AddVenda(request);
            return Ok(vendaId);
        }

        [HttpPut]
        public IActionResult UpdateVenda(Guid id, [FromBody] UpdateVendaRequest request)
        {
            if (request is null)
                return BadRequest("Request inválido.");

            _vendaService.UpdateVenda(id, request);
            return Ok("Venda Atualizada");
        }
    }
}