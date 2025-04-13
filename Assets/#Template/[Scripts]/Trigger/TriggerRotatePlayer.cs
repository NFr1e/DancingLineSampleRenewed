using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Gameplay;

public class TriggerRotatePlayer : MonoBehaviour
{
    public Player targetPlayer;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            targetPlayer.RotatePlayer();
        }
    }
}
