#if N_DATABASE_TESTS

using System;
using System.Threading.Tasks;
using N.Package.Database.Databases;
using N.Package.Database.Results;
using N.Package.Database.Statements;
using N.Package.Test.Runtime;
using N.Packages.Promises;
using SQLite;
using UnityEngine;

namespace N.Package.Database.Tests
{
  public class BasicOperationsTest : RuntimeTest
  {
    [RuntimeTest]
    public void TestBasicDatabaseOperations()
    {
      TestBasicDatabaseOperationsTask().Dispatch();
    }

    private async Task TestBasicDatabaseOperationsTask()
    {
      try
      {
        var connection = await new SqliteDatabase().Connect(":memory:");
        await PerformSomeQuery(connection);
      }
      catch (Exception error)
      {
        Failed(error);
        Debug.LogError(error);
      }

      Completed();
    }

    private async Task PerformSomeQuery(IDatabase _connection)
    {
      Log("Query request");
      await _connection.Execute(new Command()
      {
        Sql = @"
            CREATE TABLE IF NOT EXISTS Foo
            (
              FooId INTEGER
                PRIMARY KEY
              AUTOINCREMENT,
              Key   TEXT,
              Value TEXT
            );
        "
      });

      Log("Insert!");
      await _connection.Execute(new Insert<Foo>()
      {
        Objects = new[]
        {
          new Foo() {Key = "1", Value = "1"},
          new Foo() {Key = "1", Value = "2"},
          new Foo() {Key = "1", Value = "3"},
          new Foo() {Key = "1", Value = "4"},
        }
      });

      Log("PErforming query!");

      var results = await _connection.Execute(new Query<Foo>()
      {
        Sql = "SELECT * FROM Foo WHERE FooId > ?",
        Args = new object[] {1}
      }) as ResultSet<Foo>;

      Log($"Got {results.Objects.Count} objects");
      foreach (var record in results.Objects)
      {
        Log($"{record.FooId} -> {record.Key} {record.Value}");
        record.Value = "CHANGED VALUE!";
      }

      await _connection.Execute(new Update<Foo>()
      {
        Objects = results.Objects
      });

      var results2 = await _connection.Execute(new Query<Foo>()
      {
        Sql = @"
          SELECT
            FooId,
            Key   [Value],
            Value [Key]
          FROM Foo;        
        ",
      }) as ResultSet<Foo>;

      Log($"Got {results2.Objects.Count} objects");
      foreach (var record in results2.Objects)
      {
        Log($"{record.FooId} -> {record.Key} {record.Value}");
      }

      await _connection.Execute(new Command()
      {
        Sql = "DELETE FROM Foo;"
      });
    }

    public class Foo
    {
      [PrimaryKey, AutoIncrement]
      public int FooId { get; set; }

      public string Key { get; set; }
      public string Value { get; set; }
    }
  }
}

#endif