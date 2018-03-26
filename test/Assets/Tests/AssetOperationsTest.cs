using System;
using System.Threading.Tasks;
using N.Package.Database.Databases;
using N.Package.Database.Results;
using N.Package.Database.Statements;
using N.Package.Test.Runtime;
using N.Packages.Promises;
using SQLite;
using Tests;
using UnityEngine;

namespace N.Package.Database.Tests
{
  public class AssetOperationsTest : RuntimeTest
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

    private async Task PerformSomeQuery(IDatabase connection)
    {
      var foo = new FooSql();

      Log("Query request");
      await connection.Execute(foo.Get<Command>(FooSqlType.FooCreateTable));

      Log("Insert!");
      await connection.Execute(new Insert<Foo>()
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

      var query = foo.Get<Query<Foo>, Foo>(FooSqlType.FooFindByIdRange, q => q.Args = new object[] {1});
      var results = await connection.Execute(query) as ResultSet<Foo>;

      Log($"Got {results.Objects.Count} objects");
      foreach (var record in results.Objects)
      {
        Log($"{record.FooId} -> {record.Key} {record.Value}");
        record.Value = "CHANGED VALUE!";
      }

      await connection.Execute(new Update<Foo>()
      {
        Objects = results.Objects
      });

      var q2 = foo.Get<Query<Foo>, Foo>(FooSqlType.FooFindAllCustom);
      var results2 = await connection.Execute(q2) as ResultSet<Foo>;
      Log($"Got {results2.Objects.Count} objects");
      foreach (var record in results2.Objects)
      {
        Log($"{record.FooId} -> {record.Key} {record.Value}");
      }

      await connection.Execute(foo.Get<Command>(FooSqlType.FooDeleteAll));
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