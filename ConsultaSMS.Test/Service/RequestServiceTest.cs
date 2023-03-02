using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using WebConsultaSMS.Models.Entities;
using WebConsultaSMS.Models.Requests;
using WebConsultaSMS.Services;
using WebConsultaSMS.Utils;

namespace ConsultaSMS.Test.Service
{
    public class RequestServiceTest : TestApiContextMemoryData
    {
        private Mock<IBaseService> mockService;
        private Mock<IHttpContextAccessor> mockHttpAccessor;

        [SetUp]
        public void SetUp()
        {
            mockHttpAccessor = new Mock<IHttpContextAccessor>();
            mockService = new Mock<IBaseService>();
        }

        [Test]
        // UserName, Result Fake Mediator
        // Incorrect user that is not on the dabase
        [TestCase("the user dont exist on database", "BMSC-AFE55", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /> <HField name='RealDate' Value='20221125' /> <HField name='BussDate' Value='20221125' /> <HField name='Ter_Secuential' Value='0' /> <HField name='Reverso' Value='False' /> <HField name='TrnCode' Value='0006' /> <HField name='RedOrigen' Value='1' /> <HField name='EntidadOrigen' Value='1' /> <HField name='ExtremeID' Value='0' /> <HField name='SysTrnId' Value='0' /> <HField name='HostType' Value='' /> </Header> <Data> <DField Type='P' name='RetCode' Value='0' /> <DField Type='P' name='RetMsg' Value='CONSULTA OK' /> <DField Type='P' name='XMLResponse' Value='&lt;GetProductsResult&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;CA &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;11042010105587 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;2 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;0&lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;&lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;147.86 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;147.86 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;147.86 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;TC &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;220709213320577226 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;3 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;30000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;18 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;18760.35 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;476.68 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;100 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;0 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;29523.32 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;422135XXXXXX1974 &lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;PR &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;245505 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;5 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;800000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;07 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;723980.95 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;723980.95 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;746907.01 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;/GetProductsResult&gt;' /> <DField Type='P' name='MappedRespCode' Value='8096' /> <DField Type='P' name='MsgErr' Value='ALL: TRANSACCION CORRECTA' /> </Data> </ExtremeMsgReply>")]
        public async Task GetProductAsync_WithIncorrectUser_ThowError(string expectedResult, string userName, string resultMediator)
        {
            UserEntity currentUser = userEntities
                                        .FirstOrDefault(x => x.UserName == userName);
            AddClaimUserName(currentUser);
            IEnumerable<string> mediatorUrls = mediatorEntities
                                                .Select(x => x.Url);

            mockService
               .Setup(x => x.SendTransaction(It.IsIn(mediatorUrls), It.IsAny<string>()))
                .ReturnsAsync(resultMediator);

            var service = new RequestService(ApiContext, mockHttpAccessor.Object, mockService.Object);

            var response = await service.GetProductAsync(new ProductRequest()
            {
                pTiposProductos = string.Empty,
                //pCodCliente = currentUser.MDCodeClient
            });
            Assert.Multiple(() => {
                Assert.NotNull(response?.Result);
                Assert.AreEqual(expectedResult, response.Result.MessageError);
                Assert.AreEqual(StatusCodes.Status400BadRequest.ToString(), response.Result.StatusCode);
                Assert.IsTrue(response.Result.hasError);
            });
        }

        [Test]
        // UserName, Result Fake Mediator
        // Simulate the mediator got a error calling its
        [TestCase("BMSC-AFE", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /> <HField name='RealDate' Value='20221125' /> <HField name='BussDate' Value='20221125' /> <HField name='Ter_Secuential' Value='0' /> <HField name='Reverso' Value='False' /> <HField name='TrnCode' Value='0006' /> <HField name='RedOrigen' Value='1' /> <HField name='EntidadOrigen' Value='1' /> <HField name='ExtremeID' Value='0' /> <HField name='SysTrnId' Value='0' /> <HField name='HostType' Value='' /> </Header> <Data> <DField Type='P' name='RetCode' Value='0' /> <DField Type='P' name='RetMsg' Value='CONSULTA OK' /> <DField Type='P' name='XMLResponse' Value='&lt;GetProductsResult&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;CA &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;11042010105587 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;2 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;0&lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;&lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;147.86 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;147.86 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;147.86 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;TC &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;220709213320577226 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;3 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;30000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;18 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;18760.35 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;476.68 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;100 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;0 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;29523.32 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;422135XXXXXX1974 &lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;PR &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;245505 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;5 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;800000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;07 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;723980.95 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;723980.95 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;746907.01 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;/GetProductsResult&gt;' /> <DField Type='P' name='MappedRespCode' Value='8096' /> <DField Type='P' name='MsgErr' Value='ALL: TRANSACCION CORRECTA' /> </Data> </ExtremeMsgReply>")]
        public async Task GetProductAsync_WithMediatorDown_ThowError(string userName, string resultMediator)
        {
            UserEntity currentUser = userEntities
                                        .FirstOrDefault(x => x.UserName == userName);
            AddClaimUserName(currentUser);
            IEnumerable<string> mediatorUrls = mediatorEntities
                                                .Select(x => x.Url);

            mockService
               .Setup(x => x.SendTransaction(It.IsIn(mediatorUrls), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var service = new RequestService(ApiContext, mockHttpAccessor.Object, mockService.Object);

            var response = await service.GetProductAsync(new ProductRequest()
            {
                pTiposProductos = string.Empty,
                //pCodCliente = currentUser.MDCodeClient
            });
            Assert.NotNull(response?.Result);
            Assert.AreEqual(StatusCodes.Status400BadRequest.ToString(), response.Result.StatusCode);
            Assert.IsTrue(response.Result.hasError);
        }

        [Test]
        // UserName, Result Fake Mediator
        // User correct but dont have any product on the mediator
        [TestCase(0, "BMSC-AFE", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /> <HField name='RealDate' Value='20221125' /> <HField name='BussDate' Value='20221125' /> <HField name='Ter_Secuential' Value='0' /> <HField name='Reverso' Value='False' /> <HField name='TrnCode' Value='0006' /> <HField name='RedOrigen' Value='1' /> <HField name='EntidadOrigen' Value='1' /> <HField name='ExtremeID' Value='0' /> <HField name='SysTrnId' Value='0' /> <HField name='HostType' Value='' /> </Header> <Data> <DField Type='P' name='RetCode' Value='0' /> <DField Type='P' name='RetMsg' Value='CONSULTA OK' /> <DField Type='P' name='XMLResponse' Value='&lt;GetProductsResult&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;CA &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;11042010105587 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;2 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;0&lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;&lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;147.86 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;147.86 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;147.86 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;TC &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;220709213320577226 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;3 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;30000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;18 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;18760.35 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;476.68 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;100 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;0 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;29523.32 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;422135XXXXXX1974 &lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;PR &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;245505 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;5 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;800000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;07 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;723980.95 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;723980.95 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;746907.01 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;/GetProductsResult&gt;' /> <DField Type='P' name='MappedRespCode' Value='8096' /> <DField Type='P' name='MsgErr' Value='ALL: TRANSACCION CORRECTA' /> </Data> </ExtremeMsgReply>")]
        [TestCase(0, "BMSC-AFE", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /> <HField name='RealDate' Value='20221125' /> <HField name='BussDate' Value='20221125' /> <HField name='Ter_Secuential' Value='0' /> <HField name='Reverso' Value='False' /> <HField name='TrnCode' Value='0006' /> <HField name='RedOrigen' Value='1' /> <HField name='EntidadOrigen' Value='1' /> <HField name='ExtremeID' Value='0' /> <HField name='SysTrnId' Value='0' /> <HField name='HostType' Value='' /> </Header> <Data> <DField Type='P' name='RetCode' Value='0' /> <DField Type='P' name='RetMsg' Value='CONSULTA OK' /> <DField Type='P' name='XMLResponse' Value='&lt;GetProductsResult&gt;&lt;/GetProductsResult&gt;' /> <DField Type='P' name='MappedRespCode' Value='8096' /> <DField Type='P' name='MsgErr' Value='ALL: TRANSACCION CORRECTA' /> </Data> </ExtremeMsgReply>")]
        public async Task GetProductAsync_WithNoProduct_ThowError(int expectedCountResult, string userName, string resultMediator)
        {
            UserEntity currentUser = userEntities
                                        .FirstOrDefault(x => x.UserName == userName);
            AddClaimUserName(currentUser);
            IEnumerable<string> mediatorUrls = mediatorEntities
                                                .Select(x => x.Url);

            mockService
               .Setup(x => x.SendTransaction(It.IsIn(mediatorUrls), It.IsAny<string>()))
                .ReturnsAsync(resultMediator);

            var service = new RequestService(ApiContext, mockHttpAccessor.Object, mockService.Object);

            var response = await service.GetProductAsync(new ProductRequest()
            {
                pTiposProductos = string.Empty,
                //pCodCliente = currentUser.MDCodeClient
            });

            Assert.AreEqual(expectedCountResult, response.Result.Data.Count());
            Assert.AreEqual(StatusCodes.Status400BadRequest.ToString(), response.Result.StatusCode);
            Assert.IsTrue(response.Result.hasError);
        }

        [Test]
        // Expected Result,  UserName, Result Fake Mediator
        // Correct user and with products on the database
        [TestCase(1, "BMSC-AFE", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /> <HField name='RealDate' Value='20221125' /> <HField name='BussDate' Value='20221125' /> <HField name='Ter_Secuential' Value='0' /> <HField name='Reverso' Value='False' /> <HField name='TrnCode' Value='0006' /> <HField name='RedOrigen' Value='1' /> <HField name='EntidadOrigen' Value='1' /> <HField name='ExtremeID' Value='0' /> <HField name='SysTrnId' Value='0' /> <HField name='HostType' Value='' /> </Header> <Data> <DField Type='P' name='RetCode' Value='0' /> <DField Type='P' name='RetMsg' Value='CONSULTA OK' /> <DField Type='P' name='XMLResponse' Value='&lt;GetProductsResult&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;CA &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;11042010105587 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;2 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;1&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;0&lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;&lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;147.86 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;147.86 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;147.86 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;TC &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;220709213320577226 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;3 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;30000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;18 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;18760.35 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;476.68 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;100 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;0 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;29523.32 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;422135XXXXXX1974 &lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;PR &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;245505 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;5 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;800000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;07 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;723980.95 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;723980.95 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;746907.01 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;/GetProductsResult&gt;' /> <DField Type='P' name='MappedRespCode' Value='8096' /> <DField Type='P' name='MsgErr' Value='ALL: TRANSACCION CORRECTA' /> </Data> </ExtremeMsgReply>")]
        [TestCase(2, "BMSC-AFE", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /> <HField name='RealDate' Value='20221125' /> <HField name='BussDate' Value='20221125' /> <HField name='Ter_Secuential' Value='0' /> <HField name='Reverso' Value='False' /> <HField name='TrnCode' Value='0006' /> <HField name='RedOrigen' Value='1' /> <HField name='EntidadOrigen' Value='1' /> <HField name='ExtremeID' Value='0' /> <HField name='SysTrnId' Value='0' /> <HField name='HostType' Value='' /> </Header> <Data> <DField Type='P' name='RetCode' Value='0' /> <DField Type='P' name='RetMsg' Value='CONSULTA OK' /> <DField Type='P' name='XMLResponse' Value='&lt;GetProductsResult&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;CA &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;11042010105587 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;2 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;1&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;0&lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;&lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;147.86 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;147.86 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;147.86 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;TC &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;220709213320577226 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;3 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;1&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;30000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;18 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;18760.35 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;476.68 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;100 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;0 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;29523.32 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;422135XXXXXX1974 &lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;PR &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;245505 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;5 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;0&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;800000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;07 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;723980.95 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;723980.95 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;746907.01 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;/GetProductsResult&gt;' /> <DField Type='P' name='MappedRespCode' Value='8096' /> <DField Type='P' name='MsgErr' Value='ALL: TRANSACCION CORRECTA' /> </Data> </ExtremeMsgReply>")]
        [TestCase(3, "BMSC-AFE", @"<ExtremeMsgReply><Header><HField name='TrnOk' Value='1' /><HField name='TranType' Value='1' /> <HField name='RealDate' Value='20221125' /> <HField name='BussDate' Value='20221125' /> <HField name='Ter_Secuential' Value='0' /> <HField name='Reverso' Value='False' /> <HField name='TrnCode' Value='0006' /> <HField name='RedOrigen' Value='1' /> <HField name='EntidadOrigen' Value='1' /> <HField name='ExtremeID' Value='0' /> <HField name='SysTrnId' Value='0' /> <HField name='HostType' Value='' /> </Header> <Data> <DField Type='P' name='RetCode' Value='0' /> <DField Type='P' name='RetMsg' Value='CONSULTA OK' /> <DField Type='P' name='XMLResponse' Value='&lt;GetProductsResult&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;CA &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;11042010105587 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;2 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;1&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;0 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;&lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;147.86 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;147.86 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;147.86 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;TC &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;220709213320577226 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;3 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;1 &lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;30000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;18 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;18760.35 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;476.68 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;100 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;0 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;29523.32 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;422135XXXXXX1974 &lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;Row&gt;&lt;ProductBankIdentifier&gt;PR &lt;/ProductBankIdentifier&gt;&lt;ProductNumber&gt;245505 &lt;/ProductNumber&gt;&lt;ProductTypeId&gt;5 &lt;/ProductTypeId&gt;&lt;CurrencyId&gt;214 &lt;/CurrencyId&gt;&lt;ProductStatusId&gt;1&lt;/ProductStatusId&gt;&lt;CanTransact&gt;true &lt;/CanTransact&gt;&lt;AccountAvailableBalance&gt;800000 &lt;/AccountAvailableBalance&gt;&lt;ExpirationDate&gt;07 / 11 / 2022 &lt;/ExpirationDate&gt;&lt;CurrentBalance&gt;723980.95 &lt;/CurrentBalance&gt;&lt;AvailableAmount&gt;0 &lt;/AvailableAmount&gt;&lt;DollarAvailableAmount&gt;0 &lt;/DollarAvailableAmount&gt;&lt;DollarBalance&gt;723980.95 &lt;/DollarBalance&gt;&lt;LocalBalance&gt;746907.01 &lt;/LocalBalance&gt;&lt;MaskedProductNumber&gt;&lt;/MaskedProductNumber&gt;&lt;/Row&gt;&lt;/GetProductsResult&gt;' /> <DField Type='P' name='MappedRespCode' Value='8096' /> <DField Type='P' name='MsgErr' Value='ALL: TRANSACCION CORRECTA' /> </Data> </ExtremeMsgReply>")]

        public async Task GetProductAsync_WithProducts_ShouldBeOk(int expectedCountResult, string userName, string resultMediator)
        {
            UserEntity currentUser = userEntities
                                        .FirstOrDefault(x => x.UserName == userName);
            AddClaimUserName(currentUser);
            IEnumerable<string> mediatorUrls = mediatorEntities
                                                .Select(x => x.Url);

            mockService
               .Setup(x => x.SendTransaction(It.IsIn(mediatorUrls), It.IsAny<string>()))
                .ReturnsAsync(resultMediator);

            var service = new RequestService(ApiContext, mockHttpAccessor.Object, mockService.Object);

            var response = await service.GetProductAsync(new ProductRequest()
            {
                pTiposProductos = string.Empty,
            });

            Assert.AreEqual(StatusCodes.Status201Created.ToString(), response.Result.StatusCode);
            Assert.IsFalse(response.Result.hasError);
            Assert.AreEqual(expectedCountResult, response.Result.Data.Count());
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