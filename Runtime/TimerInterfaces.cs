using System;

namespace EGG.Timers
{
    public interface ITimer
    {
        ITimerOwner Owner { get; set; }

        bool IsRunning { get; }
        bool AutoRemoveOnComplete { get; set; }
        bool UseUnscaledTime { get; set; }
        bool MarkedForRemoval { get; }
        float RemainingTime { get; }
        float SpeedMultiplier { get; }

        void Start(float duration);
        void Stop();
        void Dispose();
        void Pause();
        void Unpause();
        void Increment(float amount, bool scaled = false);
        void Decrement(float amount, bool scaled = false);
        void SetSpeedMultiplier(float multiplier);
        void IncreaseSpeedMultiplier(float amount);
        void DecreaseSpeedMultiplier(float amount);
        void Tick(float deltaTime);

        event Action OnTimerCompleted;
    }
    public interface ITickTimer
    {
        ITimerOwner Owner { get; set; }

        bool IsRunning { get; }
        bool UseUnscaledTime { get; set; }
        bool MarkedForRemoval { get; }
        float RemainingTime { get; }
        float SpeedMultiplier { get; }

        void Start(float interval);
        void Stop();
        void Dispose();
        void Pause();
        void Unpause();
        void Increment(float amount, bool scaled = false);
        void Decrement(float amount, bool scaled = false);
        void SetSpeedMultiplier(float multiplier);
        void IncreaseSpeedMultiplier(float amount);
        void DecreaseSpeedMultiplier(float amount);
        void Tick(float deltaTime);

        event Action OnTick;
    }
    public interface ITimerOwner
    {

    }
}