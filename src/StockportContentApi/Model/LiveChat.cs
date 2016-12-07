﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class LiveChat
    {
        public string Title { get; }
        public string Text { get; }      

        public LiveChat(string title, string text)
        {
            Title = title;
            Text = text;
        }
    }

    public class NullLiveChat : LiveChat
    {
         public NullLiveChat() : base(string.Empty,string.Empty) { }
    }
}