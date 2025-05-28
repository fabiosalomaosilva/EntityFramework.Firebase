# API Documentation for EF Core Firestore Provider

## Overview

The EF Core Firestore Provider allows .NET applications to interact with Firebase Firestore using Entity Framework Core. This document outlines the API provided by the Firestore Provider, detailing the key classes, methods, and functionalities available for developers.

## Key Classes

### FirestoreDbContextOptionsBuilderExtensions

- **Method**: `UseFirestore`
  - **Parameters**:
    - `string projectId`: The ID of the Firestore project.
    - `string serviceAccountPath`: The path to the Service Account JSON file for authentication.
  - **Description**: Configures the `DbContext` to use Firestore as the database provider.

### FirestoreDbContextOptionsExtension

- **Properties**:
  - `string ProjectId`: The ID of the Firestore project.
  - `string ServiceAccountPath`: The path to the Service Account JSON file.

### FirestoreProviderServices

- **Description**: Implements the services required for the Firestore provider, including dependency injection and service configuration.

### FirestoreDatabaseProvider

- **Description**: Manages interactions with Firestore, handling database operations such as querying and saving data.

### FirestoreClientFactory

- **Description**: Responsible for creating and configuring the Firestore client using the provided credentials.

### FirestoreCommandExecutor

- **Description**: Contains logic for executing commands against Firestore, including Add, Update, and Delete operations.

### FirestoreQueryProvider

- **Description**: Translates LINQ queries into Firestore queries, enabling developers to use familiar LINQ syntax.

### FirestoreQueryableMethodTranslators

- **Description**: Contains methods that convert LINQ expressions into Firestore queries.

### FirestoreQueryCompilationContext

- **Description**: Manages the compilation context for Firestore queries.

## CRUD Operations

The following CRUD operations are supported:

- **Create**: Use `Add` method to create new documents in Firestore.
- **Read**: Use LINQ queries to retrieve documents.
- **Update**: Use `Update` method to modify existing documents.
- **Delete**: Use `Delete` method to remove documents from Firestore.

## Query Support

The provider supports the following LINQ operations:

- `Where`
- `Select`
- `OrderBy`
- `Take`
- `Skip`
- `FirstOrDefaultAsync`
- `AnyAsync`
- `CountAsync`

## Authentication

Authentication is performed using a Service Account Key JSON file. The following example demonstrates how to configure authentication:

```csharp
var builder = new FirestoreProviderBuilder()
    .UseProject("my-firebase-project")
    .UseCredentials("path/to/serviceAccount.json");
```

## Conclusion

This API documentation provides an overview of the EF Core Firestore Provider, detailing the key components and functionalities available for developers. For further information, please refer to the README.md file or the source code.