using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Food_Registration.Controllers;
using Food_Registration.Models;
using Food_Registration.DAL;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Food_Registration.Tests.Controllers
{
  public class ProductControllerTests
  {
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IProducerRepository> _mockProducerRepo;
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private readonly Mock<ILogger<ProductController>> _mockLogger;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
      // Setup repositories
      _mockProductRepo = new Mock<IProductRepository>();
      _mockProducerRepo = new Mock<IProducerRepository>();
      _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
      _mockLogger = new Mock<ILogger<ProductController>>();

      // Setup controller with all required dependencies
      _controller = new ProductController(
          _mockProductRepo.Object,
          _mockProducerRepo.Object,
          _mockWebHostEnvironment.Object,
          _mockLogger.Object
      );

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

    ///Unit Test 1
    [Fact]
    public async Task Create_Get_ReturnsViewWithProducerList()
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
      var result = await _controller.Create();

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      Assert.NotNull(viewResult.ViewData["Producers"]);
      var producerList = Assert.IsType<SelectList>(viewResult.ViewData["Producers"]);
      Assert.Equal(2, producerList.Count());
    }

    //Unit Test 2
    [Fact]
    public async Task Create_Post_ValidProduct_RedirectsToTable()
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
      var result = await _controller.Create(product);

      // Assert
      var redirectResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Table", redirectResult.ActionName);
      _mockProductRepo.Verify(repo => repo.AddProductAsync(product), Times.Once);
    }

    //Unit Test 3
    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsViewWithError()
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
      var result = await _controller.Create(product);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);

      Assert.False(_controller.ModelState.IsValid);
    }

    //Unit Test 4
    [Fact]
    public async Task Create_Post_UnauthorizedProducer_ReturnsUnauthorized()
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
      var result = await _controller.Create(product);

      // Assert
      Assert.IsType<RedirectResult>(result);
      _mockProductRepo.Verify(repo => repo.AddProductAsync(It.IsAny<Product>()), Times.Never);
    }

    //Unit Test 5
    [Fact]
    public async Task Create_Post_NullProducer_ReturnsBadRequest()
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
      var result = await _controller.Create(product);

      // Assert
      Assert.IsType<RedirectResult>(result);
      _mockProductRepo.Verify(repo => repo.AddProductAsync(It.IsAny<Product>()), Times.Never);
    }

    //Unit test 6
    [Fact]
    public async Task Index_ReturnsFilteredProducts()
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
        var result = await _controller.Index("Apple", "Fruits");

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      var model = Assert.IsAssignableFrom<List<Product>>(viewResult.Model);

      Assert.Single(model);
      Assert.Equal("Apple", model.First().Name);
    }


    //Unit Test 8
    [Fact]
    public async Task Delete_ProductAndProducerNotFound_ReturnsNotFound()
    {
      // Arrange
      _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(It.IsAny<int>()))
          .ReturnsAsync((Product)null); // No product found

        // Act
        var result = await _controller.Delete(999);  // Using a non-existent product ID

      // Assert
      var notFoundResult = Assert.IsType<NotFoundResult>(result);  // Expecting NotFound result
      _mockProductRepo.Verify(repo => repo.DeleteProductAsync(It.IsAny<int>()), Times.Never);  // Verify DeleteProductAsync is not called
    }
  }
}
