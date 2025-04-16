#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using DancingLineFanmade.Debugging;
using DancingLineFanmade.Triggers;


public class PathBuilder : OdinEditorWindow
{
    [MenuItem("EditUtils/PathBuilder")]
    private static void OpenWindow()
    {
        GetWindow<PathBuilder>().Show();
    }

    [BoxGroup("General")]
    public RotatePointsBuffer PointsBuffer;

    [BoxGroup("PathSettings")]
    public GameObject PathCubeInstance;
    [BoxGroup("PathSettings")]
    public float PathWidth;

    [BoxGroup("GuidanceSettings")]
    public GameObject HintBoxInstance;
    [BoxGroup("GuidanceSettings")]
    public GameObject HintLineInstance;
    [BoxGroup("GuidanceSettings")]
    public float HintLineShrink = 0;
    [BoxGroup("GuidanceSettings")]
    public float 
        longSegment = 2f, 
        shortSegment = 0.3f, 
        gap = 0.2f, 
        width = 0.15f;

    private Transform 
        _pathContainer, 
        _guidanceContainer;

    private GameObject curGroup;

    [Button("SpawnPath")]
    private void SpawnPath()
    {
        int _bufferLength = PointsBuffer.SavedPoints.Count;

        if (!_pathContainer) _pathContainer = new GameObject("Paths").transform;

        for (int i = 0; i < _bufferLength - 1; i++)
        {
            Vector3 start = PointsBuffer.SavedPoints[i].Position;
            Vector3 end = PointsBuffer.SavedPoints[i + 1].Position;

            Vector3 direction = end - start;
            float distance = direction.magnitude;

            if (distance <= Mathf.Epsilon) continue;

            Vector3 midpoint = (start + end) / 2;

            GameObject line = Instantiate(PathCubeInstance, _pathContainer);
            line.transform.position = midpoint;
            line.transform.rotation = Quaternion.LookRotation(direction);
            line.transform.localScale = new Vector3(PathWidth, PathWidth, distance + PathWidth);
        }
    }
    
    [Button("SpawnHintLine")]
    private void SpawnHintLine()
    {

        if (PointsBuffer.SavedPoints == null) return;

        int _bufferLength = PointsBuffer.SavedPoints.Count;

        if (!_guidanceContainer) _guidanceContainer = new GameObject("GuidanceGroup").transform;

        GameObject hintbox;
        HintBoxTrigger component;
        
        for (int c = 0; c < _bufferLength; c++)
        {
            hintbox = Instantiate(HintBoxInstance, PointsBuffer.SavedPoints[c].Position, Quaternion.Euler(Vector3.zero), _guidanceContainer);
            hintbox.name = $"HintBox {c}";

            component = hintbox.GetComponent<HintBoxTrigger>();
            component.TriggerTime = PointsBuffer.SavedPoints[c].Time;

            if (_bufferLength < 2) return;

            if (c < _bufferLength - 1)
            {
                curGroup = new GameObject($"HintLineGroup {c}");
                curGroup.transform.SetParent(hintbox.transform);

                component.childLine = curGroup;

                Vector3 start = PointsBuffer.SavedPoints[c].Position;
                Vector3 end = PointsBuffer.SavedPoints[c + 1].Position;
                Vector3 direction = end - start;

                float totalDistance = direction.magnitude - 2 - HintLineShrink;

                if (totalDistance < Mathf.Epsilon) continue;//¾àÀë¼«Ð¡

                Vector3 dirNormalized = direction.normalized;
                float remainingDistance = totalDistance;
                Vector3 currentPos = start + direction / direction.magnitude;

                bool isLongSegment = true;

                while (remainingDistance > 0)
                {
                    float currentLength = isLongSegment
                        ? longSegment
                        : shortSegment;
                    currentLength = Mathf.Min(currentLength, remainingDistance);

                    Vector3 segmentEnd = currentPos + dirNormalized * currentLength;

                    GameObject line = Instantiate(HintLineInstance, curGroup.transform);
                    line.transform.position = (currentPos + segmentEnd) / 2;
                    line.transform.rotation = Quaternion.LookRotation(direction);
                    line.transform.localScale = new Vector3(width, width, currentLength);

                    currentPos = segmentEnd;
                    remainingDistance -= currentLength;
                    isLongSegment = !isLongSegment;

                    if (remainingDistance > 0)
                    {
                        float actualGap = Mathf.Min(gap, remainingDistance);
                        currentPos += dirNormalized * actualGap;
                        remainingDistance -= actualGap;
                    }
                }
            }
        }
    }
    
    [Button("SpawnHintBoxOnly")]
    private void SpawnHintBox()
    {
        int _bufferLength = PointsBuffer.SavedPoints.Count;

        if (!_guidanceContainer) _guidanceContainer = new GameObject("HintBox").transform;

        GameObject hintbox;
        HintBoxTrigger component;

        for (int c = 0; c < _bufferLength; c++)
        {
            hintbox = Instantiate(HintBoxInstance,PointsBuffer.SavedPoints[c].Position,Quaternion.Euler(Vector3.zero),_guidanceContainer);
            hintbox.name = $"HintBox {c}";

            component = hintbox.GetComponent<HintBoxTrigger>();
            component.TriggerTime = PointsBuffer.SavedPoints[c].Time;
        }
    }
}
#endif