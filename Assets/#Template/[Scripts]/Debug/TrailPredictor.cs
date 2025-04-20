//Taken from MaxIceFlameTemplate
using Sirenix.OdinInspector;
using UnityEngine;

namespace DancingLineFanmade.Debugging
{
    public enum TrailType
    {
        Fall,
        Jump
    }

    [RequireComponent(typeof(LineRenderer)), DisallowMultipleComponent]
    public class TrailPredictor : MonoBehaviour
    {
        [SerializeField, EnumToggleButtons] private TrailType type = TrailType.Fall;
        [SerializeField, MinValue(0)] private int playerSpeed = 12;

        [SerializeField, MinValue(0f), ShowIf("@type == TrailType.Jump")]
        private float jumpPower = 500f;

        [SerializeField, MinValue(0)] private int pointCount = 50;
        private const float speedYConst = 79f;
        private const float speedXConst = 1.95f;
        private float speedX;
        private float speedY;
        private float finalSpeed;
        private float angle;
        private float x;
        private float y;

        private void OnValidate()
        {
#if UNITY_EDITOR
            speedX = playerSpeed / speedXConst;
            speedY = type is TrailType.Jump ? jumpPower * 53 / speedYConst : 0f;
            var lineRenderer = GetComponent<LineRenderer>();

            x = 0;
            y = 0;
            angle = Mathf.Atan(speedY / speedX);
            finalSpeed = new Vector2(speedX, speedY).magnitude;

            var points = new Vector3[pointCount];
            for (var i = 0; i < points.Length; i++)
            {
                points[i] = new Vector3(0, y, x);
                x += 1;
                y = x * Mathf.Tan(angle) - Physics.gravity.magnitude * x * x /
                    (2 * (finalSpeed * Mathf.Cos(angle)) * (finalSpeed * Mathf.Cos(angle)));
            }

            lineRenderer.positionCount = pointCount;
            lineRenderer.SetPositions(points);
#endif
        }
    }
}