using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class InformationList
    {
        public string Name { get; }

        public string Icon { get; }

        public string Text { get; }

        public string Link { get; }

        public InformationList()
        {

        }

        public InformationList(string name, string icon, string text, string link)
        {
            Name = name;
            Icon = icon;
            Text = text;
            Link = link;
        }
    }
}
