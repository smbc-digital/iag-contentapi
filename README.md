# StockportContentApi 

## Building the app
* To build and run tests:

```
make build test
```

## Running the app
* You would need to set up the following environment variables to run the app
(these need to correspond with the business Id that you use in the webapp):
  - ASPNETCORE_ENVIRONMENT with a value of **local**

### Configuration
The app has three config files which are kept in `src/StockportContentApi/app-config/`

1. `appsettings.json` - this is for generic application config.
2. `appsettings.{ASPNETCORE_ENVIRONMENT}.json` - this is for environment specific config, i.e. external service settings.
3. `injected/appsettings.{ASPNETCORE_ENVIRONMENT}.secrets.json` - this is for secret config, i.e. the Contentful space keys and access keys ([template found here](src/StockportContentApi/app-config/injected/readme.md)).

### How to run
```
make run and then visit http://localhost:5001
```

### Package
make publish-app, this will package the app in release mode and output it into src/StockportContentApi/publish dir

## Contentful Integration
* The Content Api acts as a translation layer between [Contentful](www.contentful.com) and the webapp.

```
https://cdn.contentful.com/spaces/SPACE_KEY/entries?access_token=ACCESS_TOKEN&content_type=CONTENT_TYPE
```

* `SPACE_KEY`: Corresponds to the contentful space key
* `ACCESS_TOKEN`: Corresponds to the contentful space access token
* `CONTENT_TYPE`: This relates to the content type that is stored within Contentful.

Both the `SPACE_KEY` and `ACCESS_TOKEN` are gained from the businessId specific app settings defined above. This allows for businessId specific data to be retrieved from Contentful.

## Dot net core version
This app runs using .Net Core SDK version 2.1.5.

## Swagger
Swagger is set up as an automated api documentation under /swagger/ui/index.html