using Grand.Domain.Catalog;
using Grand.Web.Models.Catalog;
using Grand.Web.Themes.Popl.Models.Catalog;
using MediatR;

namespace Grand.Web.Themes.Popl.Features.Models.Products
{
    public class MyGetProductOverview : IRequest<IEnumerable<MyProductOverviewModel>>
    {
        public IEnumerable<Product> Products { get; set; }
        public bool PreparePriceModel { get; set; } = true;
        public bool PreparePictureModel { get; set; } = true;
        public int? ProductThumbPictureSize { get; set; } = null;
        public bool PrepareSpecificationAttributes { get; set; } = false;
        public bool ForceRedirectionAfterAddingToCart { get; set; } = false;
    }
}
