using Microsoft.AspNetCore.Http;
using StockportContentApi.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace StockportContentApi.Utils
{
    public static class SensitiveDataStripper
    {
        public static T StripData<T>(this T model, IHttpContextAccessor _httpContextAccessor)
        {
            if (model == null) return default(T);

            var cannotViewSensitive = false;

            bool.TryParse(_httpContextAccessor.HttpContext.Request.Headers["cannotViewSensitive"], out cannotViewSensitive);

            if (!cannotViewSensitive) return model;

            var type = model.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties.Where(p => p.PropertyType == typeof(string)))
            {
                var attributes = property.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute.GetType() == typeof(SensitiveData))
                    {
                        property.SetValue(model, string.Empty);
                        break;
                    }
                }
            }

            return model;
        }
    }
}
