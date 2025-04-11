using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using DancingLineFanmade.Level;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Triggers
{
    [RequireComponent(typeof(Collider))]
    public class BackgroundTrigger : MonoBehaviour
    {
        private Collider _collider;

        public BackgroundManager _manager;

        public CameraClearFlags clearFlags = CameraClearFlags.SolidColor;

        [ShowIf("clearFlags", CameraClearFlags.Color), ShowIf("clearFlags", CameraClearFlags.SolidColor)]
        public Color solidColor;
        [ShowIf("clearFlags", CameraClearFlags.Skybox)]
        public Material skybox;

        [ShowIf("clearFlags", CameraClearFlags.Color)]
        public float Duration = 1;
        [ShowIf("clearFlags", CameraClearFlags.Color)]
        public Ease ease = Ease.InOutSine;

        public bool PlayerLimit = true;

        private void Start()
        {
            if(!_manager)
                _manager = BackgroundManager.instance;

            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (PlayerLimit && !other.GetComponent<Player>()) return;

            switch(clearFlags)
            {
                case CameraClearFlags.Color:
                    _manager.AlterBackgroundColor(solidColor, Duration, ease);
                    break;
                case CameraClearFlags.Skybox:
                    _manager.AlterBackgroundSkybox(skybox);
                    break;
                case CameraClearFlags.Depth:
                    _manager.AlterClearFlags(clearFlags);
                    break;
                case CameraClearFlags.Nothing:
                    _manager.AlterClearFlags(clearFlags);
                    break;
            }
        }

        [Button(Name = "GetCurrentArgs", ButtonHeight = 30)]
        private void GetCurrentArgs()
        {
            if (!FindObjectOfType<BackgroundManager>()) return;

            Camera curCamera = FindObjectOfType<BackgroundManager>().CurrentCamera;
            clearFlags = curCamera.clearFlags;
            solidColor = curCamera.backgroundColor;
            skybox = RenderSettings.skybox;
        }
    }
}
