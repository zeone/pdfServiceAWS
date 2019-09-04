using System;
using System.Text;

namespace PDFServiceAWS.Dto
{
    public class LetterFieldsDto
    {
        public int FamilyID { get; set; }

        public string Name { get; set; }

        public string Salutation { get; set; }

        public string Label { get; set; }

        public int TransType { get; set; }

        public decimal AmountDec { get; set; }
        public string Amount { get; set; }

        public string PaymentMethod { get; set; }

        public string CheckNo { get; set; }

        public int? ReceiptNo { get; set; }

        public string HonorMemory { get; set; }

        public string TodayDate { get; set; }

        public string TodayLongDate { get; set; }

        public string TodayHebrewDate { get; set; }

        public string CompanyName { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        public string AddressBlock
        {
            get
            {
                StringBuilder addrBlock = new StringBuilder();
                addrBlock.Append(Label);
                if (!string.IsNullOrEmpty(CompanyName))
                    addrBlock.Append($"\n{CompanyName}");
                addrBlock.Append($"\n{Address}");
                if (!string.IsNullOrEmpty(Address2))
                    addrBlock.Append($"\n{Address2}");
                if (!string.IsNullOrEmpty(City) || !string.IsNullOrEmpty(State) ||
                    string.IsNullOrEmpty(Zip)) addrBlock.Append("\n");
                if (!string.IsNullOrEmpty(City)) addrBlock.Append($"{City}, ");
                if (!string.IsNullOrEmpty(State)) addrBlock.Append($"{State} ");
                if (!string.IsNullOrEmpty(Zip)) addrBlock.Append($"{Zip} ");
                if (!string.IsNullOrEmpty(Country)) addrBlock.Append($"\n{Country}");

                return addrBlock.ToString();
            }
        }



        public DateTime DonationDateTime { get; set; }

        public string DonationDate { get; set; }

        public string DonationLongDate { get; set; }

        public string Solicitor { get; set; }

        public string Department { get; set; }

        public string Category { get; set; }

        public string Subcategory { get; set; }

        public string Class { get; set; }

        public string Subclass { get; set; }
    }
}