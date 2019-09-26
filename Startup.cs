using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

namespace dappersqlite
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseStaticFiles();

      // Scaffold the database schema
      var connectionString = new SqliteConnectionStringBuilder() { DataSource = "app.db" }.ToString();
      using (var sqliteConnection = new SqliteConnection(connectionString))
      {
        sqliteConnection.Open();
        using (var createThingTableCommand = sqliteConnection.CreateCommand())
        {
          createThingTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS Things (Id INTEGER PRIMARY KEY, Name TEXT)";
          createThingTableCommand.ExecuteNonQuery();
        }
      }

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGet("/", async context => await context.Response.WriteAsync("Hello World!"));

        endpoints.MapGet("/new", async context =>
        {
          using (var sqliteConnection = new SqliteConnection(connectionString))
          {
            await sqliteConnection.OpenAsync();
            await sqliteConnection.ExecuteAsync("INSERT INTO Things (Name) VALUES (@Name)", new { Name = "test " });
            await context.Response.WriteAsync("OK");
          }
        });

        endpoints.MapGet("/list", async context =>
        {
          using (var sqliteConnection = new SqliteConnection(connectionString))
          {
            await sqliteConnection.OpenAsync();
            var gridReader = await sqliteConnection.QueryMultipleAsync("SELECT * FROM Things");
            var things = await gridReader.ReadAsync<Thing>();
            await context.Response.WriteAsync(JsonSerializer.Serialize<IEnumerable<Thing>>(things));
          }
        });
      });
    }
  }

  public class Thing
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }
}
