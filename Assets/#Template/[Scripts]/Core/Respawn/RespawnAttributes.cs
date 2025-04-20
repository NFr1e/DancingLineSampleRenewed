using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using DancingLineFanmade.Audio;
using DancingLineFanmade.Gameplay;
using DancingLineFanmade.Triggers;
using DancingLineFanmade.Collectable;

public class RespawnAttributes : MonoBehaviour
{
    [BoxGroup("General"),
    SerializeField]
        private UnityEventTrigger
            CheckpointTrigger,
            RecordTrigger;
    [BoxGroup("General"),
    SerializeField]
        private bool
            AutoRecord = true,
            AutoRespawnTransform = false;
    [BoxGroup("General"),
    SerializeField]
        private Player CurrentPlayer;
    [BoxGroup("General"),
    SerializeField]
        private AudioManager CurrentAudioManager;

    [BoxGroup("PlayerAttributes")]
    [SerializeField]
        private Transform RespawnTransform;
    [BoxGroup("PlayerAttributes"),
    SerializeField]
        private Vector3 
            FirstDir = new(0,90,0),
            SecondDir = Vector3.zero;
    [BoxGroup("PlayerAttributes"),
    SerializeField]
        private float DefaultPlayerSpeed = 12f;

    [BoxGroup("AudioAttributes"),
    SerializeField]
        private float LevelSoundtrackTime = 0;

    private bool _recorded = false;

    public static event System.Action<RespawnAttributes> OnSetAttribute;
    public static event System.Action OnRecording;

    private void Start()
    {
        if(CheckpointTrigger)
            CheckpointTrigger.OnEnter.AddListener(() =>
            {
                OnSetAttribute?.Invoke(this);
            });

        
        RecordTrigger.OnEnter.AddListener(() => 
        {
            OnRecording?.Invoke();

            if (AutoRecord)
                RecordAttributes();

            RecordTrigger.gameObject.SetActive(false);
        });
    }
    private void RecordAttributes()
    {
        if (_recorded) return;

        if (CurrentPlayer)
        {
            if (AutoRespawnTransform)
            {
                RespawnTransform.position = CurrentPlayer.transform.position;
                RespawnTransform.rotation = CurrentPlayer.transform.rotation;
            }
            FirstDir = CurrentPlayer.firstDirection;
            SecondDir = CurrentPlayer.secondDirection;
            DefaultPlayerSpeed = CurrentPlayer.DefaultPlayerSpeed;
        }
        if(CurrentAudioManager)
        {
            LevelSoundtrackTime = CurrentAudioManager.CurrentLevelTime;
        }
        _recorded = true;
    }
    /// <summary>
    /// »Ö¸´Attributes
    /// </summary>
    public void ResetAttributes()
    {
        CurrentPlayer.transform.position = RespawnTransform.position;
        CurrentPlayer.transform.rotation = RespawnTransform.rotation;
        CurrentPlayer.firstDirection = FirstDir;
        CurrentPlayer.secondDirection = SecondDir;
        CurrentPlayer.DefaultPlayerSpeed = DefaultPlayerSpeed;

        AudioEvents.AlterSoundtrackTime(LevelSoundtrackTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        SceneView sceneView = SceneView.currentDrawingSceneView;
        if (sceneView == null) return;
        Camera sceneCamera = sceneView.camera;
        if (sceneCamera == null) return;
        float distance = Vector3.Distance(transform.position, sceneCamera.transform.position);
        if (distance > 40) return;

        if (!RecordTrigger) return;

        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.TRS(
            RecordTrigger.transform.position,
            RecordTrigger.transform.rotation,
            Vector3.one
        );
        Gizmos.DrawCube(Vector3.zero, RespawnTransform.localScale);
    }
#endif
}
