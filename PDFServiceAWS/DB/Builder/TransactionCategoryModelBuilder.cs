using System.Data;
using System.Reflection;
using Newtonsoft.Json;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB.Builder
{
    public class TransactionCategoryModelBuilder : BaseModelBuilder<CategoryDto>
    {
        protected override CategoryDto BuildSingleModel(IDataReader dbDataReader, PropertyInfo[] properties)
        {
            CategoryDto model;
            var xmlString = dbDataReader.ReadColumn<string>("Subcategories");
            if (xmlString != null)
            {
                //var serializer = new XmlSerializer(typeof(CategoryDto), new XmlRootAttribute("Subcategories"));
                //using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(xmlString)))
                //    model = serializer.Deserialize(stream) as CategoryDto;

                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(xmlString);
                //var subCategories = doc.SelectNodes("Subcategories/Subcategory");
                //if (subCategories != null && subCategories.Count > 0)
                //{
                //    foreach (XmlNode subcat in subCategories)
                //    {
                //        XmlNodeList listItems = subcat.SelectNodes("Items");
                //        if (listItems != null && listItems.Count > 0)
                //        {
                //            SubcategoryDto subModel;
                //            var serializerItem = new XmlSerializer(typeof(SubcategoryDto), new XmlRootAttribute("Items"));
                //            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(listItems[0].InnerXml)))
                //                subModel = serializerItem.Deserialize(stream) as SubcategoryDto;

                //            var sub = model.Subcategories.FirstOrDefault(x => x.SubcategoryId == subModel.Items[0].SubcategoryId);
                //            sub.Items = new ItemDto[subModel.Items.Length];
                //            subModel.Items.CopyTo(sub.Items, 0);
                //        }
                //    }
                //}
                model = new CategoryDto();
                model.Subcategories = JsonConvert.DeserializeObject<SubcategoryDto[]>(xmlString);
            }
            else
                model = new CategoryDto();

            model.CategoryId = dbDataReader.ReadColumn<int>("CategoryID");
            model.Category = dbDataReader.ReadColumn<string>("Category");
            model.IsActive = dbDataReader.ReadColumn<bool>("IsActive");
            model.IsReceipt = dbDataReader.ReadColumn<bool>("IsReceipt");
            model.DefaultDepartmentId = dbDataReader.ReadColumn<int?>("DefaultDepartmentID");
            model.DefaultLetterId = dbDataReader.ReadColumn<int?>("DefaultLetterID");

            return model;
        }
    }
}