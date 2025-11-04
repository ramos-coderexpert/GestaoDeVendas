using GestaoDeVendas.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoDeVendas.Infrastructure.Persistance.Configurations
{
    internal sealed class ProdutoConfig : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Preco)
                .IsRequired()
                .HasPrecision(14, 2);

            builder.Property(p => p.Estoque)
                .IsRequired();

            builder.Property(c => c.Ativo)
                .IsRequired();
        }
    }
}