<h1>ToDo List Web API</h1>

A WebAPI for a ToDo List application using .NET 9.0.

<h2>Running locally</h2>

Run the following command to start in http profile.
```
dotnet run --project .\ToDoListWebAPI\ToDoListWebAPI.csproj --launch-profile http
```

Run the following command to start in https profile.
```
dotnet run --project .\ToDoListWebAPI\ToDoListWebAPI.csproj --launch-profile https
```

Run the following command to run the tests.
```
dotnet test .\ToDoListWebAPITests\ToDoListWebAPITests.csproj
```
<h2>Endpoints</h2>

Endpoint documentation is available at `https://localhost:<port>/swagger/index.html`
<h3>ToDoItem</h3>

**GET** `https://localhost:<port>/api/ToDoItem`

Retrieve all ToDo items.

**GET** `https://localhost:<port>/api/ToDoItem/{id}`

Retrieve ToDo item by id.

**GET** `https://localhost:<port>/api/ToDoItem/history`

Retrieve completed ToDo items sorted by completion date.

**POST** `https://localhost:<port>/api/ToDoItem`

Create a new ToDo item.

**PUT** `https://localhost:<port>/api/ToDoItem/{id}`

Update ToDo item by id.

**PUT** `https://localhost:<port>/api/ToDoItem/markDone/{id}`

Change ToDo item's status to done by id.

**DELETE** `https://localhost:<port>/api/ToDoItem/{id}`

Delete ToDo item by id.

<h3>ToDoList</h3>

**GET** `https://localhost:<port>/api/ToDoList`

Retrieve all ToDo Lists and their tasks.

**GET** `https://localhost:<port>/api/ToDoList/{id}`

Retrieve ToDo List and it's tasks by id.

**POST** `https://localhost:<port>/api/ToDoList`

Create a new ToDo List.

**PUT** `https://localhost:<port>/api/ToDoList/{id}`

Update ToDo List by id.

**DELETE** `https://localhost:<port>/api/ToDoList/{id}`

Delete ToDo List by id.
