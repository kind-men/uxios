# Why Use Uxios Instead of UnityWebRequest?

Unity’s built-in `UnityWebRequest` API, while powerful, can be verbose and cumbersome for developers building
data-driven applications. Uxios streamlines HTTP networking in Unity by providing a more intuitive, feature-rich, and
developer-friendly interface that’s similar to popular web libraries like Axios. Here’s why Uxios is a great choice over
using `UnityWebRequest` directly:

---

## 1. Simplified Syntax and Readability

With `UnityWebRequest`, performing even simple HTTP requests often requires multiple lines of boilerplate code, handling
HTTP methods, parsing responses, and managing errors separately. Uxios consolidates this into easy-to-read
methods (`Get`, `Post`, `Put`, `Delete`) that automatically manage request and response processing.

For example, what might take a dozen lines of code with `UnityWebRequest` is reduced to a single line with Uxios, saving
time and reducing errors. Developers can quickly create, send, and handle requests without diving into the complexities
of Unity's networking layers.

---

## 2. Built-In Features for Effortless Networking

Uxios includes numerous built-in features that simplify networking in Unity, eliminating the need to write custom code
for common tasks. Some of these include:

- **Promises** for cleaner asynchronous operations, chaining success and error handling with `.Then` and `.Catch`
  syntax.
- **Automatic Serialization and Deserialization** using Newtonsoft JSON.net, so you can work directly with
  strongly-typed objects instead of manually parsing JSON.
- **Automatic Header Management** based on the data type, with `Content-Type` headers set automatically for JSON,
  binary, or text data.
- **Error Handling** integrated into each request, making it easier to handle and display errors to users.
- **Support for Basic Authentication** and custom headers, making it simple to connect to secured APIs.

With these features baked in, Uxios covers many common networking needs out-of-the-box, saving you from building or
maintaining your own custom code for each.

---

## 3. Less Boilerplate and Reusable Code

With `UnityWebRequest`, developers often find themselves writing repetitive code for tasks like setting up headers,
parsing JSON responses, and managing state. Uxios minimizes this by providing:

- **Reusable request methods** that handle these tasks automatically, reducing boilerplate and standardizing request
  behavior across your project.
- **Request Interceptors and Transformers**, which allow you to add custom logic to requests and responses before or
  after they are sent. This can be used to automatically add authentication tokens, log requests, or transform data
  without rewriting code for each request.

---

## 4. Efficient Resource Management

Uxios’s **Resource wrapper** and **caching capabilities** make it easy to manage frequently accessed data. This reduces
redundant network calls and helps with memory management, especially in applications that rely heavily on data
synchronization or repetitive requests.

By using Uxios, you eliminate the need to build custom caching logic or manage resource lifecycles, as Uxios does this
for you.

---

## 5. Consistent Data Handling with Automatic Serialization

Uxios automatically handles serialization of request bodies and deserialization of responses:

- **Send data**: Easily serialize request data from various formats (e.g., `string`, `byte[]`, custom objects) without
  writing manual converters. Custom objects are automatically serialized to JSON, making integration with JSON APIs
  seamless.
- **Receive data**: Response data is deserialized to strongly-typed objects, strings, textures, or sprites based on the
  expected type. This makes it easy to work with API data in Unity without having to manually parse JSON or manage
  texture loading.

---

In summary, Uxios empowers Unity developers by providing a simplified, flexible, and powerful HTTP client designed
specifically for Unity’s needs. By choosing Uxios over `UnityWebRequest` directly, you can streamline development, 
reduce boilerplate, and focus more on building features rather than managing networking code.