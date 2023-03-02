using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using WebConsultaSMS.DataBase;
using WebConsultaSMS.Models.Entities;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Models.Responses;

namespace WebConsultaSMS.Utils
{
    public class UtilsMethods
    {
        internal static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        internal static string EncodeBase64(string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        internal static string[] GetHeaderRequest(HttpRequest request)
        {
            return new string[]
            {
                GetHeaderValue(request, "x-correlation-id"),
                GetHeaderValue(request, "x-issuer-id")
            };
        }

        internal static Exception throwError(string message, Exception ex = null)
        {
            return new Exception(message, ex);
        }

        internal static XmlDocument ConvertStringToXML(string response)
        {
            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(response);
            }
            catch
            {
                throw throwError($"error to load XML: {response}", new XmlException());
            }

            return xmlDoc;
        }

        private static string GetHeaderValue(HttpRequest request, string key)
        {
            var result = string.Empty;

            if (request.Headers.TryGetValue(key, out StringValues keyValues))
            {
                result = string.Join(string.Empty, keyValues);
            }

            return result;
        }

        // TODO: return the object serialized
        internal static T GetMediatorClass<T>(
            XmlSerializer xsSubmit,
            XmlDocument xmlDoc,
            ref bool foundIt,
            GeneralParameterEntity MDSKeys,
            string RootName,
            string nodeName = "ExtremeMsgReply",
            bool resultIsAnotherXML = false, // Set true is the result has a another XML
            string subNodeNamePath = ""
        )
        {
            List<string> listErrors = new List<string>();
            var mdsSerializado = Activator.CreateInstance(typeof(T));

            var posiblesReturnString = MDSKeys.Value.Decrypt().Split(',');

            XmlNode xmlNode = null;
            foreach (string item in posiblesReturnString)
            {
                if (!foundIt)
                {
                    try
                    {
                        //TODO: Vamos a tratar de recibir mas de 1 respuesta del mediator, pero mas adelante
                        var node = $"/{nodeName}/Data/DField[@name = '{item.Trim()}']";

                        xmlNode = xmlDoc.SelectSingleNode(node);
                        if (xmlNode == null)
                        {
                            node = $"/{nodeName}/Data/DField[@name ='RetMsg']";
                            xmlNode = xmlDoc.SelectSingleNode(node);
                            string error = xmlNode?.Attributes["Value"].Value.ToString();
                            if (error != null)
                            {
                                listErrors.Add(error);
                            }
                            continue;
                        }

                        if (resultIsAnotherXML)
                        {
                            mdsSerializado = GetMDSSerializado<T>(
                                RootName,
                                xsSubmit,
                                xmlNode,
                                subNodeNamePath
                            );
                        }
                        else
                        {
                            mdsSerializado = GetMDSSerializado<T>(RootName, xsSubmit, xmlNode);
                        }

                        foundIt = true;
                    }
                    catch (Exception ex)
                    {
                        foundIt = false;
                        throw throwError($"Error SelectSingleNode | msg: {ex?.Message} ");
                    }
                }
            }

            if (listErrors.Count() == posiblesReturnString.Count())
            {
                string error = String.Join(", ", listErrors);
                throw throwError(error, new SystemException());
            }

            return (T)Convert.ChangeType(mdsSerializado, typeof(T));
        }

        private static object GetMDSSerializado<T>(
            string rootName,
            XmlSerializer xsSubmit,
            XmlNode currentNode
        )
        {
            return excuteMDSSerializado<T>(xsSubmit, currentNode, false, rootName: rootName);
        }

        internal static object GetMDSSerializado<T>(
            string subRootNode,
            XmlSerializer xsSubmit,
            XmlNode currentNode,
            string subNodeNamePath
        )
        {
            return excuteMDSSerializado<T>(
                xsSubmit,
                currentNode,
                true,
                subNodeNamePath,
                subRootNode
            );
        }

        private static object excuteMDSSerializado<T>(
            XmlSerializer xsSubmit,
            XmlNode currentNode,
            bool resultIsAnotherXML,
            string subNodeName = "",
            string subNodeRootNodeName = "",
            string rootName = ""
        )
        {
            if (
                resultIsAnotherXML
                && (string.IsNullOrEmpty(subNodeName) || string.IsNullOrEmpty(subNodeRootNodeName))
            )
            {
                throw throwError(
                    $"If you set {nameof(resultIsAnotherXML)} in true need to set the values {nameof(subNodeName)}, {nameof(subNodeRootNodeName)}",
                    new NullReferenceException()
                );
            }
            else if (
                resultIsAnotherXML
                && !string.IsNullOrEmpty(subNodeName)
                && !string.IsNullOrEmpty(subNodeRootNodeName)
            ) // If have a sub XML on the result
            {
                XmlDocument newXmlDoc = ConvertStringToXML(
                    currentNode.Attributes["Value"].Value?.ToString()
                );
                // Get the node from path
                var nextNode = newXmlDoc.SelectSingleNode($"/{subNodeName}");

                if (newXmlDoc == null || nextNode == null)
                {
                    throwError(
                        "The document XML or node response is null",
                        new NullReferenceException()
                    );
                }

                newXmlDoc = ConvertStringToXML(nextNode?.InnerText);

                string xmlParse = nextNode?.InnerText;

                using (TextReader reader = new StringReader(xmlParse))
                {
                    return GetSerializerXml<T>(subNodeRootNodeName, reader);
                }
            }
            else // If dont have a sub XML on the result
            {
                string xmlParse = currentNode
                    ?.Attributes["Value"].Value.ToString()
                    .DecodeXMLString();

                using (TextReader reader = new StringReader(xmlParse))
                {
                    return GetSerializerXml<T>(rootName, reader);
                }
            }
        }

        // TODO: Get list of properties from any class
        internal static IEnumerable<PropsName> GetPropertiersClass<T>(T request) where T : class
        {
            Type _type = request.GetType();

            IEnumerable<PropertyInfo> listaProperties = _type.GetRuntimeProperties();

            var result = listaProperties.Select(
                x => new PropsName()
                {
                    Name = x.Name,
                    Value = x.GetValue(request) == null ?
                                string.Empty :
                                x.GetValue(request).ToString()
                }
            );

            return result;
        }

        // TODO: serialized the XML to Object class
        internal static T GetSerializerXml<T>(string rootNodeName, TextReader reader)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(
                    typeof(T),
                    new XmlRootAttribute() { ElementName = rootNodeName }
                );

                return (T)serializer.Deserialize(reader);
            }
            catch
            {
                throw throwError("Internal Error parsing the XML", new XmlException());
            }
        }

        internal static string GetClaim(HttpContext httpContext, string claimName)
        {
            string result = string.Empty;
            var user = httpContext?.User;
            if (user != null && !string.IsNullOrEmpty(claimName))
            {
                var identity = user?.Identity as ClaimsIdentity;
                result = identity.FindFirst(claimName)?.Value;
            }
            return result;
        }

        internal static int? GetRolId(ApiContext dbContext)
        {
            var rol = dbContext.Roles
                .FirstOrDefaultAsync(x => x.Name == "ExternalUser")
                .GetAwaiter()
                .GetResult();

            return rol?.RolId;
        }

        internal static TType ConvertToClass<TValue, TType>(TValue value) where TType : class
        {
            return (TType)Convert.ChangeType(value, typeof(TType));
        }

        internal static TResponse ConvertGenericToObject<TData, TResponse>(TData value)
        {
            return (TResponse)Convert.ChangeType(value, typeof(TResponse));
        }

        internal static ProductTypes GetCurentProductType(string pTiposProductos)
        {
            ProductTypes result;
            switch (pTiposProductos.ToUpper())
            {
                // =========== CUENTAS CORRIENTES Y CUENTAS ===========
                case "CA":
                case "CC":
                    result = ProductTypes.CA;
                    break;
                // =========== CERTIFICADOS ===========
                case "CD":
                    result = ProductTypes.CD;
                    break;
                // =========== PRESTAMOS ===========
                case "PR":
                    result = ProductTypes.PR;
                    break;
                // =========== TARJETA DE CREDITOS ===========
                case "TC":
                    result = ProductTypes.TC;
                    break;
                default:
                    result = ProductTypes.CA;
                    break;
            }
            return result;
        }

        internal static DetailAccountResponse GetDetailAccount<TResponse>(TResponse responseData)
        {
            // TODO convert the value responseData to current class that I need to map
            DetailAccountXMLResponse converResponse = ConvertGenericToObject<
                TResponse,
                DetailAccountXMLResponse
            >(responseData);

            // TODO get the response value map
            DetailAccountResponse products = Utils.UtilsResponse.MapDetailAccountResponse(
                converResponse
            );

            return products;
        }

        internal static DetailCertificadoBankResponse GetDetailCertificados<TResponse>(
            TResponse responseData
        )
        {
            var convertCertificadoResponse = ConvertGenericToObject<
                TResponse,
                DetailCertificadoBankXMLResponse
            >(responseData);

            DetailCertificadoBankResponse certificados =
                Utils.UtilsResponse.MapDetailCertificadosResponse(convertCertificadoResponse);

            return certificados;
        }

        internal static DetailPrestamosResponse GetDetailPrestamos<TResponse>(
            TResponse responseData
        )
        {
            var convertPrestamoResponse = ConvertGenericToObject<
                TResponse,
                DetailPrestamosXMLResponse
            >(responseData);

            DetailPrestamosResponse prestamos = Utils.UtilsResponse.MapDetailPrestamosResponse(
                convertPrestamoResponse
            );

            return prestamos;
        }

        internal static DetailTarjetasCreditoResponse GetDetailTarjetaCreditos<TResponse>(
            TResponse responseData
        ) where TResponse : class
        {
            var convertTarjetasResponse = ConvertGenericToObject<
                TResponse,
                DetailTarjetasCreditoXMLResponse
            >(responseData);

            DetailTarjetasCreditoResponse tarjetaCreditos =
                Utils.UtilsResponse.MapDetailTarjetasResponse(convertTarjetasResponse);

            return tarjetaCreditos;
        }

        internal static string MapProductypeResponse(string productType)
        {
            string result = string.Empty;
            Enum.TryParse<ProductTypes>(productType, out ProductTypes productTypes);

            switch (productTypes)
            {
                case ProductTypes.TC:
                    result = "TDC Visa";
                    break;
                case ProductTypes.PR:
                    result = "Prestamo";
                    break;
                default:
                    result = productType;
                    break;
            }

            return result;
        }
    }
}
