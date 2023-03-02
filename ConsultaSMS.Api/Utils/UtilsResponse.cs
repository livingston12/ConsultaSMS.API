using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using WebConsultaSMS.Models.Responses;

namespace WebConsultaSMS.Utils
{
    public class UtilsResponse
    {
        private IConfiguration Configuration;


        public UtilsResponse(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public static string clienteNoRegistrado;
        public static string clienteConError;
        public static string clienteSinCedulaRegistrada;
        public static string clienteConCedulaIncorrecta;
        public static string clienteSinPin;
        public static string clienteValidarPin;
        public static string ClienteSinProductos;
        public static string clienteSinAccesoEnviarSMS;
        public static string clienteConPinIncorrecto;
        public static string servicioActivado;

        public void GetConfiguration()
        {
            clienteNoRegistrado =
                Configuration.GetSection("Mensajes")["clienteNoRegistrado"].ToString();
            clienteConError =
                Configuration.GetSection("Mensajes")["clienteConError"].ToString();
            clienteSinCedulaRegistrada =
                Configuration.GetSection("Mensajes")["clienteSinCedulaRegistrada"].ToString();
            clienteSinPin =
                Configuration.GetSection("Mensajes")["clienteSinPin"].ToString();
            ClienteSinProductos =
             Configuration.GetSection("Mensajes")["clienteSinProductos"].ToString();
            clienteSinAccesoEnviarSMS =
                Configuration.GetSection("Mensajes")["clienteSinAccesoEnviarSMS"].ToString();
            clienteConCedulaIncorrecta =
                Configuration.GetSection("Mensajes")["clienteConCedulaIncorrecta"].ToString();
            clienteConPinIncorrecto =
                Configuration.GetSection("Mensajes")["clienteConPinIncorrecto"].ToString();
            clienteValidarPin =
                Configuration.GetSection("Mensajes")["clienteValidarPin"].ToString();
            servicioActivado =
                Configuration.GetSection("Mensajes")["servicioActivado"].ToString();
        }

        internal static DetailAccountResponse MapDetailAccountResponse(
            DetailAccountXMLResponse responseData
        )
        {
            return new DetailAccountResponse()
            {
                Result = responseData.Details
                    .Select(
                        x => new DetailAccountDataResponse() { Balance = x.AccountAvailableBalance }
                    )
                    .ToList()
            };
        }

        internal static DetailPrestamosResponse MapDetailPrestamosResponse(
            DetailPrestamosXMLResponse responseData
        )
        {
            return new DetailPrestamosResponse()
            {
                Result = responseData.Details
                    .Select(
                        x =>
                            new DetailPrestamosDataResponse()
                            {
                                Balance = x.CancellationBalance,
                                FechaExpiracion = x.ExpirationDate,
                                Cuota = x.NextFeeNumber,
                                ProximoPago = x.NextFee_DueDate,
                            }
                    )
                    .ToList()
            };
        }

        internal static DetailCertificadoBankResponse MapDetailCertificadosResponse(
            DetailCertificadoBankXMLResponse responseData
        )
        {
            return new DetailCertificadoBankResponse()
            {
                Result = responseData.Details
                    .Select(
                        x => new DetailCertificadoBankDataResponse() { Balance = x.CurrentBalance }
                    )
                    .ToList()
            };
        }

        internal static DetailTarjetasCreditoResponse MapDetailTarjetasResponse(
            DetailTarjetasCreditoXMLResponse responseData
        )
        {
            return new DetailTarjetasCreditoResponse()
            {
                Result = responseData.Details
                    .Select(
                        x =>
                            new DetailTarjetasCreditoDataResponse()
                            {
                                BalanceDisponible = x.LocalAvailableForShopping,
                                BalanceFecha = x.LocalCurrencyInitialBalance,
                                BalanceCorte = x.LocalCurrencyCloseBalance,
                                PagoMinimo = x.LocalCurrencyMinPayment,
                                BalanceDisponibleUSD = x.DolarAvailableForShopping,
                                BalanceFechaUSD = x.DollarInitialBalance,
                                BalanceCorteUSD = x.DollarCloseBalance,
                                PagoMinimoUSD = x.DollarMinPayment,
                                FechaExpiracion = x.DueDate
                            }
                    )
                    .ToList()
            };
        }

        internal static AccountProductResponse MapProductToResult(AccountXmlResponse responseData)
        {
            return new AccountProductResponse()
            {
                Result = new Response<IEnumerable<AccountDataResponse>>()
                {
                    Data = responseData?.Productos.Select(
                        x =>
                            new AccountDataResponse()
                            {
                                AccountNumber = x.ProductNumber,
                                Currency = x.Currency,
                                ProductType = x.ProductBankIdentifier,
                                AvalibleAmount = x.Amount,
                                State = x.Status,
                                CardNumber = x.MaskedProductNumber
                            }
                    ),
                    hasError = responseData.Header.HasError,
                    MessageError = responseData.Header.Message,
                    StatusCode = responseData.Header.Code
                }
            };
        }
    }
}
