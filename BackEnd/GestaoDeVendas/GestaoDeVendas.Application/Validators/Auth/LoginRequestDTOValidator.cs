using FluentValidation;
using GestaoDeVendas.Application.DTO.Auth;

namespace GestaoDeVendas.Application.Validators.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty()
                .WithMessage("O email é obrigatório")
                .EmailAddress()
                .WithMessage("Email inválido");

            RuleFor(x => x.password)
                .NotEmpty()
                .WithMessage("A senha é obrigatória")
                .MinimumLength(6)
                .WithMessage("A senha deve ter no mínimo 6 caracteres");
        }
    }
}