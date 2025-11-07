# **Sprint 2 Worksheet**


## **1\. Regression Testing**

* Describe how you run regression testing:  
  * All of our existing tests are run as part of our CI checks as our tests can run within approximately 2 minutes and do not take an extreme amount of time and resources just yet. With this short run time we feel there’s nothing to lose and everything to gain by getting full test coverage.  
  * These tests ensure that our new code being merged in doesn’t break our existing code.  
  * Our project is currently not large enough to shorten our list of tests for regression testing.   
  * Which tests are executed?  
    * Frontend:   
      * Component Unit Tests  
    * Backend:   
      * Post Unit Tests (Controller & Service layers)  
      * Media Unit Tests (Controller layer)  
      * Post Integration Tests  
      * User Integration Tests  
      * Authentication Integration Tests  
      * Me Integration Tests  
      * Comment Integration Tests  
  * Which tool(s) are used?  
    * Frontend:   
      * Vitest  
      * Vue-Test-Utils (component)  
    * Backend:  
      * Testcontainers  
      * Moq  
      * XUnit  
* Link to the regression testing script.  
  * [CI.yml](https://github.com/Our-City/OurCity/blob/main/.github/workflows/ci.yml), which just runs all the webapp and server tests  
* Include the latest snapshot of execution results.  
  * Frontend: ![Frontend coverage report](/docs/sprint-2/images/frontend_coverage_1.png)

  * Backend: ![Backend coverage report](/docs/sprint-2/images/backend_coverage.png)

---

## **2\. Testing Slowdown**

* Have you been able to keep **all** unit and integration tests from your test plan?  
  * Frontend:   
    * No, we were not able to get to integration tests. Unit tests covered only the functionality of all components, while end-to-end tests were prioritized over integration tests to ensure that the entire system worked as intended.  
  * Backend:   
    * No, we were not able to keep all units and integration. As a good part of our code contained simple logic, we focused more heavily on the whole endpoint integration testing to ensure all of our layers integrate well with one another, and successfully processes requests, then provides useful results.   
    * We were able to implement integration testing for our major endpoints, including those for Posts, Comments, Users, Authentication, and Me.   
* Have you created **different test plans** for different release types? Explain.  
  * No, we don’t have different test plans for different release types. Since our project is fairly small at this point in development, we would like to ensure all common checks no matter the size or impact of new code.  
    * **“And we dont even have a deployment”**

---

## **3\. Not Testing**

* What parts of the system are **not** tested?  
  * Frontend:   
    * Due to time constraints, we were not able to have integration tests which ideally would have been able to granularly test user interactions that affect multiple components.  
      * We instead relied on and prioritized our end-to-end tests to simulate user workflows, catching errors that would have been caught by integration tests.  
  * Backend:   
    * We haven’t done a separate unit test for each file in our repository layer as we tend to only deal with basic fetching, creating, updating, and deleting.   
      * They are still covered in our integration tests, but we believed unit tests to be of lower priority since we didn’t have much complex logic.   
    * We haven’t tested code related to authorization as it is not fully developed yet.  
    * We haven’t tested code related to media attachments except for its endpoints receiving and responding proper requests and responses.  
      * This was due to the lack of time, and the need for us to prioritize testing more impactful parts of our program.  
* Updated systems diagram:

![Systems diagram](/docs/sprint-2/images/systems_diagram.jpg)

* For each tier, indicate which layers are:  
  * Fully tested (80%+)  
  * Mostly tested (20–80%)  
  * Somewhat tested (0–20%)  
  * Not tested  
  * Integration Tests:   
    * Frontend:   
      * Not tested  
    * Backend:   
      * Controller: Mostly tested  
      * Service: Mostly tested  
      * Infrastructure/Repository: Mostly tested  
  * Unit Tests:   
    * Frontend:  
      * Components: Mostly tested  
    * Backend:  
      * Controller: Mostly tested  
      * Service: Mostly tested  
      * Infrastructure/Repository: Not tested  
  * End-to-end Tests:  
    * Mostly tested  
* Include coverage reports for tested tiers.  
  * Frontend:  
    * Note: coverage only shows for unit tests; coverage of end-to-end tests are not shown here.

![Frontend coverage 1](/docs/sprint-2/images/frontend_coverage_1.png)

![Frontend coverage 2](/docs/sprint-2/images/frontend_coverage_2.png)

* End-to-end

![E2E coverage 1](/docs/sprint-2/images/e2e_1.png)

![E2E coverage 2](/docs/sprint-2/images/e2e_2.png)

![E2E coverage 3](/docs/sprint-2/images/e2e_3.png)

![E2E coverage 4](/docs/sprint-2/images/e2e_4.png)

* Backend: 

![Backend coverage 1](/docs/sprint-2/images/backend_coverage_extended_1.png)

![Backend coverage 2](/docs/sprint-2/images/backend_coverage_extended_2.png)

![Backend coverage 3](/docs/sprint-2/images/backend_coverage_extended_3.png)

---

## **4\. Profiler**

* Run a profiler on your API while exercising **every endpoint**.  
* Identify:  
  * Which endpoint is the slowest.  
  * Whether the slowdown is fixable — and why/why not.  
* Include profiler output (linked or attached).

---

## **5\. Last Dash**

* What issues do you foresee in the **final sprint**?  
  * Lack of time to complete every feature that we initially proposed in sprint 0\.  
  * Trying to implement “nice to have” features or small changes that might cause delays in delivering core features on time   
  * Having too many other things to work on in our other classes with end of semester time crunch  
  * Having two presentations to deal with as well can take up time away from developing new features  
* We find that our concerns revolve around not having enough time, which has been an issue throughout all of our sprints. We plan to be more proactive and aggressive in ensuring we are on track to finish our project strong.

---

## **6\. Show Off**

* Each team member highlights their **best work** (code, UI, design, integration, mocks, etc.).  
* Remember: good refactored code should be simple and elegant.  
* **Each member must commit their own update** — commit logs will be checked.  
* Jade:   
  * Updated Post and Comment endpoint implementations for the new detailed API contract for proper and smooth api request handling and responses.  
    * Narrowed down to necessary response properties and DTOs to have a consistent and clear workflow.   
    * Updated endpoint paths to commonly be all lowercases and kabab-cases.  
  * Cleaning up unnecessary response properties and DTOs played a key role in keeping the connection between frontend and backend to be simple and clear.   
  * Further, ensuring all API paths to follow a common practice allows preventing potential mixups when working with API calls from the frontend.   
* Mann  
  * Implemented Media attachment endpoints with AWS S3 integration.   
* Nathan  
  * Worked on frontend UI and components and created frontend testing.  
* Harkeet  
  * Implemented the frontend API service layer which communicates with backend endpoints, and implemented domain models for representing application entities in the frontend.  
    * This is my best code because the domain models and API service layer are designed with decoupling in mind. Rather than using raw DTO responses from the backend in our frontend, the frontend uses mapper functions to map the DTO responses into domain models that can be used in components. This ensures that our frontend components are not tightly coupled to the backend responses.  
* Andre  
  * Setting a good foundation for the codebase in [https://github.com/Our-City/OurCity/commit/8c021ff303ceb16b9cdf41d4d62bec19f26867cd](https://github.com/Our-City/OurCity/commit/8c021ff303ceb16b9cdf41d4d62bec19f26867cd)   
    * E.g. pull request template, CI pipeline, dockerfiles \+ docker compose, migrations, testcontainers,  
  * As part of that foundation, a specific thing I think was really good was Working on the CI from an early point has helped catch regressions (testing) \+ code quality/formatting issues (running linters \+ formatters)  
* Will  
  * Created several frontend components and views including the profile page and login, register, and create post views.  
  * Implemented frontend post sorting and filtering logic and related components.  
  * Developed reusable utility components including dropdown, form, multiselect, and toolbar.
