using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Level
{
    [DisallowMultipleComponent]
    public class BackgroundManager : MonoBehaviour,IResettable
    {
        public Camera CurrentCamera;

        private Tween _bgColorTransition;
        public static BackgroundManager instance;

        public void AlterClearFlags(CameraClearFlags flag)
        {
            CurrentCamera.clearFlags = flag;
        }
        public void AlterBackgroundSkybox(Material skybox)
        {
            CurrentCamera.clearFlags = CameraClearFlags.Skybox;

            RenderSettings.skybox = skybox;
        }
        public void AlterBackgroundColor(Color bgcolor,float duration,Ease ease)
        {

            CurrentCamera.clearFlags = CameraClearFlags.SolidColor;

            if (_bgColorTransition != null)
            {
                _bgColorTransition.Kill();
                _bgColorTransition = null;
            }

            _bgColorTransition = DOTween
                .To(() =>
                CurrentCamera.backgroundColor, x => CurrentCamera.backgroundColor = x
                , bgcolor, duration)
                .SetEase(ease);
        }
        void Start()
        {
            instance = this;
        }
        public void OnEnable()
        {
            RegisterResettable();
            RespawnAttributes.OnRecording += NoteArgs;
        }
        public void OnDisable()
        {
            UnregisterResettable();
            RespawnAttributes.OnRecording -= NoteArgs;
        }

        #region Reset

        private CameraClearFlags _clearFlags;
        private Color _bgColor;
        private Material _skybox;

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
            _clearFlags = CurrentCamera.clearFlags;
            _bgColor = CurrentCamera.backgroundColor;
            _skybox = RenderSettings.skybox;
        }
        public void ResetArgs()
        {
            _bgColorTransition.Kill();

            CurrentCamera.clearFlags = _clearFlags;
            CurrentCamera.backgroundColor = _bgColor;
            RenderSettings.skybox = _skybox;

            Debug.Log($"{name} Reset");
        }
        #endregion
    }
}
