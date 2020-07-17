using System.Xml.Serialization;

namespace TransactionApplication.BL.Models
{
    [XmlRoot(ElementName = "PaymentDetails")]
    public class PaymentDetails
    {
        [XmlElement(ElementName = "Amount")] public string Amount { get; set; }

        [XmlElement(ElementName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
    }
}
