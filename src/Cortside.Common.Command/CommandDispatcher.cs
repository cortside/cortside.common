using System;
using System.Threading.Tasks;

namespace Cortside.Common.Command {

    // Implementation of the command dispatcher - selects and executes the appropriate command
    public class CommandDispatcher : ICommandDispatcher {
        private readonly IServiceProvider provider;

        public CommandDispatcher(IServiceProvider provider) {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<TResult> Dispatch<TParameter, TResult>(TParameter command) where TParameter : class {
            // Find the appropriate handler to call from those registered based on the type parameters
            ICommandHandler<TParameter, TResult> handler = provider.GetService(typeof(ICommandHandler<TParameter, TResult>)) as ICommandHandler<TParameter, TResult>;
            return await handler.Execute(command);
        }

        public async Task Dispatch<TParameter>(TParameter command) where TParameter : class {
            // Find the appropriate handler to call from those registered based on the type parameters
            ICommandHandler<TParameter> handler = provider.GetService(typeof(ICommandHandler<TParameter>)) as ICommandHandler<TParameter>;
            await handler.Execute(command);
        }
    }
}
