using AutoMapper;
using Grand.Infrastructure.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Domain;
using Widgets.RepresentationRequest.Models;

namespace Widgets.RepresentationRequest.Infrastructure.Mapper
{
    public class RepresentationRequestMapperConfiguration : Profile, IAutoMapperProfile
    {
        public RepresentationRequestMapperConfiguration()
        {
            CreateMap<RepresentationRequestModel, RepresentationRequestDomain>()
                .ForMember(dest => dest.LimitedToStores, mo => mo.MapFrom(x => x.Stores != null && x.Stores.Any()))
                .ForMember(dest => dest.Locales, mo => mo.Ignore());

            CreateMap<RepresentationRequestDomain, RepresentationRequestModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore());

            CreateMap<RepresentationRequestInListModel, RepresentationRequestDomain>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.Stores, mo => mo.Ignore());

            CreateMap<RepresentationRequestDomain, RepresentationRequestInListModel>()
                .ForMember(dest => dest.UserFields, mo => mo.Ignore());
        }
        public int Order {
            get { return 0; }
        }
    }
}
