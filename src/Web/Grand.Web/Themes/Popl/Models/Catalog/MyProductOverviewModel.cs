using Grand.Web.Models.Catalog;
using Grand.Web.Models.Media;

namespace Grand.Web.Themes.Popl.Models.Catalog
{
    public class MyProductOverviewModel: ProductOverviewModel
    {
        public new IList<MyProductAttributeModel> ProductAttributeModels { get; set; }

        public class MyProductAttributeModel:ProductAttributeModel
        {
            public MyProductAttributeModel()
            {
                Values = new List<MyProductAttributeValueModel>();
            }
            public new IList<MyProductAttributeValueModel> Values { get; set; }

            public string Id { get; set; }
        }

        public class MyProductAttributeValueModel : ProductAttributeValueModel
        {
            public MyProductAttributeValueModel()
            {
                ImageSquaresPictureModel = new PictureModel();
                PictureModel = new PictureModel();
            }
            public bool IsPreSelected { get; set; }
            public string Id { get; set; }
        }
    }
}
