using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class GroupResults
    {
        public List<Group> Groups { get; set; }
        public List<GroupCategory> Categories { get; set; }
        public List<GroupSubCategory> AvailableSubCategories { get; set; }
    }
}
