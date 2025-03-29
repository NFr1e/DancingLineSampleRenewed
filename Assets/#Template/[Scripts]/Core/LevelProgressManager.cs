using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Collectable;
using DancingLineFanmade.Audio;

namespace DancingLineFanmade.Gameplay
{
    [DisallowMultipleComponent]
    public class LevelProgressManager : MonoBehaviour
    {
        public static event System.Action OnProgressUpdated;
        public void TriggerProgressUpdated() => OnProgressUpdated?.Invoke();

        public static LevelProgressManager instance;

        public AudioManager CurrentAudioManager;

        private LevelData _levelData;
        private float _levelLength;

        private List<ICollectable> _collectedDiamonds = new();
        private List<ICollectable> _collectedCheckpoints = new();

        public class LevelProgress
        {
            public int Percentage, DiamondCount, CheckpointCount;
        }
        public LevelProgress currentProgress;
        private void Init()
        {
            InitializeReference();
        }
        private void InitializeReference()
        {
            if (CurrentAudioManager == null) return;

            _levelData = CurrentAudioManager.CurLevelData;
            _levelLength = _levelData.LevelSoundtrack.length - _levelData.SoundtrackStartTime;
        }
        public void UpdateCurrentProgress()
        {
            currentProgress = new LevelProgress 
            {
                Percentage = (int)(CurrentAudioManager.CurrentLevelTime * 100 / _levelLength),
                DiamondCount = _collectedDiamonds.Count,
                CheckpointCount = _collectedCheckpoints.Count
            };

            TriggerProgressUpdated();
        }

        public void RecordCollectable(ICollectable c)
        {
            switch(c.GetType().Name)
            {
                case "Diamond":
                    if(!_collectedDiamonds.Contains(c))
                        _collectedDiamonds.Add(c);
                    break;
                case "Checkpoint":
                    //_checkpointed = true;
                    if (!_collectedCheckpoints.Contains(c))
                        _collectedCheckpoints.Add(c);
                    break;
            }
        }

        private void Awake()
        {
            instance = this;

            Init();

            _collectedDiamonds.Clear();
            _collectedCheckpoints.Clear();
        }
        private void OnEnable()
        {
            GameEvents.OnGameOver += UpdateCurrentProgress;
        }
        private void OnDestroy()
        {
            GameEvents.OnGameOver -= UpdateCurrentProgress;
        }
    }
}
