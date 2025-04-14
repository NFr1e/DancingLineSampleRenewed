using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using DancingLineFanmade.Triggers;
using DancingLineFanmade.UI;
using DancingLineFanmade.Audio;
using DancingLineFanmade.Debugging;

namespace DancingLineFanmade.Gameplay
{
    public enum OverMode
    {
        Win,
        Hit,
        Drowned,
        Fall
    }
    public static class PlayerEvents
    {
        public static event System.Action 
            OnPlayerStart,
            OnPlayerWin,
            OnPlayerHit,
            OnPlayerDrowned,
            OnPlayerFall,
            OnPlayerRotate,
            OnPlayerLanding;

        public static void TriggerStartEvent() => OnPlayerStart?.Invoke();
        public static void TriggerWinEvent() => OnPlayerWin?.Invoke();
        public static void TriggerHitEvent() => OnPlayerHit?.Invoke();
        public static void TriggerDrownEvent() => OnPlayerDrowned?.Invoke();
        public static void TriggerFallEvent() => OnPlayerFall?.Invoke();
        public static void TriggerRotateEvent() => OnPlayerRotate?.Invoke();
        public static void TriggerLandingEvent() => OnPlayerLanding?.Invoke();
    }
    [RequireComponent(typeof(Rigidbody),typeof(BoxCollider))][DisallowMultipleComponent]
    public class Player : MonoBehaviour
    {
        public static Player instance;

        public float DefaultPlayerSpeed = 12;
        [SerializeField]private float _checkGroundMaxDistance = 0.8f;
        [BoxGroup("Prefabs")]public GameObject PlayerTailPrefab;
        [BoxGroup("Prefabs")] public GameObject[] DieEffects;
        [BoxGroup("Prefabs")] public GameObject[] LandingEffects;

        [BoxGroup("Directions")] public Vector3 startDirection = new(0, 90, 0);
        [BoxGroup("Directions")] public Vector3 firstDirection = new(0, 90, 0);
        [BoxGroup("Directions")] public Vector3 secondDirection = Vector3.zero;

        [BoxGroup("Collide")] public BoxCollider CentralCollider;
        [BoxGroup("Collide")] public LayerMask HitLayer,DrownLayer,FallLayer, floorLayer;

        [BoxGroup("Debugging")]
        public bool DrawGizmos = true;
        [BoxGroup("AutoPlay")]
        public bool AutoPlay = false,FixPosition = true;
        [BoxGroup("AutoPlay")][SerializeField]
        private RotatePointsBuffer _pointsBuffer;
        [BoxGroup("AutoPlay")] [SerializeField]
        private AudioManager _audioManager;

        [HideInInspector]public Rigidbody _rigid;
        private float _playerSpeedMultiply = 1;

        private OverMode _overMode;

        private bool _isGrounded = true;
        private bool _controllable = true;
        private bool _inputCooldown = true;
        private bool _onCollider = true;
        private bool _spawnTail = true;
        private Vector3 _lastGroundPosition;
        private Vector3 _enterCollisionPosition;

        private float _playerCurSpeed
        {
            get 
            { 
                return DefaultPlayerSpeed * _playerSpeedMultiply; 
            }
        }
        private PlayerTail activeTail;
        private class PlayerTail
        {
            public Transform _transform;
            public Vector3 startPosition;
            public bool isStretching = true;
        }

        private void Awake()
        {
            instance = this;
            _rigid = GetComponent<Rigidbody>();
        }
        private void OnEnable()
        {
            StairEvents.OnEndLaunch += SetAsStartDirection;

            TriggerCallOutmapEvent.OnEnterOutmapTrigger += PlayerFall;

            TriggerCallDrownEvent.OnEnterDrownTrigger += PlayerDrown;

            GameEvents.OnEnterLevel += PlayerInit;
            GameEvents.OnStartPlay += StartPlayer;
            GameEvents.OnGamePaused += PlayerInit;

            RespawnEvents.OnEndRespawn += PlayerInit;
        }
        private void OnDisable()
        {
            StairEvents.OnEndLaunch -= SetAsStartDirection;

            TriggerCallOutmapEvent.OnEnterOutmapTrigger -= PlayerFall;

            TriggerCallDrownEvent.OnEnterDrownTrigger -= PlayerDrown;

            GameEvents.OnEnterLevel -= PlayerInit;
            GameEvents.OnStartPlay -= StartPlayer;
            GameEvents.OnGamePaused -= PlayerInit;

            RespawnEvents.OnEndRespawn -= PlayerInit;
        }
        private void Update()
        {
            HandleInput();
            UpdatePlayerMovement();
            CheckGroundStatus();
            UpdateActiveTail();

            HandleAutoPlay();
        }
        private void PlayerInit()
        {
            _rigid.useGravity = false;
            _inputCooldown = true;

            gameObject.tag = "Player";

            Debug.Log($"{GetType().Name}InitDone");

            if (!AutoPlay) return;

            SetControllable(false);
            for (int i = 0; i < _pointsBuffer.SavedPoints.Count; i++)
            {
                _pointsBuffer.SavedPoints[i].AutoPlayRotated = false;
            }
        }
        private void StartPlayer()
        {
            _rigid.useGravity = true;
            StartCoroutine(InputCooldown());
            CreateTail(transform.position);
            PlayerEvents.TriggerStartEvent();
        }
        private void UpdatePlayerMovement()
        {
            if (GameController.curGameState == GameState.Playing 
                || (GameController.curGameState == GameState.Respawning && _overMode != OverMode.Hit) 
                ||(GameController.curGameState == GameState.Over && _overMode != OverMode.Hit))
            {
                transform.Translate(new Vector3(0, 0, _playerCurSpeed) * Time.deltaTime, Space.Self);
            }
        }
        private void HandleInput()
        {
            if (_inputCooldown || GameController.curGameState != GameState.Playing) return;

            if (GameController.Clicked && _controllable && _isGrounded && _controllable)
            {
                StartCoroutine(InputCooldown());
                RotatePlayer();
            }
        }
        /// <summary>
        /// 一般在Playing状态时调用，用于输入冷却
        /// </summary>
        /// <returns></returns>
        private IEnumerator InputCooldown()
        {
            _inputCooldown = true;
            yield return new WaitForSeconds(0.05f);
            _inputCooldown = false;
        }
        /// <summary>
        /// 使Player改变方向
        /// </summary>
        public void RotatePlayer()
        {
            Quaternion targetRotation = transform.rotation.eulerAngles == firstDirection
                ? Quaternion.Euler(secondDirection)
                : Quaternion.Euler(firstDirection);

            transform.rotation = targetRotation;
            CreateTail(transform.position);
            PlayerEvents.TriggerRotateEvent();
            Debug.Log("PlayerRotated");
        }
        /// <summary>
        /// 创建新的activeTail
        /// </summary>
        /// <param name="startPos">初始位置</param>
        private void CreateTail(Vector3 startPos)
        {
            if (_spawnTail && _isGrounded)
            {
                activeTail = new PlayerTail
                {
                    _transform = Instantiate(PlayerTailPrefab, startPos, transform.rotation, GameController.PlayerRemainParent).transform,
                    startPosition = startPos,
                    isStretching = true
                };
            }
        }
        /// <summary>
        /// 拉伸ActiveTail
        /// </summary>
        private void UpdateActiveTail()
        {
            if (activeTail != null && _isGrounded && 
                (GameController.curGameState == GameState.Playing || 
                (GameController.curGameState == GameState.Over && _overMode != OverMode.Hit)||
                GameController.curGameState == GameState.Respawning))
            {
                activeTail._transform.localScale = new Vector3(activeTail._transform.localScale.x, activeTail._transform.localScale.y, Vector3.Distance(activeTail.startPosition, this.gameObject.transform.position) + 0.5f);
                activeTail._transform.position = (activeTail.startPosition + this.gameObject.transform.position) / 2;
                activeTail._transform.Translate(Vector3.back * 0.25f, Space.Self);
                activeTail._transform.LookAt(gameObject.transform);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (collision.contacts[i].thisCollider == CentralCollider && GameController.curGameState != GameState.Over)
                {
                    _overMode = OverMode.Hit;
                    PlayerDie();
                }
            }
            _enterCollisionPosition = transform.position;
        }
        private void OnCollisionStay(Collision collision) => _onCollider = true;
        private void OnCollisionExit(Collision collision) => _onCollider = false;
        /// <summary>
        /// Player坠落或超出地图范围时调用
        /// </summary>
        private void PlayerFall()
        {
            _overMode = OverMode.Fall;
            PlayerDie();
        }
        /// <summary>
        /// Player坠落于水中时调用
        /// </summary>
        private void PlayerDrown()
        {
            _overMode = OverMode.Drowned;
            PlayerDie();
        }
        /// <summary>
        /// 一般在CheckGroundStatus中调用，用于检Player测射线下的对象Layer并设置OverMode
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="mode"></param>
        private void CheckSpecialLayers(LayerMask layer, OverMode mode)
        {
            if (Physics.Raycast(transform.position, Vector3.down,
                _checkGroundMaxDistance + 0.1f, layer) && GameController.curGameState != GameState.Over)
            {
                _overMode = mode;
                PlayerDie();
            }
        }
        /// <summary>
        /// 检测Player是否落地
        /// </summary>
        private void CheckGroundStatus()
        {
            bool wasGrounded = _isGrounded;
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, _checkGroundMaxDistance, floorLayer) && _onCollider;

            CheckSpecialLayers(HitLayer, OverMode.Hit);
            CheckSpecialLayers(DrownLayer, OverMode.Drowned);
            CheckSpecialLayers(FallLayer, OverMode.Fall);

            if (!wasGrounded && _isGrounded)
            {
                PlayerLanding();
            }
            else if (_isGrounded)
            {
                _lastGroundPosition = transform.position;
            }

            if (!_isGrounded && activeTail != null)
            {
                activeTail.isStretching = false;
            }
        }
        /// <summary>
        /// 在Player刚落地时调用
        /// </summary>
        private void PlayerLanding()
        {
            Debug.Log($"{GetType().Name}:{name}Landed");

            if (GameController.curGameState == GameState.Playing ||
                (GameController.curGameState == GameState.Over && _overMode != OverMode.Hit))
            {
                PlayerEvents.TriggerLandingEvent();
                CreateTail(transform.position);
            }
            if (Vector3.Distance(_enterCollisionPosition, _lastGroundPosition) > 1f)
            {
                CreateLandingEffect();
            }
        }
        /// <summary>
        /// 一般在PlayerLanding中调用，用于生成LandingEffects等特效
        /// </summary>
        private void CreateLandingEffect()
        {
            if (GameController.curGameState == GameState.Playing || 
                (GameController.curGameState == GameState.Over && _overMode != OverMode.Hit))
            {
                foreach (GameObject a in LandingEffects)
                {
                    Instantiate(a, transform.position, transform.rotation, GameController.PlayerRemainParent);
                }
            }
        }
        private void CreateDieEffect()
        {
            switch (_overMode)
            {
                case OverMode.Hit:
                    foreach (GameObject a in DieEffects)
                    {
                        Instantiate(a, transform.position, transform.rotation, GameController.PlayerRemainParent);
                    }
                    break;
                case OverMode.Drowned:
                    break;
            }
        }
        private void PlayerDie()
        {
            if (GameController.curGameState == GameState.Respawning) return;
            //if (AutoPlay) return;

            CreateDieEffect();

            switch (_overMode)
            {
                case OverMode.Win:
                    PlayerEvents.TriggerWinEvent();
                    break;
                case OverMode.Hit:
                    PlayerInit();
                    PlayerEvents.TriggerHitEvent();
                    break;
                case OverMode.Drowned:
                    PlayerEvents.TriggerDrownEvent();
                    break;
                case OverMode.Fall:
                    PlayerEvents.TriggerFallEvent();
                    break;
            }

            Debug.Log($"{GetType().Name}: PlayerDie(),OverMode:{_overMode}");
        }
        public void SetControllable(bool ctrllable) => _controllable = ctrllable;
        public void SetSpawnTailAbility(bool ability) => _spawnTail = ability;

        /// <summary>
        /// 设置Player的角度为StartDirection
        /// </summary>
        public void SetAsStartDirection()
        {
            transform.eulerAngles = startDirection;
        }
        /// <summary>
        /// 改变Player的firstDirection和secondDirection
        /// </summary>
        /// <param name="first">new firstDirection</param>
        /// <param name="second">new secondDirection</param>
        public void SetPlayerDirection(Vector3 first,Vector3 second)
        {
            firstDirection = first;
            secondDirection = second;
        }
        /// <summary>
        /// 自动游玩
        /// </summary>
        private void HandleAutoPlay()
        {
            if (!AutoPlay) return;
            if (!_pointsBuffer) return;
            if (!_audioManager) return;

            for(int a = 1;a < _pointsBuffer.SavedPoints.Count - 1;a++)
            {
                if (!_pointsBuffer.SavedPoints[a].AutoPlayRotated && 
                    _audioManager.CurrentLevelTime >= _pointsBuffer.SavedPoints[a].Time && 
                    GameController.curGameState == GameState.Playing)
                {
                    if(FixPosition)
                        transform.position = _pointsBuffer.SavedPoints[a].Position;
                    RotatePlayer();

                    _pointsBuffer.SavedPoints[a].AutoPlayRotated = true;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!DrawGizmos) return;

            Vector3 textPosition = transform.position + Vector3.up * 1.5f;

            Color _backgroundColor = new Color(255, 255, 255, 0.5f);
            Texture2D _background = _backgroundColor.ToTexture2D();
            
            GUIStyle style = new()
            {
                fontSize = 25,
                normal = new GUIStyleState
                {
                    textColor = Color.black,
                    background = _background
                }
            };
            
            Handles.Label(textPosition, $"Direction:{transform.eulerAngles}", style);
        }
#endif
    }
}
