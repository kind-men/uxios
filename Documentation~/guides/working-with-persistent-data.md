# Working with Persistent Data

Unity’s `persistentDataPath` provides a location for storing data across sessions. This directory is
platform-independent, making it ideal for saving data locally without worrying about platform-specific paths.

!!! warning "Using `File.IO` is not recommended"

    Using `File.IO` is not recommended in cross-platform Unity projects, especially in WebGL, where `File.IO`
    lacks compatibility. Instead, leveraging Uxios with the `unity+persistent:///` scheme ensures smooth file handling
    across all platforms, including WebGL.

    We endeauvour to include all the hacks and workarounds that you normally would need to do, if you were to build this
    yourself.

---

## Step 1: Saving a File to Persistent Storage

1. **Prepare the File Data**  
   Define the content to be stored, such as a string, JSON, or binary data.

2. **Use the `unity+persistent:///` Scheme**  
   Create a URI using `unity+persistent:///` and the desired file path.

   ```csharp
   var fileUri = new Uri("unity+persistent:///myFolder/myFile.txt");
   ```

!!! info "Why Three Slashes?"  

    In `unity+persistent:///`, the three slashes indicate an empty authority (host) section, which is typical in file 
    URIs. This structure (e.g., `file:///`) designates a root path without a specific server or domain. The three-slash 
    format keeps the URI standard while targeting Unity’s `persistentDataPath` location.

3. **Send the Data Using Uxios**  
   Use Uxios’s `Put` method to save data to this URI:

   ```csharp
   var promise = uxios.Put(fileUri, data);
   ```

This approach mirrors a server file upload but instead saves to the local persistent path, ensuring cross-platform
compatibility.

---

## Step 2: Retrieving a File from Persistent Storage

1. **Define the URI for the File**  
   Specify the `unity+persistent:///` scheme with the file’s path.

   ```csharp
   var fileUri = new Uri("unity+persistent:///myFolder/myFile.txt");
   ```

2. **Retrieve with a GET Request**  
   Call Uxios’s `Get` method, passing the URI, to retrieve file contents.

   ```csharp
   var promise = uxios.Get<string>(fileUri);
   promise.Then(response => {
       Debug.Log("File content: " + response.Data);
   });
   ```

---

## Summary

Using the `unity+persistent:///` scheme:

- Simplifies cross-platform file management in Unity.
- Ensures compatibility with WebGL, mobile, and desktop by relying on `persistentDataPath`.