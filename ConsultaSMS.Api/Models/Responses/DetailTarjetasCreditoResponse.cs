using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Models.Responses
{
    [Serializable]
    public class DetailTarjetasCreditoResponse
    {
        [DisplayName("DetailTarjetasResponse")]
        public List<DetailTarjetasCreditoDataResponse> Result { get; set; } =
            new List<DetailTarjetasCreditoDataResponse>();
    }

    public class DetailTarjetasCreditoDataResponse
    {
        [Description("Disponible")]
        public decimal? BalanceDisponible { get; set; }

        [Description("Disponible USD")]
        public decimal? BalanceDisponibleUSD { get; set; }

        [Description("Balance a la fecha")]
        public decimal? BalanceFecha { get; set; }

        [Description("Balance a la fecha USD")]
        public decimal? BalanceFechaUSD { get; set; }

        [Description("Balance al corte")]
        public decimal? BalanceCorte { get; set; }

        [Description("Balance al corte USD")]
        public decimal? BalanceCorteUSD { get; set; }

        [Description("Pago minimo")]
        public decimal? PagoMinimo { get; set; }

        [Description("Pago minimo USD")]
        public decimal? PagoMinimoUSD { get; set; }

        [Description("Vence")]
        public string FechaExpiracion { get; set; }
    }

    [Serializable]
    [XmlRoot("GetCreditCardDetailsResult")]
    public class DetailTarjetasCreditoXMLResponse
    {
        [XmlElement("Row")]
        public List<DetailDataTarjetasCreditoXMLResponse> Details { get; set; } =
            new List<DetailDataTarjetasCreditoXMLResponse>();
    }

    public class DetailDataTarjetasCreditoXMLResponse
    {
        [XmlCustomAttribute(0, PadAccordingToType.String, 100, ' ')]
        public string CreditCardBankIdentifier { get; set; }

        [XmlCustomAttribute(1, PadAccordingToType.String, 100, ' ')]
        public string ClosedDate { get; set; }

        [XmlCustomAttribute(2, PadAccordingToType.String, 100, ' ')]
        public string DueDate { get; set; }

        [XmlCustomAttribute(3, PadAccordingToType.String, 100, ' ')]
        public decimal? LocalCurrencyMinPayment { get; set; }

        [XmlCustomAttribute(4, PadAccordingToType.String, 100, ' ')]
        public string LocalCurrencyMaxPayment { get; set; }

        [XmlCustomAttribute(5, PadAccordingToType.String, 100, ' ')]
        public string LocalCurrencyOverduePayment { get; set; }

        [XmlCustomAttribute(6, PadAccordingToType.String, 100, ' ')]
        public decimal? DollarMinPayment { get; set; }

        [XmlCustomAttribute(7, PadAccordingToType.String, 100, ' ')]
        public string DollarMaxPayment { get; set; }

        [XmlCustomAttribute(8, PadAccordingToType.String, 100, ' ')]
        public string DollarOverduePayment { get; set; }

        [XmlCustomAttribute(9, PadAccordingToType.String, 100, ' ')]
        public decimal? LocalCurrencyCloseBalance { get; set; }

        [XmlCustomAttribute(10, PadAccordingToType.String, 100, ' ')]
        public decimal? DollarCloseBalance { get; set; }

        [XmlCustomAttribute(11, PadAccordingToType.String, 100, ' ')]
        public decimal? LocalCurrencyInitialBalance { get; set; }

        [XmlCustomAttribute(12, PadAccordingToType.String, 100, ' ')]
        public decimal? DollarInitialBalance { get; set; }

        [XmlCustomAttribute(13, PadAccordingToType.String, 100, ' ')]
        public string LocalCreditLimit { get; set; }

        [XmlCustomAttribute(14, PadAccordingToType.String, 100, ' ')]
        public string DollarCreditLimit { get; set; }

        [XmlCustomAttribute(15, PadAccordingToType.String, 100, ' ')]
        public string PoIntsLastBalance { get; set; }

        [XmlCustomAttribute(16, PadAccordingToType.String, 100, ' ')]
        public string PoIntsEarnedThisMonth { get; set; }

        [XmlCustomAttribute(17, PadAccordingToType.String, 100, ' ')]
        public string PoIntsUsedThisMonth { get; set; }

        [XmlCustomAttribute(18, PadAccordingToType.String, 100, ' ')]
        public string PoIntsCurrentBalance { get; set; }

        [XmlCustomAttribute(19, PadAccordingToType.String, 100, ' ')]
        public string PoIntsToExpiryThisMonth { get; set; }

        [XmlCustomAttribute(20, PadAccordingToType.String, 100, ' ')]
        public string LocalAvailableForWithdrawals { get; set; }

        [XmlCustomAttribute(21, PadAccordingToType.String, 100, ' ')]
        public string DolarAvailableForWithdrawals { get; set; }

        [XmlCustomAttribute(22, PadAccordingToType.String, 100, ' ')]
        public decimal? LocalAvailableForShopping { get; set; }

        [XmlCustomAttribute(23, PadAccordingToType.String, 100, ' ')]
        public decimal? DolarAvailableForShopping { get; set; }

        [XmlCustomAttribute(24, PadAccordingToType.String, 100, ' ')]
        public decimal? LocalTotalPayment { get; set; }

        [XmlCustomAttribute(25, PadAccordingToType.String, 100, ' ')]
        public string DolarTotalPayment { get; set; }

        [XmlCustomAttribute(26, PadAccordingToType.String, 100, ' ')]
        public string LocalDebitsAfterClose { get; set; }

        [XmlCustomAttribute(27, PadAccordingToType.String, 100, ' ')]
        public string DolarDebitsAfterClose { get; set; }

        [XmlCustomAttribute(28, PadAccordingToType.String, 100, ' ')]
        public string LocalCreditsAfterClose { get; set; }

        [XmlCustomAttribute(29, PadAccordingToType.String, 100, ' ')]
        public string DolarCreditsAfterClose { get; set; }

        [XmlCustomAttribute(30, PadAccordingToType.String, 100, ' ')]
        public string LocalTransactionsIntransit { get; set; }

        [XmlCustomAttribute(31, PadAccordingToType.String, 100, ' ')]
        public string DolarTransactionsIntransit { get; set; }

        [XmlCustomAttribute(32, PadAccordingToType.String, 100, ' ')]
        public string MaskedCreditCardBankIdentifier { get; set; }
    }
}
