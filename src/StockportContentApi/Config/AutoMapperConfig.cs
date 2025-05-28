using ManagementAsset = StockportContentApi.ManagementModels.ManagementAsset;

namespace StockportContentApi.Config;

[ExcludeFromCodeCoverage]
public class AutoMapperConfig : AutoMapper.Profile
{
    public AutoMapperConfig()
    {
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

        CreateMap<ContentfulEventCategory, ManagementEventCategory>()
            .ForMember(d => d.Sys,
                opts => opts.MapFrom(src => src.Sys));

        CreateMap<TrustedLogo, ContentfulTrustedLogo>()
            .ForMember(dest => dest.Sys,
                opts => opts.Ignore())
            .ForMember(dest => dest.File,
                opts => opts.Ignore());

        CreateMap<Asset, LinkReference>()
            .ForMember(dest => dest.Sys,
                opts => opts.MapFrom(src => new ManagementAsset { Id = src.SystemProperties.Id }));

        CreateMap<ContentfulEvent, ManagementEvent>()
            .ConvertUsing<EventConverter>();
    }
}

[ExcludeFromCodeCoverage]
public class EventConverter : ITypeConverter<ContentfulEvent, ManagementEvent>
{
    public ManagementEvent Convert(ContentfulEvent source, ManagementEvent destination, ResolutionContext context)
    {
        destination.Alerts = new()
        {
            { "en-GB", source.Alerts.Select(o => context.Mapper.Map<ContentfulAlert, ManagementAlert>(o)).ToList() }
        };
        destination.BookingInformation = new() { { "en-GB", source.BookingInformation } };
        destination.Description = new() { { "en-GB", source.Description } };
        destination.Documents = new()
            { { "en-GB", source.Documents.Select(o => context.Mapper.Map<Asset, LinkReference>(o)).ToList() } };
        destination.EndTime = new() { { "en-GB", source.EndTime } };
        destination.EventCategories = new()
        {
            {
                "en-GB",
                source.EventCategories
                    .Select(context.Mapper.Map<ContentfulEventCategory, ManagementEventCategory>).ToList()
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