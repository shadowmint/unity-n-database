using System;

namespace N.Package.Database.Assets.Errors
{
  public class SqlAssetException : Exception
  {
    public SqlAssetExceptionType ExceptionType { get; }

    public SqlAssetException(SqlAssetExceptionType type, string detail = null) : base($"{type}: {detail}")
    {
      ExceptionType = type;
    }

    public SqlAssetException(SqlAssetExceptionType type, Exception innerException) : base($"{type}: {innerException.Message}")
    {
      ExceptionType = type;
    }

    public enum SqlAssetExceptionType
    {
      // The requested statement is missing from the SqlAssets
      MissingStatement,

      // The requested statement is incorrectly configured
      BadStatement,
      
      // The given statement type is not supported via SqlAsset (eg. updates must be done via Command for custom actions)
      NotSupported
    }
  }
}