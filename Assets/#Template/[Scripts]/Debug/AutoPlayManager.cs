using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DancingLineFanmade.Audio;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Debugging
{
    //bug太多了。。。不如Player里的AutoPlay一根
    public class AutoPlayEvents
    {
        public static event System.Action<Vector3> OnArriveKey;
        public static void ArriveKey(Vector3 pos) => OnArriveKey?.Invoke(pos);
    }
    public class AutoPlayManager : MonoBehaviour
    {
        [BoxGroup("AutoPlay")]
        public bool 
            AutoPlay = false, 
            FixPosition = true;
        [BoxGroup("AutoPlay")]
        [SerializeField]
        private RotatePointsBuffer _pointsBuffer;
        [BoxGroup("AutoPlay")]
        [SerializeField]
        private AudioManager _audioManager;

        private float _startTime => _audioManager ? _audioManager.CurrentLevelTime : 0;
        private int _leastNextTimeCount = 0;
        private RotatePoints _leastNextKey
        {
            get
            {
                RotatePoints result = new()
                {
                    Time = 0,
                    Position = Vector3.zero,
                    Euler = Vector3.zero
                };

                for(int i = 0;i<_pointsBuffer.SavedPoints.Count;i++)
                {
                    if (_pointsBuffer.SavedPoints[i].Time > _startTime)
                    { 
                        result.Time = _pointsBuffer.SavedPoints[i].Time - _startTime;
                        result.Position = _pointsBuffer.SavedPoints[i].Position;
                        _leastNextTimeCount = i;
                        break;
                    }
                }

                return result;
            }
        }
        
        private List<RotatePoints> _fixedKeys = new();
        private float _nextTime;

        public void OnEnable()
        {
            PlayerEvents.OnPlayerStart += InitializeKeys;
            PlayerEvents.OnPlayerStart += StartAutoPlay;
        }
        public void OnDisable()
        {
            PlayerEvents.OnPlayerStart -= InitializeKeys;
            PlayerEvents.OnPlayerStart -= StartAutoPlay;
        }

        private void InitializeKeys()
        {
            _fixedKeys.Clear();
            _fixedKeys.Add(_leastNextKey);

            for (int k = 1; k < _pointsBuffer.SavedPoints.Count - _leastNextTimeCount; k++)
            {
                int index = _leastNextTimeCount + k;
                float adjustedTime = _pointsBuffer.SavedPoints[index].Time - _startTime;
                RotatePoints point = new()
                {
                    Time = adjustedTime,
                    Position = _pointsBuffer.SavedPoints[index].Position,
                    Euler = _pointsBuffer.SavedPoints[index].Euler
                };

                _fixedKeys.Add(point);
            }
        }
        public void StartAutoPlay()
        {
            if(AutoPlay)
                StartCoroutine(DoAutoPlay());
        }
        private IEnumerator DoAutoPlay()
        {
            
            for (int i = 0; i < _fixedKeys.Count; i++)
            {
                if (i <= 0) _nextTime = _fixedKeys[i].Time;
                else _nextTime = _fixedKeys[i].Time - _fixedKeys[i - 1].Time;

                yield return new WaitForSeconds(_nextTime);
                AutoPlayEvents.ArriveKey(_fixedKeys[i].Position);
            }
        }

    }
}
