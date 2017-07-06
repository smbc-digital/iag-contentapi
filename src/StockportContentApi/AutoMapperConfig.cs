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
                    opts => opts.Ignore());

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

            CreateMap<ContentfulGroupCategory, ManagementGroupCategory>()
                .ForMember(d => d.Sys,
                    opts => opts.MapFrom(src => src.Sys));

            CreateMap<ContentfulGroup, ContentfulGroup>()
                .ForMember(dest => dest.Sys,
                    opts => opts.Ignore());

            CreateMap<ContentfulGroup, ManagementGroup>()
                .ConvertUsing<GroupConverter>();
        }
    }

    public class GroupConverter : ITypeConverter<ContentfulGroup, ManagementGroup>
    {
        public ManagementGroup Convert(ContentfulGroup source, ManagementGroup destination, ResolutionContext context)
        {

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
            destination.Website = new Dictionary<string, string> { { "en-GB", source.Website } };
            destination.DateHiddenFrom = new Dictionary<string, string> { { "en-GB", source.DateHiddenFrom != null ? source.DateHiddenFrom.Value.ToString("yyyy-MM-ddTHH:mm:ssK") : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ssK") } };
            destination.DateHiddenTo = new Dictionary<string, string> { { "en-GB",source.DateHiddenTo != null ? source.DateHiddenTo.Value.ToString("yyyy-MM-ddTHH:mm:ssK") : DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ssK") } };

            destination.CategoriesReference = new Dictionary<string, List<ManagementGroupCategory>>()
            {
                {
                    "en-GB",
                    source.CategoriesReference.Select(o => context.Mapper.Map<ContentfulGroupCategory, ManagementGroupCategory>(o)).ToList()
                }
            };

            return destination;

        }
    }
}