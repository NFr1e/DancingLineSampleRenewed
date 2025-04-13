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
        private Vector3 _gravity;//记录的gravity值

        #region Reset
        /// <summary>
        /// 一般在OnEnable中调用
        /// </summary>
        private void RegisterResettable() => ResettableManager.Register(this);
        /// <summary>
        /// 一般在OnDisable中调用
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