using System.Collections.Generic;
using System.Linq;

namespace PDFServiceAWS.Helpers
{
    public static class NameHelper
    {
        public static string GetMethodName(int? methodId, List<PaymentMethodDto> paymentMethods)
        {
            if (methodId == null || paymentMethods == null || !paymentMethods.Any())
                return string.Empty;
            var result = paymentMethods.FirstOrDefault(e => e.PaymentMethodId == methodId);
            return result == null ? string.Empty : result.Method;
        }

        public static string GetSolicitorName(int? solID, List<SolicitorDto> solicitors)
        {
            if (solID == null || solicitors == null || !solicitors.Any())
                return null;
            var result = solicitors.FirstOrDefault(e => e.SolicitorId == solID);
            return result == null ? null : result.Label;
        }


        public static string GetMailingName(int? mailId, List<MailingDto> mailings)
        {
            if (mailId == null || mailings == null || !mailings.Any())
                return null;
            var result = mailings.FirstOrDefault(e => e.MailingId == mailId);
            return result == null ? null : result.Name;
        }

        public static string GetDepartmentName(int? depId, List<DepartmentDto> departments)
        {
            if (depId == null || departments == null || !departments.Any())
                return null;
            var result = departments.FirstOrDefault(e => e.DepartmentId == depId);
            return result == null ? null : result.Name;
        }

        public static string GetCategoryName(int? catId, List<CategoryDto> categoryTree)
        {
            if (catId == null || categoryTree == null || !categoryTree.Any())
                return null;
            var result = categoryTree.FirstOrDefault(e => e.CategoryId == catId);
            return result == null ? null : result.Category;
        }
        public static string GetSubCategoryName(int? subcatId, List<CategoryDto> categoryTree)
        {
            if (subcatId == null || categoryTree == null || !categoryTree.Any())
                return null;
            //  var result = categoryTree.Select(e => e.Subcategories.FirstOrDefault(t => t.SubcategoryId == subcatId)).FirstOrDefault();
            SubcategoryDto result = null;
            foreach (CategoryDto cat in categoryTree)
            {
                if (cat.Subcategories == null || cat.Subcategories.All(d => d.SubcategoryId != subcatId)) continue;
                result = cat.Subcategories.First(r => r.SubcategoryId == subcatId);
            }
            return result == null ? null : result.Subcategory;
        }
    }
}