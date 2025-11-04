using FluentAssertions;
using GestaoDeVendas.Domain.Entity;
using Xunit;

namespace GestaoDeVendas.Tests.Domain.Entity
{
    public class BaseEntityTests
    {
        private class TestEntity : BaseEntity
        {
            public TestEntity() : base() { }
        }

        [Fact]
        public void Constructor_ShouldInitializeId()
        {
            // Act
            var entity = new TestEntity();

            // Assert
            entity.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Id_ShouldBeImmutable()
        {
            // Arrange
            var entity = new TestEntity();
            var originalId = entity.Id;

            // Act & Assert
            var property = typeof(BaseEntity).GetProperty("Id");
            var setMethod = property?.SetMethod;

            // Verifica se o setter é init-only
            setMethod.Should().NotBeNull();
            setMethod!.ReturnParameter.GetRequiredCustomModifiers()
                     .Should().Contain(typeof(System.Runtime.CompilerServices.IsExternalInit));
        }
    }
}