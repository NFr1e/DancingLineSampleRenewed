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

    [BoxGroup("HintBoxSettings")]
    public GameObject HintBoxInstance;
    [BoxGroup("HintBoxSettings")]
    public GameObject HintLineInstance;
    [BoxGroup("HintBoxSettings")]
    public float HintLineShrink = 0;

    private Transform _pathContainer, _hintBoxContainer, _hintLineContainer;

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
        int _bufferLength = PointsBuffer.SavedPoints.Count;

        if (!_hintLineContainer) _hintLineContainer = new GameObject("HintLines").transform;

        for (int i = 0; i < _bufferLength - 1; i++)
        {
            Vector3 start = PointsBuffer.SavedPoints[i].Position;
            Vector3 end = PointsBuffer.SavedPoints[i + 1].Position;

            Vector3 direction = end - start;
            float distance = direction.magnitude;

            if (distance <= Mathf.Epsilon) continue;

            Vector3 midpoint = (start + end) / 2;

            GameObject line = Instantiate(HintLineInstance, _hintLineContainer);
            line.transform.position = midpoint;
            line.transform.rotation = Quaternion.LookRotation(direction);
            line.transform.localScale = new Vector3(0.15f, 0.15f, distance - 2f - HintLineShrink);
        }
    }
    [Button("SpawnHintBox")]
    private void SpawnHintBox()
    {
        int _bufferLength = PointsBuffer.SavedPoints.Count;

        if (!_hintBoxContainer) _hintBoxContainer = new GameObject("HintBox").transform;

        GameObject hintbox;
        HintBoxTrigger component;

        for (int c = 0; c < _bufferLength; c++)
        {
            hintbox = Instantiate(HintBoxInstance,PointsBuffer.SavedPoints[c].Position,Quaternion.Euler(Vector3.zero),_hintBoxContainer);

            component = hintbox.GetComponent<HintBoxTrigger>();
            component.TriggerTime = PointsBuffer.SavedPoints[c].Time;
        }
    }
}
#endif