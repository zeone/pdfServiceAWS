using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace PDFServiceAWS.Services.Implementation
{
    public class XMLService : IXMLService
    {
        public string[] GetElements(string nodeName, string xml)
        {
            XmlNodeList elements = GetXMLElements(nodeName, xml);
            int count = elements.Count;
            string[] array = new string[count];
            for (int i = 0; i < count; i++)
                array[i] = elements[i].InnerText;

            return array;
        }

        public XmlNodeList GetXMLElements(string nodeName, string xml)
        {
            var xdoc = new XmlDocument();
            xdoc.Load(new StringReader(xml));
            XmlNodeList elements = xdoc.GetElementsByTagName(nodeName);
            return elements;
        }

        public XmlNodeList GetCategory(string xml)
        {
            return GetXMLElements("Categories", xml);
        }

        public XmlNodeList GetSubcategory(string xml)
        {
            return GetXMLElements("Subcategories", xml);
        }

        public XmlNodeList GetClass(string xml)
        {
            return GetXMLElements("Classes", xml);
        }

        public XmlNodeList GetClassId(string xml)
        {
            return GetXMLElements("ClassID", xml);
        }
        public XmlNodeList GetSubclass(string xml)
        {
            return GetXMLElements("Subclasses", xml);
        }
        public XmlNodeList GetSubclassId(string xml)
        {
            return GetXMLElements("SubclassID", xml);
        }

        public XmlNodeList GetAmount(string xml)
        {
            return GetXMLElements("Amount", xml);
        }

        public XmlNodeList GetTransactionDetailsIds(string xml)
        {
            return GetXMLElements("TransactionDetailID", xml);
        }
        public XmlNodeList GetBillIDs(string xml)
        {
            return GetXMLElements("BillID", xml);
        }

        public XmlNodeList GetCategoryID(string xml)
        {
            return GetXMLElements("CategoryID", xml);
        }
        public XmlNodeList GetSubcategoryID(string xml)
        {
            return GetXMLElements("SubcategoryID", xml);
        }

        public XmlNodeList GetDateDue(string xml)
        {
            return GetXMLElements("DateDue", xml);
        }
        public XmlNodeList GetQuantity(string xml)
        {
            return GetXMLElements("Quantity", xml);
        }
        public XmlNodeList GetUnitPrice(string xml)
        {
            return GetXMLElements("UnitPrice", xml);
        }
        public XmlNodeList GetCardCharged(string xml)
        {
            return GetXMLElements("CardCharged", xml);
        }
    }
}