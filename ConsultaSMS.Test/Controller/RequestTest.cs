using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebConsultaSMS;
using WebConsultaSMS.Controllers;
using WebConsultaSMS.Interfaces;
using WebConsultaSMS.Models;
using WebConsultaSMS.Models.Requests;
using WebConsultaSMS.Models.Responses;
using WebConsultaSMS.Utils;

namespace ConsultaSMS.Test.Controller
{
    public class RequestTest
    {
        Mock<IRequestService> MockRequestService;
        Mock<IAuthenticateService> MockAuthService;

        [SetUp]
        public void Setup()
        {
            IServiceProvider _services =
                Program.CreateHostBuilder(new string[] { }).Build().Services;
            var myService = _services.GetRequiredService<UtilsResponse>();
            myService.GetConfiguration();
            MockRequestService = new Mock<IRequestService>();
            MockAuthService = new Mock<IAuthenticateService>();
        }

        [Test]
        public async Task GetProductDetailsAsync_NoParameters_throwError()
        {
            MockAuthService
                .Setup(x => x.PostAsync(null))
                .ThrowsAsync(new Exception());
            var controller = new RequestController(MockRequestService.Object, MockAuthService.Object, null);
            BadRequestObjectResult result = await controller.GetProductDetailsAsync(null) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(result.Value, UtilsResponse.clienteConError);
        }

        [Test]
        public async Task GetProductDetailsAsync_IncorrectUser_ShouldBeOkWithMessageError()
        {
            // With Incorrect credentials
            MockAuthService
                .Setup(x => x.PostAsync(It.IsNotNull<AuthenticateRequest>()))
                .ReturnsAsync(new AuthenticateResponse() { IsValid = false, Message = UtilsResponse.clienteConError });
            var controller = new RequestController(MockRequestService.Object, MockAuthService.Object, null);
            var response = await controller.GetProductDetailsAsync(new ProductDetailRequest());
            OkObjectResult result = response as OkObjectResult;

            Assert.NotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(result.Value, UtilsResponse.clienteConError);
        }

        [Test]
        public async Task GetProductDetailsAsync_IncorrectUserInValidate_ShouldBeOkWithMessageError()
        {
            // Mock Post method
            MockAuthService
                .Setup(x => x.PostAsync(It.IsNotNull<AuthenticateRequest>()))
                .ReturnsAsync(new AuthenticateResponse() { IsValid = true, Token = "MITOKEN" });
            // Mock ValidateUserExist method
            MockAuthService
                .Setup(x => x.ValidateUserExist(It.IsNotNull<AuthenticateRequest>()))
                .ReturnsAsync(($"{UtilsResponse.clienteConCedulaIncorrecta}-false", false));

            var controller = new RequestController(MockRequestService.Object, MockAuthService.Object, null);
            var response = await controller.GetProductDetailsAsync(new ProductDetailRequest());

            OkObjectResult result = response as OkObjectResult;
            Assert.NotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(result.Value, UtilsResponse.clienteConCedulaIncorrecta);
        }

        [Test]
        public async Task GetProductDetailsAsync_WithNoProduct_ShouldBeOkWithMessageErro()
        {
            MockAuthService
                .Setup(x => x.PostAsync(It.IsNotNull<AuthenticateRequest>()))
                .ReturnsAsync(new AuthenticateResponse() { IsValid = true, Token = "MITOKEN" });
            
            MockAuthService
                .Setup(x => x.ValidateUserExist(It.IsNotNull<AuthenticateRequest>()))
                .ReturnsAsync(($"98054-true", true));

            MockRequestService
                .Setup(x => x.GetProductAsync(It.IsNotNull<ProductRequest>()))
                .ReturnsAsync(new AccountProductResponse() {Result = new Response<IEnumerable<AccountDataResponse>>()});
            var controller = new RequestController(MockRequestService.Object, MockAuthService.Object, null);
            var response = await controller.GetProductDetailsAsync(new ProductDetailRequest() {pTexto="2020",pTelefono = "829-000-0000"});
            var result = response as OkObjectResult;
            
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(result.Value, UtilsResponse.ClienteSinProductos);
        }
    }
}