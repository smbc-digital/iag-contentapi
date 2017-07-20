using Contentful.Core.Models;
using Contentful.Core.Search;
using StockportContentApi.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Repositories
{
    public abstract class BaseRepository
    {
        public async Task<ContentfulCollection<T>> GetAllEntriesAsync<T>(Contentful.Core.IContentfulClient _client, QueryBuilder<T> builder)
        {
            var result = new ContentfulCollection<T>();
            result.Items = new List<T>();

            if (!BuilderHasProperty(builder, "limit")) {
                builder.Limit(ContentfulQueryValues.LIMIT_MAX);
            }

            builder.Skip(999);
            var builderString = builder.Build();
            builderString = builderString.Replace("skip=999", "skip=xxx");
            var totalItems = 0;
            var skip = 0;
            do
            {
                builderString = builderString.Replace("skip=xxx", $"skip={skip}");
                var items = await _client.GetEntriesAsync<T>(builderString);
                builderString = builderString.Replace($"skip={skip}", "skip=xxx");
                totalItems = items.Total;
                result.Items = result.Items.Concat(items.Items);
                skip += ContentfulQueryValues.LIMIT_MAX;
            } while (result.Items.Count() < totalItems);

            return result;
        }

        private bool BuilderHasProperty<T>(QueryBuilder<T> builder, string property)
        {
            return builder.Build().Contains(property);
        }
    }
}
