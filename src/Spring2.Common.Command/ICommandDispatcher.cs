using System.Threading.Tasks;

namespace Spring2.Common.Command {

    // Interface for the command dispatcher itself
    public interface ICommandDispatcher {

	Task Dispatch<TParameter>(TParameter command) where TParameter : class;
	Task<TResult> Dispatch<TParameter, TResult>(TParameter command) where TParameter : class;
    }
}