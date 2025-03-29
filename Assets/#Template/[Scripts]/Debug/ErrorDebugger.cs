using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gravitons.UI.Modal;

public class ErrorDebugger : MonoBehaviour
{
    string lastLog;
    private void Start()
    {
        Application.logMessageReceived += HandleLog;
    }
    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logString != lastLog & type != LogType.Log)
        {
            lastLog = logString;
            Debug.Log($"{GetType().Name}:HandlingLog");
            ModalManager.Show($"{type}", $"{logString} \n{stackTrace}", new[] { new ModalButton() { Text = "È·¶¨" } });
        }
    }
}
