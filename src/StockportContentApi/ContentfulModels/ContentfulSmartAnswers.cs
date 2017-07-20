using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulSmartAnswers : ContentfulReference
    {
        public string Slug { get; set; } = string.Empty;
        public string QuestionJson { get; set; } = string.Empty;
    }
}
