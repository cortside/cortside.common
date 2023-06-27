#pragma warning disable VSTHRD003 // Avoid awaiting or returning a Task representing work that was not started within your context as that can lead to deadlocks.
// Start the work within this context, or use JoinableTaskFactory.RunAsync to start the task and await the returned JoinableTask instead.
#pragma warning disable VSTHRD105 // Avoid method overloads that assume TaskScheduler.Current. Use an overload that accepts a TaskScheduler and specify TaskScheduler.Default (or any other) explicitly

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cortside.Common.Threading.Tasks {
    public static class TaskExtensions {
        public static async Task WithTimeoutAsync(this Task task, TimeSpan timeout) {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false)) {
                return;
            }
            throw new TimeoutException();
        }

        public static async Task<TResult> WithTimeoutAsync<TResult>(this Task<TResult> task, TimeSpan timeout) {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false)) {
                return await task;
            }
            throw new TimeoutException();
        }

        public static Task WithUnwrappedTimeoutAsync(this Task task, TimeSpan timeout) {
            var timeoutTask = Task.Delay(timeout).ContinueWith(_ => task, TaskContinuationOptions.ExecuteSynchronously);
            return Task.WhenAny(task, timeoutTask);
        }

        public static Task<TResult> WithUnwrappedTimeoutAsync<TResult>(this Task<TResult> task, TimeSpan timeout) {
            var timeoutTask = Task.Delay(timeout).ContinueWith(_ => default(TResult), TaskContinuationOptions.ExecuteSynchronously);
            return Task.WhenAny(task, timeoutTask).Unwrap();
        }

        public static async Task<T> WithCancellationAsync<T>(this Task<T> task, CancellationToken cancellationToken) {
            var cancellationCompletionSource = new TaskCompletionSource<bool>();

            await using (cancellationToken.Register(() => cancellationCompletionSource.TrySetResult(true))) {
                if (task != await Task.WhenAny(task, cancellationCompletionSource.Task).ConfigureAwait(false)) {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return await task;
        }
    }
}
