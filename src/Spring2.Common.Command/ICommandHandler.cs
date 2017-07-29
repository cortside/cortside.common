using System.Threading.Tasks;

namespace Spring2.Common.Command {
    public interface ICommandHandler<in TParameter> where TParameter : class {

	Task Execute(TParameter command);
    }
    
    public interface ICommandHandler<in TParameter, TResult> where TParameter : class {

	Task<TResult> Execute(TParameter command);
    }
}