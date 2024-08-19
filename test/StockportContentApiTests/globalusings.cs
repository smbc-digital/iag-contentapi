﻿global using Contentful.Core.Errors;
global using Contentful.Core.Models;
global using Contentful.Core.Models.Management;
global using Contentful.Core.Search;
global using FluentAssertions;
global using Microsoft.AspNetCore.Mvc;
global using Moq;
global using StockportContentApi.Builders;
global using StockportContentApi.Client;
global using StockportContentApi.Config;
global using StockportContentApi.ContentfulFactories;
global using StockportContentApi.ContentfulFactories.ArticleFactories;
global using StockportContentApi.ContentfulFactories.EventFactories;
global using StockportContentApi.ContentfulFactories.GroupFactories;
global using StockportContentApi.ContentfulFactories.NewsFactories;
global using StockportContentApi.ContentfulFactories.TopicFactories;
global using StockportContentApi.ContentfulModels;
global using StockportContentApi.Extensions;
global using StockportContentApi.Factories;
global using StockportContentApi.Http;
global using StockportContentApi.Middleware;
global using StockportContentApi.Model;
global using StockportContentApi.Models.Enums;
global using StockportContentApi.Models.Exceptions;
global using StockportContentApi.Repositories;
global using StockportContentApi.Services;
global using StockportContentApi.Utils;
global using StockportContentApiTests.Builders;
global using StockportContentApiTests.Unit.Builders;
global using StockportContentApiTests.Unit.Fakes;
global using System.Net;
global using System.Reflection;
global using Xunit;
global using IContentfulClient = Contentful.Core.IContentfulClient;
global using File = Contentful.Core.Models.File;
global using HttpClient = StockportContentApi.Http.HttpClient;
global using HttpResponse = StockportContentApi.Http.HttpResponse;
global using Document = StockportContentApi.Model.Document;