using System;
using System.Threading.Tasks;
using N.Packages.Promises;
using SQLite;

namespace N.Package.Database.Databases
{
  public class SqliteDatabase : IDatabase
  {
    private SQLiteConnection _connection;

    public Task<IDatabase> Connect(string connectionString)
    {
      var deferred = new Deferred<IDatabase>();
      try
      {
        _connection = new SQLiteConnection(connectionString);
        deferred.Resolve(this);
      }
      catch (Exception error)
      {
        _connection = null;
        deferred.Reject(error);
      }

      return deferred.Task;
    }

    public Task Disconnect()
    {
      var deferred = new Deferred();
      if (_connection != null)
      {
        _connection.Dispose();
        _connection = null;
      }

      deferred.Resolve();
      return deferred.Task;
    }

    public Task<IResult> Execute(IStatement sql)
    {
      var deferred = new Deferred<IResult>();
      if (_connection == null)
      {
        deferred.Reject(new Exception("No active connection"));
      }
      else
      {
        sql.Execute(_connection, deferred);
      }

      return deferred.Task;
    }
  }
}