using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DancingLineFanmade.Gameplay;

[Serializable]
public class LinearFog
{
    public float
        fogStart = 25,
        fogEnd = 100;
    public Color fogColor = Color.white;

    public LinearFog(float s,float e,Color c) 
    {
        fogStart = s;
        fogEnd = e;
        fogColor = c;
    }
}
[Serializable]
public class ExponentialFog
{
    public float fogDensity = 0.01f;
    public Color fogColor = Color.white;
}
[Serializable]
public class ExponentialSquaredFog
{
    public float fogDensity = 0.01f;
    public Color fogColor = Color.white;
}
public enum FogType
{
    Linear,
    Exponential,
    ExponentialSquared
}
namespace DancingLineFanmade.Level
{
    public class FogManager : MonoBehaviour , IResettable
    {
        /*
        public LinearFog linearFog;
        public ExponentialFog exponentialFog;
        public ExponentialSquaredFog exponentialSquaredFog;*/

        private Tween
            t_linear_start,
            t_linear_end,
            t_linear_color,

            t_exponential_density,
            t_exponential_color,

            t_exponentialSquared_density,
            t_exponentialSquared_color;


        private void 
            SetFogLinear
            (
                float start,
                float end,
                Color colour,
                float duration = 1,
                Ease ease = Ease.InOutSine
            )
        {
            RenderSettings.fogMode = FogMode.Linear;

            t_linear_start = DOTween
                .To(() => RenderSettings.fogStartDistance, x => RenderSettings.fogStartDistance = x, start, duration)
                .SetEase(ease);
            t_linear_end = DOTween
                .To(() => RenderSettings.fogEndDistance, x => RenderSettings.fogEndDistance = x, end, duration)
                .SetEase(ease);
            t_linear_color = DOTween
                .To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, colour, duration)
                .SetEase(ease);
        }
        private void
            SetFogExponential
            (
                float density,
                Color colour,
                float duration = 1,
                Ease ease = Ease.InOutSine
            )
        {
            RenderSettings.fogMode = FogMode.Exponential;
            
            t_exponential_density = DOTween
                .To(() => RenderSettings.fogDensity, x => RenderSettings.fogStartDistance = x, density, duration)
                .SetEase(ease);
            t_exponential_color = DOTween
                .To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, colour, duration)
                .SetEase(ease);
        }
        private void
            SetFogExponentialSquared
            (
                float density,
                Color colour,
                float duration = 1,
                Ease ease = Ease.InOutSine
            )
        {
            RenderSettings.fogMode = FogMode.ExponentialSquared;

            t_exponentialSquared_density = DOTween
                .To(() => RenderSettings.fogDensity, x => RenderSettings.fogStartDistance = x, density, duration)
                .SetEase(ease);
            t_exponentialSquared_color = DOTween
                .To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, colour, duration)
                .SetEase(ease);
        }

        private void KillTweens()
        {
            t_linear_start.Kill();
            t_linear_end.Kill();
            t_linear_color.Kill();

            t_exponential_density.Kill();
            t_exponential_color.Kill();

            t_exponentialSquared_density.Kill();
            t_exponentialSquared_color.Kill();
        }

        private FogType _fogType;
        private LinearFog _linearFog;
        private ExponentialFog _expFog;
        private ExponentialSquaredFog _expSquaredFog;

        void OnEnable()
        {
            RespawnAttributes.OnRecording += NoteArgs;

            RegisterResettable();
        }
        void OnDisable()
        {
            RespawnAttributes.OnRecording -= NoteArgs;

            UnregisterResettable();
        }

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
            _linearFog = new LinearFog(RenderSettings.fogStartDistance, RenderSettings.fogEndDistance, RenderSettings.fogColor);
        }
        public void ResetArgs()
        {
            KillTweens();

            Debug.Log($"{GetType().Name} Reset");
        }
        #endregion
    }
}
