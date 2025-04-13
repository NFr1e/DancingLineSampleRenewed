using DancingLineFanmade.Gameplay;
using UnityEngine;

namespace DancingLineFanmade.Triggers
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class GravityTrigger : MonoBehaviour , IResettable
    {
        [SerializeField] private Vector3 TargetGravity;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) 
                Physics.gravity = TargetGravity;
        }

        private void OnEnable()
        {
            RegisterResettable();

            RespawnAttributes.OnRecording += NoteArgs;
        }
        private void OnDisable()
        {
            UnregisterResettable();

            RespawnAttributes.OnRecording -= NoteArgs;
        }
        private Vector3 _gravity;//��¼��gravityֵ

        #region Reset
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
            _gravity = Physics.gravity;
        }
        public void ResetArgs()
        {
            Physics.gravity = _gravity;

            Debug.Log($"{name} Reset");
        }
        #endregion
    }
}