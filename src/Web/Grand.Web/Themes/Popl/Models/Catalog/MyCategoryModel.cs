using Grand.Infrastructure.Models;
using Grand.Web.Admin.Models.Catalog;
using Grand.Web.Admin.Models.Common;
using Grand.Web.Models.Catalog;
using System.Collections.Generic;
using static Grand.Web.Models.Catalog.CategoryModel;

namespace Grand.Web.Themes.Popl.Models.Catalog
{

    public partial class MyCategoryModel : BaseEntityModel
    {
        public MyCategoryModel()
        {
            PictureModel = new PictureModel();
            FeaturedProducts = new List<MyProductOverviewModel>();
            Products = new List<MyProductOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
            SubCategories = new List<SubCategoryModel>();
        }
        public string ParentCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BottomDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public string Flag { get; set; }
        public string FlagStyle { get; set; }
        public string Icon { get; set; }
        public PictureModel PictureModel { get; set; }
        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<SubCategoryModel> SubCategories { get; set; }
        public IList<MyProductOverviewModel> FeaturedProducts { get; set; }
        public IList<MyProductOverviewModel> Products { get; set; }
        
    }
}
