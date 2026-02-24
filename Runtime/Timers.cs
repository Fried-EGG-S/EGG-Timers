using System;
using UnityEngine;

namespace EGG.Timers
{
    [Serializable]
    public class Timer : ITimer
    {
        internal Timer() { }

        public ITimerOwner Owner { get; set; }

        private bool _registered = false;

        [SerializeField] protected float _duration;
        [SerializeField] protected float _elapsed;
        public float RemainingTime => Mathf.Max(_duration - _elapsed, 0f);
        public bool IsRunning { get; protected set; }
        public bool AutoRemoveOnComplete { get; set; } = true;

        public bool MarkedForRemoval { get; private set; } = false;

        public event Action OnTimerCompleted;

        public void Register()
        {
            if (_registered) throw new InvalidOperationException("Timer is already registered with TimeManager.");

            TimeManager.Instance.AddTimer(this);
            _registered = true;
        }

        public virtual void Start(float duration)
        {
            _duration = duration;
            _elapsed = 0f;
            IsRunning = true;
        }
        public virtual void Dispose()
        {
            IsRunning = false;
            MarkedForRemoval = true;
        }
        public void Pause()
        {
            IsRunning = false;
        }
        public void Unpause()
        {
            IsRunning = true;
        }


        public virtual void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            _elapsed += deltaTime;
            if (_elapsed >= _duration)
            {
                IsRunning = false;
                Log($"Timer completed after {_elapsed} seconds.");
                if (AutoRemoveOnComplete) MarkedForRemoval = true;
                OnTimerCompleted?.Invoke();
            }
        }
        private static readonly bool _log = false;
        private static void Log(string message)
        {
            if (!_log) return;
            Debug.Log(message);
        }
    }

    [Serializable]
    public class TickTimer : ITickTimer
    {
        internal TickTimer() { }

        public ITimerOwner Owner { get; set; }

        private bool _registered = false;

        [SerializeField] protected float _interval;
        [SerializeField] private float _duration;
        [SerializeField] private float _elapsed;
        [SerializeField] private float _elapsedDuration;

        public bool HasDuration { get; set; } = false;
        public bool IsRunning { get; protected set; }
        public bool AutoRemoveOnStop { get; set; } = false;
        public float RemainingTime => Mathf.Max(_duration - _elapsedDuration, 0f);

        public bool MarkedForRemoval { get; private set; } = false;

        public event Action OnTick;
        public event Action OnTimerCompleted;

        public void Register()
        {
            if (_registered) throw new InvalidOperationException("Timer is already registered with TimeManager.");

            TimeManager.Instance.AddTimer(this);
            _registered = true;
        }

        public void Start(float interval)
        {
            _interval = interval;
            _elapsed = 0f;
            _elapsedDuration = 0f;
            IsRunning = true;
        }
        public void Start(float interval, float duration)
        {
            _interval = interval;
            _duration = duration;
            _elapsed = 0f;
            _elapsedDuration = 0f;
            HasDuration = true;
            IsRunning = true;
        }
        public void Dispose()
        {
            IsRunning = false;
            MarkedForRemoval = true;
        }
        public void Pause()
        {
            IsRunning = false;
        }
        public void Unpause()
        {
            IsRunning = true;
        }

        public virtual void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            _elapsed += deltaTime;
            if (HasDuration) _elapsedDuration += deltaTime;

            while (_elapsed >= _interval)
            {
                _elapsed -= _interval;
                OnTick?.Invoke();
            }
            if (HasDuration && _elapsedDuration >= _duration)
            {
                IsRunning = false;
                if (AutoRemoveOnStop) MarkedForRemoval = true;
                OnTimerCompleted?.Invoke();
            }
        }
    }
}