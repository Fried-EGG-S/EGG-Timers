using System.Collections.Generic;
using UnityEngine;

namespace EGG.Timers
{
    public class TimeManager : MonoBehaviour
    {
        private static TimeManager _instance;
        public static TimeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject(nameof(TimeManager));
                    _instance = go.AddComponent<TimeManager>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private readonly List<ITickTimer> _tickTimers = new();
        private readonly List<ITimer> _timers = new();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddTimer(ITimerBase newTimer)
        {
            if (newTimer == null) return;
            switch (newTimer)
            {
                case ITickTimer tickTimer: if (!_tickTimers.Contains(tickTimer)) _tickTimers.Add(tickTimer); break;
                case ITimer timer: if (!_timers.Contains(timer)) _timers.Add(timer); break;
            }
        }

        public void RemoveTimer(ITimerBase timer)
        {
            switch (timer)
            {
                case ITickTimer tickTimer: _tickTimers.Remove(tickTimer); break;
                case ITimer t: _timers.Remove(t); break;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            float unscaledDt = Time.unscaledDeltaTime;

            for (int i = _tickTimers.Count - 1; i >= 0; i--)
            {
                var timer = _tickTimers[i];
                if (timer.MarkedForRemoval || timer.Owner == null)
                {
                    _tickTimers.RemoveAt(i);
                    continue;
                }
                timer.Tick(timer.UseUnscaledTime ? unscaledDt : dt);
            }

            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                if (timer.MarkedForRemoval || timer.Owner == null)
                {
                    _timers.RemoveAt(i);
                    continue;
                }
                timer.Tick(timer.UseUnscaledTime ? unscaledDt : dt);
            }

            Count = _timers.Count + _tickTimers.Count;
        }
        public int Count;
    }
}
