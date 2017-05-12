using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.Model;

namespace StockportContentApi.Helpers
{
    public static class TypeHelper
    {
        public static string ContentfulTypeFor<T>()
        {
            Dictionary<Type, string> types = new Dictionary<Type, string>()
            {
                {typeof(Topic), "topic"},
                {typeof(Article), "article"},
                {typeof(Profile), "profile"},
                {typeof(News), "news"},
                {typeof(Event), "events"},
                {typeof(Group), "group"},
                {typeof(Payment), "payment"},
                {typeof(Showcase), "showcase"},
            };

            return types[typeof(T)];
        }

    }
}
