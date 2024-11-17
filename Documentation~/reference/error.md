# Error Classes

In Uxios, errors are handled through a base family of error classes that represent various failure scenarios. These
classes include the `Error` base class, as well as specific error types that match common HTTP and network issues. This
hierarchy allows developers to catch and manage specific error types more effectively, improving error handling and
debugging.

## Overview of Error Types

Errors in Uxios are grouped to align with UnityWebRequest’s `UnityWebRequest.Result` result types, with additional
categories to handle specific HTTP status families.

- **ConnectionError**: Represents connectivity-related issues (e.g., no internet connection, DNS resolution failure).
- **DataProcessingError**: Occurs when data processing fails, either due to network errors or issues with Uxios’s own
  processing, such as (de)serialization.
- **ProtocolError**: Relates to protocol-level errors like HTTP responses that signal a problem. This category is
  further divided into `HttpClientError` and `HttpServerError`:
    - **HttpClientError (4xx)**: Represents client-side HTTP errors (e.g., 404 Not Found, 400 Bad Request).
    - **HttpServerError (5xx)**: Represents server-side HTTP errors (e.g., 500 Internal Server Error, 503 Service
      Unavailable).

---

## Error Classes

### KindMen.Uxios.Error

The base class for all error types in Uxios. It contains details about the original request (`Config`) and the
response (`Response`) for more context.

**Fields**

- `Config`: The [`Config` object](config.md) representing the original request configuration.
- `Response`: The [`Response` object](response.md), which may contain status and data if a response was received.

### KindMen.Uxios.Errors.ConnectionError

Represents an error that occurs due to connectivity issues, such as a lost network connection or DNS resolution failure.
This type of error is particularly useful for catching network issues at the client level.

**Usage**: Typically occurs when the `UnityWebRequest.Result` is `ConnectionError`.

### KindMen.Uxios.Errors.DataProcessingError

Occurs when data processing fails. This includes errors from data parsing, serialization, and deserialization issues
within Uxios. Additionally, any failure to process or parse response data received from the server may also raise this
error.

**Usage**: Occurs when Uxios encounters data-related issues, either due to network problems or failures in internal data
handling, such as JSON deserialization.

### KindMen.Uxios.Errors.ProtocolError

The base class for HTTP protocol errors, capturing general issues related to the HTTP status code received from the
remote server.

**Fields**

- `Status`: The HTTP status code of the response, if available.

**Usage**: Raised when there is a problem at the HTTP protocol level, often signifying a client or server error reported
by the remote server.

### KindMen.Uxios.Errors.HttpClientError

A subclass of `ProtocolError` representing client-side errors (4xx HTTP status codes). These errors indicate issues
originating from the request, such as invalid parameters or unauthorized access.

**Usage**: Typically occurs when the server responds with a 4xx status code, such as 404 Not Found or 403 Forbidden.

### KindMen.Uxios.Errors.HttpServerError

A subclass of `ProtocolError` representing server-side errors (5xx HTTP status codes). These errors indicate that
something went wrong on the server, such as an internal server error or service unavailability.

**Usage**: Typically occurs when the server responds with a 5xx status code, such as 500 Internal Server Error or 502
Bad Gateway.
