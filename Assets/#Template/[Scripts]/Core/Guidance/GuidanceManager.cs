using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingLineFanmade.Gameplay
{
    public class GuidanceManager : MonoBehaviour
    {
        public GameObject GuidanceGroup;

        public static bool _isUsing = false;

        private void OnEnable()
        {
            GuidanceToggle.OnClickToggle += SwitchIsUsing;
            GuidanceToggle.OnClickToggle += HandleActive;
        }
        private void OnDisable()
        {
            GuidanceToggle.OnClickToggle -= SwitchIsUsing;
            GuidanceToggle.OnClickToggle -= HandleActive;
        }
        private void Start()
        {
            HandleActive();
        }
        private void HandleActive()
        {
            if (!GuidanceGroup) return;

            GuidanceGroup.SetActive(_isUsing);
        }
        private void SwitchIsUsing()
        {
            if (_isUsing)
                _isUsing = false;
            else _isUsing = true;
        }
    }
}
