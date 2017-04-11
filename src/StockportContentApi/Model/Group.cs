using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Group
    {
        public string Name { get; }
        public string Slug { get; }
        public string PhoneNumber { get; }
        public string Email { get; }
        public string Website { get; }
        public string Twitter { get; }
        public string Facebook { get; }
        public string Address { get; }
        public string Description { get; }
        public string ImageUrl { get; }
        public string ThumbnailImageUrl { get; }
        public List<GroupCategory> CategoriesReference { get; }
        public List<Crumb> Breadcrumbs { get; }

        public MapPosition MapPosition { get; }

        public Group(string name, string slug, string phoneNumber, string email, string website,
            string twitter, string facebook, string address, string description, string imageUrl,
            string thumbnailImageUrl, List<GroupCategory> categoriesReference, List<Crumb> breadcrumbs, MapPosition mapPosition)
        {
            Name = name;
            Slug = slug;
            PhoneNumber = phoneNumber;
            Email = email;
            Website = website;
            Twitter = twitter;
            Facebook = facebook;
            Address = address;
            Description = description;
            ImageUrl = imageUrl;
            ThumbnailImageUrl = thumbnailImageUrl;
            CategoriesReference = categoriesReference;
            Breadcrumbs = breadcrumbs;
            MapPosition = mapPosition;
        }
    }
}


