using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Models.Responses
{
    public class DetailPrestamosResponse
    {
        [DisplayName("DetailLoanResponse")]
        public List<DetailPrestamosDataResponse> Result { get; set; } =
            new List<DetailPrestamosDataResponse>();
    }

    [DisplayName("DetailLoanDataResponse")]
    public class DetailPrestamosDataResponse
    {
        public int? Cuota { get; set; }

        [Description("Balance a la fecha")]
        public decimal? Balance { get; set; }

        [Description("Proximo Pago")]
        public string ProximoPago { get; set; }

        [Description("Vence")]
        public string FechaExpiracion { get; set; }

        [Description("Estado")]
        public string Status
        {
            get { return IsExpirationDate(ProximoPago.ToDateTime()); }
        }

        private static string IsExpirationDate(DateTime expirationDate)
        {
            return expirationDate >= DateTime.Now ? "Al dia" : "Atraso";
        }
    }

    public class DetailPrestamosResultXMLResponse
    {
        [XmlCustomAttribute(0, PadAccordingToType.String, 100, ' ')]
        public string ProductBankIdentifier { get; set; }

        [XmlCustomAttribute(2, PadAccordingToType.String, 100, ' ')]
        public string BackendLoandType { get; set; }

        [XmlCustomAttribute(3, PadAccordingToType.String, 100, ' ')]
        public string BackendLoandType_en { get; set; }

        [XmlCustomAttribute(4, PadAccordingToType.String, 100, ' ')]
        public int? NextFeeNumber { get; set; }

        [XmlCustomAttribute(4, PadAccordingToType.String, 100, ' ')]
        public string NextFee_DueDate { get; set; }

        [XmlCustomAttribute(5, PadAccordingToType.String, 100, ' ')]
        public string PreviousFee_Payday { get; set; }

        [XmlCustomAttribute(6, PadAccordingToType.String, 100, ' ')]
        public string OutlayDate { get; set; }

        [XmlCustomAttribute(7, PadAccordingToType.String, 100, ' ')]
        public string ExpirationDate { get; set; }

        [XmlCustomAttribute(8, PadAccordingToType.String, 100, ' ')]
        public string CancellationDate { get; set; }

        [XmlCustomAttribute(9, PadAccordingToType.String, 100, ' ')]
        public string LastPaymentPrincipal { get; set; }

        [XmlCustomAttribute(10, PadAccordingToType.String, 100, ' ')]
        public string LastPaymentInterest { get; set; }

        [XmlCustomAttribute(11, PadAccordingToType.String, 100, ' ')]
        public string CurrentRate { get; set; }

        [XmlCustomAttribute(12, PadAccordingToType.String, 100, ' ')]
        public string PendingInterest { get; set; }

        [XmlCustomAttribute(13, PadAccordingToType.String, 100, ' ')]
        public string AmountOutlay { get; set; }

        [XmlCustomAttribute(14, PadAccordingToType.String, 100, ' ')]
        public string AmontPaid { get; set; }

        [XmlCustomAttribute(15, PadAccordingToType.String, 100, ' ')]
        public decimal? ActualBalance { get; set; }

        [XmlCustomAttribute(16, PadAccordingToType.String, 100, ' ')]
        public decimal? CancellationBalance { get; set; }

        [XmlCustomAttribute(17, PadAccordingToType.String, 100, ' ')]
        public string PaidFees { get; set; }
    }

    public class DetailPrestamosXMLResponse
    {
        [XmlElement("Row")]
        public List<DetailPrestamosResultXMLResponse> Details { get; set; }
    }
}
