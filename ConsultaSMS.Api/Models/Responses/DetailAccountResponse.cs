using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Models.Responses
{
    [Serializable]
    [DisplayName("ProductDetailResponse")]
    public class DetailAccountResponse
    {
        [DisplayName("ListProductDetailResponse")]
        public List<DetailAccountDataResponse> Result { get; set; } =
            new List<DetailAccountDataResponse>();
    }

    [DisplayName("ProductDetailDataResponse")]
    public class DetailAccountDataResponse
    {
        public decimal? Balance { get; set; }
    }

    [Serializable]
    [XmlRoot("GetAccountDetails")]
    public class DetailAccountXMLResponse
    {
        [XmlElement("Row")]
        public List<DetailAccountResultXMLResponse> Details { get; set; }
    }

    public class DetailAccountResultXMLResponse
    {
        [XmlCustomAttribute(0, PadAccordingToType.String, 100, ' ')]
        public string ProductBankIdentifier { get; set; }

        [XmlCustomAttribute(1, PadAccordingToType.String, 100, ' ')]
        public string ProductBranchName { get; set; }

        [XmlCustomAttribute(2, PadAccordingToType.String, 100, ' ')]
        public string ProductStatusId { get; set; }

        [XmlCustomAttribute(3, PadAccordingToType.String, 100, ' ')]
        public string ProductOpeningDate { get; set; }

        [XmlCustomAttribute(4, PadAccordingToType.String, 100, ' ')]
        public string ProductLastMovementDate { get; set; }

        [XmlCustomAttribute(5, PadAccordingToType.String, 100, ' ')]
        public string AccountOverdraftLimit { get; set; }

        [XmlCustomAttribute(6, PadAccordingToType.String, 100, ' ')]
        public string AccountAvailableAgreedOverdraft { get; set; }

        [XmlCustomAttribute(7, PadAccordingToType.String, 100, ' ')]
        public string AccountInterestEarnedInTheMonth { get; set; }

        [XmlCustomAttribute(8, PadAccordingToType.String, 100, ' ')]
        public string AccountInterestOverdraftAgreed { get; set; }

        [XmlCustomAttribute(9, PadAccordingToType.String, 100, ' ')]
        public string AccountInterestOverdraftNotAgreed { get; set; }

        [XmlCustomAttribute(10, PadAccordingToType.String, 100, ' ')]
        public decimal AccountAvailableBalance { get; set; }

        [XmlCustomAttribute(11, PadAccordingToType.String, 100, ' ')]
        public string CertifiedChecks { get; set; }

        [XmlCustomAttribute(12, PadAccordingToType.String, 100, ' ')]
        public string AccountRepossessedBalance { get; set; }

        [XmlCustomAttribute(13, PadAccordingToType.String, 100, ' ')]
        public string AccountFrozenBalance { get; set; }

        [XmlCustomAttribute(14, PadAccordingToType.String, 100, ' ')]
        public string AccountTotalBalance { get; set; }

        [XmlCustomAttribute(15, PadAccordingToType.String, 100, ' ')]
        public string AccountInTransitBalance { get; set; }

        [XmlCustomAttribute(16, PadAccordingToType.String, 100, ' ')]
        public string AccountBackendTypeDescription { get; set; }

        [XmlCustomAttribute(17, PadAccordingToType.String, 100, ' ')]
        public string AccountOwnerAddress { get; set; }

        [XmlCustomAttribute(18, PadAccordingToType.String, 100, ' ')]
        public string AccountCurrentBalance { get; set; }

        [XmlCustomAttribute(19, PadAccordingToType.String, 100, ' ')]
        public string RegionalAccountNumber { get; set; }

        [XmlCustomAttribute(20, PadAccordingToType.String, 100, ' ')]
        public string TransitCreditLineLimit { get; set; }

        [XmlCustomAttribute(21, PadAccordingToType.String, 100, ' ')]
        public string AvaliableTransitCreditLine { get; set; }

        [XmlCustomAttribute(22, PadAccordingToType.String, 100, ' ')]
        public string TotalOverdraftInTransit { get; set; }
    }
}
