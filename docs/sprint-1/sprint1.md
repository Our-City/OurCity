# Sprint 1 Worksheet

## 1. Testing Plan

[View our Testing Plan here](testing_plan.md)

## 2. Unit / Integration / Acceptance Testing

### Backend:
The backend is missing unit tests for users and comments.

This is because we felt that there was less importance towards them as it was just using mocking tools. To make up for that and to ensure quality, we put more emphasis on the integration tests.

After running the report generation command (see [/server/README.md](/server/README.md) for instructions), you can access the coverage report file at `/server/coveragereport/index.html` (screenshot below). 

![Backend coverage report](/docs/sprint-1/images/backend_coverage.png)

### Frontend: 
The frontend is missing unit tests for a few components.

In general, the testing in the frontend was incomplete as we had run out of time to create them. To ensure quality, we were able to manually test these UI components which was simple because they only really checked that button presses emitted the correct signals and checked that text was rendered as expected.

After running the report geneation command (see [/webapp/README.md](/webapp/README.md) for instructions), you can access the coverage report file at `/webapp/coverage/index.html` (screenshot below)

![Frontend coverage report](/docs/sprint-1/images/frontend_coverage.png)


## 3. Testing Importance
Top 3 tests for each category:

### Unit Tests:

- [https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/PostUnitTest.cs#L72](https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/PostUnitTest.cs#L72): 
    ```cs
    GetPostByIdWithNoExistingPost()
    ```
    - Tests that when getting a post that does not exist with the given ID, it returns a failure message.
    - This ensures that this crucial method is able to handle incorrect data and can prevent silent failures and crashing pages.

- [https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/PostUnitTest.cs#L26](https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/PostUnitTest.cs#L26) 
    ```cs
    GetPostsWithExistingPosts()
    ```
    - Tests that a list of posts is returned when there are existing posts.
    - This is part of the core functionality of our app which confirms that the main screen of our app works as expected.

- [https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/PostUnitTest.cs#L239](https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/PostUnitTest.cs#L239) 
    ```cs
    CreatePostWhenValid()
    ```
    - Tests that when creating a post with valid inputs successfully returns a new post. 
    - This ensures that posts can be successfully created by the user and confirms that the data is in a valid state so as to not break any API calls later on.

### Integration Tests:
- [https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/IntegrationTests/UserIntegrationTests.cs#L88](https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/IntegrationTests/UserIntegrationTests.cs#L88)
    ```cs
    CreateUserWithExistingUsernameShouldFail()
    ```
    - Tests that the same username cannot be associated with multiple users in our database.
    - This enforces the constraint that usernames must be unique.

- [https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/IntegrationTests/PostIntegrationTests.cs#L294](https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/IntegrationTests/PostIntegrationTests.cs#L294)
    ```cs
    DoubleUpvoteShouldNegatePreviousUpvote()
    ```
    - Tests the case where if a post is upvoted, and then upvoted again, the original upvote is negated.
    - Ensures our upvoting logic is correct for edge cases like this.

- [https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/IntegrationTests/CommentIntegrationTests.cs#L129](https://github.com/seigfrieds/OurCity/blob/main/server/OurCity.Api.Test/IntegrationTests/CommentIntegrationTests.cs#L129)
    ```cs
    GettingCommentsByPostIdShouldReturnComments()
    ```
    - Tests that multiple comments which are created under a post are persisted in the database and retrieved successfully.
    - Ensures that if a post has multiple comments associated with the post, those comments are being returned properly by the database.


### Acceptance Tests:
We were not able create acceptance tests in Sprint 1.


## 4. Reproducible Environments

### Group 5 - Table Track
Try running their unit, integration, and other tests.
Take screenshots of:


### Successful runs (app working, tests passing):

![successful runs](/docs/sprint-1/images/reproduce_another_group/successful_runs.png)
![app working](/docs/sprint-1/images/reproduce_another_group/app_working.png)


### Unit tests:

**Frontend**:

Failures (error messages, logs):
![frontend unit tests](/docs/sprint-1/images/reproduce_another_group/frontend_unit_tests.png)

**Backend**:

No failures, all tests passed:
![backend unit tests](/docs/sprint-1/images/reproduce_another_group/backend_unit_tests.png)


### Integration tests:

Failures (error messages, logs):
![integration fail 1](/docs/sprint-1/images/reproduce_another_group/integration_fail_1.png)
![integration fail 2](/docs/sprint-1/images/reproduce_another_group/integration_fail_2.png)
![integration fail 3](/docs/sprint-1/images/reproduce_another_group/integration_fail_3.png)



### Clarity of documentation.
- Run instructions were clear and linked in the root README.

- I was able to run the backend successfully just by following the run instructions.

- To get the frontend running I had to do some digging to find instructions in `source/frontend/web/README.md`.

- It would have been helpful to either include this information or a link to it in the run instructions.


### Whether you could run it successfully and how long it took.
- I was able to get the backend and frontend running within 15 minutes.
- It would have taken a little longer if I didn’t already have docker desktop installed.

### Issues faced (especially relevant to distributed systems).
- Unit tests all ran successfully, but half of the integration tests failed.
