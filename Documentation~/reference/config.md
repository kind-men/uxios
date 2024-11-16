# `Config` object
_[:material-github: Runtime/Scripts/Config.cs](https://github.com/kind-men/uxios/blob/main/Runtime/Scripts/Config.cs)_

The `Config` class in Uxios enables detailed customization for each HTTP request, giving you control over headers,
timeouts, request methods, and more. This configurability allows developers to precisely craft how each request is sent
and how responses are handled, which is especially useful in complex API integrations or when working with non-standard
server requirements.

However, direct interaction with `Config` is often unnecessary. Uxios provides sensible defaults and convenient methods
in the `Uxios` class and through `Resource` and `Collection` APIs. These features are designed to handle the common
requirements of most HTTP requests, enabling a more streamlined and efficient approach to network calls in Unity
applications.

## Example Use Cases

- **Custom Authentication**: When an API requires a unique authorization setup, `Config` can be used to add specific
  headers or authentication tokens.
- **Specialized Error Handling**: Use `ValidateStatus` to control how different HTTP status codes are interpreted.
- **Request Cancellation**: Attach a `CancelToken` to abort requests when they’re no longer needed, such as when a user
  navigates away from a view.

---

## Fields

### Url

**Type**: `Uri`

The primary URL for the request. This field must be set, and if the URL is a relative path (`UriKind.Relative`) and
`BaseUrl` is defined, then `BaseUrl` is prepended to `Url`.

### Method

**Type**: `HttpMethod`  
**Default**: `HttpMethod.Get`

Specifies the HTTP method (e.g., GET, POST, PUT). The default method is GET.

### BaseUrl

**Type**: `Uri`
**Default**: `null`

The base URL that combines with relative URLs in the `Url` field, or null to use `Url` directly.

### Headers

**Type**: `Headers`  
**Default**: Empty

Collection of headers to include in the request, allowing customization of standard headers like `Content-Type` and
custom headers.

### Params

**Type**: `QueryParameters`  
**Default**: Empty

Collection of query parameters to append to the URL, serialized automatically.

### Data

**Type**: `object`  
**Default**: `null`

The request body data, which can include types such as `string`, `byte[]`, or custom objects. Uxios serializes this to
JSON when supported.

### Timeout

**Type**: `int` (in milliseconds)  
**Default**: `0` (no timeout)

Maximum time to wait for a response before timing out. Exceeding this results in a timeout error.

### Auth

**Type**: `AuthenticationCredentials`
**Default**: `null`

Credentials for HTTP authentication, used for APIs requiring user authentication. This can be
`BasicAuthenticationCredentials` (for Basic Authentication), `BearerTokenCredentials` (to include an `Authorization`
header with a `bearer` token) or `QueryParameterCredentials` (to append a QueryParameter with a value to represent
the user).

### TypeOfResponseType

**Type**: `ExpectedTypeOfResponse`
**Default**: `ExpectedTypeOfResponse.JSON`

Defines the expected response type (e.g., JSON, binary, text) for automated deserialization.

### MaxRedirects

**Type**: `int`
**Default**: `5`

Sets the maximum number of allowed redirects for a request.

### ValidateStatus

**Type**: `Func<HttpStatusCode, bool>`  
**Default**: Accepts status codes in the 2xx range

Function for evaluating the HTTP status code. It’s invoked to determine if a response is considered valid, useful for
custom handling. The promise will be rejected if the request is considered invalid.

### CancelToken

**Type**: `CancellationToken`  
**Default**: `CancellationToken.None`

Allows request cancellation if the provided token is triggered, helpful in interrupting unnecessary requests. See
[Cancelling Requests](../guides/cancellation.md) for a guide how to use this.

---

## Methods

### Default

**Returns**: `Config`

Creates a default instance of Config with commonly used settings, which can be further customized.

```csharp
var defaultConfig = Config.Default();
```

### At

**Parameters**: `Uri url`  
**Returns**: `Config`

Sets the `Url` field to the specified URI and returns the updated configuration.

**Overload**: `At(Uri url, Uri baseUrl)`

Sets the `Url` field and assigns to the `BaseUrl` field if it is null, making it easy to set both in one call.

_These methods supports chaining, enabling a fluid interface for compact and readable configuration._

### UsingMethod

**Parameters**: `HttpMethod method`  
**Returns**: `Config`

Sets the HTTP method (e.g., GET, POST) for the request.

_This method supports chaining, enabling a fluid interface for compact and readable configuration._

### CancelUsing

**Parameters**: `CancellationTokenSource source`  
**Returns**: `Config`

Attaches a cancellation token, enabling request cancellation via the source token. See
[Cancelling Requests](../guides/cancellation.md) for a guide how to use this.

```csharp
var configWithCancellation = config.CancelUsing(cancellationSource);
```

_This method supports chaining, enabling a fluid interface for compact and readable configuration._

### AddParam

**Parameters**: `string key`, `object value`  
**Returns**: `Config`

Adds a query parameter to the `Params` collection (Query Parameters).

```csharp
var config = new Config().AddParam("id", 123);
```

_This method supports chaining, enabling a fluid interface for compact and readable configuration._

### AddHeader

**Parameters**: `string key`, `string value`  
**Returns**: `Config`

Adds a header to `Headers`.

```csharp
var config = new Config().AddHeader("Authorization", "Bearer token");
```

_This method supports chaining, enabling a fluid interface for compact and readable configuration._

---

## Usage Examples

### Basic Configuration

```csharp
var config = Config.Default()
    .BaseUrl(new Uri("https://api.example.com"))
    .AddHeader("Authorization", "Bearer myToken")
    .Timeout(30000);
```

### Using `CancelUsing` for Request Cancellation

```csharp
var cancellationSource = new CancellationTokenSource();
var config = Config.Default().CancelUsing(cancellationSource);
```

### Adding Query Parameters

```csharp
var config = new Config().AddParam("page", 1).AddParam("sort", "asc");
```

---

## Summary

The `Config` class allows extensive customization of HTTP requests in Uxios. It offers control over URL settings,
authentication, timeout, and more, centralizing management and enabling effective API interactions.