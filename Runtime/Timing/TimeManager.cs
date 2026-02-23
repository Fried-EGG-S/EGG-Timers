using System.Collections.Generic;
using UnityEngine;

namespace FOTF.GameManagers
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

        private readonly List<ITimer> _timers = new();
        private readonly List<ITickTimer> _tickTimers = new();

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

        public void AddTimer(ITimer timer)
        {
            if (timer == null) return;
            if (_timers.Contains(timer)) return;
            _timers.Add(timer);
        }

        public void AddTimer(ITickTimer timer)
        {
            if (timer == null) return;
            if (_tickTimers.Contains(timer)) return;
            _tickTimers.Add(timer);
        }

        public void RemoveTimer(ITimer timer) => _timers.Remove(timer);
        public void RemoveTimer(ITickTimer timer) => _tickTimers.Remove(timer);

        private void Update()
        {
            float dt = Time.deltaTime;

            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                _timers[i].Tick(dt);
                if (timer.MarkedForRemoval || timer.Owner == null)
                {
                    _timers.RemoveAt(i);
                    continue;
                }
            }

            for (int i = _tickTimers.Count - 1; i >= 0; i--)
            {
                var tickTimer = _tickTimers[i];
                tickTimer.Tick(dt);
                if (tickTimer.MarkedForRemoval || tickTimer.Owner == null)
                {
                    _tickTimers.RemoveAt(i);
                    continue;
                }
            }
            Count = _timers.Count + _tickTimers.Count;
        }
        public int Count;
    }
}