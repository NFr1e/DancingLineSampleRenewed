using DG.Tweening;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DancingLineFanmade.Triggers;

namespace DancingLineFanmade.Gameplay
{
    [DisallowMultipleComponent]
    public class CameraFollower : MonoBehaviour,IResettable
    {
        private Transform selfTransform;

        public static CameraFollower Instance { get; private set; }

        public Transform target;
        public Transform rotator;
        public Transform scale;
        public Camera followingCamera;
        public Vector3 defaultOffset = Vector3.zero;
        public Vector3 defaultRotation = new(60f, 45f, 0f);
        public Vector3 defaultScale = Vector3.one;
        public Vector3 followSpeed = new(1.2f, 3f, 6f);
        public bool follow = true;
        public bool smooth = true;

        [BoxGroup("Debugging")]
        [SerializeField] private bool DrawTargetGizmos = true,DrawConnectLine = true;
        private Tween offsetTween { get; set; }
        private Tween rotationTween { get; set; }
        private Tween scaleTween { get; set; }
        private Tween shakeTween { get; set; }
        private Tween fovTween { get; set; }
        private Tween bgcolorTween { get; set; }
        private float shakePower { get; set; }

        private readonly Quaternion rotation = Quaternion.Euler(0, -45, 0);

        private bool 
            _followable = true,
            _lastfollowable = true;

        private Vector3 Translation
        {
            get
            {
                var targetPosition = rotation * target.position;
                var selfPosition = rotation * selfTransform.position;
                return targetPosition - selfPosition;
            }
        }

        private Transform origin;

        private void Awake()
        {
            Instance = this;
            selfTransform = transform;
        }
        private void OnEnable()
        {
            RegisterResettable();

            GameEvents.OnStartPlay += SetAsLastFollowable;
            GameEvents.OnGameOver += DisableFollow;
            GameEvents.OnGamePaused += RecordFollowState;

            RespawnAttributes.OnRecording += NoteArgs;
            RespawnEvents.OnEndRespawn += ResetToPlayerTransform;

            TeleportEvents.OnTriggerCameraTeleport += ResetToPlayerTransform;
        }
        private void OnDisable()
        {
            UnregisterResettable();

            GameEvents.OnStartPlay -= SetAsLastFollowable;
            GameEvents.OnGameOver -= DisableFollow;
            GameEvents.OnGamePaused -= RecordFollowState;

            RespawnAttributes.OnRecording -= NoteArgs;
            RespawnEvents.OnEndRespawn -= ResetToPlayerTransform;

            TeleportEvents.OnTriggerCameraTeleport -= ResetToPlayerTransform;
        }

        private void Start()
        {
            selfTransform = transform;
            SetDefaultTransform();
            origin = new GameObject("CameraMovementOrigin")
            {
                transform =
                {
                    position = Vector3.zero,
                    rotation = Quaternion.Euler(0, 45, 0),
                    localScale = Vector3.one
                }
            }.transform;

            _lastfollowable = follow;
        }

        private void Update()
        {
            var translation = new Vector3(Translation.x * Time.smoothDeltaTime * followSpeed.x,
                Translation.y * Time.smoothDeltaTime * followSpeed.y,
                Translation.z * Time.smoothDeltaTime * followSpeed.z);
            if ((GameController.curGameState == GameState.Playing || GameController.curGameState == GameState.Respawning) && _followable && follow)
                selfTransform.Translate(smooth ? translation : Translation, origin);
        }

        private void EnableFollow() => _followable = true;
        private void DisableFollow() => _followable = false;
        public void SetFollowState(bool foll) => _followable = foll;
        public void RecordFollowState() => _lastfollowable = _followable;
        private void SetAsLastFollowable() => _followable = _lastfollowable;
        public void ResetToPlayerTransform() 
        {
            bool _follow = _followable;
            _followable = false;
            transform.position = target.position;
            SetFollowState(_follow);
        }
        public void Trigger(Vector3 n_offset, Vector3 n_rotation, Vector3 n_scale, float n_fov, float duration,
            Ease ease, RotateMode mode, UnityEvent callback, bool use = false, AnimationCurve curve = null)
        {
            SetOffset(n_offset, duration, ease, use, curve);
            SetRotation(n_rotation, duration, mode, ease, use, curve);
            SetScale(n_scale, duration, ease, use, curve);
            SetFov(n_fov, duration, ease, use, curve);
            rotationTween.OnComplete(callback.Invoke);
        }

        public void KillAll()
        {
            offsetTween?.Kill();
            rotationTween?.Kill();
            scaleTween?.Kill();
            shakeTween?.Kill();
            fovTween?.Kill();
        }

        private void SetOffset(Vector3 n_offset, float duration, Ease ease, bool use, AnimationCurve curve)
        {
            if (offsetTween != null)
            {
                offsetTween.Kill();
                offsetTween = null;
            }

            offsetTween = !use
                ? rotator.DOLocalMove(n_offset, duration).SetEase(ease)
                : rotator.DOLocalMove(n_offset, duration).SetEase(curve);
        }

        private void SetRotation(Vector3 n_rotation, float duration, RotateMode mode, Ease ease, bool use,
            AnimationCurve curve)
        {
            if (rotationTween != null)
            {
                rotationTween.Kill();
                rotationTween = null;
            }

            rotationTween = !use
                ? rotator.DOLocalRotate(n_rotation, duration, mode).SetEase(ease)
                : rotator.DOLocalRotate(n_rotation, duration, mode).SetEase(curve);
        }

        private void SetScale(Vector3 n_scale, float duration, Ease ease, bool use, AnimationCurve curve)
        {
            if (scaleTween != null)
            {
                scaleTween.Kill();
                scaleTween = null;
            }

            scaleTween = !use
                ? scale.DOScale(n_scale, duration).SetEase(ease)
                : scale.DOScale(n_scale, duration).SetEase(curve);
        }

        private void SetFov(float n_fov, float duration, Ease ease, bool use, AnimationCurve curve)
        {
            if (fovTween != null)
            {
                fovTween.Kill();
                fovTween = null;
            }

            fovTween = !use
                ? followingCamera.DOFieldOfView(n_fov, duration).SetEase(ease)
                : followingCamera.DOFieldOfView(n_fov, duration).SetEase(curve);
        }
        public void SetBackgroundColor(Color bgcolor,float duration,Ease ease)
        {
            if(bgcolorTween != null)
            {
                bgcolorTween.Kill();
                bgcolorTween = null;
            }

            bgcolorTween = DOTween
                .To(() => 
                followingCamera.backgroundColor, x => followingCamera.backgroundColor = x
                , bgcolor, duration)
                .SetEase(ease);
        }
        public void DoShake(float power = 1f, float duration = 3f)
        {
            if (shakeTween != null)
            {
                shakeTween.Kill();
                shakeTween = null;
            }

            shakeTween = DOTween
                .To(() => shakePower, x => shakePower = x, power, duration * 0.5f).SetEase(Ease.Linear)
                .SetLoops(2, LoopType.Yoyo)
                .OnUpdate(ShakeUpdate)
                .OnComplete(ShakeFinished);
        }

        private void ShakeUpdate()
        {
            scale.transform.localPosition = new Vector3(UnityEngine.Random.value * shakePower,
                UnityEngine.Random.value * shakePower, UnityEngine.Random.value * shakePower);
        }

        private void ShakeFinished()
        {
            scale.transform.localPosition = Vector3.zero;
        }

        private void SetDefaultTransform()
        {
            rotator.localPosition = defaultOffset;
            rotator.eulerAngles = defaultRotation - new Vector3(60f, 0f, 0f);
            scale.localScale = defaultScale;
        }

        #region Reset

        private Vector3
            r_offset = new(0,0,0),
            r_rotation = new(60,45,0),
            r_scale = new(1,1,1),
            r_lerp = new(1.2f,3,6);
        private float r_fov;
        private bool r_smooth;

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
            r_offset = rotator.localPosition;
            r_rotation = rotator.eulerAngles;
            r_scale = scale.localScale;
            r_lerp = followSpeed;
            r_fov = followingCamera.fieldOfView;
            r_smooth = smooth;
        }
        public void ResetArgs()
        {
            KillAll();

            rotator.localPosition = r_offset;
            rotator.eulerAngles = r_rotation;
            scale.localScale = r_scale;
            followSpeed = r_lerp;
            followingCamera.fieldOfView = r_fov;
            smooth = r_smooth;

            Debug.Log($"{name} Reset");
        }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                SetDefaultTransform();
        }
        private void OnDrawGizmos()
        {
            Vector3 eulerPos = followingCamera.transform.position, followSpdPos = followingCamera.transform.position + Vector3.up * 1.5f;

            Color _backgroundColor = new Color(0, 0, 0, 0.5f);
            Texture2D _background = _backgroundColor.ToTexture2D();

            GUIStyle style = new()
            {
                fontSize = 15,
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                    background = _background
                }
            };

            if (DrawConnectLine) Gizmos.DrawLine(followingCamera.transform.position, target.position);
            if (DrawTargetGizmos) Gizmos.DrawCube(target.transform.position, Vector3.one * 0.5f);

            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView == null) return;
            Camera sceneCamera = sceneView.camera;
            if (sceneCamera == null) return;
            float distance = Vector3.Distance(transform.position, sceneCamera.transform.position);
            if (distance > 40) return;

            Handles.Label(eulerPos, $"RotatorEuler:{rotator.transform.eulerAngles}", style);
            Handles.Label(followSpdPos, $"FollowSpeed:{followSpeed}", style);
        }
#endif
    }

    [Serializable]
    public class CameraSettings
    {
        public Vector3 offset;
        public Vector3 rotation;
        public Vector3 scale;
        public float fov;
        public bool follow;

        public CameraSettings GetCamera()
        {
            var settings = new CameraSettings();
            var follower = CameraFollower.Instance;
            settings.offset = follower.rotator.localPosition;
            settings.rotation = follower.rotator.localEulerAngles + new Vector3(60f, 0f, 0f);
            settings.scale = follower.scale.localScale;
            settings.fov = follower.followingCamera.fieldOfView;
            settings.follow = follower.follow;
            return settings;
        }

        public void SetCamera()
        {
            var follower = CameraFollower.Instance;
            follower.rotator.localPosition = offset;
            follower.rotator.localEulerAngles = rotation - new Vector3(60f, 0f, 0f);
            follower.scale.localScale = scale;
            follower.scale.localPosition = Vector3.zero;
            follower.followingCamera.fieldOfView = fov;
            follower.follow = follow;
        }
    }
}