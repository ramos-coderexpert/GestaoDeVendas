using GestaoDeVendas.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoDeVendas.Infrastructure.Persistance.Configurations
{
    internal sealed class VendaConfig : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.ToTable("Vendas");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.QtdProduto)
                .IsRequired();

            builder.Property(v => v.ValorUnitario)
                .IsRequired()
                .HasPrecision(14, 2);

            builder.Property(v => v.ValorTotal)
                .IsRequired()
                .HasPrecision(14, 2);

            builder.Property(v => v.DataVenda)
                .IsRequired();

            builder.HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Produto)
                .WithMany()
                .HasForeignKey(v => v.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}