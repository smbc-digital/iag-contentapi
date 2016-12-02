FROM microsoft/dotnet:1.0.0-preview2-sdk

MAINTAINER Stockport Digital By Design

# Create an app folder to run the application in
RUN mkdir -p /opt/app/src \
    && mkdir -p /opt/app/test

# Install .Net Dependencies
COPY global.json /opt/app/
COPY src/StockportContentApi/project.json /opt/app/src/StockportContentApi/
COPY test/StockportContentApiTests/project.json /opt/app/test/StockportContentApiTests/

RUN cd /opt/app/src/StockportContentApi \
    && dotnet restore \
    && cd /opt/app/test/StockportContentApiTests \
    && dotnet restore

# Copy in code and tests
COPY src/StockportContentApi /opt/app/src/StockportContentApi
COPY test/StockportContentApiTests /opt/app/test/StockportContentApiTests

# Test
WORKDIR /opt/app/test/StockportContentApiTests
RUN dotnet test

# Expose port 5001
EXPOSE 5001

# Start the application web target on running container
WORKDIR /opt/app/src/StockportContentApi
ENTRYPOINT [ "dotnet", "run" ]
