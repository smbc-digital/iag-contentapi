﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StockportContentApi.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventFrequency
    {
        [EnumMember(Value = "None")]
        None,
        [EnumMember(Value = "Daily")]
        Daily,
        [EnumMember(Value = "Weekly")]
        Weekly,
        [EnumMember(Value = "Fortnightly")]
        Fortnightly,
        [EnumMember(Value = "Monthly")]
        Monthly,
        [EnumMember(Value = "MonthlyDay")]
        MonthlyDay,
        [EnumMember(Value = "MonthlyDate")]
        MonthlyDate,
        [EnumMember(Value = "Yearly")]
        Yearly
    }
}
