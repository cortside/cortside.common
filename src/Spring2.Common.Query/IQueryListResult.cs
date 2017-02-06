using System.Collections.Generic;

namespace Spring2.Common.Query
{
    public interface IQueryListResult<T> : IQueryResult
    {
        List<T> Results { get; }
    }
}
