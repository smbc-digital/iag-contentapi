namespace StockportContentApi.Model
{
    public class GroupSubCategory
    {
        public string Name { get; }
        public string Slug { get; }

        public GroupSubCategory(string name, string slug)
        {
            Name = name;
            Slug = slug;
        }
    }
}


