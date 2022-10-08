﻿using Grand.Business.Catalog.Interfaces.Products;
using Grand.Domain.Catalog;
using Grand.Web.Common.Components;
using Grand.Web.Features.Models.Products;
using Grand.Web.Themes.Popl.Features.Models.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Components
{
    public class MyRecommendedProductsViewComponent : BaseViewComponent
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IMediator _mediator;

        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Constructors
        public MyRecommendedProductsViewComponent(
            IProductService productService,
            IMediator mediator,
            CatalogSettings catalogSettings)
        {
            _productService = productService;
            _mediator = mediator;
            _catalogSettings = catalogSettings;
        }
        #endregion

        #region Invoker

        public async Task<IViewComponentResult> InvokeAsync(string productId, int? productThumbPictureSize)
        {
            var productIds = (await _productService.GetProductById(productId)).RecommendedProduct.ToArray();
            if (!productIds.Any())
                return Content("");

            var products = await _productService.GetProductsByIds(productIds);

            if (!products.Any())
                return Content("");

            var model = await _mediator.Send(new MyGetProductOverview() {
                PreparePictureModel = true,
                PreparePriceModel = true,
                PrepareSpecificationAttributes = _catalogSettings.ShowSpecAttributeOnCatalogPages,
                ProductThumbPictureSize = productThumbPictureSize,
                Products = products
            });

            return View(model);
        }

        #endregion
    }
}
