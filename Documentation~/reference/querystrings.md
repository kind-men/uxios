# QueryStrings

The `QueryString` utility in Uxios simplifies handling URL query strings in Unity applications. It provides
functionality for encoding, decoding, parsing, stringifying, and merging query strings, while supporting serialization
to and from objects. This utility is designed to work across Unity platforms, including WebGL.

## Overview

The `QueryString` utility replicates much of the functionality of Node.js's `querystring` API. It includes:

- **Encoding/Decoding**: Converts strings to/from URL-safe formats.
- **Parsing**: Converts query strings into key-value pairs.
- **Stringifying**: Converts key-value pairs into query strings.
- **Merging**: Combines multiple query strings or key-value collections.
- **Serialization/Deserialization**: Converts objects to query strings and vice versa.

## Methods

### 1. `Escape` and `Unescape`

#### **Escape**

Escapes a string for safe use in a query string.

```csharp
string escaped = QueryString.Escape("Hello World!");
// Output: "Hello+World%21"
```

#### **Unescape**

Unescapes a query string-encoded string.

```csharp
string unescaped = QueryString.Unescape("Hello+World%21");
// Output: "Hello World!"
```

---

### 2. `Encode` and `Decode`

#### **Encode**

Converts a `NameValueCollection` into a query string.

```csharp
var collection = new NameValueCollection
{
    { "key1", "value1" },
    { "key2", "value2" }
};

string queryString = QueryString.Encode(collection);
// Output: "key1=value1&key2=value2"
```

#### **Decode**

Parses a query string into a `NameValueCollection`.

```csharp
string query = "key1=value1&key2=value2";
NameValueCollection collection = QueryString.Decode(query);

// collection["key1"] == "value1"
// collection["key2"] == "value2"
```

---

### 3. `Merge`

Combines two query strings or `NameValueCollection` objects.

#### Example: Merging Query Strings

```csharp
string query1 = "key1=value1";
string query2 = "key2=value2";

string mergedQuery = QueryString.Merge(query1, query2);
// Output: "key1=value1&key2=value2"
```

#### Example: Merging `NameValueCollection` Objects

```csharp
var collection1 = new NameValueCollection { { "key1", "value1" } };
var collection2 = new NameValueCollection { { "key2", "value2" } };

NameValueCollection mergedCollection = QueryString.Merge(collection1, collection2);
// mergedCollection["key1"] == "value1"
// mergedCollection["key2"] == "value2"
```

---

### 4. `Serialize` and `Deserialize`

#### **Serialize**

Converts an object into a query string.

```csharp
var parameters = new
{
    key1 = "value1",
    key2 = "value2"
};

string queryString = QueryString.Serialize(parameters);
// Output: "key1=value1&key2=value2"
```

#### **Deserialize**

Parses a query string into an object.

```csharp
string query = "key1=value1&key2=value2";

var parameters = QueryString.Deserialize<dynamic>(query);
// parameters.key1 == "value1"
// parameters.key2 == "value2"
```

---

### Examples of Advanced Usage

#### **Array-like Keys**

Supports array-like keys with `[]` notation.

```csharp
string query = "items[]=apple&items[]=banana";

NameValueCollection collection = QueryString.Decode(query);
// collection["items"] contains "apple" and "banana"

string encoded = QueryString.Encode(collection);
// Output: "items[]=apple&items[]=banana"
```

#### **Conditional Compilation for WebGL**

The utility uses `UnityWebRequest` methods for escaping/unescaping in WebGL builds to address known issues
with `Uri.EscapeDataString`.

---

### Key Notes

- **Platform Support**: Conditional compilation ensures compatibility with WebGL and other Unity platforms.
- **Custom Serialization**: Use `JsonProperty` attributes to customize property names in serialized objects.
- **Limitations**: Nested objects and complex data structures are not supported in serialization.
