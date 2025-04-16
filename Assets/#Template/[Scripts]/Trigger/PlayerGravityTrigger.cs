using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Gameplay;
public class PlayerGravityTrigger : MonoBehaviour
{
    public Player _player;
    public bool UseGravity = true;
    public Vector3 newGravity = new Vector3(0, -15, 0);
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (!_player) return;

            _player.useGravity = UseGravity;
            _player.selfGravity = newGravity;
        }
    }
}
