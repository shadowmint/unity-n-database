using System;
using System.Collections.Generic;
using System.Xml;
using N.Package.Database.Assets.Errors;
using N.Package.Database.Statements;
using UnityEngine;

namespace N.Package.Database.Assets
{
  /// <summary>
  /// This is the base class for a collection of sql statements.
  /// </summary>
  public abstract class SqlAssets<TStatement> where TStatement : IComparable
  {
    private readonly Dictionary<TStatement, Func<IStatement>> _builderCache = new Dictionary<TStatement, Func<IStatement>>();

    public abstract string ResourceNamespace { get; }

    /// <summary>
    /// Return the statement for the given target.
    /// </summary>
    public TOutput Get<TOutput>(TStatement target) where TOutput : class, IStatement
    {
      return Get<TOutput, bool>(target);
    }

    /// <summary>
    /// Return the statement for the given target.
    /// </summary>
    public TOutput Get<TOutput>(TStatement target, Action<TOutput> then) where TOutput : class, IStatement
    {
      return Get<TOutput, bool>(target, then);
    }

    /// <summary>
    /// Return the statement for the given target.
    /// </summary>
    public TOutput Get<TOutput, TData>(TStatement target) where TOutput : class, IStatement where TData : new()
    {
      return Get<TOutput, TData>(target, null);
    }

    /// <summary>
    /// Return the statement for the given target.
    /// </summary>
    public TOutput Get<TOutput, TData>(TStatement target, Action<TOutput> then) where TOutput : class, IStatement where TData : new()
    {
      if (_builderCache.ContainsKey(target))
      {
        var rtn = _builderCache[target]() as TOutput;
        then?.Invoke(rtn);
        return rtn;
      }

      var ns = ResourceNamespace.TrimEnd('/');
      var resourcePath = $"{ns}/{target}.sql";
      var asset = Resources.Load<TextAsset>(resourcePath);
      if (asset == null)
      {
        throw new SqlAssetException(SqlAssetException.SqlAssetExceptionType.BadStatement, $"TextAsset for {target} is NULL");
      }

      var sql = asset.text.Trim();
      if (string.IsNullOrEmpty(sql))
      {
        throw new SqlAssetException(SqlAssetException.SqlAssetExceptionType.BadStatement, $"TextAsset for {target} has no SQL");
      }

      Func<IStatement> builder = () => BuildStatement<TOutput, TData>(sql);

      var newRtn = builder();
      _builderCache[target] = builder;

      then?.Invoke(newRtn as TOutput);
      return newRtn as TOutput;
    }

    private static IStatement BuildStatement<TOutput, TData>(string sql) where TOutput : class, IStatement where TData : new()
    {
      if (typeof(TOutput) == typeof(Command))
      {
        return new Command() {Sql = sql};
      }

      if (typeof(TOutput) == typeof(Query<TData>))
      {
        return new Query<TData>() {Sql = sql};
      }

      if (typeof(TOutput) == typeof(Insert<TData>))
      {
        throw new SqlAssetException(SqlAssetException.SqlAssetExceptionType.NotSupported, $"Use Command for custom insert statements");
      }

      if (typeof(TOutput) == typeof(Update<TData>))
      {
        throw new SqlAssetException(SqlAssetException.SqlAssetExceptionType.NotSupported, $"Use Command for custom update statements");
      }

      throw new SqlAssetException(SqlAssetException.SqlAssetExceptionType.NotSupported, $"{typeof(TOutput)} is not supported");
    }
  }
}