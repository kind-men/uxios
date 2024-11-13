# Interceptors

Interceptors in Uxios are modular functions that can manipulate requests or responses at various stages of their
lifecycle. They provide powerful control over how HTTP requests and responses are processed, allowing developers to
implement custom logic such as authentication, logging, and error handling consistently across an application.

## Types of Interceptors

1. **Request Interceptors**: These interceptors run before a request is sent to the server. They are often used to
   modify headers (e.g., adding authentication tokens), transform request data, or log outgoing requests.

2. **Response Interceptors**: These interceptors run after the server’s response is received. They are commonly used for
   handling errors, transforming response data, or logging responses.

## Interceptor Priorities

In Uxios, interceptors are executed in a **priority-based order**, managed through a `PriorityList`. By default,
interceptors run in the order they were added, as they share the same default priority. However, you can influence this
execution order by assigning specific priorities to certain interceptors. For example, logging interceptors are given a
high priority (10,000), ensuring they run after other interceptors have made modifications. This allows loggers to
capture the final, modified request or response accurately.

## Example Use Cases

- **Authentication**: A request interceptor could attach a JSON Web Token (JWT) to each request’s headers, ensuring
  authenticated requests.
- **Caching**: A response interceptor could cache data for specific requests to optimize future load times.
- **Logging**: A logging interceptor with high priority can log the final version of requests and responses, capturing
  any transformations applied by prior interceptors.

## Interceptor Execution Flow

1. **Requests**: Request interceptors execute in ascending priority order, allowing higher-priority interceptors to run
   last. Modifications made by a request interceptor will be passed along to the next interceptor in the chain.

2. **Responses**: Response interceptors are also executed in ascending priority order, processing responses in a
   predictable sequence before reaching the final consumer.
