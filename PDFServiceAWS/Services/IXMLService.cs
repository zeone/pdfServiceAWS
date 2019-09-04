using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace PDFServiceAWS.Services
{
    public interface IXMLService
    {
        string[] GetElements(string nodeName, string xml);
        XmlNodeList GetXMLElements(string nodeName, string xml);

        XmlNodeList GetCategory(string xml);

        XmlNodeList GetSubcategory(string xml);

        XmlNodeList GetClass(string xml);
        XmlNodeList GetClassId(string xml);
        XmlNodeList GetSubclass(string xml);
        XmlNodeList GetSubclassId(string xml);
        XmlNodeList GetAmount(string xml);
        XmlNodeList GetTransactionDetailsIds(string xml);
        XmlNodeList GetBillIDs(string xml);
        XmlNodeList GetCategoryID(string xml);
        XmlNodeList GetSubcategoryID(string xml);
        XmlNodeList GetDateDue(string xml);
        XmlNodeList GetQuantity(string xml);
        XmlNodeList GetUnitPrice(string xml);
        XmlNodeList GetCardCharged(string xml);
    }

  

}