using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.ManagementModels;
using StockportContentApi.Model;

namespace StockportContentApi
{
    public class AutoMapperConfig : AutoMapper.Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<GroupCategory, ContentfulGroupCategory>();

            CreateMap<Group, ContentfulGroup>()
                .ForMember(dest => dest.Image,
                    opts => opts.Ignore())
                .ForMember(dest => dest.SubCategories,
                    opts => opts.Ignore())
                .ForMember(dest => dest.Organisation,
                opts => opts.Ignore())
                .ForMember(dest => dest.AdditionalDocuments,
                opts => opts.Ignore());


            CreateMap<EventCategory, ContentfulEventCategory>();

            CreateMap<Event, ContentfulEvent>()
                .ForMember(dest => dest.Frequency, opts => opts.MapFrom(src => src.EventFrequency))
                .ForMember(dest => dest.Image, opts => opts.Ignore())
                .ForMember(dest => dest.Documents, opts => opts.Ignore());

            CreateMap<SystemProperties, ManagementSystemProperties>()
                .ForMember(dest => dest.Id,
                    opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.LinkType,
                    opts => opts.Ignore())
                .ForMember(dest => dest.Type,
                    opts => opts.Ignore());

            CreateMap<SystemProperties, ManagementAsset>()
                .ForMember(dest => dest.Id,
                    opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.LinkType,
                    opts => opts.Ignore())
                .ForMember(dest => dest.Type,
                    opts => opts.Ignore());

            CreateMap<ContentfulAlert, ManagementAlert>()
                .ForMember(dest => dest.Sys,
                    opts => opts.Ignore());

            CreateMap<ContentfulGroupCategory, ManagementGroupCategory>()
                .ForMember(d => d.Sys,
                    opts => opts.MapFrom(src => src.Sys));

            CreateMap<ContentfulEventCategory, ManagementEventCategory>()
                .ForMember(d => d.Sys,
                    opts => opts.MapFrom(src => src.Sys));

            CreateMap<ContentfulGroup, ContentfulGroup>()
                .ForMember(dest => dest.Sys,
                    opts => opts.Ignore());

            CreateMap<Asset, LinkReference>()
                .ForMember(dest => dest.Sys, opts => opts.MapFrom(src => new ManagementAsset() { Id = src.SystemProperties.Id }));

            CreateMap<ContentfulGroup, ManagementGroup>()
                .ConvertUsing<GroupConverter>();

            CreateMap<ContentfulEvent, ManagementEvent>()
                .ConvertUsing<EventConverter>();
        }
    }

    public class GroupConverter : ITypeConverter<ContentfulGroup, ManagementGroup>
    {
        public ManagementGroup Convert(ContentfulGroup source, ManagementGroup destination, ResolutionContext context)
        {
            if (destination == null)
            {
                destination = new ManagementGroup();
            }

            destination.AdditionalInformation = new Dictionary<string, string> {{"en-GB", source.AdditionalInformation}};
            destination.MapPosition = new Dictionary<string, MapPosition> { { "en-GB", source.MapPosition } };
            destination.Address = new Dictionary<string, string> { { "en-GB", source.Address } };
            destination.Description = new Dictionary<string, string> { { "en-GB", source.Description } };
            destination.Email = new Dictionary<string, string> { { "en-GB", source.Email } };
            destination.Facebook = new Dictionary<string, string> { { "en-GB", source.Facebook } };
            destination.GroupAdministrators = new Dictionary<string, GroupAdministrators> { { "en-GB", source.GroupAdministrators } };
            destination.Image = string.IsNullOrWhiteSpace(source.Image.SystemProperties.Id) ? null : new Dictionary<string, LinkReference> { { "en-GB", new LinkReference() { Sys = new ManagementAsset() { Id = source.Image.SystemProperties.Id } } } };
            destination.Name = new Dictionary<string, string> { { "en-GB", source.Name } };
            destination.PhoneNumber = new Dictionary<string, string> { { "en-GB", source.PhoneNumber } };
            destination.Slug = new Dictionary<string, string> { { "en-GB", source.Slug } };
            destination.Twitter = new Dictionary<string, string> { { "en-GB", source.Twitter } };
            destination.Volunteering = new Dictionary<string, bool> { { "en-GB", source.Volunteering } };
            destination.Donations = new Dictionary<string, bool> { { "en-GB", source.Donations } };
            destination.Website = new Dictionary<string, string> { { "en-GB", source.Website } };
            destination.DateHiddenFrom = new Dictionary<string, string> { { "en-GB", source.DateHiddenFrom != null ? source.DateHiddenFrom.Value.ToString("yyyy-MM-ddTHH:mm:ssK") : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ssK") } };
            destination.DateHiddenTo = new Dictionary<string, string> { { "en-GB", source.DateHiddenTo != null ? source.DateHiddenTo.Value.ToString("yyyy-MM-ddTHH:mm:ssK") : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ssK") } };
            destination.Cost = new Dictionary<string, List<string>>()
            {
                {
                    "en-GB",
                    source.Cost?.Select(o => o).ToList()
                }
            };
            destination.CostText = new Dictionary<string, string> { { "en-GB", source.CostText } };
            destination.AbilityLevel = new Dictionary<string, string> { { "en-GB", source.AbilityLevel } };

            destination.CategoriesReference = new Dictionary<string, List<ManagementGroupCategory>>()
            {
                {
                    "en-GB",
                    source.CategoriesReference.Select(o => context.Mapper.Map<ContentfulGroupCategory, ManagementGroupCategory>(o)).ToList()
                }
            };

            destination.VolunteeringText = new Dictionary<string, string> { { "en-GB", source.VolunteeringText } };
            if (destination.Organisation != null)
            {
                destination.Organisation = new Dictionary<string, ManagementReference>() {{"en-GB", new ManagementReference { Sys = context.Mapper.Map<SystemProperties, ManagementSystemProperties>(source.Organisation.Sys) } }};    
            }
            
            destination.SubCategories = new Dictionary<string, List<ManagementReference>>()
            {
                {
                    "en-GB", 
                    source.SubCategories.Select(sc => new ManagementReference { Sys = context.Mapper.Map<SystemProperties, ManagementSystemProperties>(sc.Sys) }).ToList()
                }
            };

            destination.AgeRange = new Dictionary<string, List<string>> { { "en-GB", source.AgeRange } };
            destination.SuitableFor = new Dictionary<string, List<string>> { { "en-GB", source.SuitableFor } };
            destination.DonationsText = new Dictionary<string, string> { { "en-GB", source.DonationsText } };
            destination.DonationsUrl = new Dictionary<string, string> { { "en-GB", source.DonationsUrl } };
            return destination;

        }
    }

    public class EventConverter : ITypeConverter<ContentfulEvent, ManagementEvent>
    {
        public ManagementEvent Convert(ContentfulEvent source, ManagementEvent destination, ResolutionContext context)
        {

            destination.Alerts = new Dictionary<string, List<ManagementAlert>> { { "en-GB", source.Alerts.Select(o => context.Mapper.Map<ContentfulAlert, ManagementAlert>(o)).ToList() } };
            destination.BookingInformation = new Dictionary<string, string> { { "en-GB", source.BookingInformation } };
            destination.Categories = new Dictionary<string, List<string>> { { "en-GB", source.Categories } };
            destination.Description = new Dictionary<string, string> { { "en-GB", source.Description } };
            destination.Documents = new Dictionary<string, List<LinkReference>> { { "en-GB", source.Documents.Select(o => context.Mapper.Map<Asset, LinkReference>(o)).ToList() } };
            destination.EndTime = new Dictionary<string, string> { { "en-GB", source.EndTime } };
            destination.EventCategories = new Dictionary<string, List<ManagementEventCategory>>()
            {
                {
                    "en-GB",
                    source.EventCategories.Select(o => context.Mapper.Map<ContentfulEventCategory, ManagementEventCategory>(o)).ToList()
                }
            };
            destination.EventDate = new Dictionary<string, DateTime> { { "en-GB", source.EventDate } };
            destination.Featured = new Dictionary<string, bool> { { "en-GB", source.Featured } };
            destination.Fee = new Dictionary<string, string> { { "en-GB", source.Fee } };
            destination.Free = new Dictionary<string, bool?> { { "en-GB", source.Free } };
            if (source.Frequency != EventFrequency.None)
            {
                destination.Frequency = new Dictionary<string, EventFrequency> { { "en-GB", source.Frequency } };
                destination.Occurences = new Dictionary<string, int> { { "en-GB", source.Occurences } };
            }
            destination.Group = new Dictionary<string, ManagementReference> { { "en-GB", new ManagementReference { Sys = context.Mapper.Map<SystemProperties, ManagementSystemProperties>(source.Group.Sys) } } };
            destination.Image = string.IsNullOrWhiteSpace(source.Image.SystemProperties.Id) ? null : new Dictionary<string, LinkReference> { { "en-GB", new LinkReference() { Sys = new ManagementAsset() { Id = source.Image.SystemProperties.Id } } } };
            destination.Location = new Dictionary<string, string> { { "en-GB", source.Location } };
            destination.MapPosition = new Dictionary<string, MapPosition> { { "en-GB", source.MapPosition } };
            destination.Paid = new Dictionary<string, bool?> { { "en-GB", source.Paid } };
            destination.Slug = new Dictionary<string, string>(){{"en-GB",source.Slug } };
            destination.StartTime = new Dictionary<string, string> { { "en-GB", source.StartTime } };
            destination.SubmittedBy = new Dictionary<string, string> { { "en-GB", source.SubmittedBy } };
            destination.Tags = new Dictionary<string, List<string>> { { "en-GB", source.Tags } };
            destination.Teaser = new Dictionary<string, string> { { "en-GB", source.Teaser } };
            destination.Title = new Dictionary<string, string> { { "en-GB", source.Title } };

            return destination;
        }
    }
}