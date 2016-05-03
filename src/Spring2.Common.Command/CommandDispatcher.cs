using System;
using System.Threading.Tasks;

namespace Spring2.Common.Command {

    // Implementation of the command dispatcher - selects and executes the appropriate command
    public class CommandDispatcher : ICommandDispatcher {
	private readonly IServiceProvider provider;

	public CommandDispatcher(IServiceProvider provider) {
	    if (provider == null) {
		throw new ArgumentNullException(nameof(provider));
	    }

	    this.provider = provider;
	}

	public async Task<CommandResult> Dispatch<TParameter>(TParameter command) where TParameter : ICommand {
	    // Find the appropriate handler to call from those registered based on the type parameters
	    ICommandHandler<TParameter> handler = provider.GetService(typeof(ICommandHandler<TParameter>)) as ICommandHandler<TParameter>;
	    return await handler.Execute(command);
	}
    }
}