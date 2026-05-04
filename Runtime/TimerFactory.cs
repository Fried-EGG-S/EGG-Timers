using System;

namespace EGG.Timers
{
    public static class TimerFactory
    {
        public static ITimer CreateCooldownTimer(ITimerOwner owner, Action onCompleted = null, bool unscaled = false)
        {
            return CreateDurationTimer(owner, onCompleted, false, unscaled);
        }

        public static ITimer CreateDurationTimer(ITimerOwner owner, Action onCompleted = null, bool autoRemoveOnComplete = true, bool unscaled = false)
        {
            Timer timer = new();
            timer.OnTimerCompleted += onCompleted;
            timer.AutoRemoveOnComplete = autoRemoveOnComplete;
            timer.UseUnscaledTime = unscaled;
            timer.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            timer.Register();
            return timer;
        }

        public static ITickTimer CreateTickTimer(ITimerOwner owner, Action onTick = null, bool unscaled = false)
        {
            TickTimer tickTimer = new();
            tickTimer.OnTick += onTick;
            tickTimer.UseUnscaledTime = unscaled;
            tickTimer.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            tickTimer.Register();
            return tickTimer;
        }
    }
}