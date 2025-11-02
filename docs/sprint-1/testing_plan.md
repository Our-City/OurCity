## Testing Goals and Scope  
We aim to test our backend, frontend, and API. However, we do have a higher priority for the backend logic and repository coverage as they would act as the main backbone of our application, and we need them to work as expected before putting it to practical use. Without a strong, properly functioning backend, the frontend api calls will not have as high impact. 

### Frontend
Each of the components should have blackbox unit and integration tests using Vue Test Utils and Vitest. Components will be tested with different inputs and checked that they are emitting the correct signals to the parent, and further checked for rendered text to be as expected.

### Backend
Unit tests will be written for service layer classes, testing the business logic
Integration tests will be written for interaction between service and unit tests, testing the business logic + how it actually gets persisted into the database
It is critical to test that our logic and requests successfully reach our database and retrieve expected data for the main functionalities of our application. 

### API
Actual HTTP endpoints for core resources defined in Sprint 1 (Users, Posts, Comments, Authentication, etc.) should be tested
Swagger UI can be used to test endpoints manually, with API documentation included
We want to test APIs to ensure that the requests sent by the client side would successfully be received, and that we send the proper response. 

## Testing Frameworks and Tools
### Frontend
- **Vue Test Utils**
    - It is the official testing suite for Vue.js projects, made by the same team that made Vue. 
    - They provide detailed documentation, test examples, and should naturally integrate well with our Vue.js frontend UI components. 
- **Vitest**
    - Created by the same team who built Vite hence supporting native Vue support, allowing a deep integration with our dev setup, offering faster and more seamless testing.

### Backend
- **XUnit**
    - It is officially supported and actively maintained by Microsoft, and integrated into .NET templates.
    - Runs tests in parallel by default, speeding up large test suites. 
- **Testcontainers**
    - Makes integration testing between service and database layer simpler by creating actual containers that include a database
    - Prevents the need for complicated environment configurations for local setups. 
- **Moq**
    - Needed to mock data for our unit tests.
    - Fully supports generics, interfaces, async methods. 

## Test Organization and Structure  
### Frontend `/webapp/src/`
- `/__tests__/unit/`
    - Names of files indicate which specific component is being tested
- `/__tests__/integration/`
    - Names of files indicate which specific component is being tested

### Backend `/server`
- `/OurCity.Api.Test/`
    - Root folder which hosts backend tests
- `/OurCity.Api.Test/UnitTests/`
    - Domain entity tests (Mocking objects, testing behaviour without API)
- `/OurCity.Api.Test/EndpointsTests/`
    - Endpoint related tests (API calls with a test client, testing controller layer)
- `/OurCity.Api.Test/IntegrationTests/`
    - Service layer -> Repository layer tests (Service layer interaction with database layer)

## Coverage Targets  
**API layer**
- 100% method coverage (mentioned in sprint 1 requirements)

**Backend Service (Logic) Layer**: 
- $\ge$ 80% branch coverage 
- $\ge$ 80% line coverage
- 100% class coverage with integration tests  (mentioned in sprint 1 requirements)

**Backend Repository Layer**
- 100% repository method coverage (database interactions)

**Frontend Logic**: 
- $\ge$ 80% branch coverage
- $\ge$ 80% line coverage

## Running Tests  
```bash
# Run server tests with coverage
# From the root directory, follow the exact commands to test: 
cd server
dotnet test

# Run web app tests with coverage
# From the root directory, follow the exact commands to test: 
cd webapp
npm i
npm test
```
## Reporting and Results

### How to generate and access test coverage reports:

**Backend `/server`**
- `/OurCity.Api.Test/TestResults/`
    - Stores test coverage reports from XUnit/Coverlet
    - `coverage.cobertura.xml` contains the coverage details
    - To produce test results and the coverage report, please follow instructions provided in the 'Run the tests' section in the `/server` [README.md](/server/README.md). There is a 'Produce a coverage report' subsection which provides the complete command and instructions.
    - Coverage report will be generated in `/server/coveragereport/index.html`

**Frontend `/webapp`**
- To generate a test coverage report for the frontend tests, the `npm test -- --coverage` command can be run.
- You may need to install the coverage package beforehand locally, or, the command may prompt you to install the package during execution if not already installed.
- Coverage report will be generated in `/webapp/coverage/index.html`

## Test Data and Environment Setup  
No specific configuration or setup required for testing other than the initial project docker container setups for the frontend and backend. 

## Quality Assurance and Exceptions  
In general, the backend integration tests were prioritized first. Due to time constraints, backend unit tests were limited to posts as these were the core functionality of our app, and since we were using mocks, we believed that it wasn’t necessary to continue writing unit tests for users and comments.

Frontend testing was difficult as we weren’t able to connect our API to the frontend, so integration tests weren’t possible to create and unit tests became limited. These components were mainly tested manually as there were not many possible ways to break the components (i.e. many were simply buttons that would emit a signal to the parent).

Code reviews and automated testing/linting via our CI pipeline is how we have maintained quality this sprint. Moving forward, we intend to build more rigorous testing suites (end-to-end testing, regression tests, smoke tests, etc.) to ensure the quality of our codebase is maintained as the overall complexity increases.

## Continuous Integration [Once set up]
Tests run automatically in the Github actions CI pipeline on every pushed commit in main and in every pull request to ensure code quality and consistency.

We have automated tests in the CI pipeline, which helps with consistency since all contributors code is tested and issues are caught before merging to the main branch.

Our tests are also run in Docker to ensure the tests are run in the same environment as production. In the CI pipeline, we also have formatting and code quality checks that helps with maintaining consistency.
