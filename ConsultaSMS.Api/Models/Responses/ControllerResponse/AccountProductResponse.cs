using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Models.Responses
{
    [Serializable]
    [DisplayName("AccountProductResponse")]
    public class AccountProductResponse
    {
        [DisplayName("ListProductsResponse")]
        public Response<IEnumerable<AccountDataResponse>> Result { get; set; } =
            new Response<IEnumerable<AccountDataResponse>>();
    }

    [DisplayName("AccountProductDataResponse")]
    public class AccountDataResponse
    {
        public string CardNumber { get; set; }
        public string AccountNumber { get; set; }
        public string ProductType { get; set; }
        public decimal? AvalibleAmount { get; set; }
        public string Currency { get; set; }
        public string State { get; set; }
    }

    [Serializable]
    [XmlRoot("SMSResult")]
    public class AccountXmlResponse
    {
        [XmlIgnore]
        public AccountXmlHeaderResponse Header
        {
            get
            {  
                AccountXmlHeaderResponse result = new AccountXmlHeaderResponse();

                if (Productos.Any())
                {
                    result = new AccountXmlHeaderResponse()
                    {
                        Code = "201",
                        HasError = false,
                        Message = ""
                    };
                }
                else
                {
                    result = new AccountXmlHeaderResponse()
                    {
                        Code = "400",
                        HasError = true,
                        Message = UtilsResponse.clienteConError
                    };
                }
                return result;
            }
        }

        [XmlElement("Row")]
        public List<AccountXmlDataResponse> Productos { get; set; } =
            new List<AccountXmlDataResponse>();
    }

    public class AccountXmlDataResponse
    {
        [XmlCustomAttribute(0, PadAccordingToType.String, 100, ' ')]
        public string ProductBankIdentifier { get; set; }

        [XmlCustomAttribute(1, PadAccordingToType.String, 20, ' ')]
        public string ProductNumber { get; set; }

        [XmlCustomAttribute(2, PadAccordingToType.String, 20, ' ')]
        public string ProductTypeId { get; set; }

        [XmlCustomAttribute(3, PadAccordingToType.String, 100, ' ')]
        public string CurrencyId { get; set; }

        [XmlCustomAttribute(4, PadAccordingToType.String, 20, ' ')]
        public string ProductStatusId { get; set; }

        [XmlCustomAttribute(5, PadAccordingToType.Boolean, 20, ' ')]
        public bool? CanTransact { get; set; }

        [XmlCustomAttribute(6, PadAccordingToType.Boolean, 20, ' ')]
        public decimal? AccountAvailableBalance { get; set; }

        [XmlCustomAttribute(8, PadAccordingToType.Boolean, 20, ' ')]
        public decimal? CurrentBalance { get; set; }

        [XmlCustomAttribute(9, PadAccordingToType.Boolean, 20, ' ')]
        public decimal? AvailableAmount { get; set; }

        [XmlCustomAttribute(10, PadAccordingToType.Boolean, 20, ' ')]
        public decimal? DollarAvailableAmount { get; set; }

        [XmlCustomAttribute(11, PadAccordingToType.Boolean, 20, ' ')]
        public decimal? DollarBalance { get; set; }

        [XmlCustomAttribute(12, PadAccordingToType.Boolean, 20, ' ')]
        public decimal? LocalBalance { get; set; }

        [XmlCustomAttribute(13, PadAccordingToType.String, 20, ' ')]
        public string MaskedProductNumber { get; set; }

        [XmlIgnore]
        public string Currency
        {
            get => CurrencyId == "214" ? "DOP" : "USD";
        }

        [XmlIgnore]
        public decimal? Amount
        {
            get => CurrencyId == "214" ? AvailableAmount : DollarAvailableAmount;
        }

        [XmlIgnore]
        public string Status
        {
            get =>  ProductStatusId.Trim() == "1" ? "Active" : "Inactive";
        }
    }

    [Serializable]
    public class AccountXmlHeaderResponse
    {
        [XmlCustomAttribute(0, PadAccordingToType.Boolean, 20, ' ')]
        public bool HasError { get; set; } = true;

        [XmlCustomAttribute(1, PadAccordingToType.String, 150, ' ')]
        public string Message { get; set; }

        [XmlCustomAttribute(2, PadAccordingToType.String, 3, ' ')]
        public string Code { get; set; }
    }
}
