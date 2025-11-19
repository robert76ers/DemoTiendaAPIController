using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Services;
using DemoTienda.Domain.Entites;     
using Moq;

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
            _repoMock.Setup(r => r.ListAsync())
                     .ReturnsAsync(new List<Producto>
                     {
                         new Producto { Id = 1, Nombre = "Mouse", Precio = 25m },
                         new Producto { Id = 2, Nombre = "Teclado", Precio = 40m }
                     });

            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _repoMock.Verify(r => r.ListAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_IdValido_DeberiaRetornarProducto()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Producto { Id = 1, Nombre = "Monitor", Precio = 200m });

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
            var nuevo = new Producto { Nombre = "Impresora", Precio = 150m };

            _repoMock.Setup(r => r.AddAsync(nuevo))
                     .ReturnsAsync(new Producto { Id = 10, Nombre = "Impresora", Precio = 150m });

            // Act
            var resultado = await _service.AddAsync(nuevo);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            Assert.Equal("Impresora", resultado.Nombre);
            _repoMock.Verify(r => r.AddAsync(nuevo), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeberiaInvocarRepositorioConIdYEntidad()
        {
            // Arrange
            var producto = new Producto { Nombre = "Notebook", Precio = 800m };

            _repoMock.Setup(r => r.UpdateAsync(5, producto))
                     .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(5, producto);

            // Assert
            _repoMock.Verify(r => r.UpdateAsync(5, producto), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeberiaInvocarRepositorioConId()
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
