# OurCity Server

## Required Setup

- Ensure you have [Docker Desktop](https://www.docker.com/products/docker-desktop/).
- Ensure you have [.NET 9](https://dotnet.microsoft.com/en-us/download).
- Ensure you have .env files setup
  - The .env files should be in the /server folder
  - To run the dev environment, you will need .env.development (even if empty) or docker compose will error
  - To run the prod environment, you will need .env.production (even if empty) or docker compose will error

## Common Errors
- SQL Errors like "Relation does not exist"
  - You may need to run migrations when initially running the app to populate your database. It is NOT automatically done.

## Configuration

- For secrets, do not commit to appsettings.json.
  - On local machine, can just do with .env
    - To see the formatting of .env files, see .env.example
    - The .env files you add should correspond to the environment
      - e.g. .env.development, .env.production

## IMPORTANT NOTE: ALL COMMANDS ARE WRITTEN WITH PRESUMPTION YOU ARE IN THE /SERVER FOLDER

## Running app with Docker
### 🚨🚨🚨 Docker Desktop should be running, or these will not work. 🚨🚨🚨

### Development Environment (HMR)

1. (Re)Build iamge, and spin up .NET API and Postgres Docker containers in the background
    ```sh
    docker compose up -d --build
    ```

2. Run Migrations
    ```sh
    docker compose --profile migrate up ourcity.migrate.dev --build
    ```

3. If you successfully access our API documentation should at http://localhost:8000/scalar or http://localhost:8000/swagger, the server set up is complete. 


4. To clean up the Docker containers
    ```sh
    docker compose down
    ```

### Production Environment

API documentation is not available for production. 

**Recommended: Use the Docker images from DockerHub built by our CD pipeline.**

1. Pull the latest backend and migration images from DockerHub:
    ```sh
    docker pull itsmannpatel/ourcity-backend:<TAG>
    docker pull itsmannpatel/ourcity-migrate:<TAG>
    ```
    Replace `<TAG>` with the latest image tag (e.g., commit SHA).

2. Update `docker-compose.prod.yml` to use the correct image tags for backend and migration.

3. Navigate to the server dir and start the backend, migration, and database containers using the following command:
    ```sh
    docker compose -f docker-compose.prod.yml --profile migrate up -d
    ```

4. Test API at this endpoint[http://localhost:9000/Posts](http://localhost:9000).

5. To clean up the Docker containers:
    ```sh
    docker compose -f docker-compose.prod.yml down
    ```

**Note:**  
You do not need to build the images locally for production.  
The docker-compose.prod file is setup to use the images from DockerHub for consistency with the tested and deployed code.


## Continuous Deployment (CD)

We use GitHub Actions to automate building and publishing Docker images for the backend and migration runner.

- **Images are built and pushed to DockerHub** on manual workflow trigger.
- **Image tags** use the Git commit SHA for traceability.

### How to trigger CD

1. Go to GitHub → Actions → CD → "Run workflow" (manual trigger).
2. The workflow will build and push:
    - Backend: `itsmannpatel/ourcity-backend:<tag>`
    - Migration: `itsmannpatel/ourcity-migrate:<tag>`

### How to deploy

1. Update `docker-compose.prod.yml` to use the correct image tags.
2. Run:
    ```sh
    docker compose -f docker-compose.prod.yml --profile migrate up -d
    ```

See `.github/workflows/cd.yml` for the workflow definition.


## Tooling

### Get mandatory dotnet tools

**If you do not do this step, you may not be able to run some of the commands in this README.**

```sh
dotnet tool restore
```

### Create migrations

```sh
dotnet ef migrations add <migration-name> -p OurCity.Api
```

### Run the tests

NOTE: There's a chance tests might take a long time on first start due to setting up Testcontainers.

```sh
dotnet test
```

For running tests, you can also run by type of test / what it tests

```sh
dotnet test --filter "Type=Unit"
dotnet test --filter "Type=Integration"
dotnet test --filter "Domain=Comment"
etc
```

**Produce a coverage report:**

The following works for MacOS (verified). Other shells may need different separators between commands (e.g. ;)

In `/server`, run the following command to produce the coverage report:

```sh
dotnet test --collect:"XPlat Code Coverage" && dotnet reportgenerator -reports:"**/OurCity.Api.Test/TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html && open coveragereport/index.html
```
`/server/coveragereport/index.html` will contain the produced coverage report. 

Note: The coverage generation creates a TestResults entry in OurCity.Api.Test. If you don't delete, future runs for checking coverage might include them.

### Linting and formatting

Check formatting

```sh
dotnet csharpier check <file_path>
```

Run formatting

```sh
dotnet csharpier format <file_path>
```

Check analyzer errors (lint)

```sh
dotnet build -p lint=true
```