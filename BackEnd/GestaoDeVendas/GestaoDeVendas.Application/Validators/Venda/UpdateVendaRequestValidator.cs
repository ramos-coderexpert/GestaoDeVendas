using FluentValidation;
using GestaoDeVendas.Application.DTO.Venda;

namespace GestaoDeVendas.Application.Validators.Venda
{
    public class UpdateVendaRequestValidator : AbstractValidator<UpdateVendaRequest>
    {
        public UpdateVendaRequestValidator()
        {
            RuleFor(v => v.qtdProduto)
                .NotEmpty()
                .WithMessage("A quantidade é obrigatória")
                .GreaterThan(0)
                .WithMessage("A quantidade deve ser maior que zero")
                .LessThan(999999)
                .WithMessage("A quantidade deve ser menor que 999.999");

            RuleFor(v => v.valorUnitario)
                .NotEmpty()
                .WithMessage("O valor unitário é obrigatório")
                .GreaterThan(0)
                .WithMessage("O valor unitário deve ser maior que zero")
                .LessThan(999999.99m)
                .WithMessage("O valor unitário deve ser menor que 999.999,99");
        }
    }
}