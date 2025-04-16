using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Triggers
{
    [RequireComponent(typeof(Collider))]
    public class TriggerJump : MonoBehaviour
    {
        public float Power = 10;
        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.SetVerticalVelocity(Power);
                    Debug.Log("PlayerJump");
                }
            }
        }
    }
}
