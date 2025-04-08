using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DancingLineFanmade.Audio;
using DancingLineFanmade.UI;
using DancingLineFanmade.Triggers;

public interface IState
{
    void Enter();
    void Update();
    void Exit();
}
public class StateMachine
{
    private IState curState;
    public void ChangeState(IState newState)
    {
        curState?.Exit();
        curState = newState;
        curState.Enter();
    }
    public void Update() => curState?.Update();
}

namespace DancingLineFanmade.Gameplay
{
    public enum GameState
    {
        OnStair,
        Ready,
        Playing,
        Paused,
        Respawning,
        Over
    }
    public static class GameEvents
    {
        public static event System.Action 
            OnEnterLevel,
            OnStairEvent,
            OnGameReady,
            OnStartPlay,
            OnGamePaused,
            OnRespawning,
            OnRespawnDone,
            OnGameOver,
            OnExitLevel;

        public static void TriggerEnterLevelEvent() => OnEnterLevel?.Invoke();
        public static void TriggerOnStairEvent() => OnStairEvent?.Invoke();
        public static void TriggerReadyEvent() => OnGameReady?.Invoke();
        public static void TriggerPlayEvent() => OnStartPlay?.Invoke();
        public static void TriggerPauseEvent() => OnGamePaused?.Invoke();
        public static void TriggerStartRespawnEvent() => OnRespawning?.Invoke();
        public static void TriggerEndRespawnEvent() => OnRespawnDone?.Invoke();
        public static void TriggerOverEvent() => OnGameOver?.Invoke();
        public static void TriggerExitLevelEvent() => OnExitLevel?.Invoke();

    }

    [DisallowMultipleComponent]
    public class GameController : MonoBehaviour
    {
        public LevelData CurrentLevelData;

        public static GameController instance;

        public static float CurTimeScale => Time.timeScale;

        public static Transform PlayerRemainParent;
        public static Transform CollectableRemainParent;

        private static GameState _curGameState;
        private static readonly object _stateLock = new object();

        public static GameState curGameState
        {
            get { lock (_stateLock) return _curGameState; }
            set { lock (_stateLock) _curGameState = value; }
        }

        /// <summary>
        /// ���¼��ص�ǰ�Ѽ���ĳ���
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="MaskAnim"></param>
        public static void ReloadScene(float duration = 0.5f, bool MaskAnim = true)
        {
            if (MaskAnim)
            {
                Tweener fadeTweener;
                Image mask = UserInterfaceManager.InstantiateImage(new Color(RenderSettings.fogColor.r, RenderSettings.fogColor.g, RenderSettings.fogColor.b, 0));
                fadeTweener = mask.DOFade(1, duration);
                fadeTweener.OnComplete(() =>
                {
                    mask.DOFade(0, 1);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
                    DontDestroyOnLoad(mask.transform.parent.gameObject);
                    Destroy(mask.transform.parent.gameObject, 1f);
                });
            }
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
        /// <summary>
        /// ���߼���Ƿ���������
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static bool CheckRaycastObjects(Vector3 position)
        {
            var data = new PointerEventData(EventSystem.current)
            {
                pressPosition = new Vector2(position.x, position.y),
                position = new Vector2(position.x, position.y)
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            return results.Count > 0;
        }
        /// <summary>
        /// ����Ƿ���
        /// </summary>
        public static bool Clicked
        {
            get
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
                return Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.Space); // �Ƴ��ո�����
#endif
            }
        }
        /// <summary>
        /// �����ʱָ���Ƿ����û�������
        /// </summary>
        public static bool PointerOnUI
        {
            get
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            var touchCount = Input.touchCount;
            if (touchCount != 1) 
                return false;
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                return EventSystem.current.IsPointerOverGameObject(touch.fingerId) || CheckRaycastObjects(touch.position);
            return false;
#else
                if (Clicked)
                    return EventSystem.current.IsPointerOverGameObject() && CheckRaycastObjects(Input.mousePosition);
                return false;
#endif
            }
        }
        /// <summary>
        /// ���¿�ʼ��Ϸ��ݼ�
        /// </summary>
        public static void HandleRestartkeyUpdate()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadScene();
            }
        }
        /// <summary>
        /// ��ͣ��Ϸ��ݼ�
        /// </summary>
        /// <param name="sm"></param>
        public static void HandlePausekeyUpdate(StateMachine sm)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                sm.ChangeState(new PausedState(sm));
            }
        }
        /// <summary>
        /// ��Ϸ����ʱ����
        /// </summary>
        public void GameOver()
        {
            StateMachine stateMachine = new StateMachine();
            stateMachine.ChangeState(new OverState(stateMachine));
        }
        /// <summary>
        /// �Ӽ��㸴��ʱ����
        /// </summary>
        public void Respawn()
        {
            GameEvents.TriggerStartRespawnEvent();
        }
        /// <summary>
        /// ���ɴ��Player�����������
        /// </summary>
        public void CreatePlayerRemainParent()
        {
            if (PlayerRemainParent != null) Destroy(PlayerRemainParent.gameObject);
            PlayerRemainParent = new GameObject("PlayerRemains").transform;
        }
        /// <summary>
        /// ���ɴ��Collectable�����������
        /// </summary>
        public void CreateCollectableRemainParent()
        {
            if (CollectableRemainParent != null) Destroy(CollectableRemainParent.gameObject);
            CollectableRemainParent = new GameObject("CollectableRemains").transform;
        }
        /// <summary>
        /// �������������
        /// </summary>
        public void DestroyAllRemains()
        {
            //Debug.LogWarning($"{GetType().Name} DestroyAllRemains");

            if (CollectableRemainParent != null) Destroy(CollectableRemainParent.gameObject);
            if (PlayerRemainParent != null) Destroy(PlayerRemainParent.gameObject);
        }
        /// <summary>
        /// �л���ǰ��ϷģʽΪReadyState
        /// </summary>
        private void ChangeToReadyState() => _stateMachine.ChangeState(new ReadyState(_stateMachine));
        /// <summary>
        /// �л���ǰ��ϷģʽΪOnStairState
        /// </summary>
        private void ChangeToOnStairState() => _stateMachine.ChangeState(new OnStairState(_stateMachine));
        /// <summary>
        /// �л���ǰ��ϷģʽΪRespawnState
        /// </summary>
        private void ChangeToRespawningState() => _stateMachine.ChangeState(new RespawnState(_stateMachine));
        #region StateMachine
        /// <summary>
        /// ����ؿ�״̬
        /// </summary>
        public class EnterLevelState : IState
        {
            private StateMachine stateMachine;
            public EnterLevelState(StateMachine sm)
            {
                stateMachine = sm;
            }

            public void Enter()
            {
                GameEvents.TriggerEnterLevelEvent();

                Debug.Log($"{GetType().Name} Enter");

                stateMachine.ChangeState(new OnStairState(stateMachine));
            }
            public void Update() { }
            public void Exit() 
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        /// <summary>
        /// ��׼��̨���ϵ�״̬
        /// </summary>
        public class OnStairState : IState
        {
            private StateMachine stateMachine;
            public OnStairState(StateMachine sm)
            {
                stateMachine = sm;
            }

            public void Enter()
            {
                curGameState = GameState.OnStair;
                GameEvents.TriggerOnStairEvent();

                Debug.Log($"{GetType().Name} Enter");
            }

            public void Update()
            {
                
                
            }

            public void Exit()
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        /// <summary>
        /// Ԥ��״̬
        /// </summary>
        public class ReadyState : IState
        {
            private StateMachine stateMachine;
            public ReadyState(StateMachine sm)
            {
                stateMachine = sm;
            }

            public void Enter()
            {
                curGameState = GameState.Ready;
                GameEvents.TriggerReadyEvent();

                Debug.Log($"{GetType().Name} Enter");
                
            }

            public void Update()
            {
                if ((ReadyStairManager.curStairState == StairState.Launched || ReadyStairManager.curStairState == StairState.Disabled) && Clicked && !PointerOnUI)
                {
                    stateMachine.ChangeState(new PlayingState(stateMachine));
                }
                
            }

            public void Exit()
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        /// <summary>
        /// ������״̬
        /// </summary>
        public class PlayingState : IState
        {
            private StateMachine stateMachine;
            public PlayingState(StateMachine sm)
            {
                stateMachine = sm;
            }

            public void Enter()
            {
                curGameState = GameState.Playing;
                GameEvents.TriggerPlayEvent();

                //_ready = true;

                Debug.Log($"{GetType().Name} Enter");
            }

            public void Update()
            {
                HandleRestartkeyUpdate();
                if(curGameState == GameState.Playing) HandlePausekeyUpdate(stateMachine);
            }

            public void Exit()
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        /// <summary>
        /// ������ͣ״̬
        /// </summary>
        public class PausedState : IState
        {
            private StateMachine stateMachine;
            private GameState lastState;
            private float lastTimeScale;
            public PausedState(StateMachine sm)
            {
                stateMachine = sm;
            }

            public void Enter()
            {
                lastState = curGameState;
                lastTimeScale = CurTimeScale;

                curGameState = GameState.Paused;
                if (CurTimeScale != 0) Time.timeScale = 0;

                GameEvents.TriggerPauseEvent();

                Debug.Log($"{GetType().Name} Enter");
            }

            public void Update()
            {
                HandleRestartkeyUpdate();
                if (Input.GetKeyDown(KeyCode.Escape) || (Clicked && !PointerOnUI))
                {
                    Debug.Log($"{GetType().Name} : Inputed");

                    switch (lastState)
                    {
                        case GameState.Playing:
                            stateMachine.ChangeState(new PlayingState(stateMachine));
                            break;
                        case GameState.Over:
                            stateMachine.ChangeState(new OverState(stateMachine));
                            break;
                    }
                }
            }

            public void Exit()
            {
                Time.timeScale = lastTimeScale;
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        /// <summary>
        /// ����״̬
        /// </summary>
        public class RespawnState : IState
        {
            private StateMachine stateMachine;
            public RespawnState(StateMachine sm)
            {
                stateMachine = sm;
            }

            public void Enter()
            {
                curGameState = GameState.Respawning;
                GameEvents.TriggerStartRespawnEvent();

                Debug.Log($"{GetType().Name} Enter");
            }

            public void Update()
            {

            }

            public void Exit()
            {
                GameEvents.TriggerEndRespawnEvent();
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        /// <summary>
        /// ��Ϸ����״̬
        /// </summary>
        public class OverState : IState
        {
            private StateMachine stateMachine;
            public OverState(StateMachine sm)
            {
                stateMachine = sm;
            }

            public void Enter()
            {
                curGameState = GameState.Over;
                GameEvents.TriggerOverEvent();

                Debug.Log($"{GetType().Name} Enter");
            }

            public void Update()
            {
                HandleRestartkeyUpdate();
            }

            public void Exit()
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        /// <summary>
        /// �˳���Ϸ
        /// </summary>
        public class ExitLevelState : IState
        {
            private StateMachine stateMachine;
            public ExitLevelState(StateMachine sm)
            {
                stateMachine = sm;
            }

            public void Enter()
            {
                GameEvents.TriggerExitLevelEvent();
                Debug.Log($"{GetType().Name} Enter");
            }

            public void Update()
            {

            }

            public void Exit()
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        #endregion

        #region UnityLifeCycle
        private StateMachine _stateMachine;

        private void Awake()
        {
            instance = this;

            Screen.SetResolution(Screen.width, Screen.height, true, 240);
            Application.targetFrameRate = int.MaxValue;

            if (_stateMachine == null) _stateMachine = new StateMachine();
        }
        private void Start()
        {
            _stateMachine.ChangeState(new EnterLevelState(_stateMachine));
        }
        private void OnEnable()
        {
            GameEvents.OnGameReady += CreatePlayerRemainParent;
            GameEvents.OnGameReady += CreateCollectableRemainParent;

            StairEvents.OnEndLaunch += ChangeToReadyState;
            StairEvents.OnStartDisable += ChangeToOnStairState;

            ReadyInterface.OnPauseAlterRestart += ChangeToOnStairState;

            RespawnEvents.OnCallRespawn += ChangeToRespawningState;
            RespawnEvents.OnRespawning += DestroyAllRemains;
            RespawnEvents.OnEndRespawn += ChangeToReadyState;

            PlayerEvents.OnPlayerHit += GameOver;
            PlayerEvents.OnPlayerDrowned += GameOver;
            PlayerEvents.OnPlayerFall += GameOver;

            PyramidTrigger.OnEnterPyramidTrigger += GameOver;
        }
        private void OnDestroy()
        {
            _stateMachine = null;
            instance = null;

            GameEvents.OnGameReady += CreatePlayerRemainParent;
            GameEvents.OnGameReady += CreateCollectableRemainParent;

            StairEvents.OnEndLaunch -= ChangeToReadyState;
            StairEvents.OnStartDisable -= ChangeToOnStairState;

            ReadyInterface.OnPauseAlterRestart -= ChangeToOnStairState;

            RespawnEvents.OnCallRespawn -= ChangeToRespawningState;
            RespawnEvents.OnRespawning -= DestroyAllRemains;
            RespawnEvents.OnEndRespawn -= ChangeToReadyState;

            PlayerEvents.OnPlayerHit -= GameOver;
            PlayerEvents.OnPlayerDrowned -= GameOver;
            PlayerEvents.OnPlayerFall -= GameOver;

            PyramidTrigger.OnEnterPyramidTrigger -= GameOver;
        }

        void Update() => _stateMachine.Update();
#endregion
    }
}
