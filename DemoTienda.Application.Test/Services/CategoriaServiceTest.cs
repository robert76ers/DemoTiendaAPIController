using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Services;
using DemoTienda.Application.DTOs;
using DemoTienda.Domain.Entites;
using Microsoft.Extensions.Logging;
using Moq;
using MapsterMapper;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Application.DTOs.Request;

namespace DemoTienda.Application.Test.Services
{
    public class CategoriaServiceTests
    {
        private readonly Mock<ICategoriaRepository> _repoMock;
        private readonly Mock<ILogger<CategoriaService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CategoriaService _service;

        public CategoriaServiceTests()
        {
            _repoMock = new Mock<ICategoriaRepository>();
            _loggerMock = new Mock<ILogger<CategoriaService>>();    
            _mapperMock = new Mock<IMapper>();

            _service = new CategoriaService(_repoMock.Object, _loggerMock.Object, _mapperMock.Object);
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

            var categoriasDto = categorias.Select(c => new CategoriaResponseDTO { Id = c.Id, Nombre = c.Nombre }).ToList();

            _repoMock
                .Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categorias);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<CategoriaResponseDTO>>(categorias))
                .Returns(categoriasDto);

            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Collection(resultado,
                c => Assert.Equal("Tecnología", c.Nombre),
                c => Assert.Equal("Hogar", c.Nombre));

            _repoMock.Verify(r => r.ListAsync(It.IsAny<CancellationToken>()), Times.Once);

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
            var categoriaDto = new CategoriaResponseDTO { Id = 10, Nombre = "Libros" };

            _repoMock
                .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            _mapperMock
                .Setup(m => m.Map<CategoriaResponseDTO>(categoria))
                .Returns(categoriaDto);

            // Act
            var resultado = await _service.GetAsync(10);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado!.Id);
            Assert.Equal("Libros", resultado.Nombre);

            _repoMock.Verify(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_IdNoExistente_DeberiaRetornarNull()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Categoria?)null);

            // Act
            var resultado = await _service.GetAsync(999);

            // Assert
            Assert.Null(resultado);
            _repoMock.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DeberiaLlamarRepositorioYRetornarCategoriaCreada()
        {
            // Arrange
            var request = new CreateCategoriaRequestDTO { Nombre = "Ropa" };
            var categoria = new Categoria { Id = 5, Nombre = "Ropa" };
            var categoriaDto = new CategoriaResponseDTO { Id = 5, Nombre = "Ropa" };

            _mapperMock
                .Setup(m => m.Map<Categoria>(request))
                .Returns(categoria);

            _repoMock
                .Setup(r => r.AddAsync(categoria, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoria);

            _mapperMock
                .Setup(m => m.Map<CategoriaResponseDTO>(categoria))
                .Returns(categoriaDto);

            // Act
            var resultado = await _service.AddAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(5, resultado.Id);
            Assert.Equal("Ropa", resultado.Nombre);

            _repoMock.Verify(r => r.AddAsync(categoria, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeberiaLlamarRepositorioConIdYCategoria()
        {
            // Arrange
            var request = new UpdateCategoriaRequestDTO { Nombre = "Electrónica" };
            var categoria = new Categoria { Id = 3, Nombre = "Electrónica" };

            _mapperMock
                .Setup(m => m.Map<Categoria>(request))
                .Returns(categoria);

            _repoMock
                .Setup(r => r.UpdateAsync(3, categoria, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.UpdateAsync(3, request);

            // Assert
            Assert.True(resultado);
            _repoMock.Verify(r => r.UpdateAsync(3, categoria, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeberiaLlamarRepositorio()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(3, It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.DeleteAsync(3);

            // Assert
            Assert.True(resultado);
            _repoMock.Verify(r => r.DeleteAsync(3, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
