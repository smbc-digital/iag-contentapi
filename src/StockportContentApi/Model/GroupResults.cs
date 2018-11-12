using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class GroupResults
    {
        public List<Group> Groups { get; set; }
        public List<GroupCategory> Categories { get; set; }
        public List<GroupSubCategory> AvailableSubCategories { get; set; }
    }
}
