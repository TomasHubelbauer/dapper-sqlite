# BE

`dotnet new web`

`UseWebRoot("build")` to place CRA build artifacts in.

`UseStaticFiles()` to serve `build`.

`dotnet add package Dapper`

`dotnet add package Microsoft.Data.SQLite.Core`
https://docs.microsoft.com/en-us/windows/uwp/data-access/sqlite-databases

Seems that it is needed to add `dotnet add package Microsoft.Data.SQLite`
in order to bundle SQLite with the application.

## To-Do

### Finalize this
