#if TASKS
using Cysharp.Threading.Tasks;


namespace Stateless
{
    internal static class TaskResult
    {
        internal static readonly UniTask Done = FromResult(1);

        static UniTask<T> FromResult<T>(T value)
        {
            var tcs = new UniTaskCompletionSource<T>();
            tcs.TrySetResult(value);
            return tcs.Task;
        }
    }
}

#endif