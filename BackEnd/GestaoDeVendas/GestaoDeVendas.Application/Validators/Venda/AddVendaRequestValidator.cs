using FluentValidation;
using GestaoDeVendas.Application.DTO.Venda;

namespace GestaoDeVendas.Application.Validators.Venda
{
    public class AddVendaRequestValidator : AbstractValidator<AddVendaRequest>
    {
        public AddVendaRequestValidator()
        {
            RuleFor(v => v.nomeCliente)
                .NotEmpty()
                .WithMessage("O nome do cliente é obrigatório")
                .MinimumLength(3)
                .WithMessage("O nome do cliente deve ter no mínimo 3 caracteres")
                .MaximumLength(100)
                .WithMessage("O nome do cliente deve ter no máximo 100 caracteres");

            RuleFor(v => v.nomeProduto)
                .NotEmpty()
                .WithMessage("O nome do produto é obrigatório")
                .MinimumLength(3)
                .WithMessage("O nome do produto deve ter no mínimo 3 caracteres")
                .MaximumLength(100)
                .WithMessage("O nome do produto deve ter no máximo 100 caracteres");

            RuleFor(v => v.qtdProduto)
                .NotEmpty()
                .WithMessage("A quantidade é obrigatória")
                .GreaterThan(0)
                .WithMessage("A quantidade deve ser maior que zero")
                .LessThan(999999)
                .WithMessage("A quantidade deve ser menor que 999.999");
        }
    }
}