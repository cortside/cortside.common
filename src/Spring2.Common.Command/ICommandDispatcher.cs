using System.Threading.Tasks;

namespace Spring2.Common.Command {

    // Interface for the command dispatcher itself
    public interface ICommandDispatcher {

	Task<CommandResult> Dispatch<TParameter>(TParameter command) where TParameter : ICommand;
    }
}