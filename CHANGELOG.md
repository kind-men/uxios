# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.4.0] - 2025-07-07

### Changed

- BREAKING: All uses of NameValueCollection are replaced with QueryParameters objects because of excessive garbage 
  generation. In your code you should check if you use NameValueCollection objects to hold data on query parameters and  
  change that to QueryParameter instances. This also means that iterating the result of Encode, Decode, Serialize and 
  Deserialize will now return QueryParameter objects instead of strings. To get the actual values of a single query 
  parameter, use the Values field.

### Fixed

- Reduced the amount of garbage generated when interacting with QueryParameters and when using the QueryString utility
  class.

### Added

- QueryParameter class to represent a single QueryParameter with is, possibly multiple, values.

## [0.3.5] - 2025-06-03

### Fixed

- Sending a request with no data will no longer receive a content type of "application/octet-stream" and thus 
  potentially trigger a CORS Preflight request.
- Handling of content types when posting data has been improved to properly register the content type on the 
  UploadHandlerRaw and from that onto the Request's Headers.

## [0.3.4] - 2025-03-31

### Fixed

- `QueryString.Encode` throws a `NullValueException` when a `QueryParameters` object with a null value

## [0.3.3] - 2025-03-24

### Added

- A new `Header` value object to add headers to the Config object and Resource wrapper in an intuitive way
- `Resource` now supports the `Header` class as an argument to the `With` method set to have a convenient way to
  add headers using an intuitive interface
- `Header` supports an explicit case from `Tuple<string, string>` to `Header` to easily instantiate them.

## [0.3.2] - 2025-02-17

### Fixed

- Removed automatic setting of the application/octet-stream content type if a request's contents is null or a byte-array
  when retrieving a Byte Array, including a texture. This will always trigger a CORS preflight, and some webservers seem 
  ill-equipped to handle this.
- Only create an ExpectedTypeOfResponse object if none is present in the configuration; this way you can pass your own 
  options to certain responses, such as setting the Readable flag for a texture.

## [0.3.1] - 2025-02-10

### Fixed

- Removed automatic setting of the application/octet-stream content type if a request's contents is null or a byte-array
  when retrieving the file as a cached FileInfo. This will always trigger a CORS preflight, and some webservers seem 
  ill-equipped to handle this.

## [0.3.0]

### Added

- An uxios-specific User Agent string is now set by default instead of relying on the transport providing a user agent
- The Headers collection now features a basic series of constants describing headers that are added
- A Request Id is now available with Request objects, an internal id is used unless the user provides a header 
  X-Request-Id, in which case that will also internally be used as id for the request.
- `Uxios.AsCoroutine` - a wrapper around a promise that will allow it to be used in situations where you just _must_ 
  have a coroutine. _Don't use this when you don't need to, some libraries require coroutines_. See
  https://kind-men.github.io/uxios/explanation/coroutines/ for more information.
- Automatic cancellation of requests if Coroutines are stopped using StopCoroutine. A Keep Alive mechanism is introduced
  that will check whether a Coroutine / subprocess is alive every frame.
- Aborting a request will issue more specific Error: `RequestAbortedError`, which inherits from `ConnectionError`.
- Query strings can now be serialized and deserialized to and from structs/classes using `QueryString.Serialize` and
  `QueryString.Deserialize` for type-safe declarations of query parameters.

### Changed

- Cancellation - you must now use `Uxios.Abort` to manually abort a promise' HTTP request instead of passing a 
  CancellationToken and managing the CancellationTokenSource manually. See 
  https://kind-men.github.io/uxios/guides/cancellation/ for more information.
- Added an interface above the Response object to make it easier to replace or change the response in the future

### Removed

- Cancellation Tokens passed into the Config are now ignored - Uxios' new cancellation mechanism will completely manage
  the tokens and source, use `Uxios.Abort` to abort a running promise

### Fixed

- Downloading files using the FileInfo generic sometimes errored because application/octetstream was given as mimtype,
  but a webserver could return a 406 if it was configured to accept limited mime types for a file. To fix this, we
  do not pass an accept header at all but let the browser figure it out.

## [0.2.0] - 2024-11-18

### Added

- AuthenticationError for all Authentication related errors
- UnauthorizedError when an unauthenticated user must authenticate to access the resource, or authentication 
  failed (HTTP 401)
- ForbiddenError when an authenticated user is not allowed to access the resource (HTTP 403)
- Support for Templated URI's, you can prepare a templated URI that will resolve with given parameters
- Full support for cancellation sources/tokens
- Support for authentication in the Resource class using the 'As' method
- Support for multiple Transports based on scheme - this is an experimental feature to verify whether it is possible
  to load files from various locations based on the scheme
- `unity+persistent` scheme that allows for files to be loaded and stored to and from the persistentDataPath in Unity,
  even on WebGL

### Changed

- Errors now can -and should- have a reference to an innerException for better traceability
- Json conversion is now done as a system-default response interception
- Interceptors are no longer static - but for the moment only the ones on the default instance are executed. A future
  change will make interceptors instance specific and copy an initial set from the default instance; similar 
  to `axios.create()`
- Config is no longer cloneable - a new static method has been introduced that creates a new Config based on another
- TemplatedUri's Resolve method is more optimized to reduce garbage generation
- The "request" field and arguments in the Error classes has been renamed to "config" to better explain the difference
  between request objects and config objects
- Adding a new query parameter to a Resource class using the `With` method has its signature changed from 
  `KeyValuePair<string, string>` to the arguments `string`,`string` to simplify the API.

### Fixed

- Errors generated as part of a response were created as a generic error instead of a specific error
- Bug where an empty response in an error would throw an "Invalid Cast" issue and blocking error handling
- Various edge cases that would -or could- result in a null-value exception

## [0.1.3] - 2024-11-13

### Added

- Support for directly downloading files and returning a FileInfo object by passing the FileInfo type as a generic

### Changed

- A failure to parse the JSON response would result in an error without response data, but we _do_ want that to debug 
  the issue, Uxios now resolves the types in a second phase, enabling us to separate this out in a future change

## [0.1.2] - 2024-11-13

### Added

- `ConsoleLogger` middleware that will dispatch any request and response to the Debug.Log - for now internally used to 
  verify the concept, but intended for adding a Network Inspection tool similar to chrome
- Support for adding credentials to the query string by instantiating a `QueryParameterCredentials` object and passing it
  to the `Auth` field of a `Config` object

### Fixed

- `RequestInterceptor` was called after `Request` had been created - but must be before so that the interceptors can 
  change the `Config` object, and thus influencing `Request` creation.
- UnityWebRequest could not be found in QueryString.cs in WebGL builds

### Changed

- Refactored `UnityWebRequestTransport` to have common actions between all transports to be in `TransportActions`
- Introduced new `InMemoryTransport` for testing purposes
- Moved specialized `Response` classes for transports into `Transport` namespace and folder

## [0.1.1] - 2024-11-09

### Fixed

- Using the Sprite generic to translate URLs into Sprites work now

### Added

- Added a new Sample to demonstrate basic usage to tie Uxios onto a UI
- Support for authenticating with Bearer tokens when you instantiate BearerTokenCredentials and assign that 
  to Config.Auth

### Changed

- Timeout value is now in milliseconds - this value was in seconds and did not match how Axios works, or frequent
  uses of timeout.

## [0.1.0] - 2024-11-06

### Added

- Support for GET requests, including (de)serialization to and from JSON
- Support for downloading Texture2D's directly from online images
- Support for GETting byte arrays
- The Axios API was modelled and filled in for the most part
- A "Resource" class was introduced to provide a convenient API for Unity users who are not used to Web lingo
- QueryString handling is provided by a port from querystring.js - Unity WebGL does not have access to native C# functionality
- Error handling is gradually being fine-tuned to provide specific error classes when an exception happens
- Interceptors have been implemented and will now allow inspection and manipulation of the Config and Response
- Basic Authentication support is provided
- A TemplatedUri wrapper is introduced as a beta functionality
