using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Triggers
{
    public class TeleportEvents
    {
        public static event System.Action
            OnTriggerCameraTeleport;
        public static event System.Action<Vector3>
            OnPlayerTeleport;
        public static void TriggerCameraTeleport() => OnTriggerCameraTeleport?.Invoke();
        public static void TriggerPlayerTeleport(Vector3 pos) => OnPlayerTeleport?.Invoke(pos);
    }

    [RequireComponent(typeof(Collider))]
    public class TriggerTeleport : MonoBehaviour
    {
        public Player Player;
        public Transform TargetTransform;
        public bool 
            EditRotation = false,
            TeleportCamera = true;

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;

            if (TargetTransform.GetComponent<MeshRenderer>())
                TargetTransform.GetComponent<MeshRenderer>().enabled = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (TeleportCamera)
                TeleportEvents.TriggerCameraTeleport();
            Player.transform.position = TargetTransform.position;
            if(EditRotation) Player.transform.rotation = TargetTransform.rotation;
            TeleportEvents.TriggerPlayerTeleport(TargetTransform.position);
        }
    }
}
