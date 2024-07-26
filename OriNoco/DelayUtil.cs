using System;
using System.Collections.Generic;

using Raylib_CSharp;

namespace OriNoco
{
    public static class DelayUtil
    {
        public static List<DelayQueue> Queues = new List<DelayQueue>();
        private static List<DelayQueue> QueuesToRemove = new List<DelayQueue>();

        public static void Update()
        {
            foreach (var queue in Queues)
            {
                queue.TimeLeft -= Time.GetFrameTime();

                if (queue.IsCancelled)
                {
                    if (queue.CompleteOnCancel)
                        queue.OnComplete?.Invoke();
                    QueuesToRemove.Add(queue);
                    continue;
                }

                queue.OnUpdate?.Invoke(queue.TimeLeft / queue.Duration);

                if (queue.TimeLeft <= 0)
                {
                    queue.OnComplete?.Invoke();
                    QueuesToRemove.Add(queue);
                }
            }

            foreach (var queue in QueuesToRemove) Queues.Remove(queue);
            QueuesToRemove.Clear();
        }
    }

    public class DelayQueue
    {
        public float TimeLeft { get; set; }
        public float Duration { get; private set; }

        public Action<float>? OnUpdate { get; set; }
        public Action? OnComplete { get; set; }

        public bool IsCancelled { get; private set; }
        public bool CompleteOnCancel { get; private set; }

        public DelayQueue(float duration, Action? onComplete = null)
        {
            TimeLeft = duration;
            Duration = duration;
            OnComplete = onComplete;
        }

        public void Cancel(bool complete = false)
        {
            IsCancelled = true;
            CompleteOnCancel = complete;
        }
    }
}
