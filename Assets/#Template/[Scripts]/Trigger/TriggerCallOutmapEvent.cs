using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Triggers {
    public class TriggerCallOutmapEvent : MonoBehaviour
    {
        public static event System.Action OnEnterOutmapTrigger;
        private void TriggerOutmapEvent() => OnEnterOutmapTrigger?.Invoke();
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerOutmapEvent();
            }
        }
    }
}
