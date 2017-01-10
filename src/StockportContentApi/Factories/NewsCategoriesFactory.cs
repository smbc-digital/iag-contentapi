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

        public List<string> Build(IList<dynamic> items)
        {
            List<string> categoryStrings = new List<string>();
            foreach(var item in items)
            { 
                //JArray categories = items[0].fields[0].items.validations[0].@in;
                var name = item.name;
                if (name.Value.ToString().ToLower() == "news")
                {
                    JArray categories = item.fields[0].items.validations[0].@in;
                    for (int count = 0; count < categories.Count; count++)
                    {
                        categoryStrings.Add(items[0].fields[0].items.validations[0].@in[count].Value);
                    }
                }
            }

            return categoryStrings;
        }
    }
}
