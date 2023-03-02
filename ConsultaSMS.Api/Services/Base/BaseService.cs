using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConsultaSMS.DataBase;
using WebConsultaSMS.Models.Entities;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Models.Requests;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Services
{
    public class BaseService : ControllerBase, IBaseService
    {
        private readonly IHttpContextAccessor accessor;
        public IBaseService _baseService;

        //private readonly IBaseService baseService;
        public string transactionName { get; set; }
        private XmlMDRequest MediatorRequestData { get; set; }
        private ApiContext dbContext;
        public bool logTransaction { get; set; } = true;

        public BaseService(ApiContext dbContext, IHttpContextAccessor accessor)
        {
            this.dbContext = dbContext;
            this.accessor = accessor;
            //this.baseService = baseService;
        }

        // TODO: This Method return the request to be send to MEDIATOR
        internal virtual string MapToPayload<T>(T request) where T : class
        {
            // TODO: List Of data to be insert on <Data> </Data> from mediator XML
            var listProperties = Utils.UtilsMethods.GetPropertiersClass<T>(request);
            int concurrent = 0;
            foreach (var prop in listProperties)
            {
                if (concurrent == 0)
                {
                    // Add Firt Property on <Data> </Data>
                    MediatorRequestData.AddInitialDataToChildData(prop.Value.ToString(), prop.Name);
                    concurrent++;
                    continue;
                }
                // Add second or more Properties  on <Data> </Data>
                MediatorRequestData.AddDataToChildDataAnother(prop.Value.ToString(), prop.Name);
                concurrent++;
            }

            return MediatorRequestData?.GetRequest();
        }

        // TODO: This Method return the transaction set on database
        internal virtual async Task<TransactionEntity> GetTransaction()
        {
            TransactionEntity transaction = null;
            MediatorRequestData = null;
            try
            {

                transaction = await dbContext.Transactions
                    //.Include(x => x.Mediator)
                    .FirstOrDefaultAsync(x => x.Name == transactionName && x.Active);

                if (transaction == null)
                {
                    throw Utils.UtilsMethods.throwError(
                        "The transaction not exist on the database",
                        new DbUpdateException()
                    );
                }
                else
                {
                    transaction.Mediator = await GetCurrentMediator();
                    MediatorRequestData = new XmlMDRequest(
                        transaction.Channel,
                        transaction.TrnCode
                    );
                    return transaction;
                }
            }
            catch
            {
                throw Utils.UtilsMethods.throwError(
                    "Error Searching the transaction on database",
                    new InvalidOperationException()
                );
            }
        }

        private async Task<MediatorEntity> GetCurrentMediator()
        {
            return await dbContext
                            .Mediators
                            .FirstOrDefaultAsync(x => x.Active == true);
        }

        internal async Task<TResponse> ExecuteTransactionMDSAsync<TRequest, TResponse>(
            TRequest RequestMD,
            TResponse ResponseMD,
            string rootName,
            string rootNode = "ExtremeMsgReply",
            bool resultIsAnotherXML = false, // Set true is the result has a another XML
            string subNodeName = ""
        )
            where TRequest : class
            where TResponse : class
        {
            Stopwatch stopWatchMDS = new Stopwatch();
            Stopwatch watchTimer = new Stopwatch();
            watchTimer.Start();
            if (rootNode == string.Empty)
            {
                rootNode = "ExtremeMsgReply";
            }

            if (RequestMD == null || ResponseMD == null)
            {
                throw Utils.UtilsMethods.throwError(
                    "The Request and response should be initialize",
                    new NullReferenceException()
                );
            }

            string Host =
                $"{Request?.Headers["Origin"]} | {accessor?.HttpContext?.Connection?.RemoteIpAddress}";
            string UserAgent = Request?.Headers["User-Agent"].ToString();
            TransactionLogEntity transactionLog = new TransactionLogEntity()
            {
                CreatedDate = DateTime.Now,
                Status = Status.ACTIVE.ToString(),
                Request = JsonSerializer.Serialize(RequestMD),
                Host = Host,
                UserAgent = UserAgent
            };
            try
            {
                var transaction = await GetTransaction();
                transactionLog.TransactionId = transaction.Id;
                string request = MapToPayload(RequestMD);
                transactionLog.RequestMDS = request;
                stopWatchMDS.Start();
                string response = await _baseService.SendTransaction(transaction.Mediator?.Url, request);
                stopWatchMDS.Stop();
                transactionLog.ResponseMDS = response;
                transactionLog.EleasepTimeMDS = stopWatchMDS.Elapsed;
                ResponseMD = MapResponseToObject<TResponse>(
                    response,
                    ResponseMD,
                    rootName,
                    rootNode,
                    resultIsAnotherXML,
                    subNodeName
                );
                watchTimer.Stop();
                transactionLog.EleasepTime = watchTimer.Elapsed;
                transactionLog.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                transactionLog.ResponseCode = 400;
                throw ex;
            }
            finally
            {
                if (transactionLog.ResponseCode != 200)
                {
                    transactionLog.ResponseCode = 400;
                }
                transactionLog.Status = Status.FINALIZE.ToString();
                if (logTransaction)
                {
                    var userName = Utils.UtilsMethods.GetClaim(accessor.HttpContext, "UserName");
                    try
                    {
                        var user = await GetCurrentUser(userName, false);
                        transactionLog.UserId = user?.Id == null ? Guid.Empty : user.Id;
                    }
                    catch
                    {
                    }

                    transactionLog.Response = JsonSerializer.Serialize(ResponseMD);
                    transactionLog.TokenOrKey = accessor
                        ?.HttpContext?.Request?.Headers["Authorization"].ToString()
                        .Replace("Bearer ", "");

                    dbContext.Entry(transactionLog).State = EntityState.Added;
                }

                await dbContext.SaveChangesAsync();
            }

            return ResponseMD;
        }

        internal async Task<UserEntity> GetCurrentUser(string userName, bool throwError = true)
        {
            try
            {
                userName = FunctionsHelper.Decrypt(userName);
                var result = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
                if (result == null)
                {
                    if (throwError) throw new Exception();
                    return null;
                }
                return result;
            }
            catch
            {
                throw Utils.UtilsMethods.throwError(
                    "the user dont exist on database",
                    new DbUpdateException()
                );
            }
        }

        internal async Task IncrementCounter(string phoneNumber)
        {
            var phone = await dbContext.Phones.FirstOrDefaultAsync(x => x.Telephone == phoneNumber && x.Active);
            phone.NumberSmsSent += 1;
            dbContext.Entry(phone).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        // TODO: This method convert the string reponse to object reponse
        // PARAMS
        // PARAM response: XML from MEDIATOR
        // EXAMPLE: <ExtremeMsgReply> <test><data>example</data></test></ExtremeMsgReply>
        // PARAM result: The object response result
        // PARAM rootName: The firt object return to response MEDIATOR example
        // EXAMPLE from the value of response example rootName = test
        // PARAM nodeName: the value of root node from response nodeName = ExtremeMsgReply
        // PARAM resultIsAnotherXML: If you set that in true
        // ====> means the result in mediator is XML and need to Parse XML again
        // PARAM subNodeName: the name of root node of the sub XML on string
        // ====> for use this value need to set true resultIsAnotherXML
        private T MapResponseToObject<T>(
            string response,
            T result,
            string rootName,
            string nodeName = "ExtremeMsgReply",
            bool resultIsAnotherXML = false, // Set true is the result has a another XML
            string subNodeName = ""
        ) where T : class
        {
            var xmlDoc = Utils.UtilsMethods.ConvertStringToXML(response);
            bool goodResponseFoundMDS = false;
            T mdsSerializado;
            var MDSDatakeys = GetGeneralParameter();
            try
            {
                mdsSerializado = Utils.UtilsMethods.GetMediatorClass<T>(
                    new XmlSerializer(typeof(T)),
                    xmlDoc,
                    ref goodResponseFoundMDS,
                    MDSDatakeys,
                    rootName,
                    nodeName,
                    resultIsAnotherXML,
                    subNodeName
                );
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return mdsSerializado;
        }

        // TODO: get the general parameters from database
        private GeneralParameterEntity GetGeneralParameter()
        {
            var MDSKeys = dbContext.GeneralParameters
                .FirstOrDefaultAsync(x => x.NameAbrev == "MDSKeys")
                .GetAwaiter()
                .GetResult();
            if (MDSKeys == null)
            {
                throw Utils.UtilsMethods.throwError(
                    "MDSKeys dont exist on database",
                    new DbUpdateException()
                );
            }
            return MDSKeys;
        }

        public async Task<string> SendTransaction(string url, string requestMessage)
        {
            var endpoint = new ExtremeService.MDSClient.EndpointConfiguration();
            var mediator = new ExtremeService.MDSClient(endpoint, url);

            //mediator.Binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
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
                        requestMessage = GenerateRequest(requestMessage);
                        HttpRequestMessage request = GenerateRequest(requestMessage, url, client);

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

        public static HttpRequestMessage GenerateRequest(
           string requestMessage,
           string url,
           HttpClient client
       )
        {
            HttpContent content = new StringContent(requestMessage, Encoding.UTF8, "text/xml");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/MDS/Execute");
            return new HttpRequestMessage()
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(url)
            };
        }

        public static string GenerateRequest(string request)
        {
            return @$"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'><soapenv:Header/>
                 <soapenv:Body><tem:Execute><tem:xmlin><![CDATA[{request}
                 ]]></tem:xmlin></tem:Execute></soapenv:Body></soapenv:Envelope>";
        }
    }
}
