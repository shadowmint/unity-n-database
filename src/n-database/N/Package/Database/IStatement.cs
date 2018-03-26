using N.Packages.Promises;
using SQLite;

namespace N.Package.Database
{
  public interface IStatement
  {
    void Execute(SQLiteConnection connection, Deferred<IResult> deferred);
  }
}