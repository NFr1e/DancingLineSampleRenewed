using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DancingLineFanmade.Gameplay
{
    public class HintLineDisplayer : MonoBehaviour
    {
        public Transform StartPoint;
        public MeshRenderer Renderer;
        private bool
            _displayable = true;

        private void OnEnable()
        {
            RespawnEvents.OnRespawning += OnRespawn;
        }
        private void OnDestroy()
        {
            RespawnEvents.OnRespawning -= OnRespawn;
        }
        private void FixedUpdate()
        {
            HandleDisplay();
        }
        private void HandleDisplay()
        {
            _displayable =
                Vector3.Distance(Player.instance.transform.position, StartPoint.transform.position) <= 20;
            if (GameController.curGameState != GameState.Over)
                Renderer.enabled = _displayable;
        }
        private void OnRespawn()
        {
            _displayable = true;
            Renderer.enabled = true;
        }
    }
}
