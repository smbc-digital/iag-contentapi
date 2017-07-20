using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class SmartAnswer
    {
        public string Slug { get; } 
        public string QuestionJson { get; }

        public SmartAnswer(string slug, string questionJson)
        {
            Slug = slug;
            QuestionJson = questionJson;
        }
    }
}
