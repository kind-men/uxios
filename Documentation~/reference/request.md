# `Request` Object

The `Request` class in Uxios represents the configuration and data used to send an HTTP request. This includes the
request URL, HTTP method, headers, query parameters, and any data being sent in the request body. The `Request` object
is generated based on the initial configuration (`Config`), and may also be modified by interceptors and transformers
before it is ultimately sent to the server.

## Overview of the `Request` Fields and Methods

The `Request` class provides several fields to capture key aspects of the request. Here’s a detailed look at each field
and method.

---

## Fields

### Url

**Type**: `Uri`

**Description**: The target URL for the request. This is the full URI where the request will be sent, including the base
URL and any relative paths provided in the configuration.

**Usage**: The `Url` is typically constructed from the `Config` object, where the base URL and endpoint are combined to
create a complete URI. The URL can also be modified by interceptors or query parameters added to the `QueryString`
field.

**Example**:

```csharp
Debug.Log("Request URL: " + request.Url);
```

### Method

**Type**: `HttpMethod`

**Description**: Specifies the HTTP method for the request (e.g., GET, POST, PUT, DELETE). The default value
is `HttpMethod.Get` unless specified otherwise in the configuration.

**Usage**: The HTTP method determines the type of action to perform on the server. For example, `GET` retrieves data,
while `POST` sends new data to the server. The method is defined in the `Config` object and can be overridden before
sending the request.

**Example**:

```csharp
if (request.Method == HttpMethod.Post)
{
    Debug.Log("This is a POST request.");
}
```

### Headers

**Type**: `Headers`

**Description**: A collection of HTTP headers included in the request, stored as key-value pairs. Headers provide
additional context for the server, such as authorization tokens, content types, or custom headers specific to the
application.

**Usage**: Headers are defined in the `Config` object and can be modified or added to by interceptors. Common headers
include `Authorization` for authentication, `Content-Type` for specifying the data format, and others as needed by the
server.

**Example**:

```csharp
if (request.Headers.ContainsKey("Authorization"))
{
    Debug.Log("Authorization header present.");
}
```

### QueryString

**Type**: `QueryParameters`

**Description**: Contains the query parameters for the request, stored as a collection of key-value pairs. These
parameters are appended to the URL as part of the query string, enabling dynamic data to be passed to the server.

**Usage**: Query parameters are constructed from the URL and the parameters specified in `Config`. The `QueryString` is
useful for filtering or customizing server responses, such as adding `?page=2` to request a specific page in a paginated
response.

**Example**:

```csharp
Debug.Log("Query Parameters: " + request.QueryString.ToString());
```

### Data

**Type**: `byte[]`

**Description**: Represents the body data to be sent with the request, typically used in `POST` or `PUT` requests. The
data is serialized into a byte array format to be compatible with the HTTP request and may contain JSON, text, or binary
data.

**Usage**: The `Data` field is set based on the `Config` object, where the data is provided. This data is then
serialized into the appropriate format (e.g., JSON) and stored as a byte array, allowing it to be sent to the server.
The `Content-Type` header is set accordingly.

**Example**:

```csharp
if (request.Data != null)
{
    Debug.Log("Request contains data to send.");
}
```

---

## Methods

### FromConfig

**Type**: `static Request FromConfig<TData>(Config config) where TData : class`

**Description**: Generates a `Request` object from the provided `Config` object, which holds initial settings for the
request, such as URL, headers, query parameters, data, and authentication. This method performs several steps:

- Constructs the `Url` by combining the base URL and endpoint.
- Extracts any query parameters from the URL and moves them to the `QueryString` field.
- Adds headers defined in `Config`, including any authorization headers for basic authentication.
- Serializes the request body data (if provided) into a byte array stored in `Data`.

**Usage**: This static method is called to initialize a `Request` object from a configuration. The `Request` can then be
further modified by interceptors or transformers before being sent.

**Example**:

```csharp
Request request = Request.FromConfig<MyDataClass>(config);
Debug.Log("Generated request URL: " + request.Url);
```

---

### ConvertToByteArray

**Type**: `static (string contentType, byte[] bytes) ConvertToByteArray<T>(object data) where T : class`

**Description**: Converts the provided `data` object into a byte array, setting the `Content-Type` based on the data
type. This method handles various data types:

- `byte[]`: Directly uses the data as binary with `Content-Type: application/octet-stream`.
- `string`: Converts to UTF-8 encoded text with `Content-Type: text/plain`.
- Other objects: Serializes to JSON using UTF-8 encoding with `Content-Type: application/json`.

**Usage**: This helper method is called internally to prepare the request body data for sending. The `Data` field in
the `Request` object is populated with the byte array returned by this method.

**Example**:

```csharp
var (contentType, dataBytes) = Request.ConvertToByteArray<MyDataClass>(myDataObject);
Debug.Log("Data Content-Type: " + contentType);
```

---

## Creating and Modifying a Request

The `Request` object is derived from the initial `Config` settings but may be modified by **interceptors** and *
*transformers** before it’s sent to the server. Here’s an overview of each:

- **Interceptors**: Interceptors can inspect and modify the `Request` before it is sent or the `Response` after it’s
  received. For example, interceptors can add custom headers, update authentication tokens, or log requests for
  debugging.

- **Transformers**: Transformers allow additional modifications to the request data, such as serializing complex objects
  differently or applying compression. Transformers can add flexibility to handle unique data structures or specific
  server requirements.

---

## Summary

The `Request` object in Uxios represents the setup for an HTTP request. Each part of the request—from the URL to the
headers and data—is carefully configured and can be modified by interceptors and transformers to meet specific
requirements.

- **Url**: The target URL, combining base and endpoint.
- **Method**: Specifies the HTTP method (e.g., GET, POST).
- **Headers**: Custom headers for the request.
- **QueryString**: Query parameters appended to the URL.
- **Data**: The serialized body data for the request, stored as a byte array.

The `Request` object in Uxios provides a flexible and detailed representation of an HTTP request, allowing for easy
customization and modification to meet the needs of complex applications.