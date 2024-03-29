﻿using Grand.Business.Catalog.Interfaces.Categories;
using Grand.Business.Catalog.Interfaces.Products;
using Grand.Business.Catalog.Queries.Handlers;
using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Storage.Interfaces;
using Grand.Domain;
using Grand.Domain.Catalog;
using Grand.Domain.Customers;
using Grand.Domain.Media;
using Grand.Infrastructure.Caching;
using Grand.Web.Events.Cache;
using Grand.Web.Extensions;
using Grand.Web.Features.Models.Catalog;
using Grand.Web.Features.Models.Products;
using Grand.Web.Models.Catalog;
using Grand.Web.Models.Media;
using Grand.Web.Themes.Popl.Features.Models.Catalog;
using Grand.Web.Themes.Popl.Features.Models.Products;
using Grand.Web.Themes.Popl.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Grand.Web.Themes.Popl.Features.Handels.Catalog
{
    public class MyGetAllCategoriesHandler : IRequestHandler<MyGetAllCategories, MyCategoryModel>
    {
        private readonly IMediator _mediator;
        private readonly ICacheBase _cacheBase;
        private readonly ICategoryService _categoryService;
        private readonly IPictureService _pictureService;
        private readonly ITranslationService _translationService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CatalogSettings _catalogSettings;
        private readonly MediaSettings _mediaSettings;

        public MyGetAllCategoriesHandler(
            IMediator mediator,
            ICacheBase cacheBase,
            ICategoryService categoryService,
            IPictureService pictureService,
            ITranslationService translationService,
            ISpecificationAttributeService specificationAttributeService,
            IHttpContextAccessor httpContextAccessor,
            CatalogSettings catalogSettings,
            MediaSettings mediaSettings)
        {
            _mediator = mediator;
            _cacheBase = cacheBase;
            _categoryService = categoryService;
            _pictureService = pictureService;
            _translationService = translationService;
            _specificationAttributeService = specificationAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _catalogSettings = catalogSettings;
            _mediaSettings = mediaSettings;
        }

        public async Task<MyCategoryModel> Handle(MyGetAllCategories request, CancellationToken cancellationToken)
        {
            CategoryModel model = request.Category.ToModel(request.Language);
            var customer = request.Customer;
            var storeId = request.Store.Id;
            var languageId = request.Language.Id;
            var currency = request.Currency;

            if (request.Command != null && request.Command.OrderBy == null && request.Category.DefaultSort >= 0)
                request.Command.OrderBy = request.Category.DefaultSort;

            //view/sorting/page size
            var options = await _mediator.Send(new GetViewSortSizeOptions()
            {
                Command = request.Command,
                PagingFilteringModel = request.Command,
                Language = request.Language,
                AllowCustomersToSelectPageSize = request.Category.AllowCustomersToSelectPageSize,
                PageSizeOptions = request.Category.PageSizeOptions,
                PageSize = request.Category.PageSize
            });
            model.PagingFilteringContext = options.command;

            //price ranges
            

            //subcategories
            var subCategories = new List<CategoryModel.SubCategoryModel>();
            
            var categoryIds = new List<string>();

            if(request.Category.SeName!= "shop-all") categoryIds.Add(request.Category.Id);

            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(await _mediator.Send(new GetChildCategoryIds() { ParentCategoryId = request.Category.Id, Customer = request.Customer, Store = request.Store }));
            }
            //products
            IList<string> alreadyFilteredSpecOptionIds = await model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(
                _httpContextAccessor.HttpContext.Request.Query, _specificationAttributeService);
            var products = (await _mediator.Send(new GetSearchProductsQuery()
            {
                LoadFilterableSpecificationAttributeOptionIds = !_catalogSettings.IgnoreFilterableSpecAttributeOption,
                CategoryIds = categoryIds,
                Customer = request.Customer,
                StoreId = storeId,
                VisibleIndividuallyOnly = true,
                FeaturedProducts = _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                FilteredSpecs = alreadyFilteredSpecOptionIds,
                OrderBy = (ProductSortingEnum)request.Command.OrderBy,
                PageIndex = request.Command.PageNumber - 1,
                PageSize = request.Command.PageSize
            }));

            List<MyProductOverviewModel> myProducts = (await _mediator.Send(new MyGetProductOverview()
            {
                PrepareSpecificationAttributes = _catalogSettings.ShowSpecAttributeOnCatalogPages,
                Products = products.products,
            })).ToList();

            model.PagingFilteringContext.LoadPagedList(products.products);

            //specs
            await model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                products.filterableSpecificationAttributeOptionIds,
                _specificationAttributeService, _httpContextAccessor.HttpContext.Request.GetDisplayUrl(), request.Language.Id);


            MyCategoryModel answer = new MyCategoryModel();
            answer.Products = myProducts;
            answer.SeName = model.SeName;

            return answer;
        }
    }
}
