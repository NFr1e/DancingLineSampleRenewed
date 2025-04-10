using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using Sirenix.OdinInspector;
using DancingLineFanmade.UI;
using DancingLineFanmade.Debugging;
using DancingLineFanmade.Audio;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Triggers
{
    public class HintBoxTrigger : MonoBehaviour
    {
        public float TriggerTime;
        [SerializeField]
        private GameObject TriggeredEffect;
        [SerializeField] 
        private UnityEventTrigger _collider;
        [SerializeField]
        private MeshRenderer _renderer;
        [SerializeField,BoxGroup("Debug")] 
        private bool 
            DrawHintBoxTime = true, 
            DrawHintBoxCollider = true;

        private bool 
            _triggerable = false,
            _displayable = false,
            _triggered = false;

        private AudioManager _audioManager;

        public static float s_interval = 0.1f;
        private void OnEnable()
        {
            PlayerEvents.OnPlayerStart += CheckTriggerTime;
            PlayerEvents.OnPlayerRotate += CheckTriggerTime;
            RespawnEvents.OnRespawning += OnRespawn;

            _collider.OnStay.AddListener(() => OnPlayerStay());
        }
        private void OnDestroy()
        {
            PlayerEvents.OnPlayerStart -= CheckTriggerTime;
            PlayerEvents.OnPlayerRotate -= CheckTriggerTime;
            RespawnEvents.OnRespawning -= OnRespawn;

            _collider.OnStay.RemoveListener(() => OnPlayerStay());
        }
        private void Start()
        {
            _audioManager = AudioManager.instance;
        }
        private void FixedUpdate()
        {
            HandleDisplay();
        }
        #region Handle Display
        private void HandleDisplay()
        {
            _displayable = Vector3.Distance(transform.position, Player.instance.transform.position) <= 20;

            if (!_triggered)
                _renderer.enabled = _displayable;
        }
        #endregion

        #region Handle Trigger
        private void OnPlayerStay()
        {
            if (!_triggerable) return;

            AnimateTriggered();
        }
        private void CheckTriggerTime()
        {
            float uplimit,lowlimit;
            uplimit = TriggerTime + s_interval;
            lowlimit = TriggerTime - s_interval;

            if (
                _audioManager.CurrentLevelTime > lowlimit
                &&
                _audioManager.CurrentLevelTime < uplimit
              )
                _triggerable = true;
            else
                _triggerable = false;

        }
        private void AnimateTriggered()
        {
            _renderer.enabled = false;

            GameObject effect = Instantiate(TriggeredEffect,transform.position,transform.rotation,GameController.CollectableRemainParent);
            effect.transform.localScale = 0.15f * Vector3.one;

            effect.GetComponent<MeshRenderer>().material
                .DOFade(0, 1)
                .SetUpdate(true);
            effect.transform
                .DOScale(0.5f * Vector3.one ,1)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    Destroy(effect);
                });

            _triggerable = false;
            _triggered = true;
        }
        private void OnRespawn() => _triggered = false;
        #endregion
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView == null) return;
            Camera sceneCamera = sceneView.camera;
            if (sceneCamera == null) return;
            float distance = Vector3.Distance(transform.position, sceneCamera.transform.position);
            if (distance > 40) return;

            Vector3 textPosition = transform.position;

            Color _backgroundColor = new Color(255, 255, 255, 0.5f);
            Texture2D _background = ExtensionUtils.ToTexture2D(_backgroundColor);

            GUIStyle style = new()
            {
                fontSize = 15,
                normal = new GUIStyleState
                {
                    textColor = Color.black,
                    background = _background
                }
            };
            if (DrawHintBoxTime)
                Handles.Label(textPosition, $"Time:{TriggerTime}", style);
            if (DrawHintBoxCollider)
            {
                if (!_collider) return;
                Gizmos.color = new Color(255, 255, 255, 0.5f);
                Gizmos.DrawWireCube(_collider.transform.position, _collider.transform.localScale);
            }
        }
#endif
    }
}
