using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using DancingLineFanmade.Gameplay;

namespace DancingLineFanmade.Collectable
{
    public class CheckpointEvents
    {
        public static event System.Action OnCheckpointCollected;
        public static void TriggerCollectCheckpoint() => OnCheckpointCollected?.Invoke();
    }
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour,ICollectable
    {
        public MeshRenderer Crown;
        public MeshRenderer Icon;
        public GameObject CollectParticle;

        private Collider _collider;
        private MeshRenderer _renderer;
        private Tween iconFadeTween;
        private GameObject particle;
        private bool
            _animatedCollect = false,
            _animatedFade = false;
        [HideInInspector]
        public bool 
            _consumed = false;
        
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _renderer = Crown;
        }
        private void OnEnable()
        {
            PlayerEvents.OnPlayerStart += AnimateFade;
            Icon.material.DOFade(0, 0f);
        }
        private void OnDisable()
        {
            PlayerEvents.OnPlayerStart -= AnimateFade;
        }
        private void Update()
        {
            Crown.transform.Rotate(0, 45 * Time.deltaTime, 0);
        }
        public void Collect()
        {
            _collider.enabled = false;
            _renderer.enabled = false;

            AnimateCollect();
            
            CheckpointEvents.TriggerCollectCheckpoint();

            RespawnEvents.UpdateCheckpoint(this);
        }
        /// <summary>
        /// 执行收集到检查点的动画
        /// </summary>
        public void AnimateCollect()
        {
            if (!Crown || !Icon) return;

            particle = Instantiate(CollectParticle, transform.position, transform.rotation, GameController.CollectableRemainParent);

            Vector3 icon, crown;
            float offestHeight;
            icon = Icon.transform.position;
            crown = Crown.transform.position;
            Vector3 topvec, offestvec, vec;

            offestHeight = Vector3.Distance(icon, crown) / 1.5f;
            offestvec = new Vector3(crown.x + icon.x, crown.y + icon.y, crown.z + icon.z);//写错了 offest => offset
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
                _animatedCollect = true;
            });
        }
        /// <summary>
        /// 执行检查点被消耗的动画
        /// </summary>
        private void AnimateFade()
        {
            if (RespawnEvents.currentCheckpoint != this) return;
            if (!_consumed) return;
            if (!_animatedCollect) return;
            if (_animatedFade) return;
            if (!Crown || !Icon) return;

            if (particle) Destroy(particle);

            particle = Instantiate(CollectParticle, Icon.transform.position, transform.rotation, GameController.CollectableRemainParent);
            particle.transform.DOLocalMoveY(10, 2f).SetEase(Ease.OutQuad);

            iconFadeTween?.Kill();
            iconFadeTween = Icon.material.DOFade(0, 1);

            _animatedFade = true;
        }
    }
}
