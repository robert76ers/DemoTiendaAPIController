using DemoTienda.Api.Controllers;
using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Services;
using DemoTienda.Domain.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DemoTienda.Api.Test.Controllers
{
    public class CategoriasControllerTests
    {
        private readonly Mock<ICategoriaRepository> _repoMock;
        private readonly Mock<ILogger<CategoriaService>> _serviceLoggerMock;
        private readonly Mock<ILogger<CategoriasController>> _controllerLoggerMock;

        private readonly CategoriaService _service;
        private readonly CategoriasController _controller;

        public CategoriasControllerTests()
        {
            _repoMock = new Mock<ICategoriaRepository>();
            _serviceLoggerMock = new Mock<ILogger<CategoriaService>>();
            _controllerLoggerMock = new Mock<ILogger<CategoriasController>>();

            _service = new CategoriaService(_repoMock.Object, _serviceLoggerMock.Object);
            _controller = new CategoriasController(_service, _controllerLoggerMock.Object);
        }

        [Fact]
        public async Task Get_DeberiaRetornarOkConLista()
        {
            // Arrange
            _repoMock.Setup(r => r.ListAsync())
                     .ReturnsAsync(new List<Categoria>
                     {
                         new Categoria { Id = 1, Nombre = "Electrónicos" },
                         new Categoria { Id = 2, Nombre = "Ropa" }
                     });

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<Categoria>>(okResult.Value);

            Assert.Equal(2, items.Count());
        }

        [Fact]
        public async Task GetById_Existe_DeberiaRetornarOk()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(5))
                     .ReturnsAsync(new Categoria { Id = 5, Nombre = "Videojuegos" });

            // Act
            var result = await _controller.GetById(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categoria = Assert.IsType<Categoria>(okResult.Value);
            Assert.Equal(5, categoria.Id);
        }

        [Fact]
        public async Task GetById_NoExiste_DeberiaRetornarNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(99))
                     .ReturnsAsync((Categoria?)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_DeberiaRetornarCreatedAtAction()
        {
            // Arrange
            var request = new Categoria { Nombre = "Hogar" };

            _repoMock.Setup(r => r.AddAsync(request))
                     .ReturnsAsync(new Categoria { Id = 10, Nombre = "Hogar" });

            // Act
            var result = await _controller.Post(request);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CategoriasController.GetById), created.ActionName);

            var value = Assert.IsType<Categoria>(created.Value);
            Assert.Equal(10, value.Id);
            Assert.Equal("Hogar", value.Nombre);
        }

        [Fact]
        public async Task Put_DeberiaRetornarNoContentYActualizar()
        {
            // Arrange
            var request = new Categoria { Nombre = "Gaming" };

            _repoMock.Setup(r => r.UpdateAsync(3, request))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Put(3, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _repoMock.Verify(r => r.UpdateAsync(3, request), Times.Once);
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
    }
}
