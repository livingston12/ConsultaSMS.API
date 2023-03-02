using System.Xml.Serialization;

namespace WebConsultaSMS.Models.Responses
{
    public class Response<T>
    {
        [XmlElement("Data")]
        public T Data { get; set; }
        public bool hasError { get; set; }
        public string MessageError { get; set; }
        public string StatusCode { get; set; }
    }
}
