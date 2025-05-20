using System.Net;
using System.Data.SqlClient;

var listener = new HttpListener();
listener.Prefixes.Add("http://localhost:8080/");
listener.Start();
Console.WriteLine("Listening on http://localhost:8080/");

while (true)
{
    var context = listener.GetContext();
    var request = context.Request;
    var response = context.Response;

    var idStr = request.QueryString["id"];
    if (!int.TryParse(idStr, out var id))
    {
        WriteResponse(response, "Invalid ID");
        continue;
    }

    var connectionString = "Server=localhost;Database=TestDB;Integrated Security=true;";
    var sql = "SELECT Name FROM Users WHERE Id = @Id";

    string name = null;
    using var connection = new SqlConnection(connectionString);
    using var command = new SqlCommand(sql, connection);
    command.Parameters.AddWithValue("@Id", id);
    connection.Open();
    var result = command.ExecuteScalar();
    name = result?.ToString();

    WriteResponse(response, name ?? "User not found");
}

static void WriteResponse(HttpListenerResponse response, string content)
{
    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
    response.ContentLength64 = buffer.Length;
    using var output = response.OutputStream;
    output.Write(buffer, 0, buffer.Length);
}
