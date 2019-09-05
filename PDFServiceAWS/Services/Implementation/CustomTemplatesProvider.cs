using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services.Implementation
{
    public sealed class CustomTemplatesProvider
    {
    

        public CustomTemplatesProvider()
        {
            
        }
        


        /// <summary>
        /// Replace {{field placeholders}} with smart template 
        /// </summary>
        /// <param name="html">Template letter that need to be parsed</param>
        /// <param name="fields">Data that will be include into text</param>
        /// <returns>Sring for sending</returns>
        public string ReplaceFieldsTemplate(string html, LetterFieldsDto fields, string country = null)
        {
            string smartString = html.Replace("{{", "{").Replace("}}", "}");
            if (smartString.Contains("{TransactionList}"))
                smartString = smartString.Replace("{TransactionList}", "<<TransactionList>>");
            if (smartString.Contains("{TotalAmount}"))
                smartString = smartString.Replace("{TotalAmount}", "<<TotalAmount>>");
            if (smartString.Contains("{AddressBlock}"))
                smartString = FillAddressBlock(smartString, fields, country);

            string resultString = SmartFormat.Smart.Format(smartString, fields);

            return resultString;
        }

        /// <summary>
        /// Replace {{field placeholders}} with smart template 
        /// </summary>
        /// <param name="html">Template letter that need to be parsed</param>
        /// <param name="fields">Data that will be include into text</param>
        /// <returns>Sring for sending</returns>
        public string ReplaceFieldsTemplate(string html, YahrtzeitFieldsDto fields, string country = null)
        {
            string smartString = html.Replace("{{", "{").Replace("}}", "}");
            if (smartString.Contains("{AddressBlock}"))
                smartString = FillAddressBlock(smartString, fields, country);

            string resultString = SmartFormat.Smart.Format(smartString, fields);

            return resultString;
        }

        private string FillAddressBlock(string smartString, YahrtzeitFieldsDto fields, string country = null)
        {
            StringBuilder addrBlock = new StringBuilder();
            addrBlock.Append(fields.Label);
            if (!string.IsNullOrEmpty(fields.CompanyName))
                addrBlock.Append($"\n{fields.CompanyName}");
            addrBlock.Append($"\n{fields.Address}");
            if (!string.IsNullOrEmpty(fields.Address2))
                addrBlock.Append($"\n{fields.Address2}");
            if (!string.IsNullOrEmpty(fields.City) || !string.IsNullOrEmpty(fields.State) ||
                string.IsNullOrEmpty(fields.Zip)) addrBlock.Append("\n");
            if (!string.IsNullOrEmpty(fields.City)) addrBlock.Append($"{fields.City}, ");
            if (!string.IsNullOrEmpty(fields.State)) addrBlock.Append($"{fields.State} ");
            if (!string.IsNullOrEmpty(fields.Zip)) addrBlock.Append($"{fields.Zip} ");
            if (!string.IsNullOrEmpty(fields.Country) && !string.IsNullOrEmpty(country)
                                                    && string.Equals(fields.Country, country, StringComparison.CurrentCultureIgnoreCase))
                addrBlock.Append($"\n{fields.Country} ");
            return smartString.Replace("{AddressBlock}", addrBlock.ToString());
        }

        private string FillAddressBlock(string smartString, LetterFieldsDto fields, string country = null)
        {
            StringBuilder addrBlock = new StringBuilder();
            addrBlock.Append(fields.Label);
            if (!string.IsNullOrEmpty(fields.CompanyName))
                addrBlock.Append($"\n{fields.CompanyName}");
            addrBlock.Append($"\n{fields.Address}");
            if (!string.IsNullOrEmpty(fields.Address2))
                addrBlock.Append($"\n{fields.Address2}");
            if (!string.IsNullOrEmpty(fields.City) || !string.IsNullOrEmpty(fields.State) ||
                string.IsNullOrEmpty(fields.Zip)) addrBlock.Append("\n");
            if (!string.IsNullOrEmpty(fields.City)) addrBlock.Append($"{fields.City}, ");
            if (!string.IsNullOrEmpty(fields.State)) addrBlock.Append($"{fields.State} ");
            if (!string.IsNullOrEmpty(fields.Zip)) addrBlock.Append($"{fields.Zip} ");
            if (!string.IsNullOrEmpty(fields.Country) && !string.IsNullOrEmpty(country)
                                                      && string.Equals(fields.Country, country, StringComparison.CurrentCultureIgnoreCase))
                addrBlock.Append($"\n{fields.Country} ");
            return smartString.Replace("{AddressBlock}", addrBlock.ToString());
        }
       
    }
}