using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InterfaceBase : MonoBehaviour
    {
        public Button ExitTrigger;

        protected CanvasGroup _canvasGroup;
        /// <summary>
        /// ���ظý���ʱ����
        /// </summary>
        protected virtual void EnterInterface() 
        {
            Init();
            GameEvents.OnGameOver += ExitInterface;
            AnimateEnter();
        }
        /// <summary>
        /// �ڴ˽�������ʱ����
        /// </summary>
        protected virtual void UpdateInterface() { }
        /// <summary>
        /// �˳��ý���ʱ����
        /// </summary>
        public virtual void ExitInterface() 
        {
            GameEvents.OnGameOver -= ExitInterface;
            AnimateExit();
        }

        protected virtual void OnEnable()
        {
            EnterInterface();
        }
        private void Update()
        {
            UpdateInterface();
        }
        protected virtual void Init()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            if (ExitTrigger == null || GetType().Name == "ReadyInterface") return;
            ExitTrigger.onClick.AddListener(() => ExitInterface());
        }
        private void AnimateEnter()
        {
            _canvasGroup.DOFade(1, 0.8f).SetUpdate(true);
        }
        private void AnimateExit()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0, 0.4f).SetUpdate(true).OnComplete(() => { Destroy(gameObject); });
        }
    }
}
