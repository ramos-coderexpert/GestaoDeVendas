using FluentValidation;
using GestaoDeVendas.Application.DTO.Auth;
using GestaoDeVendas.Application.DTO.Cliente;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.Services;
using Microsoft.Extensions.Logging;

namespace GestaoDeVendas.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IAuthService _authService;
        private readonly IValidator<LoginRequestDTO> _loginValidator;
        private readonly IValidator<AddClienteRequest> _addClienteValidator;
        private readonly IValidator<UpdateClienteRequest> _updateClienteValidator;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(
            IClienteRepository clienteRepository,
            IAuthService authService,
            IValidator<LoginRequestDTO> loginValidator,
            IValidator<AddClienteRequest> addClienteValidator,
            IValidator<UpdateClienteRequest> updateClienteValidator,
            ILogger<ClienteService> logger)
        {
            _loginValidator = loginValidator;
            _clienteRepository = clienteRepository;
            _authService = authService;
            _addClienteValidator = addClienteValidator;
            _updateClienteValidator = updateClienteValidator;
            _logger = logger;
        }


        public LoginResponseDTO Login(LoginRequestDTO loginRequest)
        {
            _logger.LogInformation("Iniciando processo de login para o email: {Email}", loginRequest.email);

            var validationResult = _loginValidator.Validate(loginRequest);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validação falhou para o email: {Email}", loginRequest.email);
                throw new CredenciaisInvalidasException(loginRequest.email);
            }

            try
            {
                var passwordHash = _authService.ComputeSha256Hash(loginRequest.password);
                var cliente = _clienteRepository.ObterPorEmailPassword(loginRequest.email, passwordHash);

                if (cliente is null)
                {
                    _logger.LogWarning("Login não encontrado para o email: {Email}", loginRequest.email);
                    throw new ClienteInvalidoException($"Login não encontrado com email: {loginRequest.email}");
                }

                var token = _authService.GenerateJwtToken(cliente.Email, cliente.Role);

                _logger.LogInformation("Login realizado com sucesso para o email: {Email}, Role: {Role}", cliente.Email, cliente.Role);

                return new LoginResponseDTO(token, cliente.Email, cliente.Role);
            }
            catch (Exception ex) when (ex is not ClienteInvalidoException)
            {
                _logger.LogError(ex, "Erro ao realizar login para o email: {Email}", loginRequest.email);
                throw new ClienteException($"Erro ao realizar login: {ex.Message}");
            }
        }

        public async Task<List<GetClienteRequest>> ObterTodosAsync()
        {
            _logger.LogInformation("Obtendo todos os clientes");

            var clientes = await _clienteRepository.ObterTodosAsync();
            var clienteDTOs = clientes
                .Select(c => new GetClienteRequest(c.Id, c.Nome, c.Email, c.Role, c.Saldo))
                .ToList();

            _logger.LogInformation("Total de clientes retornados: {Count}", clienteDTOs.Count);

            return clienteDTOs;
        }

        public async Task<List<GetClienteRequest>> ObterTodosAtivosAsync()
        {
            _logger.LogInformation("Obtendo todos os clientes ativos");

            var clientes = await _clienteRepository.ObterTodosAtivosAsync();
            var clienteDTOs = clientes
                .Select(c => new GetClienteRequest(c.Id, c.Nome, c.Email, c.Role, c.Saldo))
                .ToList();

            _logger.LogInformation("Total de clientes ativos retornados: {Count}", clienteDTOs.Count);

            return clienteDTOs;
        }

        public GetClienteRequest ObterPorId(Guid id)
        {
            _logger.LogInformation("Buscando cliente por ID: {ClienteId}", id);

            var cliente = _clienteRepository.ObterPorId(id);

            if (cliente is null)
            {
                _logger.LogWarning("Cliente não encontrado com ID: {ClienteId}", id);
                throw new KeyNotFoundException("Cliente não encontrado.");
            }

            var clienteDTO = new GetClienteRequest(cliente.Id, cliente.Nome, cliente.Email, cliente.Role, cliente.Saldo);

            _logger.LogInformation("Cliente encontrado: {ClienteNome}", cliente.Nome);

            return clienteDTO;
        }

        public GetClienteRequest ObterPorNome(string nome)
        {
            _logger.LogInformation("Buscando cliente por nome: {ClienteNome}", nome);

            var cliente = _clienteRepository.ObterPorNome(nome);

            if (cliente is null)
            {
                _logger.LogWarning("Cliente não encontrado com nome: {ClienteNome}", nome);
                throw new ClienteNaoEncontradoException(nome);
            }

            var clienteDTO = new GetClienteRequest(cliente.Id, cliente.Nome, cliente.Email, cliente.Role, cliente.Saldo);

            _logger.LogInformation("Cliente encontrado: {ClienteId}", cliente.Id);

            return clienteDTO;
        }

        public Guid AddCliente(AddClienteRequest clienteRequest)
        {
            _logger.LogInformation("Iniciando adição de cliente: {Email}", clienteRequest.email);

            var validationResult = _addClienteValidator.Validate(clienteRequest);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao adicionar cliente: {Email}. Erros: {Errors}",
                    clienteRequest.email, string.Join("; ", validationResult.Errors));
                throw new ClienteInvalidoException(string.Join("; ", validationResult.Errors));
            }

            try
            {
                var passwordHash = _authService.ComputeSha256Hash(clienteRequest.password);
                var cliente = new Cliente
                (
                    clienteRequest.nome,
                    clienteRequest.email,
                    passwordHash,
                    clienteRequest.role,
                    clienteRequest.saldo
                );

                _clienteRepository.Add(cliente);
                _clienteRepository.SaveChanges();

                _logger.LogInformation("Cliente adicionado com sucesso. ID: {ClienteId}, Nome: {ClienteNome}",
                    cliente.Id, cliente.Nome);

                return cliente.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar cliente: {Email}", clienteRequest.email);
                throw new ClienteException($"Erro ao adicionar cliente: {ex.Message}");
            }
        }

        public void UpdateCliente(Guid id, UpdateClienteRequest clienteRequest)
        {
            _logger.LogInformation("Iniciando atualização de cliente. ID: {ClienteId}", id);

            var validationResult = _updateClienteValidator.Validate(clienteRequest);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validação falhou ao atualizar cliente ID: {ClienteId}. Erros: {Errors}",
                    id, string.Join("; ", validationResult.Errors));
                throw new ClienteInvalidoException(string.Join("; ", validationResult.Errors));
            }

            var cliente = _clienteRepository.ObterPorId(id);

            if (cliente is null)
            {
                _logger.LogWarning("Cliente não encontrado para atualização. ID: {ClienteId}", id);
                throw new ClienteNaoEncontradoException(id);
            }

            try
            {
                cliente.AtualizarNome(clienteRequest.nome);
                cliente.AtualizarRole(clienteRequest.role);
                cliente.AtualizarSaldo(clienteRequest.saldo);

                _clienteRepository.Update(cliente);
                _clienteRepository.SaveChanges();

                _logger.LogInformation("Cliente atualizado com sucesso. ID: {ClienteId}, Nome: {ClienteNome}",
                    cliente.Id, cliente.Nome);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente ID: {ClienteId}", id);
                throw new ClienteException($"Erro ao atualizar cliente: {ex.Message}");
            }
        }

        public void DeleteCliente(Guid id)
        {
            _logger.LogInformation("Iniciando desativação de cliente. ID: {ClienteId}", id);

            var cliente = _clienteRepository.ObterPorId(id);

            if (cliente is null)
            {
                _logger.LogWarning("Cliente não encontrado para desativação. ID: {ClienteId}", id);
                throw new ClienteNaoEncontradoException(id);
            }

            try
            {
                cliente.Desativar();
                _clienteRepository.Update(cliente);
                _clienteRepository.SaveChanges();

                _logger.LogInformation("Cliente desativado com sucesso. ID: {ClienteId}, Nome: {ClienteNome}",
                    cliente.Id, cliente.Nome);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar cliente ID: {ClienteId}", id);
                throw new ClienteException($"Erro ao deletar cliente: {ex.Message}");
            }
        }
    }
}