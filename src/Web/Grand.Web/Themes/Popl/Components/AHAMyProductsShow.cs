using Grand.Business.Catalog.Interfaces.Products;
using Grand.Domain.Catalog;
using Grand.Infrastructure;
using Grand.Infrastructure.Caching;
using Grand.Web.Common.Components;
using Grand.Web.Features.Models.Products;
using Grand.Web.Events.Cache;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.ViewComponents
{
    public class AHAMyProductsShow : BaseViewComponent
    {
        private readonly IProductService _productService;
        private readonly IMediator _mediator;
        private readonly ICacheBase _cacheBase;
        private readonly IWorkContext _workContext;
        private readonly CatalogSettings _catalogSettings;

        public AHAMyProductsShow(
            IProductService productService,
            IMediator mediator,
            ICacheBase cacheBase,
            IWorkContext workContext,
            CatalogSettings catalogSettings)
        {
            _productService = productService;
            _mediator = mediator;
            _cacheBase = cacheBase;
            _workContext = workContext;
            _catalogSettings = catalogSettings;
        }
        public async Task<IViewComponentResult> InvokeAsync(string type, int? productThumbPictureSize)
        {
            var productsIds = await _cacheBase.GetAsync(CacheKeyConst.HOMEPAGE_PRODUCTS_MODEL_KEY,
                    async () => await _productService.GetAllProductsDisplayedOnHomePage());

            var products = await _productService.GetProductsByIds(productsIds.ToArray());

            if (!products.Any())
                return Content("");

            var model = await _mediator.Send(new GetProductOverview() {
                PreparePictureModel = true,
                PreparePriceModel = true,
                PrepareSpecificationAttributes = true,// _catalogSettings.ShowSpecAttributeOnCatalogPages,
                ProductThumbPictureSize = productThumbPictureSize,
                Products = products,
            });

            return View(model);
        }
    }
}
