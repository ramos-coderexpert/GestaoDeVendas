using GestaoDeVendas.Application.DTO.Auth;
using GestaoDeVendas.Application.DTO.Cliente;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeVendas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var response = _clienteService.Login(request);
                return Ok(response);
            }
            catch (CredenciaisInvalidasException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ObterTodos()
        {
            var clientes = await _clienteService.ObterTodosAsync();

            return Ok(clientes);
        }

        [HttpGet("ativos")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ObterTodosAtivos()
        {
            var clientes = await _clienteService.ObterTodosAtivosAsync();

            return Ok(clientes);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public IActionResult ObterPorId(Guid id)
        {
            var cliente = _clienteService.ObterPorId(id);
            return Ok(cliente);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddCliente([FromBody] AddClienteRequest request)
        {
            if (request is null)
                return BadRequest("Request inválido.");

            var clienteId = _clienteService.AddCliente(request);
            return Ok(clienteId);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateCliente(Guid id, [FromBody] UpdateClienteRequest request)
        {
            if (request is null)
                return BadRequest("Request inválido.");

            _clienteService.UpdateCliente(id, request);
            return Ok("Cliente Atualizado");
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteCliente(Guid id)
        {
            _clienteService.DeleteCliente(id);
            return Ok("Cliente Excluido");
        }
    }
}