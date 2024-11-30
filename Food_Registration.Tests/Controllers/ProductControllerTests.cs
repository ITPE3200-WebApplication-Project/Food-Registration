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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Food_Registration.Tests.Controllers
{
  public class ProductControllerTests
  {
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IProducerRepository> _mockProducerRepo;
    private readonly ProductController _controller;

    private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private readonly Mock<ILogger<ProductController>> _mockLogger;


  public ProductControllerTests()
        {
            // Mock repository ve bağımlılıkları ayarlıyoruz
            _mockProductRepo = new Mock<IProductRepository>();
            _mockProducerRepo = new Mock<IProducerRepository>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<ProductController>>();

            // WebRootPath'i mock'luyoruz, böylece Path.Combine düzgün çalışacak
            _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("C:\\fakepath");

            // Controller'ı mock'larla başlatıyoruz
            _controller = new ProductController(
                _mockProductRepo.Object,
                _mockProducerRepo.Object,
                _mockWebHostEnvironment.Object,
                _mockLogger.Object
            );

            // Kullanıcıyı mock'lıyoruz
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.Name, "test@test.com"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // TempData'yı ayarlıyoruz
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );
        }

        // Mock bir dosya oluşturmak için yardımcı metot
        private IFormFile CreateMockFile(string fileName, string contentType, string content)
        {
            var fileMock = new Mock<IFormFile>();
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(contentBytes);

            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.ContentType).Returns(contentType);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
            fileMock.Setup(_ => _.Length).Returns(contentBytes.Length);

            return fileMock.Object;
        }

        // Create metodunu test eden test
        [Fact]
        public async Task Create_Post_ValidProduct_RedirectsToTable()
        {
            // Arrange: Test verisini hazırlıyoruz
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

            var mockFile = CreateMockFile("image.jpg", "image/jpeg", "fake-image-content");

            // Producer repository mock'ını ayarlıyoruz
            _mockProducerRepo.Setup(repo => repo.GetProducerByIdAsync(1))
                .ReturnsAsync(producer);

            // Act: Create metodunu çağırıyoruz
            var result = await _controller.Create(product, mockFile);

            // Assert: Yönlendirme ve veri doğrulaması
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Table", redirectResult.ActionName);
            _mockProductRepo.Verify(repo => repo.CreateProductAsync(product), Times.Once);
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

  
    }
}  