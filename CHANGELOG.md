# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- AuthenticationError for all Authentication related errors
- UnauthorizedError when an unauthenticated user must authenticate to access the resource, or authentication 
  failed (HTTP 401)
- ForbiddenError when an authenticated user is not allowed to access the resource (HTTP 403)

### Changed

- Errors now can -and should- have a reference to an innerException for better traceability
- Json conversion is now done as a system-default response interception
- Interceptors are no longer static - but for the moment only the ones on the default instance are executed. A future
  change will make interceptors instance specific and copy an initial set from the default instance; similar 
  to `axios.create()`

### Fixed

- Errors generated as part of a response were created as a generic error instead of a specific error
- Bug where an empty response in an error would throw an "Invalid Cast" issue and blocking error handling

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