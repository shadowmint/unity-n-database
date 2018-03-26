using System.Collections.Generic;

namespace N.Package.Database.Results
{
  public class ResultSet<T> : IResult
  {
    public List<T> Objects { get; set; }
  }
}