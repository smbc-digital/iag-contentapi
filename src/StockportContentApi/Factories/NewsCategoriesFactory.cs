using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;



namespace StockportContentApi.Factories
{
    public interface INewsCategoriesFactory
    {
        List<string> Build(IList<dynamic> contentTypes);
    }

    public class NewsCategoriesFactory : INewsCategoriesFactory
    {
        public static List<string> emptyList = new List<string>();

        public List<string> Build(IList<dynamic> contentTypes)
        {
            return ExtractCategoriesFromNewsType(FindNewsType(contentTypes));
        }

        private dynamic FindNewsType(IList<dynamic> items)
        {
            foreach (var item in items)
            {
                var name = item.name;
                if (name.Value.ToString().ToLower() == "news")
                {
                    return item;
                }
            }
            return null;
        }

        private static List<string> ExtractCategoriesFromNewsType(dynamic newsType)
        {
            foreach (var field in newsType.fields)
            {
                if (field.name.Value == "Categories")
                {
                    return ExtractCategories(field);
                }
            }
            return emptyList;
        }

        private static List<string> ExtractCategories(dynamic categoryField)
        {
            var categoryStrings = new List<string>();
            dynamic validCategories = ValidValues(categoryField);
            foreach (var validCategory in validCategories)
            {
                categoryStrings.Add(validCategory.Value);
            }
            return categoryStrings;
        }

        private static dynamic ValidValues(dynamic field)
        {
            return field.items.validations[0].@in;
        }
    }
}
