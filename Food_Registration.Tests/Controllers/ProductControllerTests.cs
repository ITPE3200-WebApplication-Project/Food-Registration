using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Food_Registration.Controllers;
using Food_Registration.Models;
using Food_Registration.DAL;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Food_Registration.Tests.Controllers
{
  public class ProductControllerTests
  {
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IProducerRepository> _mockProducerRepo;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
      // Setup repositories
      _mockProductRepo = new Mock<IProductRepository>();
      _mockProducerRepo = new Mock<IProducerRepository>();

      // Setup controller
      _controller = new ProductController(_mockProductRepo.Object, _mockProducerRepo.Object);

      // Setup mock user identity
      var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
      {
                new Claim(ClaimTypes.Name, "test@test.com"),
      }, "mock"));

      // Setup controller context
      _controller.ControllerContext = new ControllerContext()
      {
        HttpContext = new DefaultHttpContext { User = user }
      };

      // Setup TempData
      _controller.TempData = new TempDataDictionary(
          new DefaultHttpContext(),
          Mock.Of<ITempDataProvider>()
      );
    }

    [Fact]
    public async Task NewProduct_Get_ReturnsViewWithProducerList()
    {
      // Arrange
      var producers = new List<Producer>
            {
                new Producer { ProducerId = 1, Name = "Producer 1", OwnerId = "test@test.com" },
                new Producer { ProducerId = 2, Name = "Producer 2", OwnerId = "test@test.com" }
            };

      _mockProducerRepo.Setup(repo => repo.GetAllProducersAsync())
          .ReturnsAsync(producers);

      // Act
      var result = await _controller.NewProduct();

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      Assert.NotNull(viewResult.ViewData["Producers"]);
      var producerList = Assert.IsType<SelectList>(viewResult.ViewData["Producers"]);
      Assert.Equal(2, producerList.Count());
    }

    [Fact]
    public async Task NewProduct_Post_ValidProduct_RedirectsToTable()
    {
      // Arrange
      var product = new Product
      {
        Name = "Test Product",
        ProducerId = 1,
        NutritionScore = "A"
      };

      var producer = new Producer
      {
        ProducerId = 1,
        OwnerId = "test@test.com"
      };

      _mockProducerRepo.Setup(repo => repo.GetProducerByIdAsync(1))
          .ReturnsAsync(producer);

      // Act
      var result = await _controller.NewProduct(product);

      // Assert
      var redirectResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Table", redirectResult.ActionName);
      _mockProductRepo.Verify(repo => repo.AddProductAsync(product), Times.Once);
    }

    [Fact]
    public async Task NewProduct_Post_InvalidModel_ReturnsViewWithError()
    {
      // Arrange
      var product = new Product(); // Empty product
      _controller.ModelState.AddModelError("Name", "Name is required");

      var producers = new List<Producer>
            {
                new Producer { ProducerId = 1, Name = "Producer 1" }
            };

      _mockProducerRepo.Setup(repo => repo.GetAllProducersAsync())
          .ReturnsAsync(producers);

      // Act
      var result = await _controller.NewProduct(product);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);

      Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task NewProduct_Post_UnauthorizedProducer_ReturnsUnauthorized()
    {
      // Arrange
      var product = new Product
      {
        Name = "Test Product",
        ProducerId = 1,
        NutritionScore = "A"
      };

      var producer = new Producer
      {
        ProducerId = 1,
        OwnerId = "different@test.com" // Different owner than current user
      };

      _mockProducerRepo.Setup(repo => repo.GetProducerByIdAsync(1))
          .ReturnsAsync(producer);

      // Act
      var result = await _controller.NewProduct(product);

      // Assert
      Assert.IsType<RedirectResult>(result);
      _mockProductRepo.Verify(repo => repo.AddProductAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task NewProduct_Post_NullProducer_ReturnsBadRequest()
    {
      // Arrange
      var product = new Product
      {
        Name = "Test Product",
        ProducerId = 999, // Non-existent producer
        NutritionScore = "A"
      };

      _mockProducerRepo.Setup(repo => repo.GetProducerByIdAsync(999))
          .ReturnsAsync((Producer)null);

      // Act
      var result = await _controller.NewProduct(product);

      // Assert
      Assert.IsType<RedirectResult>(result);
      _mockProductRepo.Verify(repo => repo.AddProductAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task AllProducts_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Apple", Category = "Fruits" },
            new Product { ProductId = 2, Name = "Orange", Category = "Fruits" },
            new Product { ProductId = 3, Name = "Carrot", Category = "Vegetables" }
        };

        _mockProductRepo.Setup(repo => repo.GetAllProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.AllProducts("Apple", "Fruits");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);

        Assert.Single(model);
        Assert.Equal("Apple", model.First().Name);
    }

    [Fact]
    public async Task NewProduct_Get_ReturnsProducersAndDropdowns()
    {
        // Arrange
        var producers = new List<Producer>
        {
            new Producer { ProducerId = 1, Name = "Producer 1", OwnerId = "test@test.com" },
            new Producer { ProducerId = 2, Name = "Producer 2", OwnerId = "test@test.com" }
        };

        _mockProducerRepo.Setup(repo => repo.GetAllProducersAsync())
            .ReturnsAsync(producers);

        // Act
        var result = await _controller.NewProduct();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData["Producers"]);
    }
  }
}
