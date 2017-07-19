using System;
using System.Collections.Generic;
using StockportContentApi.ContentfulModels;

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
        public bool Volunteering { get; }
        public List<Event> Events { get; private set; }
        public GroupAdministrators GroupAdministrators { get; set; }
        public DateTime? DateHiddenFrom { get; set; }
        public DateTime? DateHiddenTo { get; set; }
        public string Status { get; set; }
        public string Cost { get; set; }
        public string CostText { get; set; }
        public string AbilityLevel { get; set; }

        public Group(string name, string slug, string phoneNumber, string email, string website,
            string twitter, string facebook, string address, string description, string imageUrl,
            string thumbnailImageUrl, List<GroupCategory> categoriesReference, List<Crumb> breadcrumbs, 
            MapPosition mapPosition, bool volunteering, GroupAdministrators groupAdministrators, 
            DateTime? dateHiddenFrom, DateTime? dateHiddenTo, string status, string cost, string costText, string abilityLevel)
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
            Volunteering = volunteering;
            GroupAdministrators = groupAdministrators;
            DateHiddenFrom = dateHiddenFrom;
            DateHiddenTo = dateHiddenTo;
            Status = status;
            Cost = cost;
            CostText = costText;
            AbilityLevel = abilityLevel;
        }

        public void SetEvents(List<Event> events)
        {
            Events = events;
        }
    }
}


