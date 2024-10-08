﻿namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class GroupResults
{
    public List<Group> Groups { get; set; }
    public List<GroupCategory> Categories { get; set; }
    public List<GroupSubCategory> AvailableSubCategories { get; set; }
}