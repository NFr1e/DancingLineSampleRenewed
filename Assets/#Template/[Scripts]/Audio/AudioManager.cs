using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DancingLineFanmade.Gameplay;
using DancingLineFanmade.Collectable;

namespace DancingLineFanmade.Audio
{
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour
    {
        public static void PlayAudioClip(AudioClip clip)
        {
            AudioSource player = new GameObject(clip.name + "Player").AddComponent<AudioSource>();
            player.clip = clip;
            player.Play();
            Destroy(player, clip.length + 0.1f);//安全缓冲时间
        }

        public GameController Controller;
        public LevelData CurLevelData => Controller ? Controller.CurrentLevelData : null;

        public static AudioManager instance;
        public float LevelMusicSyncDelay { get; set; }

        private AudioSource _levelSoundtrackPlayer;
        private float _lastSoundtrackTime;
        private float _baseSoundtrackStartTime;
        private float _originalVolume;

        private Tween _audioFadeoutTween;
        public float CurrentLevelTime
        {
            get
            {
                return _levelSoundtrackPlayer.time - _baseSoundtrackStartTime;
            }
        }
        #region 订阅事件
        private void Awake()
        {
            instance = this;

            if(CurLevelData)_baseSoundtrackStartTime = CurLevelData.SoundtrackStartTime;

            GameEvents.OnEnterLevel += CreateMusicPlayer;
            GameEvents.OnStartPlay += PlayLevelSoundtrack;
            GameEvents.OnStartPlay += ResetSoundTrackVolume;
            GameEvents.OnGamePaused += RecordLevelSoundtrackTime;
            GameEvents.OnGamePaused += PauseLevelSoundtrack;
            CollectorEvents.OnCollectCheckpoint += RecordLevelSoundtrackTime;
            GameEvents.OnGameOver += FadeoutLevelSoundtrack;
            GameEvents.OnRespawnDone += SetLevelSoundtrackTime;

            if (CurLevelData == null)
            {
                Debug.LogError("CurLevelData is not assigned in AudioManager!");
                return;
            }
        }
        private void OnDisable()
        {
            GameEvents.OnEnterLevel -= CreateMusicPlayer;
            GameEvents.OnStartPlay -= PlayLevelSoundtrack;
            GameEvents.OnStartPlay -= ResetSoundTrackVolume;
            GameEvents.OnGamePaused -= RecordLevelSoundtrackTime;
            GameEvents.OnGamePaused -= PauseLevelSoundtrack;
            CollectorEvents.OnCollectCheckpoint -= RecordLevelSoundtrackTime;
            GameEvents.OnGameOver -= FadeoutLevelSoundtrack;
            GameEvents.OnRespawnDone -= SetLevelSoundtrackTime;
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
            if(CurLevelData) _levelSoundtrackPlayer.clip = CurLevelData.LevelSoundtrack;
            _levelSoundtrackPlayer.time = _baseSoundtrackStartTime;
            _originalVolume = _levelSoundtrackPlayer.volume;
        }
        private void RecordLevelSoundtrackTime()
        {
            _lastSoundtrackTime = _levelSoundtrackPlayer.time;
        }
        private void SetLevelSoundtrackTime()
        {
            _levelSoundtrackPlayer.time = _lastSoundtrackTime;
        }
        private void PlayLevelSoundtrack()
        {
            StartCoroutine(_PlayLevelSoundtrack());
        }
        private IEnumerator _PlayLevelSoundtrack()
        {
            if (_levelSoundtrackPlayer == null) yield break;
            yield return new WaitForSeconds(Mathf.Abs(LevelMusicSyncDelay));
            _levelSoundtrackPlayer.Play();
        }
        private void PauseLevelSoundtrack()
        {
            _levelSoundtrackPlayer.Pause();
        }
        private void FadeoutLevelSoundtrack()
        {
            if (_audioFadeoutTween != null)
            {
                _audioFadeoutTween.Kill();
                _audioFadeoutTween = null;
            }
            _audioFadeoutTween = _levelSoundtrackPlayer.DOFade(0, 2f).OnComplete(() => _audioFadeoutTween.Kill(false));
            Debug.Log("SoundtrackFaded");
        }
        private void ResetSoundTrackVolume()
        {
            if (_audioFadeoutTween != null)
            {
                _audioFadeoutTween.Kill();
                _audioFadeoutTween = null;
            }
            _levelSoundtrackPlayer.volume = _originalVolume;
        }
    }
}
