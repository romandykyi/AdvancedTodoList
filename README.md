# AdvancedTodoList
**AdvancedTodoList**, developed as my personal project, is a Web API for managing tasks in team settings, where each member can have a role and different sets of permissions. It's based on ASP.NET Core and Entity Framework Core with SQL Server running on Docker.

## Technologies and tools used
* .NET 8.0
* ASP.NET Core
* Mapster
* Entity Framework Core with SQL Server
* Swashbuckle
* FluentValidation
* SharpGrip.FluentValidation.AutoValidation

## Features

* **Collaborative to-do list management**: The API allows users to add new tasks (`TodoListItem`) to their to-do lists, update task details, mark tasks as completed/skipped, or delete them when they're no longer needed.
* **Invitation links**: Users can create invitation links for their to-do lists to allow other users to join them.
* **Search and Filtering**: Most endpoints allow users to specify filtering criteria and search prompts.
* **Authentication and Authorization**: This feature utilizes JWT and refresh tokens for secure user authentication and authorization.
* **Role Permissions and Priorities system**: Each to-do list has its own roles with different sets of permissions. Additionally, each role is assigned a priority level. Roles with the highest priority can modify and assign roles with lower priority.
* **Swagger documentation**: The API is documented using Swagger, providing a convenient way for developers to explore and understand the API endpoints.
* **Integration testing**: Integration tests utilize `Testcontainers` for comprehensive testing of API functionality and interactions.

## Architecture Overview
This project follows a pragmatic, clean-architecture inspired layout that balances separation of concerns with developer productivity. The solution preserves clear layer responsibilities and interfaces while choosing to use EF Core directly where it simplifies the codebase. 

### Core Layer (`AdvancedTodoList.Core`)

A layer that defines the application's domain and data access contracts. 

* Contains data access services interfaces (e.g. repositories).
* Defines the entities and value objects representing the domain model.
* Defines specifications to query and filter parameters.

### Application Layer (`AdvancedTodoList.Application`)

The business logic layer that depends on the Core.

* Provides services for business logic such as CRUD operations, task management, user authentication, authorization, etc.
* Implements validation logic using FluentValidation for ensuring data integrity and consistency.
* Contains data transfer objects and application options.

### Infrastructure Layer (`AdvancedTodoList.Infrastructure`)

Handles the direct EF Core data access. Depends on the Core.

* Handles data access and infrastructure-related concerns.
* Implements repositories and data access logic using Entity Framework Core.
* Manages database context and configuration.

### Presentation Layer (`AdvancedTodoList.WebApi`)

A layer that enables HTTP methods. Directly depends on the Core (filter and pagination parameters) and Application (service interfaces) layers. Indirectly depends on the Infrastructure, but only for mapping services to the interfaces for Dependency Injection.

* Implements RESTful API endpoints using ASP.NET Core Web API.
* Handles incoming HTTP requests, validates input data (using middleware), and delegates to services for business logic execution.
* Uses authentication and authorization mechanisms to secure endpoints.
* Facilitates error handling and response formatting.
* Exposes Swagger documentation.

## Design Choices Justifications

* Use of EF Core in the domain layer - despite being a clean-architecture violation, this choice significantly reduces mapping boilerplate and makes many queries simpler and more expressive by leveraging EF Core/LINQ directly against the model. It is justified here because the project commits to a single, stable data provider that is unlikely to change.
* Repository and Unit of Work implementations over `DbContext` - repository + UoW provide clear, testable contracts and centralize common data operations (pagination, specification application, common query helpers), which reduces duplication and keeps transactional boundaries explicit.
* Creating interfaces for services that have only one implementation - primarily improves testability (easy mocking/faking) and decouples callers from concrete implementations, which helps when composing dependencies in DI.

## Testing Approach
The `AdvancedTodoList` application uses two types of automated tests: unit tests and integration tests.

### Integration tests
Integration tests validate the interaction between different components and layers of the application.

#### Scope:
* **Endpoints layer**: API endpoints are tested using a simulated HTTP client to verify correct request handling, response generation, and error handling.
* **Service layer**: The data access layer is mocked, and services are tested to ensure proper business logic execution, error handling, and interaction with external dependencies.
* **Data access layer**: Repository implementations are tested with a database running on a test container to ensure that repositories interact correctly with Entity Framework Core and the MsSql database.

### Unit tests
Unit tests validate individual units of code in isolation from external dependencies.

#### Scope:
* **Core Logic**: Simple service methods, validators, specifications, and other core components.
* **Utilities and Helpers**: Utility classes, helper functions, and extension methods.

## Tools used for testing
* NUnit
* NSubstitutes
* Microsoft.AspNetCore.Mvc.Testing
* Testcontainers.MsSql
