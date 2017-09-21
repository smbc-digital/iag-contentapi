using System;
using System.Collections.Generic;
using System.Linq;

namespace StockportContentApi.Model
{
    public class ApiKey
    {
        public string Name { get; }
        public string Key { get; } 
        public string Email { get; } 
        public DateTime ActiveFrom { get; } 
        public DateTime ActiveTo { get; } 
        public List<string> EndPoints { get; }
        public int Version { get; }
        public bool CanViewSensitive { get; }
        public List<string> AllowedVerbs{ get; }

        public ApiKey(string name, string key, string email, DateTime activeFrom, DateTime activeTo, List<string> endPoints, int version, bool canViewSensitive, List<string> allowedVerbs)
        {
            Name = name;
            Key = key;
            Email = email;
            ActiveFrom = activeFrom;
            ActiveTo = activeTo;
            EndPoints = endPoints;
            Version = version;
            CanViewSensitive = canViewSensitive;
            AllowedVerbs = allowedVerbs;
        }
    }
}