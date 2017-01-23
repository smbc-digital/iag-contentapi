using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;



namespace StockportContentApi.Factories
{
    public interface IEventCategoriesFactory
    {
        List<string> Build(IList<dynamic> contentTypes);
    }

    public class EventCategoriesFactory : IEventCategoriesFactory
    {
        public static List<string> emptyList = new List<string>();

        public List<string> Build(IList<dynamic> contentTypes)
        {
            return ExtractCategoriesFromEventType(FindEventType(contentTypes));
        }

        private dynamic FindEventType(IList<dynamic> items)
        {
            foreach (var item in items)
            {
                var name = item.name;
                if (name.Value.ToString().ToLower() == "events")
                {
                    return item;
                }
            }
            return null;
        }

        private static List<string> ExtractCategoriesFromEventType(dynamic newsType)
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
