# REST(ful) API's

## What is a REST(ful) API

In the world of APIs, **REST (Representational State Transfer)** is a popular architectural framework that provides
guidelines for structuring and interacting with networked resources. Quite a few API's are often referred to as 
“RESTful” because it's not a strict standard — implementations vary in adherence to REST principles, leading to 
differences across APIs.

**Richardson’s Maturity Model**, developed by Leonard Richardson and expanded upon by Roy Fielding, categorizes REST
implementations into four levels of maturity:

- **Level 0**: The API treats each call as an isolated request.
- **Level 1**: The API organizes resources using URIs to represent objects or entities.
- **Level 2**: The API uses standard HTTP methods (GET, POST, PUT, DELETE) to perform operations on resources.
- **Level 3**: The API includes hypermedia controls (HATEOAS) to allow clients to dynamically discover actions and
  resources.

**Uxios** is designed to provide out-of-the-box support up to **Level 2** of this model through its `Resource` and 
`Collection` classes, with partial support for **Level 3**. These classes offer a structured way to interact with
resources and collections in RESTful APIs.

Let's introduce these two briefly:

- **Resource**: A single entity or object, like a specific user or item. The `Resource` class in Uxios allows you to
  fetch, update, and delete individual resources.

- **Collection**: A group or list of resources, like a list of users or items. The `Collection` class in Uxios provides
  methods for interacting with and managing collections of resources.

Since REST is a flexible framework, Uxios covers many common patterns, but the specifics, such as 
**Content Negotiation**, may require customization. OpenAPI, for example, builds additional standards on top of REST to 
provide a more uniform interface that can be added on top of Uxios, but is not provided out of the box.

In the following chapters, we’ll explore how to use Uxios’s `Resource` and `Collection` classes to interact with RESTful
APIs in a structured and effective way.