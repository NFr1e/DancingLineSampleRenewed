using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DancingLineFanmade.Triggers
{
    public class UnityEventTrigger : MonoBehaviour
    {
        public UnityEvent
            OnEnter = new(),
            OnStay = new(),
            OnExit = new();
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                OnEnter?.Invoke();
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
                OnStay?.Invoke();
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                OnExit?.Invoke();
        }

    }
}
