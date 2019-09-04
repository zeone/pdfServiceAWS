using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB.Builder
{
    public class TransactionPaymentModelBuilder : BaseModelBuilder<PaymentDto>
    {
        protected override PaymentDto BuildSingleModel(IDataReader dbDataReader, PropertyInfo[] properties)
        {
            PaymentDto model;
            var xmlString = dbDataReader.ReadColumn<string>("PaymentDetails");
            if (xmlString != null)
            {
                var serializer = new XmlSerializer(typeof(PaymentDto),
                    new XmlRootAttribute("PaymentDetails"));
                using (var stream =
                    new MemoryStream(Encoding.Unicode.GetBytes(xmlString)))
                    model = serializer.Deserialize(stream) as PaymentDto;
            }
            else
                model = new PaymentDto();

            model.TransactionId = dbDataReader.ReadColumn<int>("TransactionId");
            model.Family = dbDataReader.ReadColumn<string>("Family");
            model.FamilyId = dbDataReader.ReadColumn<int>("FamilyId");
            model.MemberId = dbDataReader.ReadColumn<int?>("MemberId");
            model.AddressId = dbDataReader.ReadColumn<int>("AddressId");
            model.Date = dbDataReader.ReadColumn<DateTime>("Date");
            model.Amount = dbDataReader.ReadColumn<decimal>("Amount");
            model.CheckNo = dbDataReader.ReadColumn<string>("CheckNo");
            model.PayId = dbDataReader.ReadColumn<int?>("PayId");
            model.AuthNumber = dbDataReader.ReadColumn<string>("AuthNumber");
            model.AuthTransactionId = dbDataReader.ReadColumn<string>("AuthTransactionId");
            model.PaymentMethodId = dbDataReader.ReadColumn<byte?>("PaymentMethodId");
            model.PaymentMethod = dbDataReader.ReadColumn<string>("PaymentMethod");
            model.IsReceipt = dbDataReader.ReadColumn<bool>("IsReceipt");
            model.ReceiptNo = dbDataReader.ReadColumn<int?>("ReceiptNo");
            model.ReceiptSent = dbDataReader.ReadColumn<bool>("ReceiptSent");
            model.LetterId = dbDataReader.ReadColumn<short?>("LetterId");
            model.SolicitorId = dbDataReader.ReadColumn<int?>("SolicitorId");
            model.DepartmentId = dbDataReader.ReadColumn<int>("DepartmentId");
            model.MailingId = dbDataReader.ReadColumn<int?>("MailingId");
            model.HonorMemory = dbDataReader.ReadColumn<string>("HonorMemory");
            model.Category = dbDataReader.ReadColumn<string>("Category");
            model.Subcategory = dbDataReader.ReadColumn<string>("Subcategory");
            model.Class = dbDataReader.ReadColumn<string>("Class");
            model.Subclass = dbDataReader.ReadColumn<string>("Subclass");
            model.Note = dbDataReader.ReadColumn<string>("Note");
            model.SubmissionId = dbDataReader.ReadColumn<int?>("SubmissionId");
            return model;
        }
    }
}