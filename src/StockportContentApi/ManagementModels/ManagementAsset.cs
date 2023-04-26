﻿namespace StockportContentApi.ManagementModels;

public class ManagementAsset
{
    public string Id;
    public string LinkType = "Asset";
    public string Type = "Link";
}

public class LinkReference
{
    public ManagementAsset Sys { get; set; }
}
