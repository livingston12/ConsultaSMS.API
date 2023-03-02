using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebConsultaSMS.DataBase;
using WebConsultaSMS.Interfaces;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Models.Requests;
using WebConsultaSMS.Models.Responses;

namespace WebConsultaSMS.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApiContext dbContext;
        private IHttpContextAccessor accessor;
        public BaseService baseService { get; set; }
        public RequestService(
                ApiContext context,
                IHttpContextAccessor accessor,
                IBaseService baseService
        )
        //: base(context, accessor, baseService)
        {
            dbContext = context;
            this.accessor = accessor;

            this.baseService = new BaseService(dbContext, accessor);
            this.baseService._baseService = baseService;
        }

        public async Task<AccountProductResponse> GetProductAsync(ProductRequest request)
        {
            baseService.transactionName = "ConsultaProductos";
            AccountProductResponse result = new AccountProductResponse();

            try
            {
                var userName = Utils.UtilsMethods.GetClaim(accessor.HttpContext, "UserName");
                var user = await baseService.GetCurrentUser(userName);
                AccountProductsRequest filters = new AccountProductsRequest()
                {
                    pCodCliente = request.pCodCliente,
                    pTiposProductos = request.pTiposProductos
                };
                AccountXmlResponse responseData = new AccountXmlResponse();
                responseData = await GetXmlObjectResponse(
                    filters,
                    responseData,
                    "GetProductsResult"
                );
                responseData.Productos = responseData?.Productos
                    .Where(
                        x =>
                            x.Status == "Active"
                            // this comment is to remove the certificados
                            && x.ProductBankIdentifier != ProductTypes.CD.ToString()
                    )
                    .ToList();

                if (responseData.Productos.Any() && !string.IsNullOrEmpty(filters.pTiposProductos))
                {
                    responseData.Productos = responseData.Productos
                        .Where(x => x.Status == filters.pTiposProductos)
                        .ToList();
                }
                result = Utils.UtilsResponse.MapProductToResult(responseData);
            }
            catch (System.Exception ex)
            {
                result.Result.hasError = true;
                result.Result.MessageError = ex?.Message;
                result.Result.StatusCode = "400";
            }

            return result;
        }

        private async Task<Response<TResponse>> GetProductDetailsAsync<TResponse>(
            ProductDetailRequest filters
        ) where TResponse : class
        {
            Response<TResponse> result = new Response<TResponse>();

            try
            {
                ProductTypes currentType = Utils.UtilsMethods.GetCurentProductType(
                    filters.pTiposProductos
                );
                switch (currentType)
                {
                    // ============= CUENTAS CORRIENTES Y CUENTAS RESULT RESPONSE  =============
                    case ProductTypes.CA:
                    case ProductTypes.CC:
                        baseService.transactionName = "DetalleCuentas";
                        var account = new DetailAccountResponse();
                        var currentObject = await GetProductDetailsAsync(
                            filters,
                            account,
                            currentType
                        );
                        var response = Utils.UtilsMethods.ConvertToClass<
                            DetailAccountResponse,
                            TResponse
                        >(currentObject);
                        result.Data = response;
                        break;
                    // ============= CERTIFICADOS RESULT RESPONSE =============
                    case ProductTypes.CD:
                        baseService.transactionName = "DetalleCertificados";
                        var certificado = new DetailCertificadoBankResponse();
                        var currentObjectCD = await GetProductDetailsAsync(
                            filters,
                            certificado,
                            currentType
                        );
                        var responseCD = Utils.UtilsMethods.ConvertToClass<
                            DetailCertificadoBankResponse,
                            TResponse
                        >(currentObjectCD);
                        result.Data = responseCD;
                        break;
                    // ============= PRESTAMOS RESULT RESPONSE =============
                    case ProductTypes.PR:
                        baseService.transactionName = "DetallePrestamos";
                        var prestamo = new DetailPrestamosResponse();
                        var currentObjectPR = await GetProductDetailsAsync(
                            filters,
                            prestamo,
                            currentType
                        );
                        var responsePR = Utils.UtilsMethods.ConvertToClass<
                            DetailPrestamosResponse,
                            TResponse
                        >(currentObjectPR);
                        result.Data = responsePR;
                        break;
                    // ============= TARJETA DE CREDITO RESULT RESPONSE =============
                    case ProductTypes.TC:
                        baseService.transactionName = "DetalleTarjetaCredito";
                        var tarjetaCredito = new DetailTarjetasCreditoResponse();
                        var currentObjectTC = await GetProductDetailsAsync(
                            filters,
                            tarjetaCredito,
                            currentType
                        );
                        var responseTC = Utils.UtilsMethods.ConvertToClass<
                            DetailTarjetasCreditoResponse,
                            TResponse
                        >(currentObjectTC);
                        result.Data = responseTC;
                        break;
                    default:
                        throw Utils.UtilsMethods.throwError("The type of product dont exist");
                }
                result.hasError = false;
                result.StatusCode = "201";
            }
            catch (System.Exception ex)
            {
                result.hasError = true;
                result.StatusCode = "400";
                result.MessageError = ex.Message;
            }

            return result;
        }

        private async Task<TResponse> GetProductDetailsAsync<TResponse>(
            ProductDetailRequest filters,
            TResponse result,
            ProductTypes currentType
        ) where TResponse : class
        {
            try
            {
                switch (currentType)
                {
                    // ========================== CUENTAS CORRIENTES Y CUENTAS ================================
                    // TODO Search products (CUENTAS, CUENTAS CORRIENTES)
                    case ProductTypes.CA:
                    case ProductTypes.CC:
                        var xmlResponse = new DetailAccountXMLResponse();
                        var responseData = await GetDetailsAsync(
                            filters,
                            xmlResponse,
                            "GetAccountDetails"
                        );
                        var resultCuenta = Utils.UtilsMethods.GetDetailAccount(responseData);
                        return Utils.UtilsMethods.ConvertToClass<DetailAccountResponse, TResponse>(
                            resultCuenta
                        );
                    // ========================== CERTIFICADOS ================================
                    case ProductTypes.CD:
                        var xmlResponseCD = new DetailCertificadoBankXMLResponse();
                        var responseDataCD = await GetDetailsAsync(
                            filters,
                            xmlResponseCD,
                            "GetFixedTermDepositResult"
                        );
                        var resultCD = Utils.UtilsMethods.GetDetailCertificados(responseDataCD);
                        return Utils.UtilsMethods.ConvertToClass<
                            DetailCertificadoBankResponse,
                            TResponse
                        >(resultCD);
                    // ========================== PRESTAMOS ================================
                    case ProductTypes.PR:
                        var xmlResponsePR = new DetailPrestamosXMLResponse();
                        var responseDataPR = await GetDetailsAsync(
                            filters,
                            xmlResponsePR,
                            "GetLoanResult"
                        );
                        var resultPR = Utils.UtilsMethods.GetDetailPrestamos(responseDataPR);
                        return Utils.UtilsMethods.ConvertToClass<
                            DetailPrestamosResponse,
                            TResponse
                        >(resultPR);
                    // ========================== TARJETA CREDITO ================================
                    case ProductTypes.TC:
                        var xmlResponseTC = new DetailTarjetasCreditoXMLResponse();
                        var responseDataTC =
                            await GetDetailsAsync<DetailTarjetasCreditoXMLResponse>(
                                filters,
                                xmlResponseTC,
                                "GetCreditCardDetailsResult"
                            );
                        var resultTC = Utils.UtilsMethods.GetDetailTarjetaCreditos(responseDataTC);
                        return Utils.UtilsMethods.ConvertToClass<
                            DetailTarjetasCreditoResponse,
                            TResponse
                        >(resultTC);
                    default:
                        throw Utils.UtilsMethods.throwError("The type of product dont exist");
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private async Task<TResponse> GetDetailsAsync<TResponse>(
            ProductDetailRequest filters,
            TResponse responseData,
            string rootName
        ) where TResponse : class
        {
            try
            {
                return await GetXmlObjectResponse(filters, responseData, rootName);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private async Task<TResponse> GetXmlObjectResponse<TRequest, TResponse>(
            TRequest filters,
            TResponse responseData,
            string rootName
        )
            where TRequest : class
            where TResponse : class
        {
            try
            {
                return await baseService.ExecuteTransactionMDSAsync<TRequest, TResponse>(
                    filters,
                    responseData,
                    rootName,
                    "ExtremeMsgReply",
                    false,
                    ""
                );
            }
            catch (System.Exception ex)
            {
                throw Utils.UtilsMethods.throwError(ex.Message, ex);
            }
        }

        public async Task<Response<DetailAccountResponse>> GetDetailAccount(
            ProductDetailRequest filters
        )
        {
            return await GetProductDetailsAsync<DetailAccountResponse>(filters);
        }

        public async Task<Response<DetailCertificadoBankResponse>> GetDetailCertificados(
            ProductDetailRequest filters
        )
        {
            return await GetProductDetailsAsync<DetailCertificadoBankResponse>(filters);
        }

        public async Task<Response<DetailPrestamosResponse>> GetDetailPrestamos(
            ProductDetailRequest filters
        )
        {
            return await GetProductDetailsAsync<DetailPrestamosResponse>(filters);
        }

        public async Task<Response<DetailTarjetasCreditoResponse>> GetDetailTarjetaCreditos(
            ProductDetailRequest filters
        )
        {
            return await GetProductDetailsAsync<DetailTarjetasCreditoResponse>(filters);
        }

        public async Task IncrementSMSCounter(string Telephone)
        {
            await baseService.IncrementCounter(Telephone);
        }
    }
}
