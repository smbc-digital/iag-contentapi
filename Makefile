in_docker_machine = $(shell docker-machine env $(MACHINE_NAME))

.PHONY: help
help:
		@cat ./MakefileHelp

# Docker machine targets: (manage the environment to run docker locally)
# ---------------------------------------------------------------------------------------
MACHINE_NAME = smbc

.PHONY: machine-create machine-start machine-stop machine-rm machine-ip machine-env

machine-create:
	docker-machine create --driver virtualbox \
		--engine-env HTTP_PROXY=$(HTTP_PROXY) \
		--engine-env HTTPS_PROXY=$(HTTPS_PROXY) \
		--engine-env NO_PROXY=$(NO_PROXY) \
		$(MACHINE_NAME)

machine-start:
	docker-machine start $(MACHINE_NAME)

machine-stop:
	docker-machine stop $(MACHINE_NAME)

machine-rm:
	docker-machine rm $(MACHINE_NAME)

machine-ip:
	docker-machine ip $(MACHINE_NAME)

machine-env:
	docker-machine env $(MACHINE_NAME)

# Project automation targets: (these run in docker)
# ---------------------------------------------------------------------------------------
PROJECT_NAME = StockportContentApi
CONTAINER_NAME = content
IMAGE = smbc/contentapi
TAG = latest
APP_VERSION ?= $(GO_PIPELINE_LABEL)
AWS_DEFAULT_REGION ?= eu-west-1
AWS_ACCOUNT ?= 390744977344
DOCKER_REPOSITORY = $(AWS_ACCOUNT).dkr.ecr.$(AWS_DEFAULT_REGION).amazonaws.com

.PHONY: build run clean
build:
	git rev-parse HEAD > src/$(PROJECT_NAME)/sha.txt
	echo $(APP_VERSION) > src/$(PROJECT_NAME)/version.txt
	./docker.sh build $(IMAGE) $(TAG) Dockerfile

run: clean
	eval "$(in_docker_machine)" ; \
	docker run --name $(CONTAINER_NAME) \
		-p 5001:5001 \
		-e HTTP_PROXY=$(HTTP_PROXY) \
		-e HTTPS_PROXY=$(HTTPS_PROXY) \
		-e NO_PROXY=$(NO_PROXY) \
		-e HEALTHYSTOCKPORT_ACCESS_KEY=$(HEALTHYSTOCKPORT_ACCESS_KEY) \
		-e HEALTHYSTOCKPORT_SPACE=$(HEALTHYSTOCKPORT_SPACE) \
		-e STOCKPORTGOV_ACCESS_KEY=$(STOCKPORTGOV_ACCESS_KEY) \
		-e STOCKPORTGOV_SPACE=$(STOCKPORTGOV_SPACE) \
		$(IMAGE):$(TAG)

clean:
	eval "$(in_docker_machine)" ; \
	docker rm -f $(CONTAINER_NAME) ; exit 0

# Deployment targets: (these push to Amazon ECR and EB, and require AWS creds)
# ---------------------------------------------------------------------------------------
AWS_DEFAULT_REGION = eu-west-1

.PHONY: tag login push publish docker-clean
tag:
	docker tag $(IMAGE) $(DOCKER_REPOSITORY)/$(IMAGE):$(APP_VERSION)

login:
	eval $$(aws --region $(AWS_DEFAULT_REGION) ecr get-login)

push: login
	docker push $(DOCKER_REPOSITORY)/$(IMAGE):$(APP_VERSION)

publish: build tag push

docker-clean:
	@rm -rf ~/.docker/config.json
	docker rmi $(IMAGE):latest
	docker rmi $(DOCKER_REPOSITORY)/$(IMAGE):$(APP_VERSION)
