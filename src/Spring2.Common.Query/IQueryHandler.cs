using System.Threading.Tasks;

namespace Spring2.Common.Query {

    // Interface for query handlers - has two type parameters for the query and the query result
    public interface IQueryHandler<TParameter, TResult>
       where TResult : IQueryResult
       where TParameter : IQuery {

	Task<TResult> Retrieve(TParameter query);
    }
}