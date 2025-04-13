using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingLineFanmade.Triggers
{
    public class TriggerCallDrownEvent : MonoBehaviour
    {
        public static event System.Action OnEnterDrownTrigger;
        private void TriggerDrownEvent() => OnEnterDrownTrigger?.Invoke();

        private bool _triggerable = true;

        private void Start()
        {
            if (this.gameObject.layer == LayerMask.NameToLayer("Water"))
                _triggerable = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _triggerable)
            {
                TriggerDrownEvent();
            }
        }
    }
}
