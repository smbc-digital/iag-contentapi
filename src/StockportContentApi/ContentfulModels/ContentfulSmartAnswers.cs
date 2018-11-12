using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulSmartAnswers : ContentfulReference
    {
        public new string Title { get; set; }
        public new string Slug { get; set; }
        public List<QuestionJson> QuestionJson { get; set; }
    }
}
