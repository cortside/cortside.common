using System.Threading.Tasks;

namespace Spring2.Common.Query {

    // Interface for the query dispatcher itself
    public interface IQueryDispatcher {

	Task<TResult> Dispatch<TParameter, TResult>(TParameter query)
	    where TParameter : class;
    }
}