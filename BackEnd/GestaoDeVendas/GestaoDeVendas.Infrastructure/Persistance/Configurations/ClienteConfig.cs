using GestaoDeVendas.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoDeVendas.Infrastructure.Persistance.Configurations
{
    internal sealed class ClienteConfig : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Email)
                .IsUnicode(true)
                .HasMaxLength(100);

            builder.Property(c => c.Password)
                .IsRequired()
                .IsUnicode(true);

            builder.Property(c => c.Role)
               .IsRequired();

            builder.Property(c => c.Saldo)
                .IsRequired()
                .HasPrecision(14, 2);

            builder.Property(c => c.Ativo)
                .IsRequired();

            builder.Property(c => c.DataRegistro)
               .IsRequired();
        }
    }
}