using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Playable
{
    public class TimeLineManager : MonoBehaviour,IResettable
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
        private bool 
            stopped,
            _stopped;//复活前是否停止

        private void Start()
        {
            instance = this;

            MainDirectorInit();
            
            PlayerEvents.OnPlayerStart += PlayMainTimeLine;

            GameEvents.OnGamePaused += PauseMainTimeLine;
            GameEvents.OnGamePaused += NoteArgs;
            GameEvents.OnGameOver += PauseMainTimeLine;

            RespawnAttributes.OnRecording += NoteArgs;

            MainDirector.director.stopped += OnTimeLineStopped;

            RegisterResettable();
        }
        private void OnDestroy()
        {
            PlayerEvents.OnPlayerStart -= PlayMainTimeLine;

            GameEvents.OnGamePaused -= PauseMainTimeLine;
            GameEvents.OnGamePaused -= NoteArgs;
            GameEvents.OnGameOver -= PauseMainTimeLine;

            RespawnAttributes.OnRecording -= NoteArgs;

            MainDirector.director.stopped += OnTimeLineStopped;

            UnregisterResettable();
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

        #region Reset
        /// <summary>
        /// 一般在OnEnable中调用
        /// </summary>
        private void RegisterResettable() => ResettableManager.Register(this);
        /// <summary>
        /// 一般在OnDisable中调用
        /// </summary>
        private void UnregisterResettable() => ResettableManager.Unregister(this);

        public void NoteArgs() 
        {
            lastTime = MainDirector.currentDirectorTime;
            _stopped = stopped;
        }
        public void ResetArgs()
        {
            stopped = _stopped;

            StartCoroutine(InitTimeline());
            
            Debug.Log($"{name} Reset");
        }
        IEnumerator InitTimeline()
        {
            PlayMainTimeLine();
            yield return new WaitForEndOfFrame();
            PauseMainTimeLine();
        }
        #endregion
    }
}
