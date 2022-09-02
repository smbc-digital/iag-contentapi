using Contentful.Core.Models;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public record ContentfulCallToAction(string Title, string Text, Link Link, Asset Image);
}
