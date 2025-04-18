using UnityEngine;
using UnityEditor;
using DG.Tweening;
using Sirenix.OdinInspector;
using DancingLineFanmade.Audio;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Triggers
{
    public class HintBoxTrigger : MonoBehaviour , IResettable
    {
        public float TriggerTime;
        [SerializeField]
        private GameObject TriggeredEffect;
        [SerializeField] 
        private UnityEventTrigger _collider;
        [SerializeField]
        private MeshRenderer _renderer;

        public GameObject childLine;

        [SerializeField,BoxGroup("Debug")] 
        private bool 
            DrawHintBoxTime = true, 
            DrawHintBoxCollider = true;

        private bool
            _triggerable = false,
            _displayable = false,
            _triggered = false,
            _lineDisapperaTimeArrive = false;
        

        private float s_interval = 0.1f;

        private AudioManager _audioManager;

        private void OnEnable()
        {
            RegisterResettable();

            PlayerEvents.OnPlayerStart += CheckTriggerTime;
            PlayerEvents.OnPlayerRotate += CheckTriggerTime;

            _collider.OnEnter.AddListener(GetAudioManager);
            _collider.OnStay.AddListener(OnPlayerStay);
        }
        private void OnDisable()
        {
            UnregisterResettable();

            PlayerEvents.OnPlayerStart -= CheckTriggerTime;
            PlayerEvents.OnPlayerRotate -= CheckTriggerTime;

            _collider.OnEnter.RemoveListener(GetAudioManager);
            _collider.OnStay.RemoveListener(OnPlayerStay);
        }
        private void FixedUpdate() => HandleDisplay();
        private void Start() 
        { 
            GetAudioManager();
        }
        private void GetAudioManager() => _audioManager = AudioManager.instance;

        #region Handle Display
        private void HandleDisplay()
        {
            _displayable = _audioManager.CurrentLevelTime >  TriggerTime - 2;

            if (!_triggered && GameController.curGameState != GameState.Over)
            {
                _renderer.gameObject.SetActive(_displayable);
                if(!_lineDisapperaTimeArrive && childLine) childLine.SetActive(_displayable);//使用空条件运算符通过不了Unity的序列化检查
            }
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
            if (!GuidanceManager._isUsing) return;

            float uplimit,lowlimit;
            uplimit = TriggerTime + s_interval;
            lowlimit = TriggerTime - s_interval;

            _triggerable = 
                _audioManager.CurrentLevelTime > lowlimit 
                && _audioManager.CurrentLevelTime < uplimit;

            if (_audioManager.CurrentLevelTime > lowlimit)
            {
                _lineDisapperaTimeArrive = true;
                if(childLine)childLine.SetActive(false);
            }
        }
        private void AnimateTriggered()
        {
            _renderer.gameObject.SetActive(false);

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
        #endregion

        #region Reset
        /// <summary>
        /// 一般在OnEnable中调用
        /// </summary>
        private void RegisterResettable() => ResettableManager.Register(this);
        /// <summary>
        /// 一般在OnDisable中调用
        /// </summary>
        private void UnregisterResettable() => ResettableManager.Unregister(this);

        public void NoteArgs() { }
        public void ResetArgs()
        {
            _triggered = false;
            _lineDisapperaTimeArrive = false;
            if(childLine)childLine.SetActive(true);

            Debug.Log($"{name} Reset");
        }
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
            Texture2D _background = _backgroundColor.ToTexture2D();

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
