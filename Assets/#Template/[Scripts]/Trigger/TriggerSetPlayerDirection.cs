using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Gameplay;
namespace DancingLineFanmade.Triggers {
    public class TriggerSetPlayerDirection : MonoBehaviour
    {
        public Player PlayerInstance;
        public Vector3 FirstDirection, SecondDirection;
        private void Awake()
        {
            if(PlayerInstance == null)
            {
                PlayerInstance = Player.instance;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            PlayerInstance.firstDirection = FirstDirection;
            PlayerInstance.secondDirection = SecondDirection;
        }
    } 
}
