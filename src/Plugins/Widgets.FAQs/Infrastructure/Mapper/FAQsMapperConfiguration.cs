using AutoMapper;
using Grand.Infrastructure.Mapper;
using Widgets.FAQs.Domain;
using Widgets.FAQs.Models;

namespace Widgets.FAQs.Infrastructure.Mapper
{
    public class FAQsMapperConfiguration : Profile, IAutoMapperProfile
    {
        public FAQsMapperConfiguration()
        {
            CreateMap<FaqModel, FAQ>()
                .ForMember(dest => dest.LimitedToStores, mo => mo.MapFrom(x => x.Stores != null && x.Stores.Any()))
                .ForMember(dest => dest.Locales, mo => mo.Ignore());

            CreateMap<FAQ, FaqModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore());

            CreateMap<FaqInListModel, FAQ>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.Stores, mo => mo.Ignore());

            CreateMap<FAQ, FaqInListModel>()
                .ForMember(dest => dest.UserFields, mo => mo.Ignore());
        }
        public int Order
        {
            get { return 0; }
        }
    }
}
