﻿global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Net;
global using System.Reflection;
global using System.Runtime.Serialization; 
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Xml.Linq;
global using AutoMapper;
global using Contentful.Core;
global using Contentful.Core.Errors;
global using Contentful.Core.Models;
global using Contentful.Core.Models.Management;
global using Contentful.Core.Search;
global using GeoCoordinatePortable;
global using Jose;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.DataProtection.Repositories;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Options;
global using Microsoft.OpenApi.Models;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Converters;
global using Serilog;
global using StackExchange.Redis;
global using StockportContentApi;
global using StockportContentApi.Builders;
global using StockportContentApi.Client;
global using StockportContentApi.Config;
global using StockportContentApi.ContentfulFactories;
global using StockportContentApi.ContentfulFactories.ArticleFactories;
global using StockportContentApi.ContentfulFactories.EventFactories;
global using StockportContentApi.ContentfulFactories.NewsFactories;
global using StockportContentApi.ContentfulFactories.TopicFactories;
global using StockportContentApi.ContentfulModels;
global using StockportContentApi.Extensions;
global using StockportContentApi.Factories;
global using StockportContentApi.Http;
global using StockportContentApi.ManagementModels;
global using StockportContentApi.Middleware;
global using StockportContentApi.Models;
global using StockportContentApi.Models.Enums;
global using StockportContentApi.Models.Exceptions;
global using StockportContentApi.Repositories;
global using StockportContentApi.Services;
global using StockportContentApi.Utils;
global using StockportGovUK.AspNetCore.Logging.Elasticsearch.Aws;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using File = Contentful.Core.Models.File;
global using ILogger = Microsoft.Extensions.Logging.ILogger;
global using HttpClient = StockportContentApi.Http.HttpClient;
global using HttpResponse = StockportContentApi.Http.HttpResponse;
global using Directory = StockportContentApi.Models.Directory;
global using Document = StockportContentApi.Models.Document;
global using Profile = StockportContentApi.Models.Profile;