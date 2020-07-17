using System.Xml.Serialization;

namespace TransactionApplication.BL.Models
{
    [XmlRoot(ElementName = "Transaction")]
    public class TransactionItem
    {
        [XmlElement(ElementName = "TransactionDate")]
        public string TransactionDate { get; set; }

        [XmlElement(ElementName = "PaymentDetails")]
        public PaymentDetails PaymentDetails { get; set; }

        [XmlElement(ElementName = "Status")] public string Status { get; set; }
        [XmlAttribute(AttributeName = "id")] public string PublicId { get; set; }
    }
}
