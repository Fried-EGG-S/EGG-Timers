using System;
using UnityEngine;

namespace FriedEGGS.Timers
{
    internal static class TimerScheduler
    {
        private static readonly TimerHeap Scaled = new();
        private static readonly TimerHeap Unscaled = new();

        internal static void Schedule(TimerBase timer) => HeapFor(timer).AddOrUpdate(timer);
        internal static void Unschedule(TimerBase timer) => HeapFor(timer).Remove(timer);

        internal static void Update()
        {
            FireDue(Scaled, Time.timeAsDouble);
            FireDue(Unscaled, Time.unscaledTimeAsDouble);
        }

        internal static void Clear()
        {
            Scaled.Clear();
            Unscaled.Clear();
        }

        private static TimerHeap HeapFor(TimerBase timer) => timer.UseUnscaledTime ? Unscaled : Scaled;

        // Strictly before now: anything (re)started inside a callback ends at now or later, so it waits for the
        // next frame instead of looping in this one. Fire reschedules before it invokes, so a throwing callback
        // can't leave its timer at the top of the heap.
        private static void FireDue(TimerHeap heap, double now)
        {
            while (heap.Count > 0 && heap.Earliest.EndTime < now)
            {
                try
                {
                    heap.Earliest.Fire();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }
}
