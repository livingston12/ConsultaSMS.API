using System.Data.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebConsultaSMS;
using WebConsultaSMS.Models;
using WebConsultaSMS.Models.Entities;
using WebConsultaSMS.Services;
using WebConsultaSMS.Utils;

namespace ConsultaSMS.Test.Service
{

    public class AuthenticateServiceTest : TestApiContextMemoryData
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

        // Cuando no se envia informacion
        [Test]
        [TestCase(null, "")]
        [TestCase(null, "8298888884")]
        [Order(0)]
        public async Task PostAsync_noValues_ThowError(string cedula, string telefono)
        {
            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );

            var exeption = Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.PostAsync(new AuthenticateRequest()
                {
                    pCedula = cedula,
                    pTelefono = telefono
                });
            });
            var transactionLogsCount = ApiContext.TransactionLogs.Count();

            Assert.IsNotNull(exeption);
            Assert.AreEqual(UtilsResponse.clienteConError, exeption?.Message);
            Assert.IsInstanceOf<Exception>(exeption);
            Assert.AreEqual(0, transactionLogsCount);
        }

        // Cuando es un cliente que no existe en banca 2000 osea el numero ingresado no existe en el sistema.
        [Test]
        [TestCase("balance", "8292204658")]
        [Order(1)]
        public async Task PostAsync_NotRegisterB2000_ThowError(string cedula, string telefono)
        {
            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );

            var response = await service.PostAsync(new AuthenticateRequest()
            {
                pCedula = cedula,
                pTelefono = telefono
            });

            var transactionLogsCount = ApiContext.TransactionLogs.Count();

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsValid);
            Assert.AreEqual(response.Message, UtilsResponse.clienteNoRegistrado);
            Assert.AreEqual(1, transactionLogsCount);
        }

        // Cuando el cliente se esta enrolando por primera vez y no a registrado su cedula por primera vez. 
        [Test]
        [TestCase(2, "balance", "8098166513", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /><HField name='RealDate' Value='20221124' /><HField name='BussDate' Value='20221124' /><HField name='Ter_Secuential' Value='0' /><HField name='Reverso' Value='False' /><HField name='TrnCode' Value='1805' /><HField name='RedOrigen' Value='1' /><HField name='EntidadOrigen' Value='1' /><HField name='ExtremeID' Value='0' /><HField name='SysTrnId' Value='0' /><HField name='HostType' Value='' /></Header><Data><DField Type='P' name='RetCode' Value='0' /><DField Type='P' name='RetMsg' Value='Transaccion No de puede procesar, Informe' /><DField Type='P' name='ResponseXML' Value='&amp;lt;SMSValidate&amp;gt;&amp;lt;ClientId&amp;gt;0&amp;lt;/ClientId&amp;gt;&amp;lt;Message&amp;gt;Cliente encontrado&amp;lt;/Message&amp;gt;&amp;lt;Code&amp;gt;00&amp;lt;/Code&amp;gt;&amp;lt;/SMSValidate&amp;gt;' /><DField Type='P' name='MappedRespCode' Value='8096' /><DField Type='P' name='MsgErr' Value='ALL: Error general' /></Data></ExtremeMsgReply>")]
        [TestCase(3, "balance", "8098166514", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /><HField name='RealDate' Value='20221124' /><HField name='BussDate' Value='20221124' /><HField name='Ter_Secuential' Value='0' /><HField name='Reverso' Value='False' /><HField name='TrnCode' Value='1805' /><HField name='RedOrigen' Value='1' /><HField name='EntidadOrigen' Value='1' /><HField name='ExtremeID' Value='0' /><HField name='SysTrnId' Value='0' /><HField name='HostType' Value='' /></Header><Data><DField Type='P' name='RetCode' Value='0' /><DField Type='P' name='RetMsg' Value='Transaccion No de puede procesar, Informe' /><DField Type='P' name='ResponseXML' Value='&amp;lt;SMSValidate&amp;gt;&amp;lt;ClientId&amp;gt;0&amp;lt;/ClientId&amp;gt;&amp;lt;Message&amp;gt;Cliente encontrado&amp;lt;/Message&amp;gt;&amp;lt;Code&amp;gt;00&amp;lt;/Code&amp;gt;&amp;lt;/SMSValidate&amp;gt;' /><DField Type='P' name='MappedRespCode' Value='8096' /><DField Type='P' name='MsgErr' Value='ALL: Error general' /></Data></ExtremeMsgReply>")]
        [Order(2)]
        public async Task PostAsync_ClientEnrrollFirtTime_ShouldBeOk(int logCountExpected ,string documentId, string phone, string resultMediator)
        {
            UserEntity currentUser = userEntities
                                                    .FirstOrDefault(x => x.UserName == phone);
            AddClaimUserName(currentUser);

            IEnumerable<string> mediatorUrls = mediatorEntities
                                                .Select(x => x.Url);
            mockService
               .Setup(x => x.SendTransaction(It.IsIn(mediatorUrls), It.IsAny<string>()))
                .ReturnsAsync(resultMediator);

            var service = new AuthenticateService(
                ApiContext,
                mockHttpAccessor.Object,
                Configuration,
                mockService.Object
             );

            var response = await service.PostAsync(new AuthenticateRequest()
            {
                pCedula = documentId,
                pTelefono = phone
            });

            var phoneInserted = ApiContext.Phones.FirstOrDefault(x => x.User.UserName == phone);
            var transactionLogsCount = ApiContext.TransactionLogs.Count();

            Assert.IsNotNull(response);
            Assert.IsNotNull(phoneInserted);
            Assert.AreEqual(logCountExpected, transactionLogsCount);
            Assert.IsTrue(response.IsValid);
            Assert.IsNotNull(response.Token);
            Assert.IsEmpty(response.Message);
        }

        private void AddClaimUserName(UserEntity currentUser)
        {
            if (currentUser != null)
            {
                var claims = new List<Claim>();
                claims.AddRange(
                    new List<Claim>()
                    {
                    new Claim("UserName", FunctionsHelper.Encryptor(currentUser.UserName)),
                    new Claim("Email", FunctionsHelper.Encryptor(currentUser.Email))
                    }
                );
                ClaimsIdentity identity = new ClaimsIdentity(claims);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                mockHttpAccessor
                    .Setup(x => x.HttpContext.User)
                    .Returns(principal);
            }
        }

    }
}