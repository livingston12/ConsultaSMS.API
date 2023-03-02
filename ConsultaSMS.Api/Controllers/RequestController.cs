using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebConsultaSMS.Interfaces;
using WebConsultaSMS.Models;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Models.Requests;
using WebConsultaSMS.Models.Responses;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Controllers
{
    [ApiController]
    [Route("api/request")]
    public class RequestController : ControllerBase
    {
        public IRequestService _accountService;
        public IAuthenticateService _authenticateService;
        IConfiguration Configuration;

        public RequestController(
            IRequestService accountService,
            IAuthenticateService authenticateService,
            IConfiguration configuration
        )
        {
            _accountService = accountService;
            _authenticateService = authenticateService;
            Configuration = configuration;
        }

        [HttpPost]
        [AuthorizeCustomAttribute("DetalleCuentas", "Admin")]
        [Route("account/products/details")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(Response<string>),
            StatusCodes.Status400BadRequest
        )]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductDetailsAsync(
            [FromBody] ProductDetailRequest request
        )
        {
            StringBuilder text = new StringBuilder();
            try
            {
                var isNumber = int.TryParse(request.pTexto, out int valid);
                request.pTexto = !isNumber ? string.Empty : request.pTexto;

                AuthenticateRequest requestAuth = new AuthenticateRequest()
                {
                    pCedula = request.pTexto,
                    pTelefono = request.pTelefono
                };
                var userExist = await _authenticateService.PostAsync(requestAuth);

                if (!userExist.IsValid)
                {
                    return Ok(userExist.Message);
                }

                // Validate user
                var isValidUser = await _authenticateService.ValidateUserExist(requestAuth);
                var parts = isValidUser.message.Split("-");
                string message = parts[0];
                bool.TryParse(parts[1], out bool isNew);

                if (!isValidUser.next)
                {
                    return Ok(message);
                }

                // TODO Search the product
                var product = await _accountService.GetProductAsync(
                    new ProductRequest() { pTiposProductos = string.Empty, pCodCliente = message }
                );
                var products = product?.Result.Data;

                // TODO validate if the client have products
                if (products == null || !products.Any())
                {
                    return Ok(UtilsResponse.ClienteSinProductos);
                }

                // TODO Get message from account
                var builderResult = await GetBuilderResult(request, products, isNew);
                text.Append(builderResult);
                await _accountService.IncrementSMSCounter(request.pTelefono);
            }
            catch
            {
                return BadRequest(UtilsResponse.clienteConError);
            }

            return Ok(text.ToString());
        }

        private async Task<string> GetBuilderResult(ProductDetailRequest request, IEnumerable<AccountDataResponse> products, bool isNew)
        {
            StringBuilder result = new StringBuilder();

            if (isNew)
            {
                result.Append(UtilsResponse.servicioActivado);
                result.AppendLine();
            }
            result.Append("BALANCE GENERAL");
            result.AppendLine();
            foreach (var prod in products)
            {
                request.pTiposProductos = prod.ProductType;
                request.pNumCuenta = prod.AccountNumber;
                bool isCreditCard = prod.ProductType == ProductTypes.TC.ToString();
                string accountNumber = isCreditCard ? prod.CardNumber : prod.AccountNumber;
                string ProductType = UtilsMethods.MapProductypeResponse(prod.ProductType);

                string header = $"{ProductType} {accountNumber} ";
                var actionResult = await ExecuteDetail(request);
                if (
                    actionResult.StatusCode == StatusCodes.Status400BadRequest
                    || actionResult.StatusCode == StatusCodes.Status502BadGateway
                )
                {
                    throw new Exception();
                }

                result.AppendLine();
                result.Append(header);
                result.AppendLine();
                result.Append(actionResult.Value);
                result.AppendLine();
            }

            return result.ToString();
        }

        private async Task<ObjectResult> ExecuteDetail(ProductDetailRequest request)
        {
            var currentType = Utils.UtilsMethods.GetCurentProductType(request.pTiposProductos);
            StringBuilder message = new StringBuilder();
            string moneda = request.pNumCuenta.StartsWith("2") ? "USD" : "RD";
            switch (currentType)
            {
                case ProductTypes.CA:
                case ProductTypes.CC:
                    var resultCA = await _accountService.GetDetailAccount(request);
                    message.Append($"Balance: {moneda}$ {resultCA.Data.Result?.FirstOrDefault().Balance?.ToString("C2")}");
                    return GetResultIAction(message.ToString(), resultCA.hasError);
                case ProductTypes.CD:
                    var resultCD = await _accountService.GetDetailCertificados(request);
                    message.Append($"RD$ {resultCD.Data.Result?.FirstOrDefault().Balance?.ToString("C2")}");
                    return GetResultIAction(message.ToString(), resultCD.hasError);
                case ProductTypes.PR:
                    var resultPR = await _accountService.GetDetailPrestamos(request);
                    foreach (var result in resultPR.Data.Result)
                    {
                        message.Append(result.ToProperties().ToString());
                    }

                    return GetResultIAction(message.ToString(), resultPR.hasError);
                case ProductTypes.TC:
                    var resultTC = await _accountService.GetDetailTarjetaCreditos(request);
                    foreach (var result in resultTC.Data.Result)
                    {
                        message.Append(result.ToProperties().ToString());
                    }
                    return GetResultIAction(message.ToString(), resultTC.hasError);
                default:
                    throw Utils.UtilsMethods.throwError(
                        $"This type of product is not mapped {request?.pTiposProductos}"
                    );
            }
        }

        private ObjectResult GetResultIAction(string response, bool hasError = false)
        {
            if (hasError)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
