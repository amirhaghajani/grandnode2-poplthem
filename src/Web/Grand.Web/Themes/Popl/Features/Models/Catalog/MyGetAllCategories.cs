using Grand.Domain.Catalog;
using Grand.Domain.Customers;
using Grand.Domain.Directory;
using Grand.Domain.Localization;
using Grand.Domain.Stores;
using Grand.Web.Models.Catalog;
using Grand.Web.Themes.Popl.Models.Catalog;
using MediatR;

namespace Grand.Web.Themes.Popl.Features.Models.Catalog
{
    public class MyGetAllCategories : IRequest<MyCategoryModel>
    {
        public Customer Customer { get; set; }
        public Store Store { get; set; }
        public Language Language { get; set; }
        public Currency Currency { get; set; }
        public Category Category { get; set; }
        public CatalogPagingFilteringModel Command { get; set; }
    }
}
