using System;

namespace EGG.Timers
{
    public interface ITimer
    {
        ITimerOwner Owner { get; set; }
        bool IsRunning { get; }
        bool AutoRemoveOnComplete { get; set; }
        bool MarkedForRemoval { get; }
        float RemainingTime { get; }
        void Start(float duration);
        void Dispose();
        void Tick(float deltaTime);
        void Pause();
        void Unpause();

        event Action OnTimerCompleted;
    }
    public interface ITickTimer
    {
        ITimerOwner Owner { get; set; }
        bool IsRunning { get; }
        bool AutoRemoveOnStop { get; set; }
        bool MarkedForRemoval { get; }
        bool HasDuration { get; set; }
        float RemainingTime { get; }

        void Start(float interval);
        void Start(float interval, float duration);
        void Dispose();
        void Tick(float deltaTime);
        void Pause();
        void Unpause();

        event Action OnTick;
        event Action OnTimerCompleted;
    }
    public interface ITimerOwner
    {

    }
}