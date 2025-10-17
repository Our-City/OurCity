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

1. (Re)Build image, and spin up .NET API and Postgres Docker containers in the background
    ```sh
    docker compose -f docker-compose.prod.yml up -d --build
    ```

2. Run migrations
    ```sh
    docker compose -f docker-compose.prod.yml --profile migrate up ourcity.migrate.prod --build
    ```
    the server setup is completed here. 

3. To clean up the Docker containers
    ```sh
    docker compose -f docker-compose.prod.yml down
    ```



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