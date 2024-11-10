# Features

Designed with Unity developers in mind, Uxios brings the simplicity and flexibility of modern HTTP clients like Axios to
the Unity environment. Whether you're building data-driven applications for WebGL, mobile, or desktop, Uxios provides an
intuitive API to streamline HTTP requests, manage asynchronous data, and simplify error handling — all with minimal 
code.

Uxios offers a rich set of built-in features, from automatic serialization and deserialization to request interceptors
and response transformations, allowing you to focus more on functionality and less on networking boilerplate. Explore
the features below to see how Uxios can enhance your Unity project’s networking capabilities.

&nbsp;

<div class="grid cards" markdown>

-   #### Intuitive HTTP Requests for Unity

    ---
    Send HTTP requests with familiar `Get`, `Post`, `Put`, and `Delete` methods, inspired by popular web libraries like
    Axios, making it easy for Unity developers to integrate APIs into their projects.

-   #### Resource Wrapper for RESTful API's with caching

    ---
    Simplify API interactions with Uxios’s `Resource` wrapper, which provides built-in caching for frequently accessed
    endpoints, improving efficiency and reducing redundant network calls.
  
- #### Automatic Request Body Serialization

    ---
    Easily send data in multiple formats, including `string`, `byte[]`, and custom objects, which are automatically
    serialized to JSON using Newtonsoft JSON.net. This streamlines data handling and allows for quick integration with
    JSON-based APIs.


-   #### Strongly-Typed Responses with Automatic Deserialization

    ---
    Specify custom data types to directly map JSON responses into Unity objects using Newtonsoft JSON.net, reducing the
    need for manual parsing and enabling seamless data handling in Unity scripts.

-   #### Support for Binary, Text, Texture, and Sprite Responses

    ---
    Retrieve data in various formats, including JSON, binary, plain text, textures, and sprites. Uxios automatically
    deserializes and converts API responses into the appropriate format, so you can work with images, JSON objects, or
    binary data directly.

-   #### Promises for Asynchronous Operations  

    ---
    Use promises to handle async tasks, chaining actions and managing errors with ease. Built-in support for `.Then`
    and `.Catch` methods makes asynchronous workflows straightforward and keeps code clean.

-   #### Easy Support for Basic Authentication

    ---
    Integrate basic authentication effortlessly by providing credentials in the request configuration, making it easier
    to interact with protected API endpoints that require user credentials.

-   #### Automatic Handling of Headers for API Data Types

    ---
    Automatically add headers based on the type of object being sent or received. Uxios configures headers such
    as `Content-Type` for JSON or binary data, reducing manual setup and potential errors in data handling.

-   #### Request Cancellation
  
    ---
    Gain control over long-running or unnecessary requests with built-in support for request cancellation. This allows you
    to optimize performance and resource use, particularly in complex applications with frequent network activity.

-   #### Flexible Request and Response Customization

    ---
    Fine-tune requests with customizable headers, query parameters, and body content. Each response includes detailed
    metadata, like headers and status codes, for advanced control and debugging.

-   #### Built-In Error Handling  

    ---
    Catch and handle errors at each step in the request process, ensuring that your app provides clear feedback and robust
    error messaging without additional complexity.

-   #### Interceptor and Transformer Support

    ---
    Modify requests and responses on the fly with interceptors and transformers, allowing for custom handling like token
    authentication or data transformation before or after a request.

-   #### Complete API Integration Sample  

    ---
    Explore the included `Pokemon` sample, demonstrating a fully functional integration with an API, showcasing how to
    fetch data, populate UI elements, handle loading states, and manage errors—all using Uxios’s intuitive structure.

</div>