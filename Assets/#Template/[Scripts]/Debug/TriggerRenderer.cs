//Thanks LAM for providing method.
//Taken from MaxIceFlameTemplate
using UnityEngine;

namespace DancingLineFanmade.Debugging
{
    [DisallowMultipleComponent, RequireComponent(typeof(BoxCollider))]
    public class TriggerRenderer : MonoBehaviour
    {
        [SerializeField] private bool renderBox;
        [SerializeField] private bool renderWire = true;
        [SerializeField] private Color boxColor = new(0f, 1f, 0f, 0.5f);
        [SerializeField] private Color wireColor = Color.green;

        private BoxCollider box;
        
#if UNITY_EDITOR
        private static Vector3 MultiplyVector3(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        private void OnDrawGizmos()
        {
            if (!box) 
                box = GetComponent<BoxCollider>();

            var trans = transform;
            var matrix = new Matrix4x4();
            matrix.SetTRS(trans.position + box.center, transform.rotation, MultiplyVector3(trans.lossyScale, box.size));
            Gizmos.matrix = matrix;

            if (renderBox)
            {
                Gizmos.color = boxColor;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }

            if (!renderWire) 
                return;
            Gizmos.color = wireColor;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
#endif
    }
}