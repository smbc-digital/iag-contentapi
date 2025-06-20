﻿namespace StockportContentApi.ContentfulFactories.ArticleFactories;

public class ArticleContentfulFactory : IContentfulFactory<ContentfulArticle, Article>
{
    private readonly IContentfulFactory<ContentfulSection, Section> _sectionFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
    private readonly IContentfulFactory<ContentfulArticle, Topic> _parentTopicFactory;
    private readonly IContentfulFactory<Asset, Document> _documentFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IVideoRepository _videoRepository;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> _trustedLogoFactory;
    private readonly IContentfulFactory<ContentfulInlineQuote, InlineQuote> _inlineQuoteContentfulFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionContentfulFactory;

    public ArticleContentfulFactory(IContentfulFactory<ContentfulSection, Section> sectionFactory,
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        IContentfulFactory<ContentfulProfile, Profile> profileFactory,
        IContentfulFactory<ContentfulArticle, Topic> parentTopicFactory,
        IContentfulFactory<Asset, Document> documentFactory,
        IVideoRepository videoRepository,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory,
        IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
        IContentfulFactory<ContentfulInlineQuote, InlineQuote> inlineQuoteContentfulFactory,
        IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionContentfulFactory)
    {
        _sectionFactory = sectionFactory;
        _crumbFactory = crumbFactory;
        _profileFactory = profileFactory;
        _documentFactory = documentFactory;
        _videoRepository = videoRepository;
        _parentTopicFactory = parentTopicFactory;
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _trustedLogoFactory = trustedLogoFactory;
        _subitemFactory = subitemFactory;
        _inlineQuoteContentfulFactory = inlineQuoteContentfulFactory;
        _callToActionContentfulFactory = callToActionContentfulFactory;
    }

    public Article ToModel(ContentfulArticle entry)
    {
        DateTime sectionUpdatedAt = entry.Sections.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                        .Where(section => section.Sys.UpdatedAt is not null)
                                        .Select(section => section.Sys.UpdatedAt.Value)
                                        .OrderByDescending(section => section)
                                        .FirstOrDefault();

        return new()
        {
            Body = !string.IsNullOrEmpty(entry.Body) 
                ? _videoRepository.Process(entry.Body) 
                : string.Empty,
            
            Slug = entry.Slug,
            Title = entry.Title,
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            Icon = entry.Icon,
            
            BackgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                ? entry.BackgroundImage.File.Url 
                : string.Empty,
            
            Image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                ? entry.Image.File.Url 
                : string.Empty,
            
            AltText = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                ? entry.Image.Description 
                : string.Empty,
            
            Sections = entry.Sections.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                        .Select(_sectionFactory.ToModel).ToList(),
            
            Breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                            .Select(_crumbFactory.ToModel).ToList(),
            
            Alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                        .Where(alert => !alert.Severity.Equals("Condolence"))
                        .Select(_alertFactory.ToModel),
            
            Profiles = entry.Profiles.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                        .Select(_profileFactory.ToModel).ToList(),
            
            TrustedLogos = entry.TrustedLogos is not null 
                ? entry.TrustedLogos.Where(trustedLogo => trustedLogo is not null)
                    .Select(_trustedLogoFactory.ToModel).ToList() 
                : new(),
            
            LogoAreaTitle = entry.LogoAreaTitle,
            ParentTopic = _parentTopicFactory.ToModel(entry) ?? new NullTopic(),
            
            Documents = entry.Documents.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                            .Select(_documentFactory.ToModel).ToList(),
            
            RelatedContent = entry.RelatedContent.Where(rc => ContentfulHelpers.EntryIsNotALink(rc.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(rc.SunriseDate, rc.SunsetDate))
                                .Select(_subitemFactory.ToModel).ToList(),
            
            SunriseDate = entry.SunriseDate,
            SunsetDate = entry.SunsetDate,
            
            AlertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                            .Where(alert => !alert.Severity.Equals("Condolence"))
                            .Select(_alertFactory.ToModel),
                        
            UpdatedAt = sectionUpdatedAt > entry.Sys.UpdatedAt.Value 
                ? sectionUpdatedAt 
                : entry.Sys.UpdatedAt.Value,

            PublishedOn = entry.Sys.CreatedAt.Value,

            HideLastUpdated = entry.HideLastUpdated,
            Author = entry.Author,
            Photographer = entry.Photographer,
            InlineQuotes = entry.InlineQuotes.Select(_inlineQuoteContentfulFactory.ToModel).ToList(),
            AssociatedTagCategory = entry.AssociatedTagCategory,
            CallToActionBanners = entry.CallToActionBanners.Select(_callToActionContentfulFactory.ToModel).ToList()
        };
    }
}