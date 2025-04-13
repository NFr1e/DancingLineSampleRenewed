using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DancingLineFanmade.Gameplay;
using DancingLineFanmade.Triggers;

namespace DancingLineFanmade.Collectable
{
    public class Pyramid : MonoBehaviour,IResettable
    {
        public UnityEventTrigger Opener;

        public Transform 
            Left,              
            Right;
        
        public float 
            Width = 2f, 
            OpenDuration = 1f;

        private Vector3
            _originLeftLocalPos,
            _originRightLocalPos;
        private Tween
            _leftTween,
            _rightTween;

        private void Start()
        {
            Opener?.OnEnter.AddListener
                (
                    () => OpenPyramid()
                );
        }
        private void OpenPyramid()
        {
            if (Left)
                Left.transform.DOLocalMoveX(Width, OpenDuration);
         
            if (Right) 
                Right.transform.DOLocalMoveX(-Width, OpenDuration); 
        }

        void OnEnable()
        {
            NoteArgs();
            RegisterResettable();
        }
        void OnDisable() => UnregisterResettable();

        #region Reset
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
            _originLeftLocalPos = Left.localPosition;
            _originRightLocalPos = Right.localPosition;
        }
        public void ResetArgs()
        {
            _leftTween?.Kill();
            _rightTween?.Kill();
            Left.localPosition = _originLeftLocalPos;
            Right.localPosition = _originRightLocalPos;

            Debug.Log($"{name} Reset");
        }
        #endregion

    }
}
