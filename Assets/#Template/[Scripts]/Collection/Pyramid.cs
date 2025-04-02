using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DancingLineFanmade.Triggers;

namespace DancingLineFanmade.Collectable
{
    public class Pyramid : MonoBehaviour
    {
        public UnityEventTrigger Opener;

        public Transform Left, 
                         Right;

        public float Width = 2f, OpenDuration = 1f;

        private void Start()
        {
            Opener?.OnEnter.AddListener
                (
                    () => OpenPyramid()
                );
        }
        private void OpenPyramid()
        {
            if (Left) Left.transform.DOLocalMoveX(Width, OpenDuration);
            if (Right) Right.transform.DOLocalMoveX(-Width, OpenDuration);
        }
    }
}
