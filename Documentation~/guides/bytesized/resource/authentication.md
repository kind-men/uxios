# Authenticating

## Overview

This guide shows you how to add (basic) authentication to your Uxios requests by providing a username and password, or
using a token or credentials. With this quick setup, you can seamlessly access protected API endpoints that require user
authentication.

---

## Steps

```csharp
// 1. Describe a `Resource` .. 
var resource = Resource<LoggedIn>
    // *At* a URL (as string) and
    .At("https://httpbin.org/basic-auth/username/password")
    // *As* a User - identified by a Username and Password
    .As("username", "password");

// 2. Start fetching the Value using `Promise<Post>`
Promise<LoggedIn> promise = resource.Value;

// 3. Declare what needs to happen on success - a lambda is used here, but a method works too 
promise.Then(loggedIn => Debug.Log(loggedIn.authenticated));

// 4. Declare what needs to happen on success - a lambda is used here, but a method works too 
promise.Catch(exception => Debug.LogError(exception.Message));
```

---

## Highlights

- The `As` method on a Resource helps you to describe "as what user" you want to interact with the Resource
- You can chain a series of options that describe the Resource (`At` and `As` in this case)

---

## Quick Tips

- You can also pass an instance of `Credentials` to the `As` method, for example a `BearerTokenCredential`.
- The `Exception` in the `Catch` part of the promise is an instance of `KindMen.Uxios.Error`, you can get more 
  information there.
- Authentication failures also end up in the `Catch` section - and they can easily be identified by their type: 
  `KindMen.Uxios.Errors.Http.AuthenticationError` or its subtypes for even more specificity.