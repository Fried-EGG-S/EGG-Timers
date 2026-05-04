using System;

namespace EGG.Timers
{
    public interface ITimerBase
    {
        ITimerOwner Owner { get; set; }

        bool IsRunning { get; }
        bool UseUnscaledTime { get; set; }
        bool MarkedForRemoval { get; }
        float RemainingTime { get; }
        float SpeedMultiplier { get; }

        void Start(float time);
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
    }

    public interface ITimer : ITimerBase
    {
        bool AutoRemoveOnComplete { get; set; }

        event Action OnTimerCompleted;
    }

    public interface ITickTimer : ITimerBase
    {
        event Action OnTick;
    }

    public interface ITimerOwner
    {

    }
}
