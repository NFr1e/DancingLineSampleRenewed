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
            GameEvents.OnRespawnDone += ResetObject;
        }
        private void OnDisable()
        {
            GameEvents.OnRespawnDone -= ResetObject;
        }
        private void Update()
        {
            transform.Rotate(0, 45*Time.deltaTime, 0);
        }

        public void NoteVar()
        { }
        public void ResetVar()
        {
            _collider.enabled = false;
            _renderer.enabled = false;
        }
        public void Collect()
        {
            _collider.enabled = false;
            _renderer.enabled = false;

            LevelProgressManager.instance.RecordCollectable(this);

            foreach(GameObject a in CollectParticle)
            {
                Destroy(Instantiate(a,transform.position,transform.rotation,GameController.CollectableRemainParent), 15f);
            }
        }
        private void ResetObject()
        {
            _collider.enabled = false;
            _renderer.enabled = false;
        }
    } 
}
