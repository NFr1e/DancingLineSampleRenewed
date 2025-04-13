using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DancingLineFanmade.UI;

namespace DancingLineFanmade.Gameplay
{
    public static class StairEvents
    {
        public static event System.Action
            OnStartLaunch,
            OnEndLaunch,
            OnStartDisable,
            OnEndDisable;

        public static void TriggerStartLaunchEvent() => OnStartLaunch?.Invoke();
        public static void TriggerEndLaunchEvent() => OnEndLaunch?.Invoke();
        public static void TriggerStartDisableEvent() => OnStartDisable?.Invoke();
        public static void TriggerEndDisableEvent() => OnEndDisable?.Invoke();
    }
    public enum StairState
    {
        Launched,
        Disabled,
        Uncontrallable
    }
    public class ReadyStairManager : MonoBehaviour
    {
        public class LaunchingState : IState //正在下降
        {
            private StateMachine stateMachine;
            public LaunchingState(StateMachine sm)
            {
                stateMachine = sm;
            }
            public void Enter() 
            {
                StairEvents.TriggerStartLaunchEvent();
                curStairState = StairState.Uncontrallable;
                Debug.Log($"{GetType().Name} Enter");
            }
            public void Update() { }
            public void Exit() 
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        public class LaunchedState : IState //下降完成
        {
            private StateMachine stateMachine;
            public LaunchedState(StateMachine sm)
            {
                stateMachine = sm;
            }
            public void Enter()
            {
                StairEvents.TriggerEndLaunchEvent();
                curStairState = StairState.Launched;
                Debug.Log($"{GetType().Name} Enter");
            }
            public void Update() 
            {
                if(Input.GetKeyDown(KeyCode.Escape) && !started)
                {
                    stateMachine.ChangeState(new DisablingState(stateMachine));
                }
            }
            public void Exit()
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        public class DisablingState : IState //正在上升
        {
            private StateMachine stateMachine;
            public DisablingState(StateMachine sm)
            {
                stateMachine = sm;
            }
            public void Enter() 
            {
                StairEvents.TriggerStartDisableEvent();
                curStairState = StairState.Uncontrallable;
                Debug.Log($"{GetType().Name} Enter");
            }
            public void Update() { }
            public void Exit() 
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }
        public class DisabledState : IState //上升完成
        {
            private StateMachine stateMachine;
            public DisabledState(StateMachine sm)
            {
                stateMachine = sm;
            }
            public void Enter()
            {
                StairEvents.TriggerEndDisableEvent();
                curStairState = StairState.Disabled;
                Debug.Log($"{GetType().Name} Enter");
            }
            public void Update() 
            {
                if (GameController.Clicked && !GameController.PointerOnUI && !started)
                {
                    stateMachine.ChangeState(new LaunchingState(stateMachine));
                }
            }
            public void Exit()
            {
                Debug.Log($"{GetType().Name} Exit");
            }
        }

        private StateMachine _stateMachine;
        private Sequence sequence;
        private Tween playbackTween;
        
        private float originValue;

        public static bool started = false;
        public static StairState curStairState;
        public static ReadyStairManager instance;

        public float Height = 2;
        public Vector3 startRotation = new Vector3(0,90,0);
        private Vector3 _disabledRotation => new Vector3(startRotation.x, startRotation.y+90, startRotation.z);
        public Transform Stair,PlayerTrans,BaseTransform;
        public GameObject[] LaunchEffect;

        private void Awake()
        {
            started = false;

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }
        private void OnDisable()
        {
            ReadyInterface.OnReadyToOnstair -= ChangeToDisablingState;

            GameEvents.OnStartPlay -= ChangeToDisabledState;
            GameEvents.OnStartPlay -= ChangeToStarted;
            GameEvents.OnEnterLevel -= ChangeToUnstarted;

            StairEvents.OnStartLaunch -= AnimateLaunching;
            StairEvents.OnStartDisable -= AnimateDisabling;
        }
        void OnEnable()
        {
            ReadyInterface.OnReadyToOnstair += ChangeToDisablingState;

            GameEvents.OnStartPlay += ChangeToDisabledState;
            GameEvents.OnStartPlay += ChangeToStarted;
            GameEvents.OnEnterLevel += ChangeToUnstarted;

            StairEvents.OnStartLaunch += AnimateLaunching;
            StairEvents.OnStartDisable += AnimateDisabling;

            InitReferance();
            InitStateMachine();
        }

        void Update() => _stateMachine.Update();

        private void ChangeToStarted() => started = true;
        private void ChangeToUnstarted() => started = false;
        private void ChangeToDisablingState() => _stateMachine.ChangeState(new DisablingState(_stateMachine)); 
        private void ChangeToDisabledState() => _stateMachine.ChangeState(new DisabledState(_stateMachine)); 
        private void InitReferance()
        {
            originValue = Stair.position.y;

            PlayerTrans ??= Player.instance.transform;
            BaseTransform ??= GameController.instance.transform;

            PlayerTrans.eulerAngles = _disabledRotation;
        }

        private void InitStateMachine()
        {
            _stateMachine = new StateMachine();
            _stateMachine.ChangeState(new DisabledState(_stateMachine));
        }

        private void AnimateLaunching()
        {
            foreach(GameObject a in LaunchEffect)
            {
                Destroy(Instantiate(a, PlayerTrans.position, PlayerTrans.rotation, GameController.PlayerRemainParent), 2f);
            }
            PlayerTrans.SetParent(Stair);

            PlayerTrans.DORotate(startRotation, 0.8f);

            sequence = DOTween.Sequence();
            sequence.Append(Stair.DOMoveY(-Height / 5, 0.4f).SetEase(Ease.InSine));
            sequence.Append(Stair.DOMoveY(-Height, 0.5f).SetEase(Ease.OutSine));
            sequence.AppendCallback(() =>
            {
                PlayerTrans.SetParent(BaseTransform);
                sequence?.Kill(false);
                _stateMachine.ChangeState(new LaunchedState(_stateMachine));
            });
        }
        private void AnimateDisabling()
        {
            if (!started)
            {
                PlayerTrans.SetParent(Stair);

                PlayerTrans.DORotate(_disabledRotation, 0.8f);

                playbackTween = Stair.DOMoveY(originValue, 0.8f).SetEase(Ease.OutSine);
                playbackTween.OnComplete(() =>
                {
                    PlayerTrans.SetParent(BaseTransform);
                    _stateMachine.ChangeState(new DisabledState(_stateMachine));
                });
            }
        }
    }
}
