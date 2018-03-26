using N.Package.Database.Assets;

namespace Tests
{
  public class FooSql : SqlAssets<FooSqlType>
  {
    public override string ResourceNamespace => "Data/Foo";
  }

  public enum FooSqlType
  {
    FooCreateTable,
    FooFindAllCustom,
    FooDeleteAll,
    FooFindByIdRange
  }
}