using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Playable
{
    public class TimeLineManager : MonoBehaviour
    {
        [System.Serializable]public class Director
        {
            public PlayableDirector director;
            public double initialTime = 0;
            public double currentDirectorTime => director.time;
        }

        [BoxGroup("MainDirector")]public Director MainDirector;

        public static TimeLineManager instance;

        private double lastTime;
        private bool stopped;

        private void Start()
        {
            instance = this;

            MainDirectorInit();
            
            PlayerEvents.OnPlayerStart += PlayMainTimeLine;
            GameEvents.OnGamePaused += PauseMainTimeLine;
            GameEvents.OnGamePaused += RecordMainPlayableTime;

            MainDirector.director.stopped += OnTimeLineStopped;
        }
        private void OnDestroy()
        {
            PlayerEvents.OnPlayerStart -= PlayMainTimeLine;
            GameEvents.OnGamePaused -= PauseMainTimeLine;
            GameEvents.OnGamePaused -= RecordMainPlayableTime;

            MainDirector.director.stopped += OnTimeLineStopped;
        }
        private void MainDirectorInit()
        {
            stopped = false;
            MainDirector.director.playOnAwake = false;
            MainDirector.director.initialTime = MainDirector.initialTime;
            PauseMainTimeLine();
        }
        private void PlayMainTimeLine()
        {
            if (stopped) return;

            MainDirector.director.time = lastTime;
            MainDirector.director.Play();
            Debug.Log($"{GetType().Name}:TimeLineStarted");
        }
        private void PauseMainTimeLine()
        {
            MainDirector.director.Pause();
            Debug.Log($"{GetType().Name}:TimeLinePaused");
        }
        private void OnTimeLineStopped(PlayableDirector playable)
        {
            stopped = true;

            Debug.Log($"{GetType().Name}:TimeLineStopped");
        }
        private void RecordMainPlayableTime()
        {
            lastTime = MainDirector.currentDirectorTime;
        }
    }
}
