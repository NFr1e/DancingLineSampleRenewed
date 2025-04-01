using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingLineFanmade.Triggers
{
    public class PyramidTrigger : MonoBehaviour
    {
        public static event System.Action OnEnterPyramidTrigger;
        public static void TriggerEnterPyramidTrigger() => OnEnterPyramidTrigger?.Invoke();
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
                TriggerEnterPyramidTrigger();
        }
    }
}
