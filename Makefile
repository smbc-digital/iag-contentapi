.PHONY: help
help:
		@cat ./MakefileHelp

APPLICATION_ROOT_PATH = ./src/StockportContentApi
APPLICATION_TESTS_PATH = ./test/StockportContentApiTests

.PHONY: clean
clean: 
	rm -rf $(APPLICATION_ROOT_PATH)/bin

.PHONY: build
build:
	dotnet restore; cd $(APPLICATION_ROOT_PATH); dotnet build
	
.PHONY: test
test:
	cd $(APPLICATION_TESTS_PATH); dotnet test

.PHONY: run
run:
	cd $(APPLICATION_ROOT_PATH); dotnet run

.PHONY: restore
restore:
	dotnet restore

.PHONY: publish
publish:
	cd $(APPLICATION_ROOT_PATH) && dotnet publish -c Release -o publish

.PHONY: unit-test
unit-test:
	cd $(APPLICATION_TESTS_PATH); dotnet test

# ---------------------------------------------------------------------------------------
# -- Unit tests coverage
# ---------------------------------------------------------------------------------------
.PHONY: coverage
coverage:
	cd $(APPLICATION_TESTS_PATH);rm TestResults -r -f
	dotnet build
	dotnet test -l "console;verbosity=normal" -p:CollectCoverage=true -p:CoverletOutputFormat=\"opencover\" -p:CoverletOutput=TestResults/Coverage.xml -p:SkipAutoProps=true /p:Exclude=\"[*]StockportContentApi.Builders*,[*]StockportContentApi.Controllers*,**/Constants/*,[*]StockportContentApi.ContentfulModels*,[*]StockportContentApi.ManagementModels*,[*]StockportContentApi.Enums*,[*]StockportContentApi.Models.Enums*,[*]StockportContentApi.Models.Exceptions*\" -p:ExcludeByAttribute="ExcludeFromCodeCoverage"

# ---------------------------------------------------------------------------------------
# -- Unit tests coverage with threshold
# ---------------------------------------------------------------------------------------
.PHONY: coverage-threshold
coverage-threshold:
	cd $(APPLICATION_TESTS_PATH);rm TestResults -r -f
	dotnet test -l "console;verbosity=normal" -p:CollectCoverage=true -p:CoverletOutputFormat=\"opencover\" -p:CoverletOutput=TestResults/Coverage.xml -p:SkipAutoProps=true /p:Exclude=\"[*]StockportContentApi.Builders*,**/Constants/*,[*]StockportContentApi.ContentfulModels*,[*]StockportContentApi.ManagementModels*,[*]StockportContentApi.Enums*,[*]StockportContentApi.Models.Enums*,[*]StockportContentApi.Models.Exceptions*\" -p:ExcludeByAttribute="ExcludeFromCodeCoverage" -p:Threshold=$(threshold)

# ---------------------------------------------------------------------------------------
# -- Unit tests coverage report, opens index.html in Chrome
# ---------------------------------------------------------------------------------------
.PHONY: report
report:
	cd $(APPLICATION_TESTS_PATH); rm TestCoverageResults -r -f && \
	reportgenerator -reports:TestResults/Coverage.xml -targetdir:TestCoverageResults -reporttypes:Html && \
	start Chrome $$PWD/TestCoverageResults/index.html

# ---------------------------------------------------------------------------------------
# -- Unit tests coverage tools
# ---------------------------------------------------------------------------------------
.PHONY: coverage-tools
coverage-tools:
	cd $(APPLICATION_TESTS_PATH) && \
    dotnet tool install -g dotnet-reportgenerator-globaltool && \
    echo "Please ensure that the path environment variable us updated with `C:\Users\{​​​​​​​​​​​​​​​​​​​​​​your-user}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​\.dotnet\tools`" && \
    reportgenerator

# ---------------------------------------------------------------------------------------
# -- Unit tests clear coverage report
# ---------------------------------------------------------------------------------------
.PHONY: clear-report
clear-report:
	cd $(APPLICATION_TESTS_PATH);rm TestResults -r -f; rm TestCoverageResults -r -f
