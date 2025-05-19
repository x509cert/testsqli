using Microsoft.Data.SqlClient;
using Dapper;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string connectionString = "Server=localhost;Database=MyAppDb;Trusted_Connection=True;";

app.MapGet("/users", async (string? name) =>
{
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest("Name query parameter is required.");

    using var connection = new SqlConnection(connectionString);
    //var sql = "SELECT Id, Name, Email FROM Users WHERE Name = @Name";
    var sql = "SELECT Id, Name, Email FROM Users WHERE Name = " + name;

    var users = await connection.QueryAsync<User>(sql, new { Name = name });
    return Results.Ok(users);
});

app.Run();

record User(int Id, string Name, string Email);
