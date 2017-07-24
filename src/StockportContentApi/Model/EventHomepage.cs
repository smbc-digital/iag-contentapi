﻿using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class EventHomepageRow
    {
        public bool IsLatest { get; set; }
        public string Tag { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }

    public class EventHomepage
    {
        public IEnumerable<EventHomepageRow> Rows { get; }

        public EventHomepage(IEnumerable<EventHomepageRow> rows)
        {
            Rows = rows;
        }
    }
}
