using System;
using System.Collections.Generic;
using System.Linq;
using N.Package.Database.Results;
using N.Packages.Promises;
using SQLite;
using UnityEngine;

namespace N.Package.Database.Statements
{
  /// <summary>
  /// Note this requires there exists a table with the name matching T.
  /// </summary>
  public class Insert<T> : IStatement
  {
    public IEnumerable<T> Objects { get; set; }

    public void Execute(SQLiteConnection connection, Deferred<IResult> deferred)
    {
      try
      {
        var data = Objects?.ToArray() ?? new T[] { };
        var results = connection.InsertAll(data, typeof(T));
        deferred.Resolve(new RowsAffected() {Count = results});
      }
      catch (Exception error)
      {
        deferred.Reject(error);
      }
    }
  }
}