using System;
using System.Collections.Generic;
using System.Linq;
using N.Package.Database.Results;
using N.Packages.Promises;
using SQLite;

namespace N.Package.Database.Statements
{
  public class Command : IStatement
  {
    public string Sql { get; set; }

    public IEnumerable<object> Args { get; set; }

    protected object[] GetCommandArguments()
    {
      return Args?.ToArray() ?? new object[] { };
    }

    public virtual void Execute(SQLiteConnection connection, Deferred<IResult> deferred)
    {
      try
      {
        var count = connection.Execute(Sql, GetCommandArguments());
        deferred.Resolve(new RowsAffected() {Count = count});
      }
      catch (Exception error)
      {
        deferred.Reject(error);
      }
    }
  }
}