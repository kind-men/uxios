# Contributing to Uxios

Thank you for your interest in contributing to Uxios! We welcome improvements, bug fixes, and documentation
enhancements. Please follow the steps below to ensure a smooth and effective contribution process.

## How to Contribute

1. **Fork the Repository** Begin by forking the Uxios repository to create your own copy.

2. **Create a Branch** Work on a separate branch for each change. This keeps your contributions organized and makes it
   easier for us to review.

    ```bash
    git checkout -b feature/your-feature-name
    ```

3. **Make Changes and Add Tests**

    * Code Changes: All changes should be covered by PlayMode tests and, preferably, EditMode tests to ensure robust
      functionality.
    * Documentation: If you update or add to the documentation, please do so in the Documentation~ folder.

4. **Test Your Changes** Ensure all tests pass before you submit your pull request.

5. **Verify Documentation Changes** If you made changes to the documentation, you can preview them locally using the
   following command:

   ```bash
   npm run watch
   ```

   This command runs mkdocs in watch mode using Docker. Ensure Docker is installed before running this command. Any
   changes in the Documentation~ folder will be visible instantly.

6. **Submit a Pull Request** Once your changes are complete and tested, push your branch and submit a pull request.
   Provide a clear description of the changes, explaining the purpose and functionality of your modifications.

## Code Style and Guidelines

* Follow existing code conventions.
* Document your code where necessary, especially for complex logic or public APIs.

Thank you for helping make Uxios better for everyone! If you have any questions, please reach out or open an issue to
discuss your ideas before making significant changes.