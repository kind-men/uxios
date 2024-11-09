# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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