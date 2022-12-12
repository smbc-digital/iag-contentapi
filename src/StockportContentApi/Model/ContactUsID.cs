namespace StockportContentApi.Model
{
    public class ContactUsId
    {
        public string Name { get; }
        public string Slug { get; }
        public string EmailAddress { get; }
        public string SuccessPageButtonText { get; }
        public string SuccessPageReturnUrl { get; }

        public ContactUsId(string name, string slug, string emailAddress, string successPageButtonText, string successPageReturnUrl)
        {
            Name = name;
            Slug = slug;
            EmailAddress = emailAddress;
            SuccessPageButtonText = successPageButtonText;
            SuccessPageReturnUrl = successPageReturnUrl;
        }
    }

    public class NullContactUsId : ContactUsId
    {
        public NullContactUsId() : base(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) { }
    }
}
