using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Collectable 
{
    [RequireComponent(typeof(Collider),typeof(MeshRenderer))]
    public class Diamond : MonoBehaviour, ICollectable,IResettable
    {
        public GameObject[] CollectParticle;

        private Collider _collider;
        private MeshRenderer _renderer;
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _renderer = GetComponent<MeshRenderer>();
        }
        private void OnEnable()
        {
            RegisterResettable();
        }
        private void OnDisable()
        {
            UnregisterResettable();
        }
        private void Update()
        {
            transform.Rotate(0, 45*Time.deltaTime, 0);
        }
        public void Collect()
        {
            _collider.enabled = false;
            _renderer.enabled = false;

            LevelProgressManager.instance.RecordCollectable(this);

            foreach(GameObject a in CollectParticle)
            {
                Destroy(
                    Instantiate(
                    a
                    ,transform.position
                    ,transform.rotation
                    ,GameController.CollectableRemainParent)
                , 15f);
            }
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

        public void NoteArgs() { }
        public void ResetArgs()
        {
            _collider.enabled = true;
            _renderer.enabled = true;

            Debug.Log($"{name} Reset");
        }
        #endregion
    }
}
