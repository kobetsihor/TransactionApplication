using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransactionApplication.BL.Models
{
    [XmlRoot(ElementName = "Transactions")]
    public class XmlTransactionsDto
    {
        [XmlElement(ElementName = "Transaction")]
        public List<TransactionItem> Transactions { get; set; }
    }
}
