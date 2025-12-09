using DemoTienda.Api.Controllers;
using DemoTienda.Application.DTOs.Request;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Services;
using DemoTienda.Domain.Entites;
using MapsterMapper;
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
        private readonly Mock<IMapper> _mapperMock;

        private readonly CategoriaService _service;
        private readonly CategoriasController _controller;

        public CategoriasControllerTests()
        {
            _repoMock = new Mock<ICategoriaRepository>();
            _serviceLoggerMock = new Mock<ILogger<CategoriaService>>();
            _controllerLoggerMock = new Mock<ILogger<CategoriasController>>();
            _mapperMock = new Mock<IMapper>();

            _service = new CategoriaService(_repoMock.Object, _serviceLoggerMock.Object, _mapperMock.Object);
            _controller = new CategoriasController(_service, _controllerLoggerMock.Object);
        }

        [Fact]
        public async Task Get_DeberiaRetornarOkConLista()
        {
            // Arrange
            var categorias = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Electrónicos" },
                new Categoria { Id = 2, Nombre = "Ropa" }
            };

            var categoriasDto = categorias.Select(c => new CategoriaResponseDTO { Id = c.Id, Nombre = c.Nombre }).ToList();

            _repoMock.Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(categorias);

            _mapperMock.Setup(m => m.Map<IEnumerable<CategoriaResponseDTO>>(categorias))
                       .Returns(categoriasDto);

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<CategoriaResponseDTO>>(okResult.Value);

            Assert.Equal(2, items.Count());
        }

        [Fact]
        public async Task GetById_Existe_DeberiaRetornarOk()
        {
            // Arrange
            var categoria = new Categoria { Id = 5, Nombre = "Videojuegos" };
            var categoriaDto = new CategoriaResponseDTO { Id = categoria.Id, Nombre = categoria.Nombre };

            _repoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(categoria);

            _mapperMock.Setup(m => m.Map<CategoriaResponseDTO>(categoria))
                       .Returns(categoriaDto);

            // Act
            var result = await _controller.GetById(5, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categoriaResult = Assert.IsType<CategoriaResponseDTO>(okResult.Value);
            Assert.Equal(5, categoriaResult.Id);
        }

        [Fact]
        public async Task GetById_NoExiste_DeberiaRetornarNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Categoria?)null);

            // Act
            var result = await _controller.GetById(99, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_DeberiaRetornarCreatedAtAction()
        {
            // Arrange
            var request = new CreateCategoriaRequestDTO { Nombre = "Hogar" };
            var categoria = new Categoria { Id = 10, Nombre = "Hogar" };
            var categoriaDto = new CategoriaResponseDTO { Id = categoria.Id, Nombre = categoria.Nombre };

            _mapperMock.Setup(m => m.Map<Categoria>(request))
                       .Returns(categoria);

            _repoMock.Setup(r => r.AddAsync(categoria, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(categoria);

            _mapperMock.Setup(m => m.Map<CategoriaResponseDTO>(categoria))
                       .Returns(categoriaDto);

            // Act
            var result = await _controller.Post(request, CancellationToken.None);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CategoriasController.GetById), created.ActionName);

            var value = Assert.IsType<CategoriaResponseDTO>(created.Value);
            Assert.Equal(10, value.Id);
            Assert.Equal("Hogar", value.Nombre);
        }

        [Fact]
        public async Task Put_DeberiaRetornarNoContentYActualizar()
        {
            // Arrange
            var request = new UpdateCategoriaRequestDTO { Nombre = "Gaming" };
            var categoria = new Categoria { Id = 3, Nombre = "Gaming" };

            _mapperMock.Setup(m => m.Map<Categoria>(request))
                       .Returns(categoria);

            _repoMock.Setup(r => r.UpdateAsync(3, categoria, It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Put(3, request, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _repoMock.Verify(r => r.UpdateAsync(3, categoria, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_DeberiaRetornarNoContentYEliminar()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(4, It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(4, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _repoMock.Verify(r => r.DeleteAsync(4, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
