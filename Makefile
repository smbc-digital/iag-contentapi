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