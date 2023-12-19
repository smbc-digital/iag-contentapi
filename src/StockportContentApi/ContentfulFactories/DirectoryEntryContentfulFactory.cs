﻿using StockportContentApi.Models;
using Directory = StockportContentApi.Model.Directory;

namespace StockportContentApi.ContentfulFactories
{
    public class DirectoryEntryContentfulFactory : IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>
    {
        private readonly IContentfulFactory<ContentfulDirectory, Directory> _directoryFactory;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;

        private readonly DateComparer _dateComparer;

        public DirectoryEntryContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory, IContentfulFactory<ContentfulDirectory, Directory> directoryFactory, ITimeProvider timeProvider)
        {
            _directoryFactory = directoryFactory;
            _alertFactory = alertFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public DirectoryEntry ToModel(ContentfulDirectoryEntry entry)
        {
            if (entry is null)
                return null;

            var directoryEntry = new DirectoryEntry
            {
                Slug = entry.Slug,
                Title = entry.Title,
                Body = entry.Body,
                Teaser = entry.Teaser,
                MetaDescription = entry.MetaDescription,
                MapPosition = entry.MapPosition,
                PhoneNumber = entry.PhoneNumber,
                Email = entry.Email,
                Website = entry.Website,
                Twitter = entry.Twitter,
                Facebook = entry.Facebook,
                Address = entry.Address,
                Directories = entry.Directories?.Select(contentfulDirectory => new DirectoryEntry.MinimalDirectory(contentfulDirectory.Slug, contentfulDirectory.Title)),
                Alerts = entry.Alerts?
                            .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                            .Select(alert => _alertFactory.ToModel(alert)),
                Themes = entry
                    .Filters?
                    .Select(filter => filter.Theme)
                    .Distinct()
                    .Select(theme => new FilterTheme()
                    {
                        Title = theme,
                        Filters = entry.Filters
                            .Where(filter => filter.Theme.Equals(theme))
                            .Select(filter => new Filter(filter))
                    })
            };

            return directoryEntry;
        }
    }
}