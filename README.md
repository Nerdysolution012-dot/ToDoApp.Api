# ToDoApp.Api Capstone Project

## Project Overview

ToDoApp.Api is an ASP.NET Core Web API for managing task lists, task items and tags. It uses Entity Framework Core with SQL Server, JWT authentication, role-based authorization and ownership-based authorization.

The application allows users to register, log in, create task lists, add tasks, create tags and assign tags to tasks. Members can only access their own task lists and task items, while Admin users can access resources belonging to any user.

## Technologies Used

* ASP.NET Core Web API
* .NET 8
* Entity Framework Core
* SQL Server
* JWT Bearer Authentication
* ASP.NET Core PasswordHasher
* Swagger/OpenAPI

## Database Relationships

### User and TaskList — One-to-Many

A single user can own several task lists, but each task list belongs to exactly one user.

The `UserId` property in the `TaskList` table is the foreign key that connects a task list to its owner.

This means one row in the `Users` table can be connected to many rows in the `TaskLists` table.

### TaskList and TaskItem — One-to-Many

A task list can contain many task items, but each task item belongs to one task list.

The `TaskListId` property in the `TaskItem` table is the foreign key that connects a task to its parent list.

Deleting a task list also deletes its related task items because cascade deletion is configured for this relationship.

### TaskItem and Tag — Many-to-Many

A task item can have several tags, and one tag can be assigned to several task items.

The many-to-many relationship is implemented using the `TaskTag` join entity.

`TaskTag` contains:

* `TaskItemId`
* `TagId`
* `TaggedAt`

`TaskTag` is treated as a full entity rather than a basic join table because it stores the date and time when the tag was assigned.

A unique composite index on `TaskItemId` and `TagId` prevents the same tag from being assigned to the same task more than once.

## JWT Authentication

When a user registers, their password is hashed using ASP.NET Core's `PasswordHasher<User>` before it is stored in the database.

When the user logs in, the submitted password is verified against the stored password hash.

After a successful registration or login, the API generates a JWT containing the following claims:

* User ID
* Email address
* Role

The JWT is signed using the configured secret key.

For protected requests, the JWT bearer authentication middleware reads the token from the HTTP authorization header:

```text
Authorization: Bearer <token>
```

The middleware validates:

* The token signature
* The token issuer
* The token audience
* The token expiration time

All TaskList, TaskItem, Tag and TaskTag endpoints use the `[Authorize]` attribute. A request without a valid JWT receives a `401 Unauthorized` response.

## Ownership Authorization

The `[Authorize]` attribute confirms that the caller is logged in, but it does not confirm that the requested task list or task item belongs to that caller.

For this reason, ownership checks are performed inside the service layer.

The controller reads the authenticated user's ID, email and role from the JWT claims and passes them to the service through a `CallerContext` object.

Before returning, updating or deleting a task list, the service compares the stored `TaskList.UserId` with the authenticated user's ID.

For task items, the service checks the owner of the task's parent task list.

If a Member attempts to access another user's resource, the API returns:

```text
403 Forbidden
```

Admin users bypass the ownership check and may access task lists and task items belonging to any user.

When a Member creates a task list, the owner ID is taken from the JWT claim rather than from the request body. This prevents a Member from creating a task list on behalf of another user.

## Validation Rules

The API uses Data Annotations for automatic validation, including:

* Required properties
* Email format
* Maximum string lengths
* Priority range from 1 to 3

The service layer handles business validation, including:

* Duplicate email addresses
* Duplicate tag names
* Missing related users
* Missing task lists
* Past due dates
* Duplicate tag assignments
* Ownership validation

## Main HTTP Status Codes

* `200 OK` — successful GET request or login
* `201 Created` — successful registration or resource creation
* `204 No Content` — successful update or deletion
* `400 Bad Request` — invalid data, missing related resource or past due date
* `401 Unauthorized` — missing or invalid JWT, or incorrect login details
* `403 Forbidden` — authenticated Member does not own the requested resource
* `404 Not Found` — requested task list, task item or tag does not exist
* `409 Conflict` — duplicate email, tag name or task-tag assignment

## Running the Project

Restore the packages:

```bash
dotnet restore
```

Create the migration:

```bash
dotnet ef migrations add InitialCreate
```

Apply the migration:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

Open Swagger using the URL displayed in the terminal:

```text
https://localhost:<port>/swagger
```

## Git Branch

The required submission branch is:

```text
capstone/todo-api
```
