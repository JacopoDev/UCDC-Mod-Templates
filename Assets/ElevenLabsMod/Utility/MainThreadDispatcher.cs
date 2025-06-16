using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

namespace ElevenLabsMod.Utility
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        public static void Enqueue(Action action)
        {
            actions.Enqueue(action);
        }

        void Update()
        {
            while (actions.TryDequeue(out var action))
            {
                action();
            }
        }

        public static Task<T> EnqueueAsync<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();

            Enqueue(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }
    }
}