using DancingLineFanmade.Gameplay;
using UnityEngine;

namespace DancingLineFanmade.Triggers
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class GravityTrigger : MonoBehaviour
    {
        [SerializeField] private Vector3 TargetGravity;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) 
                Physics.gravity = TargetGravity;
        }
    }
}