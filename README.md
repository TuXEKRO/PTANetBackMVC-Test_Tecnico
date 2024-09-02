# Interview Project API

## Getting Started

### 1. Initializing Docker Compose

Follow these steps to set up and run the API and SQL Server using Docker Compose:

1. **Ensure Docker and Docker Compose are installed** on your machine.

2. **Navigate to the project directory** where the `docker-compose.yml` file is located.

   ```bash
   cd path/to/project
   ```

3. **Build and start the containers** by running the following command:

   ```bash
   docker-compose up --build
   ```

   This command will:
   - Pull the required Docker images.
   - Build the API application container.
   - Start both the API and SQL Server containers.
   - The API will be accessible at `http://localhost:8081/swagger`.

4. **Access the API** via a web browser at `http://localhost:8081/swagger`or using your preferred API client.

### 2. Running Tests

To run the tests for the Interview Project API, follow these steps:

1. **Ensure the containers are running** as described in the previous section.

2. **Navigate to the test project directory** named `interviewProject.Tests`.

   ```bash
   cd path/to/project/interviewProject.Tests
   ```

3. **Run the tests** using the .NET CLI:

   ```bash
   dotnet test
   ```

   This command will execute all the unit tests within the project and provide a summary of the test results in the terminal.

### Conclusion

With the API and SQL Server containers running, you can interact with the API endpoints and run tests to ensure everything is working as expected. For more detailed information on the API endpoints and usage, please refer to the API documentation.
