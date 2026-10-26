using System;
using System.Threading;
using UnityEngine;

namespace FriedEGGS.Timers
{
    public abstract class TimerBase : IDisposable
    {
        private enum State { Stopped, Running, Paused }

        private State _state;
        private double _remainingAtAnchor;
        private double _anchor;
        private bool _cancelled;

        internal int HeapIndex { get; set; } = -1;
        internal double EndTime { get; private set; }

        public bool UseUnscaledTime { get; }
        public float SpeedMultiplier { get; private set; } = 1f;
        public bool IsPaused => _state == State.Paused;
        // A timer with listeners runs until its callback fires; one without (a cooldown) runs until its time is up.
        public bool IsRunning => _state == State.Running && (HasListeners || Remaining > 0d);
        public float RemainingTime => (float)Remaining;

        private protected abstract bool HasListeners { get; }

        private double Now => UseUnscaledTime ? Time.unscaledTimeAsDouble : Time.timeAsDouble;
        private double Remaining => _state switch
        {
            State.Running => Math.Max(_remainingAtAnchor - (Now - _anchor) * SpeedMultiplier, 0d),
            State.Paused => _remainingAtAnchor,
            _ => 0d
        };

        internal TimerBase(bool unscaled, CancellationToken cancellation)
        {
            UseUnscaledTime = unscaled;
            if (cancellation.CanBeCanceled) cancellation.Register(Cancel);
        }

        public abstract void Start(float time);

        public void Stop()
        {
            _state = State.Stopped;
            _remainingAtAnchor = 0d;
            Reschedule();
        }
        public void Dispose() => Stop();

        public void Pause()
        {
            if (!IsRunning) return;
            Reanchor();
            _state = State.Paused;
            Reschedule();
        }
        public void Unpause()
        {
            if (_state != State.Paused) return;
            _anchor = Now;
            _state = State.Running;
            Reschedule();
        }

        public void Increment(float amount, bool scaled = false) => Shift(-(scaled ? amount * SpeedMultiplier : amount));
        public void Decrement(float amount, bool scaled = false) => Shift(scaled ? amount * SpeedMultiplier : amount);

        public void SetSpeedMultiplier(float multiplier)
        {
            if (multiplier < 0f)
            {
                Debug.LogWarning("SetSpeedMultiplier called with negative multiplier. Clamping to 0.");
                multiplier = 0f;
            }
            Reanchor();
            SpeedMultiplier = multiplier;
            Reschedule();
        }
        public void IncreaseSpeedMultiplier(float amount)
        {
            if (amount < 0f)
            {
                Debug.LogWarning("IncreaseSpeedMultiplier called with negative amount. Use DecreaseSpeedMultiplier instead.");
                return;
            }
            SetSpeedMultiplier(SpeedMultiplier + amount);
        }
        public void DecreaseSpeedMultiplier(float amount)
        {
            if (amount < 0f)
            {
                Debug.LogWarning("DecreaseSpeedMultiplier called with negative amount. Use IncreaseSpeedMultiplier instead.");
                return;
            }
            SetSpeedMultiplier(Mathf.Max(SpeedMultiplier - amount, 0f));
        }

        private protected void Run(double time)
        {
            if (_cancelled) return;
            _remainingAtAnchor = time;
            _anchor = Now;
            _state = State.Running;
            Reschedule();
        }

        // Anchors the next period at the previous end, not at now, so a long frame fires every period it covered.
        private protected void Repeat(double period)
        {
            _anchor = EndTime;
            _remainingAtAnchor = period;
            Reschedule();
        }

        private protected void ListenersChanged() => Reschedule();

        internal abstract void Fire();

        private void Shift(double delta)
        {
            if (!IsRunning && !IsPaused) return;
            Reanchor();
            _remainingAtAnchor = Math.Max(_remainingAtAnchor + delta, 0d);
            Reschedule();
        }

        private void Reanchor()
        {
            _remainingAtAnchor = Remaining;
            _anchor = Now;
        }

        private void Reschedule()
        {
            if (_state != State.Running || SpeedMultiplier <= 0f || !HasListeners)
            {
                TimerScheduler.Unschedule(this);
                return;
            }
            EndTime = _anchor + _remainingAtAnchor / SpeedMultiplier;
            TimerScheduler.Schedule(this);
        }

        private void Cancel()
        {
            _cancelled = true;
            Stop();
        }
    }

    public sealed class Timer : TimerBase
    {
        private Action _completed;

        public event Action OnTimerCompleted
        {
            add { _completed += value; ListenersChanged(); }
            remove { _completed -= value; ListenersChanged(); }
        }

        private protected override bool HasListeners => _completed != null;

        public Timer(Action onCompleted = null, bool unscaled = false, CancellationToken cancellation = default) : base(unscaled, cancellation) => _completed = onCompleted;

        public override void Start(float duration) => Run(Math.Max(duration, 0f));

        internal override void Fire()
        {
            Stop();
            _completed?.Invoke();
        }
    }

    public sealed class TickTimer : TimerBase
    {
        private Action _ticked;
        private float _interval;

        public event Action OnTick
        {
            add { _ticked += value; ListenersChanged(); }
            remove { _ticked -= value; ListenersChanged(); }
        }

        private protected override bool HasListeners => _ticked != null;

        public TickTimer(Action onTick = null, bool unscaled = false, CancellationToken cancellation = default) : base(unscaled, cancellation) => _ticked = onTick;

        public override void Start(float interval)
        {
            if (interval <= 0f) throw new ArgumentOutOfRangeException(nameof(interval), interval, "A tick interval must be above 0.");
            _interval = interval;
            Run(interval);
        }

        internal override void Fire()
        {
            Repeat(_interval);
            _ticked?.Invoke();
        }
    }
}
