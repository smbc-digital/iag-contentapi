﻿using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApi.Model
{
    public class Event
    {
        public string Title { get; }
        public string Slug { get; }
        public string Teaser { get; }
        public string ImageUrl { get; }
        public string ThumbnailImageUrl { get; }
        public string Description { get; }
        public string Fee { get; }
        public string Location { get; }
        public string SubmittedBy { get; }
        public DateTime EventDate { get; }
        public string StartTime { get; }
        public string EndTime { get; }
        public int Occurences { get; } 
        public EventFrequency Frequency { get; }
        public List<Crumb> Breadcrumbs { get; }
        public List<Document> Documents { get; }
        public List<string> Categories { get; }
        public MapPosition MapPosition { get; }
        public string BookingInformation { get; }
        public bool Featured { get; }
        public DateTime UpdatedAt { get; }
        public List<string> Tags { get; }
        public Group Group { get; }


        public Event(string title, string slug, string teaser, string imageUrl, string description, string fee, 
                     string location, string submittedBy, DateTime eventDate, string startTime, string endTime, 
                     int occurences, EventFrequency frequency, List<Crumb> breadcrumbs, string thumbnailImageUrl, 
                     List<Document> documents, List<string> categories, MapPosition mapPosition, bool featured,string bookingInformation, DateTime updatedAt, List<string> tags, Group group ) 
            {
                Title = title;
                Slug = slug;
                Teaser = teaser;
                Description = description;
                Fee = fee;
                Location = location;
                SubmittedBy = submittedBy;
                EventDate = eventDate;
                StartTime = startTime;
                EndTime = endTime;
                Occurences = occurences;
                Frequency = frequency;
                Breadcrumbs = breadcrumbs;
                ThumbnailImageUrl = thumbnailImageUrl;
                ImageUrl = imageUrl;
                Documents = documents;
                Categories = categories.Select(s => s.ToLower()).ToList();
                MapPosition = mapPosition;
                BookingInformation = bookingInformation;
                Featured = featured;
                UpdatedAt = updatedAt;
                Tags = tags.Select(s => s.ToLower()).ToList();
                Group = group;
            }

        public bool ShouldSerializeFrequency()
        {
            return false;
        }

        public bool ShouldSerializeOccurences()
        {
            return false;
        }
    }


   
}
