using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulLiveChatBuilder
    {
        private string _title = "title";
        private string _text = "text";

        public ContentfulLiveChat Build()
        {
            return new ContentfulLiveChat()
            {
                Title = _title,
                Text = _text,
                Sys = new SystemProperties
                {
                    Type = "Entry",
                }
           };
        }
    }
}
