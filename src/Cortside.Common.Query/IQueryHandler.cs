using System.Threading.Tasks;

namespace Cortside.Common.Query {

    // Interface for query handlers - has two type parameters for the query and the query result
    public interface IQueryHandler<TParameter, TResult>
       where TParameter : class {

        Task<TResult> Retrieve(TParameter query);
    }
}
