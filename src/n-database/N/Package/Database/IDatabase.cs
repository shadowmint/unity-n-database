using System.Threading.Tasks;

namespace N.Package.Database
{
  public interface IDatabase
  {
    Task<IDatabase> Connect(string connectionString);

    Task Disconnect();

    Task<IResult> Execute(IStatement sql);
  }
}