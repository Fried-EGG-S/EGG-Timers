using System;

namespace EGG.Timers
{
    public static class TimerFactory
    {
        public static ITimer CreateCooldownTimer(ITimerOwner owner, Action onCompleted = null)
        {
            Timer timer = new();
            timer.OnTimerCompleted += onCompleted;
            timer.AutoRemoveOnComplete = false;
            timer.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            timer.Register();
            return timer;
        }
        public static ITimer CreateDurationTimer(ITimerOwner owner, Action onCompleted = null)
        {
            Timer timer = new();
            timer.OnTimerCompleted += onCompleted;
            timer.AutoRemoveOnComplete = true;
            timer.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            timer.Register();
            return timer;
        }
        public static ITickTimer CreateTickTimer(ITimerOwner owner, Action onTick = null, Action onTimerCompleted = null, bool hasDuration = false, bool autoRemoveOnStop = false)
        {
            TickTimer tickTimer = new();
            tickTimer.OnTick += onTick;
            if (hasDuration)
            {
                tickTimer.OnTimerCompleted += onTimerCompleted;
                tickTimer.HasDuration = hasDuration;
            }
            tickTimer.AutoRemoveOnStop = autoRemoveOnStop;
            tickTimer.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            tickTimer.Register();
            return tickTimer;
        }
    }
}