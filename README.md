**AdvancedTodoList**, developed as my personal project, is a Web API for managing to-do lists in teams, where each member can have a role and different sets of permissions. It's based on ASP.NET Core and Entity Framework Core with SQL Server running on Docker.

# Technologies and tools used
* .NET 8.0
* ASP.NET Core
* Mapster
* Entity Framework Core with SQL Server
* Swashbuckle
* FluentValidation
* SharpGrip.FluentValidation.AutoValidation

# Features
* **Collaborative to-do list management**: The API allows users to add new tasks (`TodoListItem`) to their to-do lists, update task details, mark tasks as completed/skipped, or delete them when they're no longer needed.
* **Invitation links**: Users can create invitation links for their to-do lists to allow other users to join them.
* **Search and Filtering**: Most endpoints allow users to specify filtering criteria and search prompts.
* **Authentication and Authorization**: This feature utilizes JWT and refresh tokens for secure user authentication and authorization.
* **Role Permissions and Priorities system**: Each to-do list has its own roles with different sets of permissions. Additionally, each role is assigned a priority level. Roles with the highest priority can modify and assign roles with lower priority.
* **Swagger documentation**: The API is documented using Swagger, providing a convenient way for developers to explore and understand the API endpoints.
* **Integration testing**: Integration tests utilize `Testcontainers` for comprehensive testing of API functionality and interactions.

# Architecture Overview
The Advanced Todo List application follows a clean architecture. The main three layers are:

## Core Layer (`AdvancedTodoList.Core`)
* Contains services and repositories interfaces.
* Defines the entities and value objects representing the domain model.
* Provides services for CRUD operations, task management, user authentication, authorization, etc.
* Defines specifications to query and filter parameters.
* Implements validation logic using FluentValidation for ensuring data integrity and consistency.

## Infrastructure Layer (`AdvancedTodoList.Infrastructure`)
* Handles data access and infrastructure-related concerns.
* Implements repositories and data access logic using Entity Framework Core.
* Manages database context and configuration.
* Handles JWT and refresh tokens.
* Implements services and specifications.

## Application Layer (`AdvancedTodoList`)
* Implements RESTful API endpoints using ASP.NET Core Web API.
* Handles incoming HTTP requests, validates input data, and delegates to services for business logic execution.
* Uses authentication and authorization mechanisms to secure endpoints.
* Facilitates error handling and response formatting.
* Exposes Swagger documentation.

# Testing Approach
The `AdvancedTodoList` application uses two types of automated tests: unit tests and integration tests.

## Integration tests
Integration tests validate the interaction between different components and layers of the application.

### Scope:
* **Endpoints layer**: API endpoints are tested using a simulated HTTP client to verify correct request handling, response generation, and error handling.
* **Service layer**: The data access layer is mocked, and services are tested to ensure proper business logic execution, error handling, and interaction with external dependencies.
* **Data access layer**: Repository implementations are tested with a database running on a test container to ensure that repositories interact correctly with Entity Framework Core and the MsSql database.

## Unit tests
Unit tests validate individual units of code in isolation from external dependencies.

### Scope:
* **Core Logic**: Simple service methods, validators, specifications, and other core components.
* **Utilities and Helpers**: Utility classes, helper functions, and extension methods.

## Tools used for testing
* NUnit
* NSubstitutes
* Microsoft.AspNetCore.Mvc.Testing
* Testcontainers.MsSql
