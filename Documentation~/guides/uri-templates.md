# Using placeholders in URLs

Templated URIs in Uxios allow you to define dynamic URLs with placeholders that can be filled in at runtime. This
feature is especially useful when working with RESTful APIs that require parameters in the URL path. This guide will
show you how to use Templated URIs, as well as explain a key caveat when using them.

## Introduction

A Templated URI is a URI string with placeholders, such as `{id}`, that can be replaced with actual values at runtime.
Uxios immediately resolves these placeholders when they are passed to one of its actions (e.g., `Get`, `Post`). This
immediate resolution helps prevent unintended replacements by clearly differentiating between templated and regular
URIs. For example, if a URI includes characters resembling placeholders that aren't meant to be replaced, Uxios ensures
they remain unaffected.

### Example Use Cases

- Fetching a specific resource by ID.
- Building dynamic API paths with multiple parameters.

---

## Step-by-Step Instructions

### Step 1: Define a Templated URI

Start by defining a `TemplatedUri` with placeholders for the parameters you want to replace. In this example, `{id}` is
a placeholder for the resource ID in the URL path.

```csharp
TemplatedUri template = new TemplatedUri("https://jsonplaceholder.typicode.com/posts/{id}");
```

This URL defines `{id}` as a placeholder, which can be replaced with an actual value when the request is made.

### Step 2: Resolve the Templated URI with `With`

The simplest way to resolve the placeholder is by using the `With` method on the `TemplatedUri` itself. This method
directly supplies the necessary parameters, making it easy to replace placeholders without an intermediate configuration
object.

```csharp
TemplatedUri url = template.With("id", "1");
var promise = uxios.Get<ExamplePost>(url);
```

In this approach:

- A new `TemplatedUri` instance is created, Where the `{id}` placeholder will be replaced by the value `1` upon
  passing `url` to `uxios.Get`.
- This means that your original `template` remains unchanged and can be reused as a template for future requests.

This approach provides both flexibility and simplicity, allowing you to define reusable URI templates and resolve them
with different parameters as needed.

---

## Alternative: Use `Config` and `Using` for More Flexibility

For additional control, you can use a `Config` object to add parameters to the URI independently of the template, then
resolve the URI with `Using`.

1. Set up a `Config` object with the required parameters.

   ```csharp
   var config = new Config().AddParam("id", "1");
   ```

2. Use the `Using` method on the Templated URI, passing in the parameters from the `Config` object.

   ```csharp
   TemplatedUri url = template.Using(config.Params);
   var promise = uxios.Get<ExamplePost>(url, config);
   ```

In this approach:

- Upon passing `url` to `uxios.Get`, `{id}` is immediately replaced with the value from `config.Params`.
- Any resolved parameters are removed from the `Params` collection, allowing for reuse and cleaner configurations in
  defining query strings or by interceptors.

---

## Caveat: Immediate Resolution of Templated URIs

Templated URIs in Uxios are resolved as soon as they’re passed to an action like `Get` or `Post`. This immediate
resolution helps prevent unintended replacements in URIs that may contain placeholders resembling template properties,
which are not meant to be replaced. Using Templated URIs also ensures a clear distinction between dynamic and static
URIs.

---

## Full Code Example

Here’s the complete code example, demonstrating both methods of using Templated URIs with Uxios:

```csharp
using KindMen.Uxios;

public void FetchPost()
{
    // Define a templated URI with a placeholder for 'id'
    var template = new TemplatedUri("https://jsonplaceholder.typicode.com/posts/{id}");

    // Option 1: Use the 'With' method for direct parameter replacement
    var promiseDirect = uxios.Get<ExamplePost>(template.With("id", "1"));

    // Option 2: Use a Config object to add parameters and resolve the URI
    var config = new Config().AddParam("id", "1");
    var promiseWithConfig = uxios.Get<ExamplePost>(template.Using(config.Params), config);
}
```

---

## Summary

This guide showed how to use Templated URIs in Uxios for dynamic API requests:

- **Using `With`**: Directly replace placeholders with specific values for simplicity.
- **Using `Config` and `Using`**: Set parameters in a `Config` object and resolve placeholders for more flexibility.