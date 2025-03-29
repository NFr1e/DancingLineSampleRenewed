using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingLineFanmade.Collectable
{
    public static class CollectorEvents
    {
        public static event System.Action
            OnCollect,
            OnCollectDiamond,
            OnCollectCheckpoint;

        public static void TriggerCollect() => OnCollect?.Invoke();
        public static void TriggerCollectDiamond() => OnCollectDiamond?.Invoke();
        public static void TriggerCollectCheckpoint() => OnCollectCheckpoint?.Invoke();
    }
    [RequireComponent(typeof(Rigidbody))]
    public class CollectionCollector : MonoBehaviour
    {
        public bool IsPlayer = true;
        private void OnTriggerEnter(Collider other)
        {
            ICollectable collectable = other.GetComponent<ICollectable>();

            if (collectable != null)
            {
                collectable.Collect();
                Debug.Log($"{other.name} Collected");

                CollectorEvents.TriggerCollect();

                switch (other.GetType().Name)
                {
                    case "Diamond":
                        CollectorEvents.TriggerCollectDiamond();
                        break;
                    case "Checkpoint":
                        CollectorEvents.TriggerCollectCheckpoint();
                        break;
                }       
            }
        }
    }
}
