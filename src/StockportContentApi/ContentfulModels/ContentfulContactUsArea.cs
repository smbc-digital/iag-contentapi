namespace StockportContentApi.ContentfulModels
{
    public class ContentfulContactUsArea : ContentfulReference
    {
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public string CategoriesTitle { get; set; } = string.Empty;
        public IEnumerable<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
        public List<ContentfulReference> PrimaryItems { get; set; } = new List<ContentfulReference>();
        public string InsetTextTitle { get; set; } = string.Empty;
        public string InsetTextBody { get; set; } = string.Empty;
        public IEnumerable<ContentfulContactUsCategory> ContactUsCategories { get; set; } = new List<ContentfulContactUsCategory>();
        public string MetaDescription { get; set; } = string.Empty;
    }
}