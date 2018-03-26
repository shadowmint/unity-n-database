using System;
using N.Package.Database.Results;
using N.Packages.Promises;
using SQLite;

namespace N.Package.Database.Statements
{
  public class Query<T> : Command where T : new()
  {
    public override void Execute(SQLiteConnection connection, Deferred<IResult> deferred)
    {
      try
      {
        var results = connection.Query<T>(Sql, GetCommandArguments());
        deferred.Resolve(new ResultSet<T>() {Objects = results});
      }
      catch (Exception error)
      {
        deferred.Reject(error);
      }
    }
  }
}