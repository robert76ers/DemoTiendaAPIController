using DemoTienda.Api.Controllers;
using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Services;
using DemoTienda.Domain.Entites;
using DemoTiendaAPIController.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace DemoTienda.Api.Test.Controllers
{
    public class ProductoControllerTests
    {
        private readonly Mock<IProductoRepository> _repoMock;
        private readonly Mock<IOptionsSnapshot<ProductoSettings>> _optionsMock;

        private readonly ProductoService _service;
        private readonly ProductoController _controller;

        public ProductoControllerTests()
        {
            _repoMock = new Mock<IProductoRepository>();
            _optionsMock = new Mock<IOptionsSnapshot<ProductoSettings>>();

            _optionsMock.Setup(o => o.Value)
                        .Returns(new ProductoSettings
                        {
                            DefaultCurrency = "EUR",
                            MaxResults = 10
                        });

            _service = new ProductoService(_repoMock.Object);
            _controller = new ProductoController(_service, _optionsMock.Object);
        }

        [Fact]
        public async Task Get_DeberiaRetornarOkConLista()
        {
            // Arrange
            _repoMock.Setup(r => r.ListAsync())
                     .ReturnsAsync(new List<Producto>
                     {
                         new Producto { Id = 1, Nombre = "Mouse", Precio = 25m },
                         new Producto { Id = 2, Nombre = "Teclado", Precio = 40m }
                     });

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<Producto>>(okResult.Value);

            Assert.Equal(2, items.Count());
        }

        [Fact]
        public async Task GetById_Existe_DeberiaRetornarOk()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(5))
                     .ReturnsAsync(new Producto { Id = 5, Nombre = "Monitor", Precio = 200m });

            // Act
            var result = await _controller.GetById(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var prod = Assert.IsType<Producto>(okResult.Value);
            Assert.Equal(5, prod.Id);
        }

        [Fact]
        public async Task GetById_NoExiste_DeberiaRetornarNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(99))
                     .ReturnsAsync((Producto?)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_DeberiaRetornarCreatedAtAction()
        {
            // Arrange
            var request = new Producto { Nombre = "Impresora", Precio = 150m };

            _repoMock.Setup(r => r.AddAsync(request))
                     .ReturnsAsync(new Producto { Id = 10, Nombre = "Impresora", Precio = 150m });

            // Act
            var result = await _controller.Post(request);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ProductoController.GetById), created.ActionName);

            var value = Assert.IsType<Producto>(created.Value);
            Assert.Equal(10, value.Id);
            Assert.Equal("Impresora", value.Nombre);
        }

        [Fact]
        public async Task Put_DeberiaRetornarNoContentYActualizar()
        {
            // Arrange
            var request = new Producto { Nombre = "Notebook", Precio = 800m };

            _repoMock.Setup(r => r.UpdateAsync(7, request))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Put(7, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _repoMock.Verify(r => r.UpdateAsync(7, request), Times.Once);
        }

        [Fact]
        public async Task Delete_DeberiaRetornarNoContentYEliminar()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(4))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(4);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _repoMock.Verify(r => r.DeleteAsync(4), Times.Once);
        }

        [Fact]
        public void Config_DeberiaRetornarConfiguracion()
        {
            // Act
            var result = _controller.Config();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var cfg = Assert.IsType<ProductoSettings>(okResult.Value);

            Assert.Equal("EUR", cfg.DefaultCurrency);
            Assert.Equal(10, cfg.MaxResults);
        }
    }
}
