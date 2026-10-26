using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FriedEGGS.Timers
{
    internal static class TimerLoop
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            TimerScheduler.Clear();

            var timerSystem = new PlayerLoopSystem { type = typeof(TimerTick), updateDelegate = TimerScheduler.Update };

            var currentLoop = PlayerLoop.GetCurrentPlayerLoop();

            var updateIndex = IndexOf(currentLoop.subSystemList, typeof(Update));

            if (updateIndex == -1)
            {
                Debug.LogError("[FriedEGGS.Timers] No Update phase in the PlayerLoop; timers won't tick.");
                return;
            }

            var updateSystem = currentLoop.subSystemList[updateIndex];
            var systems = new List<PlayerLoopSystem>(updateSystem.subSystemList);

            int tickIndex = IndexOf(systems, typeof(TimerTick));
            if (tickIndex != -1) systems.RemoveAt(tickIndex);

            var scriptRunBehaviourUpdateIndex = IndexOf(systems, typeof(Update.ScriptRunBehaviourUpdate));

            if (scriptRunBehaviourUpdateIndex == -1)
            {
                Debug.LogError("[FriedEGGS.Timers] No ScriptRunBehaviourUpdate system in the Update phase; timers won't tick.");
                return;
            }

            systems.Insert(scriptRunBehaviourUpdateIndex, timerSystem);

            updateSystem.subSystemList = systems.ToArray();
            currentLoop.subSystemList[updateIndex] = updateSystem;
            PlayerLoop.SetPlayerLoop(currentLoop);
        }

        private static int IndexOf(IReadOnlyList<PlayerLoopSystem> systems, Type type)
        {
            if (systems == null) return -1;

            for (int i = 0; i < systems.Count; i++)
            {
                if (systems[i].type == type) return i;
            }

            return -1;
        }

        private struct TimerTick { }

#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void HookPlayModeExit()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingPlayMode) return;

            TimerScheduler.Clear();
        }

#endif
    }
}
