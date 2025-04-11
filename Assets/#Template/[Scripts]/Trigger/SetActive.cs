using DancingLineFanmade.Gameplay;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DancingLineFanmade.Triggers
{
    [Serializable]
    public class SingleActive
    {
        public GameObject target;
        public bool active;

        public SingleActive(GameObject target, bool active)
        {
            this.target = target;
            this.active = active;
        }

        public void SetActive()
        {
            target.SetActive(active);
        }
    }

    [DisallowMultipleComponent]
    public class SetActive : MonoBehaviour , IResettable
    {
        public bool activeOnAwake = false;
        [TableList] 
        public List<SingleActive> actives = new();

        private List<bool> _actives = new();

        private void Start()
        {
            if (!activeOnAwake) 
                return;
            foreach (var s in actives)
            {
                s.SetActive();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || activeOnAwake)
                return;
            
            foreach (var s in actives)
            {
                s.SetActive();
            }
        }

        void OnEnable() 
        {
            RespawnAttributes.OnRecording += NoteArgs;

            RegisterResettable();
        }
        void OnDisable()
        {
            RespawnAttributes.OnRecording -= NoteArgs;

            UnregisterResettable();
        }
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
            _actives.Clear();

            for(int a = 0; a < actives.Count; a++)
            {
                _actives.Add(actives[a].target.activeSelf);
            }
        }
        public void ResetArgs()
        {
            for(int r = 0; r < _actives.Count; r++)
            {
                actives[r].target.SetActive(_actives[r]);
            }
            Debug.Log($"{name} Reset");
        }
        #endregion
    }
}