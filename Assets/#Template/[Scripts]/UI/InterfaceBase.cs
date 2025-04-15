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
        /// 加载该界面时调用
        /// </summary>
        protected virtual void EnterInterface() 
        {
            Init();
            //GameEvents.OnGameOver += ExitInterface; 这行写在这是干啥的?不知道。。。先不删，哈哈
            AnimateEnter();
        }
        /// <summary>
        /// 在此界面滞留时调用
        /// </summary>
        protected virtual void UpdateInterface() { }
        /// <summary>
        /// 退出该界面时调用
        /// </summary>
        public virtual void ExitInterface() 
        {
            //GameEvents.OnGameOver -= ExitInterface; 同上，，不知道啥时候脑子抽了加的
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
            if (!GetComponent<CanvasGroup>()) return;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            if (ExitTrigger == null || this is ReadyInterface) return;
            ExitTrigger.onClick.AddListener(ExitInterface);
        }
        protected virtual void AnimateEnter()
        {
            _canvasGroup.DOFade(1, 0.8f).SetUpdate(true);
        }
        protected void AnimateExit()
        {
            if (!GetComponent<CanvasGroup>()) return;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0, 0.4f).SetUpdate(true).OnComplete(() => { Destroy(this.gameObject); });
        }
    }
}
