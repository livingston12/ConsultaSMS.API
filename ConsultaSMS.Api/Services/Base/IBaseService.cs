using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebConsultaSMS.Services
{
    public interface IBaseService
    {
        public async Task<string> SendTransaction(string url, string requestMessage)
        {
            var endpoint = new ExtremeService.MDSClient.EndpointConfiguration();
            var mediator = new ExtremeService.MDSClient(endpoint, url);

            if (mediator.Endpoint.Address.Uri.OriginalString.ToString().Contains("https"))
            {
                HttpResponseMessage response = new HttpResponseMessage();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback += delegate
                    {
                        return true;
                    };
                    using (var client = new HttpClient(httpClientHandler))
                    {
                        requestMessage = BaseService.GenerateRequest(requestMessage);
                        HttpRequestMessage request = BaseService.GenerateRequest(requestMessage, url, client);

                        response = await client.SendAsync(request);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var XmlResult = Utils.UtilsMethods.ConvertStringToXML(result);
                        return XmlResult.LastChild.InnerText;
                    }
                }
            }
            return await mediator.ExecuteAsync(requestMessage);
        }
    }
}