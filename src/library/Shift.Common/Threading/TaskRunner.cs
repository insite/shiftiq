using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shift.Common
{
    public static class TaskRunner
    {
        public static void RunSync(Func<Task> action)
        {
            Task.Run(action)
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(Func<Task> action, CancellationToken token)
        {
            Task.Run(action, token)
                .GetAwaiter()
                .GetResult();
        }

        public static T RunSync<T>(Func<Task<T>> action)
        {
            return Task.Run(action)
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSyncWithConfig(Func<Task> action, bool continueOnCapturedContext)
        {
            Task.Run(action)
                .ConfigureAwait(continueOnCapturedContext)
                .GetAwaiter()
                .GetResult();
        }

        public static T RunSyncWithConfig<T>(Func<Task<T>> action, bool continueOnCapturedContext)
        {
            return Task.Run(action)
                .ConfigureAwait(continueOnCapturedContext)
                .GetAwaiter()
                .GetResult();
        }

        public static TOut RunSync<TIn, TOut>(Func<TIn, Task<TOut>> action, TIn param1)
        {
            return RunSync(() => action(param1));
        }

        public static TOut RunSync<TIn1, TIn2, TOut>(Func<TIn1, TIn2, Task<TOut>> action, TIn1 param1, TIn2 param2)
        {
            return RunSync(() => action(param1, param2));
        }

        public static TOut RunSync<TIn1, TIn2, TIn3, TOut>(Func<TIn1, TIn2, TIn3, Task<TOut>> action, TIn1 param1, TIn2 param2, TIn3 param3)
        {
            return RunSync(() => action(param1, param2, param3));
        }

        public static TOut RunSync<TIn1, TIn2, TIn3, TIn4, TOut>(Func<TIn1, TIn2, TIn3, TIn4, Task<TOut>> action, TIn1 param1, TIn2 param2, TIn3 param3, TIn4 param4)
        {
            return RunSync(() => action(param1, param2, param3, param4));
        }

        public static TOut RunSyncWithConfig<TIn, TOut>(Func<TIn, Task<TOut>> action, bool continueOnCapturedContext, TIn param1)
        {
            return RunSyncWithConfig(() => action(param1), continueOnCapturedContext);
        }

        public static TOut RunSyncWithConfig<TIn1, TIn2, TOut>(Func<TIn1, TIn2, Task<TOut>> action, bool continueOnCapturedContext, TIn1 param1, TIn2 param2)
        {
            return RunSyncWithConfig(() => action(param1, param2), continueOnCapturedContext);
        }

        public static TOut RunSyncWithConfig<TIn1, TIn2, TIn3, TOut>(Func<TIn1, TIn2, TIn3, Task<TOut>> action, bool continueOnCapturedContext, TIn1 param1, TIn2 param2, TIn3 param3)
        {
            return RunSyncWithConfig(() => action(param1, param2, param3), continueOnCapturedContext);
        }

        public static TOut RunSyncWithConfig<TIn1, TIn2, TIn3, TIn4, TOut>(Func<TIn1, TIn2, TIn3, TIn4, Task<TOut>> action, bool continueOnCapturedContext, TIn1 param1, TIn2 param2, TIn3 param3, TIn4 param4)
        {
            return RunSyncWithConfig(() => action(param1, param2, param3, param4), continueOnCapturedContext);
        }
    }
}
