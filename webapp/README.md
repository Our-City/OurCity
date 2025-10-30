# OurCity Frontend

## Required Setup

- Ensure you have [Docker Desktop](https://www.docker.com/products/docker-desktop/).
- Ensure you have [Node.js (>= v22) and npm](https://nodejs.org/en/download).
- Ensure you have .env files setup
  - To run the dev environment, you will need .env.development (even if empty) or docker compose will error
  - To run the prod environment, you will need .env.production (even if empty) or docker compose will error

## Configuration

Configuration is done through .env

- To add to .env:
  - env.d.ts for TypeScript typing
  - .env.example to show example of how it may be used
  - config.ts for usage within files
- .env files should follow the environment you are targeting
  - e.g. .env.development, .env.production

## Running app with Docker

### Development Environment (HMR)

1. (Re)Build image, and spin up Docker container in the background
    ```sh
    docker compose up -d --build
    ```

2. Webapp should be accessible at http://localhost:5173

3. To clean up the Docker container (-v is required to clean up the containers node_modules)
    ```sh
    docker compose down -v
    ```

### Production Environment

**Recommended: Use the Docker image from DockerHub built by our CD pipeline.**

1. Pull the latest frontend image from DockerHub:
    ```sh
    docker pull itsmannpatel/ourcity-frontend:<TAG>
    ```
    Replace `<TAG>` with the latest image tag (e.g., commit SHA ).

2. Update `docker-compose.prod.yml` to use the correct image tag.

3. Navigate to the webapp dir and start the frontend container:
    ```sh
    docker compose -f docker-compose.prod.yml up -d
    ```

4. The webapp should be accessible at [http://localhost](http://localhost) (maps to port 80).

5. To clean up the Docker container:
    ```sh
    docker compose -f docker-compose.prod.yml down
    ```

**Note:**  
You do not need to build the images locally for production.  
The docker-compose.prod file is setup to use the images from DockerHub for consistency with the tested and deployed code.

## Continuous Deployment (CD)

We use GitHub Actions to automate building and publishing Docker images for the frontend.

- **Images are built and pushed to DockerHub** on manual workflow trigger.
- **Image tags** use the Git commit SHA for traceability.

### How to trigger CD

1. Go to GitHub → Actions → CD → "Run workflow" (manual trigger).
2. The workflow will build and push:
    - Frontend: `itsmannpatel/ourcity-frontend:<tag>`

### How to deploy

1. Update `docker-compose.prod.yml` to use the correct image tag.
2. From the `/webapp` directory, run:
    ```sh
    docker compose -f docker-compose.prod.yml up -d
    ```

See `.github/workflows/cd.yml` for the workflow definition.

## Running app locally on machine

### 1. Install dependencies
```sh
npm install
```

### 2a. Compile and Hot-Reload for Development

```sh
npm run dev
```

### 2b. Build for production

To build files

```sh
npm run build
```

To run the built files

```sh
npm run build:preview
```

### 3. Webapp should be accessible at http://localhost:5173


## Tooling

### Type-Check

```sh
npm run typecheck
```

### Run Tests

Run the tests

```sh
npm run test
```

Run tests on code change (watching for changes)

```sh
npm run test:watch
```

Run the tests and generate a coverage report

```sh
npm test -- --coverage
```

### Linting and Formatting

Run lint

```sh
npm run lint
```

Run lint, and apply fixes

```sh
npm run lint:fix
```

Run format

```sh
npm run format
```

Run format, and apply fixes

```sh
npm run format:fix
```

## Programming Standards (WIP, add more)

- .vue files
  - \<script setup lang="ts"></script>
  - \<template></template>
  - \<style scoped></style>
- kebab-case? camelCase? PascalCase?

## TODO: Things to Establish

Here's a list of things that I think whoever the main frontend people can _consider_ looking forward.

- Responsive design
  - Not sure how to do, I tried a little with css 'rem', but if somebody has better suggestion can do that
- Component Library
  - Quick search for Vue component/styling frameworks/libraries
    - Vuetify, Quasar, PrimeVue, Element Plus, Nuxt, Shadcn, Bootstrap, and more
- Folder structure
  - What do you guys like that you have worked with in the past?
  - Obviously, structure can grow with the project, but can think about it
  - Possible examples
    - Slice code into features (e.g. /src/features/reports has /components, /composables, /services, /views)
    - Slice code by technical concern (e.g. /src/components, /src/views, /src/api)
    - Others (e.g. Atomic design)
- What do we want to test
  - Just helper .ts files? Component tests? E2E tests?
- Test Location
  - Should tests be colocated next to the files?
  - Should tests just be in a common "tests" folder?

Also, a list of things/patterns/libraries we may want to look into when needed

- API wrapper
  - e.g. Axios
    - e.g. can add interceptors
      - With interceptors, can log requests, inject data, attach auth tokens (if not doing cookie auth), common error handling, etc
- Query library (e.g. TanStack Query)
  - Can provide built in query states (e.g. return isLoading, isError, etc), caching, background data syncing,
- Global state management (e.g. Pinia)
- Form handler (e.g. FormKit)
- Runtime object validator (e.g. Zod)
- Toast notifications
- Error boundaries
