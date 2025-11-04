using FluentValidation;
using GestaoDeVendas.Application.DTO.Auth;
using GestaoDeVendas.Application.DTO.Cliente;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Domain.Exceptions;
using GestaoDeVendas.Domain.IRepositories;
using GestaoDeVendas.Domain.Entity;
using GestaoDeVendas.Domain.Services;

namespace GestaoDeVendas.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IAuthService _authService;
        private readonly IValidator<LoginRequestDTO> _loginValidator;
        private readonly IValidator<AddClienteRequest> _addClienteValidator;
        private readonly IValidator<UpdateClienteRequest> _updateClienteValidator;

        public ClienteService(
            IClienteRepository clienteRepository,
            IAuthService authService,
            IValidator<LoginRequestDTO> loginValidator,
            IValidator<AddClienteRequest> addClienteValidator,
            IValidator<UpdateClienteRequest> updateClienteValidator)
        {
            _loginValidator = loginValidator;
            _clienteRepository = clienteRepository;
            _authService = authService;
            _addClienteValidator = addClienteValidator;
            _updateClienteValidator = updateClienteValidator;
        }


        public LoginResponseDTO Login(LoginRequestDTO loginRequest)
        {
            var validationResult = _loginValidator.Validate(loginRequest);

            if (!validationResult.IsValid)
                throw new CredenciaisInvalidasException(loginRequest.email);

            try
            {
                var passwordHash = _authService.ComputeSha256Hash(loginRequest.password);
                var cliente = _clienteRepository.ObterPorEmailPassword(loginRequest.email, passwordHash);

                if (cliente is null)
                    throw new ClienteInvalidoException($"Login não encontrado com email: {loginRequest.email}");

                var token = _authService.GenerateJwtToken(cliente.Email, cliente.Role);

                return new LoginResponseDTO(token, cliente.Email, cliente.Role);
            }
            catch (Exception ex)
            {
                throw new ClienteException($"Erro ao realizar login: {ex.Message}");
            }
        }

        public async Task<List<GetClienteRequest>> ObterTodosAsync()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            var clienteDTOs = clientes
                .Select(c => new GetClienteRequest(c.Id, c.Nome, c.Email, c.Role, c.Saldo))
                .ToList();

            return clienteDTOs;
        }

        public async Task<List<GetClienteRequest>> ObterTodosAtivosAsync()
        {
            var clientes = await _clienteRepository.ObterTodosAtivosAsync();
            var clienteDTOs = clientes
                .Select(c => new GetClienteRequest(c.Id, c.Nome, c.Email, c.Role, c.Saldo))
                .ToList();

            return clienteDTOs;
        }

        public GetClienteRequest ObterPorId(Guid id)
        {
            var cliente = _clienteRepository.ObterPorId(id);

            if (cliente is null)
                throw new KeyNotFoundException("Cliente não encontrado.");

            var clienteDTO = new GetClienteRequest(cliente.Id, cliente.Nome, cliente.Email, cliente.Role, cliente.Saldo);

            return clienteDTO;
        }

        public GetClienteRequest ObterPorNome(string nome)
        {
            var cliente = _clienteRepository.ObterPorNome(nome);

            if (cliente is null)
                throw new ClienteNaoEncontradoException(nome);

            var clienteDTO = new GetClienteRequest(cliente.Id, cliente.Nome, cliente.Email, cliente.Role, cliente.Saldo);

            return clienteDTO;
        }

        public Guid AddCliente(AddClienteRequest clienteRequest)
        {
            var validationResult = _addClienteValidator.Validate(clienteRequest);

            if (!validationResult.IsValid)
                throw new ClienteInvalidoException(string.Join("; ", validationResult.Errors));

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

                return cliente.Id;
            }
            catch (Exception ex)
            {
                throw new ClienteException($"Erro ao adicionar cliente: {ex.Message}");
            }
        }

        public void UpdateCliente(Guid id, UpdateClienteRequest clienteRequest)
        {
            var validationResult = _updateClienteValidator.Validate(clienteRequest);

            if (!validationResult.IsValid)
                throw new ClienteInvalidoException(string.Join("; ", validationResult.Errors));

            var cliente = _clienteRepository.ObterPorId(id);

            if (cliente is null)
                throw new ClienteNaoEncontradoException(id);

            try
            {
                cliente.AtualizarNome(clienteRequest.nome);
                cliente.AtualizarRole(clienteRequest.role);
                cliente.AtualizarSaldo(clienteRequest.saldo);

                _clienteRepository.Update(cliente);
                _clienteRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ClienteException($"Erro ao atualizar cliente: {ex.Message}");
            }
        }

        public void DeleteCliente(Guid id)
        {
            var cliente = _clienteRepository.ObterPorId(id);

            if (cliente is null)
                throw new ClienteNaoEncontradoException(id);

            try
            {
                cliente.Desativar();
                _clienteRepository.Update(cliente);
                _clienteRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ClienteException($"Erro ao deletar cliente: {ex.Message}");
            }
        }
    }
}