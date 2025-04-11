using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using DancingLineFanmade.Gameplay;
using DancingLineFanmade.Level;

namespace DancingLineFanmade.Triggers {
    [RequireComponent(typeof(Collider))]
    public class FogTrigger : MonoBehaviour
    {
        public bool PlayerLimit = true;

        public FogType Type = FogType.Linear;

        [ShowIf("Type",FogType.Linear)]
        public LinearFog Linear = new(25, 100, Color.white);
        [ShowIf("Type", FogType.Exponential)]
        public ExponentialFog Exponential = new(0.01f, Color.white);
        [ShowIf("Type", FogType.ExponentialSquared)]
        public ExponentialSquaredFog ExponentialSquared = new(0.01f, Color.white);

        [BoxGroup("AnimateArgs")]
        public float Duration = 1;
        [BoxGroup("AnimateArgs")]
        public Ease ease = Ease.InOutSine;

        private FogManager _manager;
        private Collider _collider;

        private LinearFog _linear;
        private ExponentialFog _exp;
        private ExponentialSquaredFog _expSquared;
        private void Start()
        {
            _manager = FogManager.instance;

            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;

            _linear = new(Linear.fogStart, Linear.fogEnd, Linear.fogColor);
            _exp = new(Exponential.fogDensity, Exponential.fogColor);
            _expSquared = new(ExponentialSquared.fogDensity, ExponentialSquared.fogColor);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (PlayerLimit && !other.GetComponent<Player>())
                return;
            else SetFog();
        }
        private void SetFog()
        {
            switch(Type)
            {
                case FogType.Linear:
                    _manager.SetFogLinear(_linear, Duration, ease);
                    break;
                case FogType.Exponential:
                    _manager.SetFogExponential(_exp, Duration, ease);
                    break;
                case FogType.ExponentialSquared:
                    _manager.SetFogExponentialSquared(_expSquared, Duration, ease);
                    break;
            }
        }
        [Button("GetCurrentFog",buttonSize:30)]
        private void GetCurrentFog()
        {
            switch(RenderSettings.fogMode)
            {
                case FogMode.Linear:
                    Type = FogType.Linear;
                    Linear.fogStart = RenderSettings.fogStartDistance;
                    Linear.fogEnd = RenderSettings.fogEndDistance;
                    Linear.fogColor = RenderSettings.fogColor;
                    break;
                case FogMode.Exponential:
                    Type = FogType.Exponential;
                    Exponential.fogDensity = RenderSettings.fogDensity;
                    Exponential.fogColor = RenderSettings.fogColor;
                    break;
                case FogMode.ExponentialSquared:
                    Type = FogType.ExponentialSquared;
                    ExponentialSquared.fogDensity = RenderSettings.fogDensity;
                    ExponentialSquared.fogColor = RenderSettings.fogColor;
                    break;
            }
        }
    } }
