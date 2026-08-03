# ToDoApp.Api Capstone

ASP.NET Core Web API capstone implementing EF Core relationships, DTOs, validation, JWT authentication, role authorization and ownership authorization.

## Technology

- ASP.NET Core Web API (`net8.0`)
- Entity Framework Core with SQL Server
- Controllers inheriting from `ControllerBase`
- JWT bearer authentication
- `PasswordHasher<User>` for password hashing
- Swagger/OpenAPI for endpoint testing

## Relationships in my own words

### User to TaskList: one-to-many

One user can own several task lists, but every task list has one required `UserId`. `TaskList.UserId` is therefore the foreign key on the many side.

### TaskList to TaskItem: one-to-many

One list can contain several task items, while every task item belongs to exactly one list through `TaskItem.TaskListId`.

### TaskItem to Tag: many-to-many through TaskTag

A task can have several tags and a tag can appear on several tasks. `TaskTag` joins them using `TaskItemId` and `TagId`. It is an entity rather than a hidden join table because it also stores `TaggedAt`. A unique composite index prevents the same tag from being assigned to the same task twice.

## How JWT authentication works

Registration hashes the supplied password before the user is saved. Login verifies the supplied password against the stored hash. A successful register or login issues a signed JWT containing the user's ID, email and role.

The JWT bearer middleware reads the token from the `Authorization: Bearer <token>` header. It validates the signature, issuer, audience and expiry. Endpoints decorated with `[Authorize]` reject anonymous or invalid-token requests with `401 Unauthorized`.

## How ownership is enforced

`[Authorize]` only proves that the caller is authenticated. Controllers read the caller's ID, email and role from JWT claims and pass a `CallerContext` into the service layer.

Before returning, updating or deleting a list or task, the service queries the database for its owner. A Member whose ID does not match the stored owner receives `403 Forbidden`. Admins bypass the ownership restriction. When a Member creates a task list, its `UserId` is always taken from the JWT and not trusted from the request body.

## Secret configuration

Do not commit a real JWT key. From the project folder run:

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_AT_LEAST_32_CHARACTERS"
```

## Database setup

Update `DefaultConnection` in `appsettings.json` if necessary, then run:

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Open the Swagger URL printed in the terminal, normally:

```text
https://localhost:<port>/swagger
```

## Admin testing

All registrations intentionally create a `Member`. To test the required Admin bypass, register an account and update only that test user's `Role` column to `Admin` in SQL Server Management Studio. Restart or log in again to receive a JWT containing the new role.

## Main status codes

- `201 Created`: successful registration or creation
- `204 No Content`: successful update or deletion
- `400 Bad Request`: annotation validation, missing related row, or past due date
- `401 Unauthorized`: no token, invalid token, or invalid login
- `403 Forbidden`: signed-in Member trying to access another user's resource
- `404 Not Found`: requested list, task or tag does not exist
- `409 Conflict`: duplicate email, duplicate tag name, or duplicate tag assignment

## Required Git workflow

```bash
git init
git checkout -b capstone/todo-api
git add .
git commit -m "Build ToDo API capstone"
git remote add origin <your-repository-url>
git push -u origin capstone/todo-api
```

Add GitHub user `giddy11` as a repository collaborator from repository Settings > Collaborators.
