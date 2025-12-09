using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Services;
using DemoTienda.Application.DTOs;
using DemoTienda.Domain.Entites;
using Moq;
using MapsterMapper;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Application.DTOs.Request;

namespace DemoTienda.Application.Test.Services
{
    public class ProductoServiceTests
    {
        private readonly Mock<IProductoRepository> _repoMock;
        private readonly ProductoService _service;

        public ProductoServiceTests()
        {
            _repoMock = new Mock<IProductoRepository>();
            _service = new ProductoService(_repoMock.Object);
        }

        [Fact]
        public async Task ListAsync_DeberiaRetornarListado()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Mouse", Precio = 25m },
                new Producto { Id = 2, Nombre = "Teclado", Precio = 40m }
            };

            _repoMock.Setup(r => r.ListAsync())
                     .ReturnsAsync(productos);

            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Collection(resultado,
                p => Assert.Equal("Mouse", p.Nombre),
                p => Assert.Equal("Teclado", p.Nombre));
            _repoMock.Verify(r => r.ListAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_IdValido_DeberiaRetornarProducto()
        {
            // Arrange
            var producto = new Producto { Id = 1, Nombre = "Monitor", Precio = 200m };

            _repoMock.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(producto);

            // Act
            var resultado = await _service.GetAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Monitor", resultado.Nombre);
        }

        [Fact]
        public async Task AddAsync_DeberiaAgregarYRetornarProducto()
        {
            // Arrange
            var request = new CreateProductoRequestDTO
            {
                Nombre = "Impresora",
                Precio = 150m,
                IdCategoria = 2
            };

            var producto = new Producto
            {
                Id = 10,
                Nombre = "Impresora",
                Precio = 150m,
                IdCategoria = 2
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Producto>()))
                     .ReturnsAsync(producto);

            // Act
            var resultado = await _service.AddAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            Assert.Equal("Impresora", resultado.Nombre);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Producto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeberiaInvocarRepositorioConIdYEntidad()
        {
            // Arrange
            var request = new UpdateProductoRequestDTO
            {
                Nombre = "Notebook",
                Precio = 800m,
                IdCategoria = 3
            };

            _repoMock.Setup(r => r.UpdateAsync(5, It.IsAny<Producto>()))
                     .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.UpdateAsync(5, request);

            // Assert
            Assert.True(resultado);
            _repoMock.Verify(r => r.UpdateAsync(5, It.IsAny<Producto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeberiaInvocarRepositorioConId()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(3))
                     .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.DeleteAsync(3);

            // Assert
            Assert.True(resultado);
            _repoMock.Verify(r => r.DeleteAsync(3), Times.Once);
        }
    }
}
