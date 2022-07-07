using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cortside.Common.Threading.Tasks {
    public static class TaskExtensions {
        public static async Task WithTimeout(this Task task, TimeSpan timeout) {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false)) {
                return;
            }
            throw new TimeoutException();
        }

        public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout) {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false)) {
                return await task;
            }
            throw new TimeoutException();
        }

        public static Task WithUnwrappedTimeout(this Task task, TimeSpan timeout) {
            var timeoutTask = Task.Delay(timeout).ContinueWith(_ => task, TaskContinuationOptions.ExecuteSynchronously);
            return Task.WhenAny(task, timeoutTask);
        }

        public static Task<TResult> WithUnwrappedTimeout<TResult>(this Task<TResult> task, TimeSpan timeout) {
            var timeoutTask = Task.Delay(timeout).ContinueWith(_ => default(TResult), TaskContinuationOptions.ExecuteSynchronously);
            return Task.WhenAny(task, timeoutTask).Unwrap();
        }

        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken) {
            var cancellationCompletionSource = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(() => cancellationCompletionSource.TrySetResult(true))) {
                if (task != await Task.WhenAny(task, cancellationCompletionSource.Task).ConfigureAwait(false)) {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return await task;
        }
    }
}
