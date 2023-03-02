using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebConsultaSMS;
using WebConsultaSMS.Models;
using WebConsultaSMS.Services;
using WebConsultaSMS.Utils;

namespace ConsultaSMS.Test.Service
{
    public class AuthenticateServiceValidateUserExistTest : TestApiContextMemoryData
    {
        private Mock<IHttpContextAccessor> mockHttpAccessor;
        private IConfiguration Configuration;
        private Mock<IBaseService> mockService;
        [SetUp]
        public void SetUp()
        {
            IServiceProvider _services =
               Program.CreateHostBuilder(new string[] { }).Build().Services;
            var myService = _services.GetRequiredService<UtilsResponse>();
            myService.GetConfiguration();
            Configuration = _services.GetRequiredService<IConfiguration>();

            mockHttpAccessor = new Mock<IHttpContextAccessor>();
            mockService = new Mock<IBaseService>();
        }


        // clienteNoRegistrado: Cuando es un cliente que no existe en banca 2000 osea el numero ingresado no existe en el sistema.
        [Test]
        [TestCase("balance", "8098166584")]
        [TestCase("1234", "8098166584")]
        [Order(0)]
        public async Task ValidateUserExist_UserNotExistB200_ShouldBeThrowError(string numDocument, string phone)
        {
            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );
            AuthenticateRequest request = new AuthenticateRequest()
            {
                pCedula = numDocument,
                pTelefono = phone
            };

            var response = Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.ValidateUserExist(request);
            });

            Assert.IsNotNull(response);
            Assert.AreEqual(UtilsResponse.clienteNoRegistrado, response?.Message);
            Assert.IsInstanceOf<Exception>(response);
        }

        // clienteConCedulaIncorrecta: Cuando el cliente ingresa su cedula incorrecta.

        [Test]
        [TestCase("1234", "8098166515")]
        [TestCase("2010", "8098166515")]
        [Order(2)]
        public async Task ValidateUserExist_UserWithIncorrectDoc_ShouldBeOkMessageNumDocIncorrect(string numDocument, string phone)
        {
            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );
            AuthenticateRequest request = new AuthenticateRequest()
            {
                pCedula = numDocument,
                pTelefono = phone
            };
            var response = await service.ValidateUserExist(request);

            Assert.IsFalse(response.next);
            Assert.That(response.message, Does.Contain(UtilsResponse.clienteConCedulaIncorrecta));

        }

        // clienteConPinIncorrecto: Cuando el cliente ingresa su pin de seguridad incorrecto.
        [Test]
        [TestCase("1111", "8098166516")]
        [TestCase("2010", "8098166516")]
        [Order(3)]
        public async Task ValidateUserExist_UserWithIncorrectPin_ShouldBeOkMessagePinIncorrect(string numDocument, string phone)
        {
            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );
            AuthenticateRequest request = new AuthenticateRequest()
            {
                pCedula = numDocument,
                pTelefono = phone
            };
            var response = await service.ValidateUserExist(request);

            Assert.IsFalse(response.next);
            Assert.That(response.message, Does.Contain(UtilsResponse.clienteConPinIncorrecto));

        }


        // clienteSinPin: Cuando el cliente se está enrolando por primera vez y no a registrado su pin de seguridad.
        [Test]
        [TestCase("4321", "8098166515")]
        [Order(4)]
        public async Task ValidateUserExist_UserWithCorrectNumDoc_ShouldBeOkMessageDigitPin(string numDocument, string phone)
        {
            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );
            AuthenticateRequest request = new AuthenticateRequest()
            {
                pCedula = numDocument,
                pTelefono = phone
            };
            var response = await service.ValidateUserExist(request);

            Assert.IsFalse(response.next);
            Assert.That(response.message, Does.Contain(UtilsResponse.clienteSinPin));
        }

        // clienteValidarPin Mensaje para que el cliente ingrese su pin de seguridad. ya enrolado
        [Test]
        [TestCase("balance", "8098166516")]
        [Order(5)]
        public async Task ValidateUserExist_UserEnrrolment_ShouldBeOkMessageDigitPin(string numDocument, string phone)
        {
            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );
            AuthenticateRequest request = new AuthenticateRequest()
            {
                pCedula = numDocument,
                pTelefono = phone
            };
            var response = await service.ValidateUserExist(request);

            Assert.IsFalse(response.next);
            Assert.That(response.message, Does.Contain(UtilsResponse.clienteValidarPin));
        }

        // Cliente con pin correcto
        [Test]
        [TestCase("1234", "8098166516")]
        [Order(6)]
        public async Task ValidateUserExist_UserCorrectValues_ShouldBeOk(string numDocument, string phone)
        {
            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );
            AuthenticateRequest request = new AuthenticateRequest()
            {
                pCedula = numDocument,
                pTelefono = phone
            };
            var response = await service.ValidateUserExist(request);

            Assert.IsTrue(response.next);
        }
    }
}