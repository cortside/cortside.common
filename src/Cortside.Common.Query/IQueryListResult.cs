using System.Collections.Generic;

namespace Cortside.Common.Query {
    public interface IQueryListResult<T> : IQueryResult {
        List<T> Results { get; }
    }
}
