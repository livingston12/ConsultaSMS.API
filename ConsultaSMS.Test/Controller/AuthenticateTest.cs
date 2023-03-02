using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebConsultaSMS;
using WebConsultaSMS.Controllers;
using WebConsultaSMS.Interfaces;
using WebConsultaSMS.Utils;

namespace ConsultaSMS.Test.Controller
{
    public class AuthenticateTest
    {
        private Mock<IAuthenticateService> mockService;

        [SetUp]
        public void Setup()
        {
            IServiceProvider _services =
                Program.CreateHostBuilder(new string[] { }).Build().Services;
            var myService = _services.GetRequiredService<UtilsResponse>();
            myService.GetConfiguration();
            mockService = new Mock<IAuthenticateService>();
        }

        [Test]
        public async Task Post_NoParameter_ShouldBeThrowError()
        {
            //Arrange
            mockService
                .Setup(m => m.PostInternalAsync(null))
                .ThrowsAsync(new Exception());
            var controller = new AuthenticateController(mockService.Object);

            //Act
            var result = await controller.Post(null);
            var badResult = result as BadRequestObjectResult;
            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(badResult.StatusCode, StatusCodes.Status400BadRequest);
        }

        [Test]
        public async Task Post_WithInccorectPass_ShouldBeOkWithMessageError()
        {
            //Arrange
            mockService
                .Setup(m => m.PostInternalAsync(SeedTest.AuthInternalRequestIncorrectPass))
                .ReturnsAsync(SeedTest.AuthInternalResponse);
            var controller = new AuthenticateController(mockService.Object);

            //Act
            var result = await controller.Post(SeedTest.AuthInternalRequestIncorrectPass);
            var okResult = result as OkObjectResult;
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(SeedTest.AuthInternalResponse, okResult.Value);
        }

        [Test]
        public async Task Post_WithValues_ShouldBeOkWithMessageCorrect()
        {
            SeedTest.AuthInternalResponse.IsValid = true;
            
            //Arrange
            mockService
                .Setup(m => m.PostInternalAsync(SeedTest.AuthInternalRequest))
                .ReturnsAsync(SeedTest.AuthInternalResponse);
            var controller = new AuthenticateController(mockService.Object);

            //Act
            var result = await controller.Post(SeedTest.AuthInternalRequest);
            var okResult = result as OkObjectResult;
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(SeedTest.AuthInternalResponse, okResult.Value);
        }

    }
}