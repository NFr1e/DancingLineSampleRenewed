using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using DancingLineFanmade.UI;
using DancingLineFanmade.Debugging;

public class HintBoxTrigger : MonoBehaviour
{
    public float TriggerTime;
    [SerializeField]private BoxCollider _collider;
    [SerializeField] [BoxGroup("Debug")] private bool DrawHintBoxTime = true,DrawHintBoxCollider = true;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        SceneView sceneView = SceneView.currentDrawingSceneView;
        if (sceneView == null) return;
        Camera sceneCamera = sceneView.camera;
        if (sceneCamera == null) return;
        float distance = Vector3.Distance(transform.position, sceneCamera.transform.position);
        if (distance > 40) return;

        Vector3 textPosition = transform.position;

        Color _backgroundColor = new Color(255, 255, 255, 0.5f);
        Texture2D _background = UserInterfaceManager.ToTexture2D(_backgroundColor);
        
        GUIStyle style = new()
        {
            fontSize = 15,
            normal = new GUIStyleState
            {
                textColor = Color.black,
                background = _background
            }
        };
        if(DrawHintBoxTime)
            Handles.Label(textPosition, $"Time:{TriggerTime}", style);
        if (DrawHintBoxCollider)
        {
            if (!_collider) return;
            Gizmos.color = new Color(255, 255, 255, 0.5f);
            Gizmos.DrawWireCube(_collider.transform.position, _collider.transform.localScale);
        }
    }
#endif
}
