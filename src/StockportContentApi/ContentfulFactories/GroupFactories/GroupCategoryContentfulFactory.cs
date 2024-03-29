﻿namespace StockportContentApi.ContentfulFactories.GroupFactories;

public class GroupCategoryContentfulFactory : IContentfulFactory<ContentfulGroupCategory, GroupCategory>
{
    public GroupCategory ToModel(ContentfulGroupCategory entry)
    {
        var name = !string.IsNullOrEmpty(entry.Name)
            ? entry.Name
            : "";

        var slug = !string.IsNullOrEmpty(entry.Slug)
            ? entry.Slug
            : "";

        var icon = !string.IsNullOrEmpty(entry.Icon)
            ? entry.Icon
            : "";

        var image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ?
            entry.Image.File.Url : string.Empty;

        return new GroupCategory(name, slug, icon, image);
    }
}