using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Models.Responses
{
    public class DetailCertificadoBankResponse
    {
        [DisplayName("DetailLoanResponse")]
        public List<DetailCertificadoBankDataResponse> Result { get; set; } =
            new List<DetailCertificadoBankDataResponse>();
    }

    public class DetailCertificadoBankDataResponse
    {
        public decimal? Balance { get; set; }
    }

    public class DetailCertificadoBankXMLResponse
    {
        [XmlElement("Row")]
        public List<DetailCertificadoBankXMLResultResonse> Details { get; set; } =
            new List<DetailCertificadoBankXMLResultResonse>();
    }

    public class DetailCertificadoBankXMLResultResonse
    {
        [XmlCustomAttribute(0, PadAccordingToType.String, 100, ' ')]
        public string ProductBankIdentifier { get; set; }

        [XmlCustomAttribute(1, PadAccordingToType.String, 100, ' ')]
        public string OriginalAmount { get; set; }

        [XmlCustomAttribute(2, PadAccordingToType.String, 100, ' ')]
        public string Rate { get; set; }

        [XmlCustomAttribute(3, PadAccordingToType.String, 100, ' ')]
        public string Term { get; set; }

        [XmlCustomAttribute(4, PadAccordingToType.String, 100, ' ')]
        public string Term_en { get; set; }

        [XmlCustomAttribute(5, PadAccordingToType.String, 100, ' ')]
        public decimal? CurrentBalance { get; set; }

        [XmlCustomAttribute(6, PadAccordingToType.String, 100, ' ')]
        public string RenewalDate { get; set; }

        [XmlCustomAttribute(7, PadAccordingToType.String, 100, ' ')]
        public string CdpName { get; set; }

        [XmlCustomAttribute(8, PadAccordingToType.String, 100, ' ')]
        public string CdpName_en { get; set; }

        [XmlCustomAttribute(9, PadAccordingToType.String, 100, ' ')]
        public string CdpNumber { get; set; }

        [XmlCustomAttribute(10, PadAccordingToType.String, 100, ' ')]
        public string InterestEarned { get; set; }

        [XmlCustomAttribute(11, PadAccordingToType.String, 100, ' ')]
        public string InterestPaid { get; set; }

        [XmlCustomAttribute(12, PadAccordingToType.String, 100, ' ')]
        public string InterestPayingAccount { get; set; }

        [XmlCustomAttribute(13, PadAccordingToType.String, 100, ' ')]
        public string InterestPayingAccount_en { get; set; }

        [XmlCustomAttribute(14, PadAccordingToType.String, 100, ' ')]
        public string StartDate { get; set; }

        [XmlCustomAttribute(15, PadAccordingToType.String, 100, ' ')]
        public string ExpirationDate { get; set; }
    }
}
