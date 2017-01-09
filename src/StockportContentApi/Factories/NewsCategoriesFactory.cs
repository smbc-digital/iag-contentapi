using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace StockportContentApi.Factories
{
    public class NewsCategoriesFactory
    {

        public List<string> Build(dynamic contentTypes)
        {  
            Newtonsoft.Json.Linq.JArray categories = contentTypes.items[0].fields[0].items.validations[0].NewsCategoryList;

            List<string> categoryStrings = new List<string>();
            for(int count = 0; count < categories.Count; count++)
            {
                categoryStrings.Add(contentTypes.items[0].fields[0].items.validations[0].NewsCategoryList[count].Value);
            }
           
            return categoryStrings;
        }
    }
}
