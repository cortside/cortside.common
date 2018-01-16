using System;
using System.Threading.Tasks;
using Serilog;

namespace Cortside.Common.Query {

    // Implementation of the query dispatcher - selects and executes the appropriate query
    public class QueryDispatcher : IQueryDispatcher {
        private readonly IServiceProvider provider;

        public QueryDispatcher(IServiceProvider provider) {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<TResult> Dispatch<TParameter, TResult>(TParameter query)
            where TParameter : class {
            IQueryHandler<TParameter, TResult> handler = provider.GetService(typeof(IQueryHandler<TParameter, TResult>)) as IQueryHandler<TParameter, TResult>;
            Log.Information(handler.GetType().ToString());

            return await handler.Retrieve(query);
        }
    }
}
