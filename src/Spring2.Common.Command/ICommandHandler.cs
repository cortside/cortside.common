using System.Threading.Tasks;

namespace Spring2.Common.Command {

    // Interface for command handlers - has a type parameters for the command
    public interface ICommandHandler<in TParameter> where TParameter : ICommand {

	Task<CommandResult> Execute(TParameter command);
    }
}