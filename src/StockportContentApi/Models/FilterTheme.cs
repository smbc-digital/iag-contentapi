﻿namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class FilterTheme
{
    IEnumerable<Filter> filters;

    public string Title { get; set; }

    public IEnumerable<Filter> Filters
    {
        get { return filters; }
        set { filters = value.Distinct(); }
    }
}