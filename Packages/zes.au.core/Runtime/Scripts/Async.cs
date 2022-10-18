using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Au
{
    /// <summary>
    /// Async help class
    /// </summary>
    public static class Async
    {
        /// <summary>
        /// Wait until condition return true
        /// </summary>
        /// <param name="condition">condition function</param>
        /// <returns>nothing</returns>
        public static async Task WaitUntil(Func<bool> condition)
        {
            while (!condition())
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// Wait an unity async operation
        /// </summary>
        /// <param name="op"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static async Task WaitAsyncOperation(AsyncOperation op, Action<float> progress = null)
        {
            while (!op.isDone)
            {
                progress?.Invoke(op.progress);
                await Task.Yield();
            }
            progress?.Invoke(1);
        }
    }
}
