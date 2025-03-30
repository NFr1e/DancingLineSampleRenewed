using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DancingLineFanmade.Gameplay;
using DancingLineFanmade.Audio;

namespace DancingLineFanmade.Debugging
{
    [System.Serializable]public class RotatePoints
    {
        [BoxGroup("Vectors")]
        public Vector3 Position;
        [BoxGroup("Vectors")]
        public Vector3 Euler;
        public float Time;
        public bool AutoPlayRotated = false;
    }

    public class PlayerRotatePointRecorder : MonoBehaviour
    {
        public Player CurrentPlayer;
        public AudioManager CurrentAudioManager;
        public bool ContainStart;

        public RotatePointsBuffer Buffer;

        public List<RotatePoints> Points;
        private void Start()
        {
            if (ContainStart)
                PlayerEvents.OnPlayerStart += RecordPoint;
            PlayerEvents.OnPlayerRotate += RecordPoint;
        }
        private void RecordPoint()
        {
            if (!CurrentPlayer || !CurrentAudioManager) 
            {
                Debug.LogError($"{GetType().Name}:Œ¥∂®“ÂCurrentPlayer||CurrentAudioManager");
                return; 
            }

            RotatePoints point = new()
            {
                Position = CurrentPlayer.transform.position,
                Euler = CurrentPlayer.transform.eulerAngles,
                Time = CurrentAudioManager.CurrentLevelTime
            };

            Points.Add(point);
        }
        
        [Button(Name ="SaveInBuffer",ButtonHeight = 20)]
        private void SaveInBuffer()
        {
            if (!Buffer) return;

            if (Buffer.SavedPoints == Points) 
            {
                Debug.Log($"{GetType().Name}:Value equals");
                return; 
            }

            Buffer.SavedPoints = Points;

            Debug.Log($"{GetType().Name}:Rotate Points Saved In Buffer");
        }
    }
}
