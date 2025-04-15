using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingLineFanmade.Triggers
{
    [RequireComponent(typeof(Collider))]
    public class TriggerJump : MonoBehaviour
    {
        public Rigidbody targetRigid;
        public float Power = 10;
        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                targetRigid.AddForce(Vector3.up * Power, ForceMode.Impulse);
        }
    }
}
