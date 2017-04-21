# StockportContentApi

## Building the app
* To build and run tests:

```
make build
```

## Running the app
* You would need to set up the following environment variables to run the app
(these need to correspond with the business Id that you use in the webapp):
  - {`BUSINESS_ID`}_ACCESS_KEY
  - {`BUSINESS_ID`}_SPACE
* The app can run several different business Ids at once, as long as the corresponding
business Ids are provided.

### Configuration
The app has three config files which are kept in `src/StockportContentApi/app-config/`

1. `appsettings.json` - this is for generic application config.
2. `appsettings.{ASPNETCORE_ENVIRONMENT}.json` - this is for environment specific config, i.e. external service settings.
3. `injected/appsettings.{ASPNETCORE_ENVIRONMENT}.secrets.json` - this is for secret config, i.e. the Contentful space keys and access keys ([template found here](src/StockportContentApi/app-config/injected/readme.md)).

### How to run
```
make run
```

### Package
make win-package

## Contentful Integration
* The Content Api acts as a translation layer between [Contentful](contentful.com) and the webapp.

```
https://cdn.contentful.com/spaces/SPACE_KEY/entries?access_token=ACCESS_TOKEN&content_type=CONTENT_TYPE
```

* `SPACE_KEY`: Corresponds to the contentful space key
* `ACCESS_TOKEN`: Corresponds to the contentful space access token
* `CONTENT_TYPE`: This relates to the content type that is stored within Contentful.

Both the `SPACE_KEY` and `ACCESS_TOKEN` are gained from the businessId specific environment variables defined above. This allows for businessId specific data to be retrieved from Contentful.

### Prerequisites:
This app runs using at least .Net Core SDK version 1.0.0-preview2-003121.

## Postman tests

### Stockport gov
[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/00f1a874443afbd38801)

### Healthystockport
[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/5e7a2148c69eaf36d18e)
