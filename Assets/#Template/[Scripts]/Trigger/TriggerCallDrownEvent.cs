using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingLineFanmade.Triggers
{
    public class TriggerCallDrownEvent : MonoBehaviour
    {
        public static event System.Action OnEnterDrownTrigger;
        private void TriggerDrownEvent() => OnEnterDrownTrigger?.Invoke();
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerDrownEvent();
            }
        }
    }
}
