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

    public ExponentialFog(float d,Color c)
    {
        fogDensity = d;
        fogColor = c;
    }
}
[Serializable]
public class ExponentialSquaredFog
{
    public float fogDensity = 0.01f;
    public Color fogColor = Color.white;

    public ExponentialSquaredFog(float d, Color c)
    {
        fogDensity = d;
        fogColor = c;
    }
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
        private Tween
            t_linear_start,
            t_linear_end,
            t_linear_color,

            t_exponential_density,
            t_exponential_color,

            t_exponentialSquared_density,
            t_exponentialSquared_color;


        public void 
            SetFogLinear
            (
                LinearFog args,
                float duration = 0,
                Ease ease = Ease.InOutSine
            )
        {
            KillTweens();

            RenderSettings.fogMode = FogMode.Linear;
            _fogType = FogType.Linear;

            t_linear_start = DOTween
                .To(() => RenderSettings.fogStartDistance, x => RenderSettings.fogStartDistance = x, args.fogStart, duration)
                .SetEase(ease);
            t_linear_end = DOTween
                .To(() => RenderSettings.fogEndDistance, x => RenderSettings.fogEndDistance = x, args.fogEnd, duration)
                .SetEase(ease);
            t_linear_color = DOTween
                .To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, args.fogColor, duration)
                .SetEase(ease);
        }
        public void
            SetFogExponential
            (
                ExponentialFog args,
                float duration = 0,
                Ease ease = Ease.InOutSine
            )
        {
            KillTweens();

            RenderSettings.fogMode = FogMode.Exponential;
            _fogType = FogType.Exponential;

            t_exponential_density = DOTween
                .To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, args.fogDensity, duration)
                .SetEase(ease);
            t_exponential_color = DOTween
                .To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, args.fogColor, duration)
                .SetEase(ease);
        }
        public void
            SetFogExponentialSquared
            (
                ExponentialSquaredFog args,
                float duration = 0,
                Ease ease = Ease.InOutSine
            )
        {
            KillTweens();

            RenderSettings.fogMode = FogMode.ExponentialSquared;
            _fogType = FogType.ExponentialSquared;

            t_exponentialSquared_density = DOTween
                .To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, args.fogDensity, duration)
                .SetEase(ease);
            t_exponentialSquared_color = DOTween
                .To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, args.fogColor, duration)
                .SetEase(ease);
        }

        /// <summary>
        /// Kill所有雾气动画Tween
        /// </summary>
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

        public static FogManager instance;

        private FogType _fogType;
        private LinearFog _linearFog;
        private ExponentialFog _expFog;
        private ExponentialSquaredFog _expSquaredFog;

        void OnEnable()
        {
            instance = this;

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
            _linearFog = new(RenderSettings.fogStartDistance, RenderSettings.fogEndDistance, RenderSettings.fogColor );
            _expFog = new(RenderSettings.fogDensity, RenderSettings.fogColor);
            _expSquaredFog = new(RenderSettings.fogDensity, RenderSettings.fogColor);
        }
        public void ResetArgs()
        {
            KillTweens();

            switch(_fogType)
            {
                case FogType.Linear:
                    SetFogLinear(_linearFog);
                    break;
                case FogType.Exponential:
                    SetFogExponential(_expFog);
                    break;
                case FogType.ExponentialSquared:
                    SetFogExponentialSquared(_expSquaredFog);
                    break;
            }

            Debug.Log($"{GetType().Name} Reset");
        }
        #endregion
    }
}
