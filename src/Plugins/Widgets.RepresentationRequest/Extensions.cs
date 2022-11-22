using Grand.Domain.Localization;
using Grand.Infrastructure.Mapper;
using Grand.Web.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Domain;
using Widgets.RepresentationRequest.Models;

namespace Widgets.RepresentationRequest
{
    public static class Extensions
    {
        public static RepresentationRequestModel ToModel(this RepresentationRequestDomain entity)
        {
            return entity.MapTo<RepresentationRequestDomain, RepresentationRequestModel>();
        }

        public static RepresentationRequestDomain ToEntity(this RepresentationRequestModel model)
        {
            return model.MapTo<RepresentationRequestModel, RepresentationRequestDomain>();
        }


        public static RepresentationRequestInListModel ToListModel(this RepresentationRequestDomain entity)
        {
            return entity.MapTo<RepresentationRequestDomain, RepresentationRequestInListModel>();
        }

        public static List<TranslationEntity> ToLocalizedProperty<T>(this IList<T> list) where T : ILocalizedModelLocal
        {
            var local = new List<TranslationEntity>();
            foreach (var item in list)
            {
                Type[] interfaces = item.GetType().GetInterfaces();
                PropertyInfo[] props = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                foreach (var prop in props)
                {
                    bool insert = true;

                    foreach (var i in interfaces)
                    {
                        if (i.HasProperty(prop.Name))
                        {
                            insert = false;
                        }
                    }

                    if (insert && prop.GetValue(item) != null)
                        local.Add(new TranslationEntity() {
                            LanguageId = item.LanguageId,
                            LocaleKey = prop.Name,
                            LocaleValue = prop.GetValue(item).ToString(),
                        });
                }
            }
            return local;
        }

        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }
    }
}
