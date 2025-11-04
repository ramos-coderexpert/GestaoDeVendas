using Xunit;
using FluentAssertions;
using GestaoDeVendas.Domain.Entity;

namespace GestaoDeVendas.Tests.Domain.Entity
{
    public class DeactivatableEntityTests
    {
        private class TestDeactivatableEntity : DeactivatableEntity
        {
            public TestDeactivatableEntity(string nome) : base(nome) { }
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            string nome = "Test Entity";

            // Act
            var entity = new TestDeactivatableEntity(nome);

            // Assert
            entity.Nome.Should().Be(nome);
            entity.Ativo.Should().BeTrue();
            entity.Id.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineData("New Name")]
        [InlineData("Another Name")]
        public void AtualizarNome_WithValidName_ShouldUpdateName(string novoNome)
        {
            // Arrange
            var entity = new TestDeactivatableEntity("Original Name");

            // Act
            entity.AtualizarNome(novoNome);

            // Assert
            entity.Nome.Should().Be(novoNome);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AtualizarNome_WithInvalidName_ShouldThrowArgumentException(string novoNome)
        {
            // Arrange
            var entity = new TestDeactivatableEntity("Original Name");

            // Act
            var action = () => entity.AtualizarNome(novoNome);

            // Assert
            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("Nome não pode ser vazio*")
                .WithParameterName("novoNome");
        }

        [Fact]
        public void Desativar_ShouldSetAtivoToFalse()
        {
            // Arrange
            var entity = new TestDeactivatableEntity("Test Entity");

            // Act
            entity.Desativar();

            // Assert
            entity.Ativo.Should().BeFalse();
        }

        [Fact]
        public void Desativar_WhenCalledMultipleTimes_ShouldRemainInactive()
        {
            // Arrange
            var entity = new TestDeactivatableEntity("Test Entity");

            // Act
            entity.Desativar();
            entity.Desativar();

            // Assert
            entity.Ativo.Should().BeFalse();
        }
    }
}