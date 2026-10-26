using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace FriedEGGS.Timers
{
    [Serializable]
    public sealed class TimerReadout
    {
        // Only the Editor refresh writes these; they stay in builds so the serialized layout matches the Editor's.
#pragma warning disable CS0169
        [SerializeField] private float _remainingTime;
        [SerializeField] private bool _running;
        [SerializeField] private bool _paused;
#pragma warning restore CS0169

#if UNITY_EDITOR
        [NonSerialized] private TimerBase _timer;
#endif

        public TimerReadout(TimerBase timer)
        {
            if (timer == null) throw new ArgumentNullException(nameof(timer));
#if UNITY_EDITOR
            _timer = timer;
            Readouts.Add(new WeakReference<TimerReadout>(this));
#endif
        }

#if UNITY_EDITOR
        private const double RefreshInterval = 0.05;
        private static double _nextRefresh;

        private static readonly List<WeakReference<TimerReadout>> Readouts = new();

        [InitializeOnLoadMethod]
        private static void HookIntoEditor()
        {
            EditorApplication.update += RefreshAll;
        }

        private static void RefreshAll()
        {
            double now = EditorApplication.timeSinceStartup;
            if (now < _nextRefresh) return;
            _nextRefresh = now + RefreshInterval;

            for (int i = Readouts.Count - 1; i >= 0; i--)
            {
                if (!Readouts[i].TryGetTarget(out TimerReadout readout))
                {
                    Readouts.RemoveAt(i);
                    continue;
                }
                readout.Refresh();
            }
        }

        private void Refresh()
        {
            _remainingTime = _timer.RemainingTime;
            _running = _timer.IsRunning;
            _paused = _timer.IsPaused;
        }
#endif
    }
}
