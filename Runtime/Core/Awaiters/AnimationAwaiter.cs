using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements.Experimental;

namespace Dephion.Ui.Core.Awaiters
{
    [DebuggerNonUserCode]
    public readonly struct AnimationAwaiter<T> : INotifyCompletion
    {
        private readonly ValueAnimation<T> _asyncOperation;

        public bool IsCompleted => _asyncOperation.isRunning;

        public AnimationAwaiter(ValueAnimation<T> asyncOperation)
        {
            _asyncOperation = asyncOperation;
            if (!_asyncOperation.isRunning)
                _asyncOperation.Start();
        }

        public void OnCompleted(Action continuation) => _asyncOperation.OnCompleted(continuation);

        public ValueAnimation<T> GetResult() => _asyncOperation;
    }

    public static class AnimationAwaiterExtensions
    {
        public static AnimationAwaiter<T> GetAwaiter<T>(this ValueAnimation<T> asyncOp)
        {
            return new AnimationAwaiter<T>(asyncOp);
        }
    }
}