using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using DancingLineFanmade.Gameplay;
using DancingLineFanmade.Collectable;

namespace DancingLineFanmade.Audio
{
    public class AudioEvents
    {
        public static event System.Action<float> OnAlterSoundtrackTime;
        public static void AlterSoundtrackTime(float time) => OnAlterSoundtrackTime?.Invoke(time);
    }
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour
    {
        public static void PlayAudioClip(AudioClip clip,float startTime = 0)
        {
            AudioSource player = new GameObject(clip.name + "Player").AddComponent<AudioSource>();
            player.clip = clip;
            player.time = startTime;
            player.Play();
            Destroy(player.gameObject, clip.length + 0.1f);//安全缓冲时间
        }
        public static void PlayFadeutAudioClip(AudioClip clip, float startTime = 0,float duration = 1,float volume = 1)
        {
            AudioSource player = new GameObject(clip.name + "FadePlayer").AddComponent<AudioSource>();
            DontDestroyOnLoad(player);
            player.clip = clip;
            player.time = startTime;
            player.Play();
            player.DOFade(0, duration).SetUpdate(true).OnComplete(() => Destroy(player.gameObject));
        }

        public GameController Controller;

        [BoxGroup("Sounds")]
        public AudioClip DrownSound,
                         HitSound;
        public LevelData CurLevelData => 
            Controller 
            ? Controller.CurrentLevelData 
            : null;

        public static AudioManager instance;
        public float LevelMusicSyncDelay { get; set; }

        private AudioSource _levelSoundtrackPlayer;
        private float _lastSoundtrackTime;
        private float _baseSoundtrackStartTime;
        private float _originalVolume;

        private Tween _audioFadeoutTween;
        
        [HideInInspector]public float CurrentLevelTime => 
            _levelSoundtrackPlayer
            ?1f * _levelSoundtrackPlayer.timeSamples/_levelSoundtrackPlayer.clip.frequency - _baseSoundtrackStartTime
            :0;

        private void Start()
        {
            if (CurLevelData) _baseSoundtrackStartTime = CurLevelData.SoundtrackStartTime;
            else UnityEngine.Debug.LogError($"{GetType().Name} CurLevelData为空，检查GameController是否赋值?");
        }
        #region 订阅事件
        private void Awake()
        {
            instance = this;

            CreateMusicPlayer();

            GameEvents.OnStartPlay += PlayLevelSoundtrack;
            GameEvents.OnStartPlay += ResetSoundTrackVolume;
            GameEvents.OnGamePaused += RecordLevelSoundtrackTime;
            GameEvents.OnGamePaused += PauseLevelSoundtrack;

            PlayerEvents.OnPlayerHit += PlayPlayerHitSound;
            PlayerEvents.OnPlayerDrowned += PlayPlayerDrownSound;

            AudioEvents.OnAlterSoundtrackTime += AlterLevelSoundtrackTime;

            RespawnAttributes.OnRecording += RecordLevelSoundtrackTime;

            CollectorEvents.OnCollectCheckpoint += RecordLevelSoundtrackTime;

            GameEvents.OnGameOver += FadeoutLevelSoundtrack;
        }
        private void OnDisable()
        {
            
            GameEvents.OnStartPlay -= PlayLevelSoundtrack;
            GameEvents.OnStartPlay -= ResetSoundTrackVolume;
            GameEvents.OnGamePaused -= RecordLevelSoundtrackTime;
            GameEvents.OnGamePaused -= PauseLevelSoundtrack;

            PlayerEvents.OnPlayerHit -= PlayPlayerHitSound;
            PlayerEvents.OnPlayerDrowned -= PlayPlayerDrownSound;

            AudioEvents.OnAlterSoundtrackTime -= AlterLevelSoundtrackTime;

            RespawnAttributes.OnRecording -= RecordLevelSoundtrackTime;

            CollectorEvents.OnCollectCheckpoint -= RecordLevelSoundtrackTime;

            GameEvents.OnGameOver -= FadeoutLevelSoundtrack;
        }
        #endregion

        private void CreateMusicPlayer()
        {
            if (_levelSoundtrackPlayer != null)
            {
                Destroy(_levelSoundtrackPlayer.gameObject);
            }
            _levelSoundtrackPlayer = new GameObject("LevelMusicPlayer").AddComponent<AudioSource>();
            _levelSoundtrackPlayer.playOnAwake = false;

            if (CurLevelData) 
                _levelSoundtrackPlayer.clip = CurLevelData.LevelSoundtrack;

            _levelSoundtrackPlayer.time = _baseSoundtrackStartTime;
            _originalVolume = _levelSoundtrackPlayer.volume;
            _levelSoundtrackPlayer.volume = _originalVolume;
        }
        private void RecordLevelSoundtrackTime()
        {
            _lastSoundtrackTime = _levelSoundtrackPlayer.time;
        }
        public void AlterLevelSoundtrackTime(float time)
        {
            _levelSoundtrackPlayer.timeSamples = (int)(time * _levelSoundtrackPlayer.clip.frequency);
        }
        private void PlayLevelSoundtrack()
        {
            if (_levelSoundtrackPlayer == null) return;
            _levelSoundtrackPlayer.Play((ulong)Mathf.Abs(LevelMusicSyncDelay));
        }
        private void PauseLevelSoundtrack()
        {
            _levelSoundtrackPlayer.Pause();
        }
        /// <summary>
        /// 喝喝喝，好暴力的办法...
        /// </summary>
        private void FadeoutLevelSoundtrack()
        {
            PauseLevelSoundtrack();

            PlayFadeutAudioClip(CurLevelData.LevelSoundtrack,CurrentLevelTime,2,_originalVolume);

            UnityEngine.Debug.Log("SoundtrackFaded");

            #region OldMethod
            //之前的方法会导致因音频未暂停，复活后会重新开始播放的问题
            /*if (_audioFadeoutTween != null)
            {
                _audioFadeoutTween.Kill();
                _audioFadeoutTween = null;
            }
            _audioFadeoutTween = _levelSoundtrackPlayer.DOFade(0, 2f).OnComplete(() => 
            { 
                _audioFadeoutTween.Kill(false);
            });*/
            #endregion
        }
        private void ResetSoundTrackVolume()
        {
            if (_audioFadeoutTween != null)
            {
                _audioFadeoutTween?.Kill();
                _audioFadeoutTween = null;
            }
            _levelSoundtrackPlayer.volume = _originalVolume;
        }

        private void PlayPlayerDrownSound()
        {
            if (DrownSound)
                PlayAudioClip(DrownSound);
        }
        private void PlayPlayerHitSound()
        {
            if(HitSound)
                PlayAudioClip(HitSound); 
        }
    }
}
