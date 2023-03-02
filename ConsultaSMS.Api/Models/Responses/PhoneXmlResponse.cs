using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Models.Responses
{
    public class PhoneXmlResponse
    {
        [XmlCustomAttribute(0, PadAccordingToType.Int, 20, ' ')]
        public string ClientId { get; set; }
        [XmlCustomAttribute(1, PadAccordingToType.String, 4, ' ')]
        public string Code { get; set; }
        [XmlCustomAttribute(2, PadAccordingToType.String, 500, ' ')]
        public string Message { get; set; }
    }
}