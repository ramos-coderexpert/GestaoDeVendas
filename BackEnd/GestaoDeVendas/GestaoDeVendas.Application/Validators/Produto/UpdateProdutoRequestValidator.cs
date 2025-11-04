using FluentValidation;
using GestaoDeVendas.Application.DTO.Produto;

namespace GestaoDeVendas.Application.Validators.Produto
{
    public class UpdateProdutoRequestValidator : AbstractValidator<UpdateProdutoRequest>
    {
        public UpdateProdutoRequestValidator()
        {
            RuleFor(p => p.nome)
                .NotEmpty()
                .WithMessage("O nome do produto é obrigatório")
                .MinimumLength(3)
                .WithMessage("O nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100)
                .WithMessage("O nome deve ter no máximo 100 caracteres");

            RuleFor(p => p.preco)
                .NotEmpty()
                .WithMessage("O preço é obrigatório")
                .GreaterThan(0)
                .WithMessage("O preço deve ser maior que zero")
                .LessThan(999999.99m)
                .WithMessage("O preço deve ser menor que 999.999,99");

            RuleFor(p => p.estoque)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O estoque não pode ser negativo")
                .LessThan(999999)
                .WithMessage("O estoque deve ser menor que 999.999");
        }
    }
}