.PHONY: help
help:
		@cat ./MakefileHelp

require-envs-%:
	@for i in $$(echo $* | sed "s/,/ /g"); do \
	  if [[ -z $${!i} ]]; then echo "[FAIL] Environment variable $$i is not set." && FAIL=yes; fi \
	done; \
	if [[ -n $$FAIL ]]; then echo "Aborting..." && exit 1; fi

# ---------------------------------------------------------------------------------------
PROJECT_NAME = StockportContentApi

APPLICATION_ROOT_PATH = ./src/StockportContentApi
APPLICATION_PUBLISH_PATH = $(APPLICATION_ROOT_PATH)/publish/

APP_VERSION ?= $(BUILD_NUMBER)

.PHONY: build
build: clean dotnet-restore dotnet-test version publish-app package-app

.PHONY: clean
clean: 
	rm -rf $(APPLICATION_ROOT_PATH)/bin
	
.PHONY: dotnet-restore
dotnet-restore:
	dotnet restore

.PHONY: dotnet-test
dotnet-test:
	cd test/StockportContentApiTests; dotnet test

.PHONY: test
test:
	cd test/StockportContentApiTests; dotnet test

.PHONY: build-and-test
build-and-test:
	dotnet restore; dotnet build; cd test/StockportContentApiTests; dotnet test

.PHONY: run
run:
	cd src/StockportContentApi; dotnet run

.PHONY: publish-app
publish-app:
	cd src/StockportContentApi; dotnet publish --configuration Release -o publish;

.PHONY: version
version:
	git rev-parse HEAD > src/$(PROJECT_NAME)/sha.txt
	echo $(APP_VERSION) > src/$(PROJECT_NAME)/version.txt

.PHONY: package-app
package-app:
	rm -f iag-contentapi.zip
	cd $(APPLICATION_PUBLISH_PATH); zip -r ../../../iag-contentapi.zip ./*
