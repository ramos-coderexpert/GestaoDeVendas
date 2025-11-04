using FluentValidation;
using GestaoDeVendas.Application.DTO.Cliente;

namespace GestaoDeVendas.Application.Validators.Cliente
{
    public class AddClienteRequestValidator : AbstractValidator<AddClienteRequest>
    {
        public AddClienteRequestValidator()
        {
            RuleFor(c => c.nome)
                .NotEmpty()
                .WithMessage("O nome é obrigatório")
                .MinimumLength(3)
                .WithMessage("O nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100)
                .WithMessage("O nome deve ter no máximo 100 caracteres");

            RuleFor(c => c.email)
                .NotEmpty()
                .WithMessage("O email é obrigatório");

            RuleFor(c => c.password)
                .NotEmpty()
                .WithMessage("A senha é obrigatória")
                .MinimumLength(6)
                .WithMessage("A senha deve ter no mínimo 6 caracteres");

            RuleFor(c => c.role)
                .NotEmpty()
                .WithMessage("O papel é obrigatório")
                .Must(role => role != null && (role.ToLower() == "admin" || role.ToLower() == "user"))
                .WithMessage("O papel deve ser 'admin' ou 'user'");

            RuleFor(c => c.saldo)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O saldo não pode ser negativo");
        }
    }
}