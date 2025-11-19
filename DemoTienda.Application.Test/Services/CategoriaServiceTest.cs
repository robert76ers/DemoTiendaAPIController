using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Services;
using DemoTienda.Domain.Entites;
using Microsoft.Extensions.Logging;
using Moq;

namespace DemoTienda.Application.Test.Services
{
    public class CategoriaServiceTests
    {
        private readonly Mock<ICategoriaRepository> _repoMock;
        private readonly Mock<ILogger<CategoriaService>> _loggerMock;
        private readonly CategoriaService _service;

        public CategoriaServiceTests()
        {
            _repoMock = new Mock<ICategoriaRepository>();
            _loggerMock = new Mock<ILogger<CategoriaService>>();

            _service = new CategoriaService(_repoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ListAsync_DeberiaRetornarListaDeCategorias_YRegistrarLog()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Tecnología" },
                new Categoria { Id = 2, Nombre = "Hogar" }
            };

            _repoMock
                .Setup(r => r.ListAsync())
                .ReturnsAsync(categorias);

            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Collection(resultado,
                c => Assert.Equal("Tecnología", c.Nombre),
                c => Assert.Equal("Hogar", c.Nombre));

            _repoMock.Verify(r => r.ListAsync(), Times.Once);

            // Verificar que se escribió el log de información (opcional)
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Listando categorías")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<Func<It.IsAnyType, System.Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAsync_IdExistente_DeberiaRetornarCategoria()
        {
            // Arrange
            var categoria = new Categoria { Id = 10, Nombre = "Libros" };

            _repoMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(categoria);

            // Act
            var resultado = await _service.GetAsync(10);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado!.Id);
            Assert.Equal("Libros", resultado.Nombre);

            _repoMock.Verify(r => r.GetByIdAsync(10), Times.Once);
        }

        [Fact]
        public async Task GetAsync_IdNoExistente_DeberiaRetornarNull()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Categoria?)null);

            // Act
            var resultado = await _service.GetAsync(999);

            // Assert
            Assert.Null(resultado);
            _repoMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DeberiaLlamarRepositorioYRetornarCategoriaCreada()
        {
            // Arrange
            var categoriaNueva = new Categoria { Nombre = "Ropa" };
            var categoriaCreada = new Categoria { Id = 5, Nombre = "Ropa" };

            _repoMock
                .Setup(r => r.AddAsync(categoriaNueva))
                .ReturnsAsync(categoriaCreada);

            // Act
            var resultado = await _service.AddAsync(categoriaNueva);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(5, resultado.Id);
            Assert.Equal("Ropa", resultado.Nombre);

            _repoMock.Verify(r => r.AddAsync(categoriaNueva), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeberiaLlamarRepositorioConIdYCategoria()
        {
            // Arrange
            var categoriaActualizada = new Categoria { Id = 3, Nombre = "Electrónica" };

            _repoMock
                .Setup(r => r.UpdateAsync(3, categoriaActualizada))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(3, categoriaActualizada);

            // Assert
            _repoMock.Verify(r => r.UpdateAsync(3, categoriaActualizada), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeberiaLlamarRepositorio()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(3))
                     .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(3);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(3), Times.Once);
        }
    }
}
