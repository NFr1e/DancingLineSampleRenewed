using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Collectable;
using DancingLineFanmade.Audio;
using DancingLineFanmade.Triggers;

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
        private bool _isPassed = false;

        private List<ICollectable> _collectedDiamonds = new();
        private List<ICollectable> _collectedCheckpoints = new();

        [System.Serializable]public class LevelProgress
        {
            public int Percentage, DiamondCount, CheckpointCount;
        }
        public LevelProgress currentProgress;
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
                Percentage = _isPassed 
                             ? 100 
                             : (int)(CurrentAudioManager.CurrentLevelTime * 100 / _levelLength),
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
                    if (!_collectedCheckpoints.Contains(c))
                        _collectedCheckpoints.Add(c);
                    break;
            }
        }
        private void GamePassed() => _isPassed = true;
        private void Awake()
        {
            instance = this;

            InitializeReference();

            ClearCollectedDiamonds();
            ClearCollectedCheckpoints();
        }
        private void ClearCollectedDiamonds() => _collectedDiamonds.Clear();
        private void ClearCollectedCheckpoints() => _collectedCheckpoints.Clear();
        public void ClearConsumedCheckpopint(ICollectable target) => _collectedCheckpoints.Remove(target);
        private void OnEnable()
        {
            RespawnEvents.OnRespawning += ClearCollectedDiamonds;
            PyramidTrigger.OnEnterPyramidTrigger += GamePassed;

            GameEvents.OnGameOver += UpdateCurrentProgress;
        }
        private void OnDestroy()
        {
            RespawnEvents.OnRespawning -= ClearCollectedDiamonds;
            PyramidTrigger.OnEnterPyramidTrigger -= GamePassed;

            GameEvents.OnGameOver -= UpdateCurrentProgress;
        }
    }
}
