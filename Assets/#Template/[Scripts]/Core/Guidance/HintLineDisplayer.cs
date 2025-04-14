using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DancingLineFanmade.Gameplay
{
    public class HintLineDisplayer : MonoBehaviour , IResettable
    {
        public Transform StartPoint;
        public MeshRenderer Renderer;
        private bool
            _displayable = true;

        private void OnEnable()
        {
            RegisterResettable();
        }
        private void OnDestroy()
        {
            UnregisterResettable();
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
        /// <summary>
        /// һ����OnEnable�е���
        /// </summary>
        private void RegisterResettable() => ResettableManager.Register(this);
        /// <summary>
        /// һ����OnDisable�е���
        /// </summary>
        private void UnregisterResettable() => ResettableManager.Unregister(this);
        public void NoteArgs()
        {

        }
        public void ResetArgs()
        {
            _displayable = true;
            Renderer.enabled = true;
        }
    }
}
