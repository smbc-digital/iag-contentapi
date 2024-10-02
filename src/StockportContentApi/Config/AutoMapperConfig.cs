namespace StockportContentApi.Config;

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

        CreateMap<GroupBranding, ContentfulGroupBranding>()
            .ForMember(dest => dest.Sys,
                opts => opts.Ignore())
            .ForMember(dest => dest.File,
                opts => opts.Ignore());

        CreateMap<Asset, LinkReference>()
            .ForMember(dest => dest.Sys,
                opts => opts.MapFrom(src => new ManagementAsset { Id = src.SystemProperties.Id }));

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
        if (destination is null)
            destination = new();

        destination.AdditionalInformation = new() { { "en-GB", source.AdditionalInformation } };
        destination.MapPosition = new() { { "en-GB", source.MapPosition } };
        destination.Address = new() { { "en-GB", source.Address } };
        destination.Description = new() { { "en-GB", source.Description } };
        destination.Email = new() { { "en-GB", source.Email } };
        destination.Facebook = new() { { "en-GB", source.Facebook } };
        destination.GroupAdministrators = new() { { "en-GB", source.GroupAdministrators } };
        destination.Image = string.IsNullOrWhiteSpace(source.Image.SystemProperties.Id)
            ? null
            : new Dictionary<string, LinkReference> { { "en-GB", new() { Sys = new() { Id = source.Image.SystemProperties.Id } } } };
        destination.Name = new() { { "en-GB", source.Name } };
        destination.PhoneNumber = new() { { "en-GB", source.PhoneNumber } };
        destination.Slug = new() { { "en-GB", source.Slug } };
        destination.Twitter = new() { { "en-GB", source.Twitter } };
        destination.Volunteering = new() { { "en-GB", source.Volunteering } };
        destination.Donations = new() { { "en-GB", source.Donations } };
        destination.Website = new() { { "en-GB", source.Website } };
        destination.DateHiddenFrom = new()
        {
            {
                "en-GB",
                source.DateHiddenFrom is not null
                    ? source.DateHiddenFrom.Value.ToString("yyyy-MM-ddTHH:mm:ssK")
                    : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ssK")
            }
        };
        destination.DateHiddenTo = new()
        {
            {
                "en-GB",
                source.DateHiddenTo is not null
                    ? source.DateHiddenTo.Value.ToString("yyyy-MM-ddTHH:mm:ssK")
                    : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ssK")
            }
        };
        destination.Cost = new()
        {
            {
                "en-GB",
                source.Cost?.Select(o => o).ToList()
            }
        };
        destination.CostText = new() { { "en-GB", source.CostText } };
        destination.AbilityLevel = new() { { "en-GB", source.AbilityLevel } };

        destination.CategoriesReference = new()
        {
            {
                "en-GB",
                source.CategoriesReference
                    .Select(o => context.Mapper.Map<ContentfulGroupCategory, ManagementGroupCategory>(o)).ToList()
            }
        };

        destination.VolunteeringText = new() { { "en-GB", source.VolunteeringText } };

        if (destination.Organisation is not null)
            destination.Organisation = new()
            {
                {
                    "en-GB",
                    new()
                    {
                        Sys = context.Mapper.Map<SystemProperties, ManagementSystemProperties>(source.Organisation.Sys)
                    }
                }
            };

        destination.SubCategories = new()
        {
            {
                "en-GB",
                source.SubCategories.Select(sc => new ManagementReference
                    { Sys = context.Mapper.Map<SystemProperties, ManagementSystemProperties>(sc.Sys) }).ToList()
            }
        };

        destination.AgeRange = new() { { "en-GB", source.AgeRange } };
        destination.SuitableFor = new() { { "en-GB", source.SuitableFor } };
        destination.DonationsText = new() { { "en-GB", source.DonationsText } };
        destination.DonationsUrl = new() { { "en-GB", source.DonationsUrl } };
        destination.GroupBranding = new()
        {
            {
                "en-GB",
                source.GroupBranding.Select(sc => new ManagementReference
                    { Sys = context.Mapper.Map<SystemProperties, ManagementSystemProperties>(sc.Sys) }).ToList()
            }
        };

        return destination;
    }
}

public class EventConverter : ITypeConverter<ContentfulEvent, ManagementEvent>
{
    public ManagementEvent Convert(ContentfulEvent source, ManagementEvent destination, ResolutionContext context)
    {
        destination.Alerts = new()
        {
            { "en-GB", source.Alerts.Select(o => context.Mapper.Map<ContentfulAlert, ManagementAlert>(o)).ToList() }
        };
        destination.BookingInformation = new() { { "en-GB", source.BookingInformation } };
        destination.Categories = new() { { "en-GB", source.Categories } };
        destination.Description = new() { { "en-GB", source.Description } };
        destination.Documents = new()
            { { "en-GB", source.Documents.Select(o => context.Mapper.Map<Asset, LinkReference>(o)).ToList() } };
        destination.EndTime = new() { { "en-GB", source.EndTime } };
        destination.EventCategories = new()
        {
            {
                "en-GB",
                source.EventCategories
                    .Select(o => context.Mapper.Map<ContentfulEventCategory, ManagementEventCategory>(o)).ToList()
            }
        };
        destination.EventDate = new() { { "en-GB", source.EventDate } };
        destination.Featured = new() { { "en-GB", source.Featured } };
        destination.Fee = new() { { "en-GB", source.Fee } };
        destination.Free = new() { { "en-GB", source.Free } };
        if (!source.Frequency.Equals(EventFrequency.None))
        {
            destination.Frequency = new() { { "en-GB", source.Frequency } };
            destination.Occurences = new() { { "en-GB", source.Occurences } };
        }

        destination.Group = new()
        {
            {
                "en-GB",
                new() { Sys = context.Mapper.Map<SystemProperties, ManagementSystemProperties>(source.Group.Sys) }
            }
        };
        destination.Image = string.IsNullOrWhiteSpace(source.Image.SystemProperties.Id)
            ? null
            : new Dictionary<string, LinkReference> { { "en-GB", new() { Sys = new() { Id = source.Image.SystemProperties.Id } } } };
        destination.Location = new() { { "en-GB", source.Location } };
        destination.MapPosition = new() { { "en-GB", source.MapPosition } };
        destination.Paid = new() { { "en-GB", source.Paid } };
        destination.Slug = new() { { "en-GB", source.Slug } };
        destination.StartTime = new() { { "en-GB", source.StartTime } };
        destination.SubmittedBy = new() { { "en-GB", source.SubmittedBy } };
        destination.Tags = new() { { "en-GB", source.Tags } };
        destination.Teaser = new() { { "en-GB", source.Teaser } };
        destination.Title = new() { { "en-GB", source.Title } };

        return destination;
    }
}