using System;
using System.Linq;
using System.Xml.Linq;

namespace WebConsultaSMS.Models.Requests
{
    public class XmlMDRequest
    {
        private XDocument Request { get; set; }

        public XmlMDRequest(string channel, string trnCode)
        {
            _TrnCode = trnCode;
            _channel = channel;
            Request = InitialiceHeader(trnCode, channel);
        }

        private XDocument InitialiceHeader(string trnCode, string channel)
        {
            var name = "name";
            var value = "Value";
            return new XDocument(
                new XElement(
                    "ExtremeMsg",
                    new XElement(
                        "Header",
                        new XElement(
                            "HField",
                            new XAttribute(name, "TraceId"),
                            new XAttribute(value, TraceId)
                        ),
                        new XElement(
                            "HField",
                            new XAttribute(name, "TranType"),
                            new XAttribute(value, TranType)
                        ),
                        new XElement(
                            "HField",
                            new XAttribute(name, "RealDate"),
                            new XAttribute(value, RealDate)
                        ),
                        new XElement(
                            "HField",
                            new XAttribute(name, "BussDate"),
                            new XAttribute(value, BussDate)
                        ),
                        new XElement(
                            "HField",
                            new XAttribute(name, "Company"),
                            new XAttribute(value, Company)
                        ),
                        new XElement(
                            "HField",
                            new XAttribute(name, "Canal"),
                            new XAttribute(value, channel)
                        ),
                        new XElement(
                            "HField",
                            new XAttribute(name, "TrnCode"),
                            new XAttribute(value, trnCode)
                        ),
                        new XElement(
                            "HField",
                            new XAttribute(name, "SerialTrn"),
                            new XAttribute(value, SerialTrn)
                        )
                    )
                )
            );
        }

        public string TraceId { get; set; } = "0";
        public string TranType { get; set; } = "1";
        public string RealDate { get; set; } = DateTime.Now.ToString("yyyyMMdd HH:Mm:ss");
        public string BussDate { get; set; } = DateTime.Now.ToString("yyyyMMdd");
        public string Company { get; set; } = "1";
        private string _channel;
        public string Channel
        {
            get { return _channel; }
        }
        private string _TrnCode;
        public string TrnCode
        {
            get { return _TrnCode; }
        }
        public string SerialTrn { get; set; } = "0";

        public void AddInitialDataToChildData(string data, string name = "payload")
        {
            var insertData = new XElement(
                "Data",
                new XElement(
                    "DField",
                    new XAttribute("Type", "P"),
                    new XAttribute("name", name),
                    new XAttribute("Value", data)
                )
            );
            Request.Descendants("Data").FirstOrDefault()?.Remove();
            var header = Request.Descendants("Header").FirstOrDefault();
            header.AddAfterSelf(insertData);
        }

        public void AddDataToChildDataAnother(string data, string name = "payload")
        {
            var insertData = new XElement(
                "DField",
                new XAttribute("Type", "P"),
                new XAttribute("name", name),
                new XAttribute("Value", data)
            );
            var newField = this.Request.Descendants("Data").Last();
            newField.Add(insertData);
        }

        public string GetRequest() => Request.ToString();
    }
}
