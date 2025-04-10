using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DancingLineFanmade.UI;
using DancingLineFanmade.Collectable;

namespace DancingLineFanmade.Gameplay
{
    public class RespawnEvents
    {
        public static event System.Action
            OnCallRespawn,
            OnStartRespawn,
            OnRespawning,
            OnEndRespawn;
        public static event System.Action<Checkpoint> OnUpdateCheckpoint;

        public static Checkpoint currentCheckpoint = null;
        public static void UpdateCheckpoint(Checkpoint target) 
        {
            currentCheckpoint = target;
            
            OnUpdateCheckpoint?.Invoke(target); 
        }
        public static void CallResapwan() => OnCallRespawn?.Invoke();
        public static void TriggerStartResapwn() => OnStartRespawn?.Invoke();
        public static void TriggerRespawning() => OnRespawning?.Invoke();
        public static void TriggerEndRespawn() 
        { 
            OnEndRespawn?.Invoke();
            currentCheckpoint._consumed = true;
        }
    }
    /// <summary>
    /// ��ʼ����״̬(ִ��Ч��)
    /// </summary>
    public class StartRespawnState : IState
    {
        StateMachine stateMachine;
        public StartRespawnState (StateMachine sm)
        {
            stateMachine = sm;
        }
        public void Enter()
        {
            Debug.Log($"{GetType().Name} Enter");

            RespawnEvents.TriggerStartResapwn();
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
    /// ������״̬(�㲥��������¼�)
    /// </summary>
    public class RespawningState : IState
    {
        StateMachine stateMachine;
        public RespawningState(StateMachine sm)
        {
            stateMachine = sm;
        }
        public void Enter()
        {
            Debug.Log($"{GetType().Name} Enter");

            RespawnEvents.TriggerRespawning();

            stateMachine.ChangeState(new EndRespawnState(stateMachine));
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
    /// ��������״̬(��β)
    /// </summary>
    public class EndRespawnState : IState
    {
        StateMachine stateMachine;
        public EndRespawnState(StateMachine sm)
        {
            stateMachine = sm;
        }
        public void Enter()
        {
            Debug.Log($"{GetType().Name} Enter");

            RespawnEvents.TriggerEndRespawn();
        }
        public void Update()
        {

        }
        public void Exit()
        {
            Debug.Log($"{GetType().Name} Exit");
        }
    }
    public class RespawnManager : MonoBehaviour
    {
        private RespawnAttributes _curAttributes;

        private StateMachine _stateMachine;
        private void OnEnable()
        {
            _curAttributes = null;

            RespawnAttributes.OnSetAttribute += SetCurrentAttributes;

            RespawnEvents.OnCallRespawn += StartRespawn;
            RespawnEvents.OnStartRespawn += AnimateMask;
            RespawnEvents.OnRespawning += ResetAttributes;
        }
        private void OnDestroy()
        {
            _curAttributes = null;

            RespawnAttributes.OnSetAttribute -= SetCurrentAttributes;

            RespawnEvents.OnCallRespawn -= StartRespawn;
            RespawnEvents.OnStartRespawn -= AnimateMask;
            RespawnEvents.OnRespawning -= ResetAttributes;
        }
        private void Start()
        {
            if (_stateMachine == null) _stateMachine = new StateMachine();
        }
        /// <summary>
        /// �л�����״̬��ģʽΪStartRespawnState
        /// </summary>
        private void StartRespawn()
        {
            _stateMachine.ChangeState(new StartRespawnState(_stateMachine));
        }
        /// <summary>
        /// ִ�����ֶ������л�״̬��ģʽΪRespawningState
        /// </summary>
        private void AnimateMask()
        {
            float duration = 0.2f;

            Tweener fadeTweener;
            Image mask = UserInterfaceManager.InstantiateImage(new Color(RenderSettings.fogColor.r, RenderSettings.fogColor.g, RenderSettings.fogColor.b, 0));
            fadeTweener = mask.DOFade(1, duration);
            fadeTweener.OnComplete(() =>
            {
                mask.DOFade(0, 0.6f).SetUpdate(true).OnComplete(() => Destroy(mask.transform.parent.gameObject));

                _stateMachine.ChangeState(new RespawningState(_stateMachine));
            });
        }
        /// <summary>
        /// ��_curAttributes��ֵΪtarget
        /// </summary>
        /// <param name="target">�л�����Attribute</param>
        private void SetCurrentAttributes(RespawnAttributes target)
        {
            _curAttributes = target;
        }
        /// <summary>
        /// ʹ_curAttributesִ��ResetAttributes()
        /// </summary>
        private void ResetAttributes()
        {
            if (!_curAttributes) return;
            _curAttributes.ResetAttributes();
        }
    }
    public class ResettableManager
    {
        private static readonly List<IResettable> instances = new List<IResettable>();

        /// <summary>
        /// ע��IResettableʵ��
        /// </summary>
        /// <param name="resettable"></param>
        public static void Register(IResettable resettable)
        {
            if (!instances.Contains(resettable))
                instances.Add(resettable);
        }

        /// <summary>
        /// ע��IResettableʵ��
        /// </summary>
        /// <param name="resettable"></param>
        public static void Unregister(IResettable resettable)
        {
            instances.Remove(resettable);
        }
        /// <summary>
        /// ��OnRespawning���Ļָ�����
        /// </summary>
        static ResettableManager()
        {
            RespawnEvents.OnRespawning += OnRespawnTriggered;
        }

        private static void OnRespawnTriggered()
        {
            foreach (var instance in instances.ToArray())
            {
                if (instance is MonoBehaviour behaviour && behaviour != null)
                {
                    instance.ResetArgs();
                }
                else
                {
                    instances.Remove(instance);
                }
            }
        }
    }
}
