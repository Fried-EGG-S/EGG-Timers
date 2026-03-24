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
        public float SpeedMultiplier { get; private set; } = 1f;

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
        public void Stop()
        {
            _elapsed = 0f;
            _duration = 0f;
            IsRunning = false;
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

        public void Increment(float amount, bool scaled = false)
        {
            if (!IsRunning) return;

            var effectiveAmount = scaled ? amount * SpeedMultiplier : amount;
            _elapsed += effectiveAmount;
            CheckIfCompleted();
        }
        public void Decrement(float amount, bool scaled = false)
        {
            if (!IsRunning) return;
            var effectiveAmount = scaled ? amount * SpeedMultiplier : amount;
            _elapsed = Mathf.Max(_elapsed - effectiveAmount, 0f);
            CheckIfCompleted();
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            SpeedMultiplier = multiplier;
        }

        public void IncreaseSpeedMultiplier(float amount)
        {
            if (amount < 0f) Debug.LogWarning("IncreaseSpeedMultiplier called with negative amount. Use DecreaseSpeedMultiplier instead.");
            SpeedMultiplier += amount;
        }

        public void DecreaseSpeedMultiplier(float amount)
        {
            if (amount < 0f) Debug.LogWarning("DecreaseSpeedMultiplier called with negative amount. Use IncreaseSpeedMultiplier instead.");
            SpeedMultiplier = Mathf.Max(SpeedMultiplier - amount, 0f);
        }

        public virtual void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            _elapsed += deltaTime * SpeedMultiplier;
            CheckIfCompleted();
        }

        private bool CheckIfCompleted()
        {
            if (_elapsed >= _duration)
            {
                IsRunning = false;
                Log($"Timer completed after {_elapsed} seconds.");
                if (AutoRemoveOnComplete) MarkedForRemoval = true;
                OnTimerCompleted?.Invoke();
                return true;
            }
            return false;
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
        [SerializeField] protected float _elapsed;

        public bool IsRunning { get; protected set; }
        public float RemainingTime => Mathf.Max(_interval - _elapsed, 0f);

        public bool MarkedForRemoval { get; private set; } = false;
        public float SpeedMultiplier { get; private set; } = 1f;

        public event Action OnTick;

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
            IsRunning = true;
        }

        public void Stop()
        {
            _elapsed = 0f;
            _interval = 0f;
            IsRunning = false;
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

        public void Increment(float amount, bool scaled = false)
        {
            if (!IsRunning) return;
            var effectiveAmount = scaled ? amount * SpeedMultiplier : amount;
            _elapsed += effectiveAmount;
        }
        public void Decrement(float amount, bool scaled = false)
        {
            if (!IsRunning) return;
            var effectiveAmount = scaled ? amount * SpeedMultiplier : amount;
            _elapsed = Mathf.Max(_elapsed - effectiveAmount, 0f);
        }


        public void SetSpeedMultiplier(float multiplier)
        {
            if (multiplier < 0f)
            {
                multiplier = 0f;
                Debug.LogWarning("SetSpeedMultiplier called with negative multiplier. Clamping to 0.");
            }
            SpeedMultiplier = multiplier;
        }

        public void IncreaseSpeedMultiplier(float amount)
        {
            if (amount < 0f)
            {
                Debug.LogWarning("IncreaseSpeedMultiplier called with negative amount. Use DecreaseSpeedMultiplier instead.");
                return;
            }
            SpeedMultiplier += amount;
        }

        public void DecreaseSpeedMultiplier(float amount)
        {
            if (amount < 0f)
            {
                Debug.LogWarning("DecreaseSpeedMultiplier called with negative amount. Use IncreaseSpeedMultiplier instead.");
                return;
            }
            SpeedMultiplier = Mathf.Max(SpeedMultiplier - amount, 0f);
        }
        public void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            _elapsed += deltaTime * SpeedMultiplier;

            while (_elapsed >= _interval)
            {
                _elapsed -= _interval;
                OnTick?.Invoke();
            }
        }
    }
}