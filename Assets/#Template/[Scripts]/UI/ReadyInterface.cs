using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.UI
{
    public class ReadyInterface : InterfaceBase
    {
        public Button RestartButton;
        public Transform Hand;

        private Vector3 originVec;
        private Sequence handSequence;

        public static event System.Action OnReadyToOnstair,OnPauseAlterRestart;
        public static void TriggerReadyToOnStair() => OnReadyToOnstair?.Invoke();
        public static void TriggerPauseAlterRestart() => OnPauseAlterRestart?.Invoke();

        private void InitInterface()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            GameEvents.OnStartPlay += ExitInterface;

            InitializeHandAnimation();

            float shrink;
            shrink = UserInterfaceManager.CurrentResolution().x / 2560;
            if (shrink < 0.7f) shrink = 0.7f;

            if (Hand) Hand.localScale = new Vector3(shrink, shrink, 1);

            if(RestartButton)RestartButton.onClick.AddListener(() =>
            {
                TriggerPauseAlterRestart();
                GameController.ReloadScene();
                
                ExitInterface();
            });

            if (ExitTrigger == null) return;
            ExitTrigger.onClick.AddListener(() => HandleTriggerExit());
        }

        protected override void EnterInterface()
        {
            base.EnterInterface();
            InitInterface();
            AnimateHandEnter();

            UserInterfaceEvents.TriggerReadyEnterEvent();
        }

        public override void ExitInterface()
        {
            GameEvents.OnStartPlay -= ExitInterface;

            base.ExitInterface();
            AnimateHandExit();

            UserInterfaceEvents.TriggerReadyExitEvent();
        }
        private void HandleTriggerExit()
        {
            if (ReadyStairManager.curStairState == StairState.Disabled)
                ExitInterface();
            if (ReadyStairManager.curStairState == StairState.Launched)
                TriggerReadyToOnStair();
        }
        private void InitializeHandAnimation()
        {
            if (!Hand) return;

            float originx = Hand.position.x;
            float originz = Hand.position.z;
            originVec = new Vector3(originx,UserInterfaceManager.CurrentResolution().y * 2/7,originz);

            handSequence = DOTween.Sequence().SetUpdate(true)
                .Append(Hand.DOMoveY(originVec.y + 40f, 0.6f).SetEase(Ease.InOutSine))
                .Append(Hand.DOMoveY(originVec.y, 0.8f).SetEase(Ease.InOutSine))
                .SetLoops(-1)
                .Pause();
        }

        private void AnimateHandEnter()
        {
            if (!Hand) return;

            handSequence.Pause();
            Hand.position = new Vector3(
                originVec.x,
                originVec.y - UserInterfaceManager.CurrentResolution().y * 0.3f,
                originVec.z
            );

            Hand.DOMoveY(originVec.y, 0.6f).SetUpdate(true)
                .OnComplete(() => handSequence.Play());
        }

        private void AnimateHandExit()
        {
            if (!Hand) return;

            handSequence.Pause();

            Hand.DOMoveY(
                originVec.y - UserInterfaceManager.CurrentResolution().y * 0.3f,
                0.6f
            ).SetUpdate(true);
        }

        private void OnDestroy()
        {
            handSequence?.Kill();
        }
    }
}