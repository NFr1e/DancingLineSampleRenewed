using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Collectable
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour,ICollectable
    {
        public MeshRenderer Crown;
        public MeshRenderer Icon;
        public BoxCollider CheckpointTrigger;
        public GameObject CollectParticle;

        private Collider _collider;
        private MeshRenderer _renderer;
        private Tween iconFadeTween;
        private GameObject particle;
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _renderer = Crown;
        }
        private void OnEnable()
        {
            GameEvents.OnRespawnDone += ResetObject;

            Icon.material.DOFade(0, 0f);
        }
        private void OnDisable()
        {
            GameEvents.OnRespawnDone -= ResetObject;
        }
        private void Update()
        {
            Crown.transform.Rotate(0, 45 * Time.deltaTime, 0);
        }
        public void Collect()
        {
            _collider.enabled = false;
            _renderer.enabled = false;

            ParticleRun();
        }
        public void ParticleRun()
        {
            if (!Crown || !Icon) return;

            particle = Instantiate(CollectParticle, transform.position, transform.rotation, GameController.CollectableRemainParent);

            Vector3 icon, crown;
            float offestHeight;
            icon = Icon.transform.position;
            crown = Crown.transform.position;
            Vector3 topvec, offestvec, vec;

            offestHeight = Vector3.Distance(icon, crown) / 1.5f;
            offestvec = new Vector3(crown.x + icon.x, crown.y + icon.y, crown.z + icon.z);//Ð´´íÁË offest => offset
            vec = new Vector3(offestvec.x / 2, offestvec.y / 2, offestvec.z / 2);
            topvec = new Vector3(vec.x, vec.y + offestHeight, vec.z);

            iconFadeTween?.Kill();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(particle.transform.DOMove(topvec, 0.5f).SetEase(Ease.Linear));
            sequence.Append(particle.transform.DOMove(icon, 0.5f).SetEase(Ease.Linear));
            sequence.AppendCallback(() =>
            {
                iconFadeTween = Icon.material.DOFade(1f, 1f);
                Destroy(particle, 1f);
            });

        }
        private void ResetObject()
        {
            _collider.enabled = false;
            _renderer.enabled = false;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView == null) return;
            Camera sceneCamera = sceneView.camera;
            if (sceneCamera == null) return;
            float distance = Vector3.Distance(transform.position, sceneCamera.transform.position);
            if (distance > 40) return;

            if (!CheckpointTrigger) return;

            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(
                CheckpointTrigger.transform.position,
                CheckpointTrigger.transform.rotation,
                Vector3.one
            );
            Gizmos.DrawWireCube(Vector3.zero, CheckpointTrigger.transform.localScale);
        }
#endif
    }
}
