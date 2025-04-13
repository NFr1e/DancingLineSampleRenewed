using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DancingLineFanmade.Gameplay;
using DancingLineFanmade.Collectable;
using DancingLineFanmade.Triggers;

namespace DancingLineFanmade.UI
{
    public static class UserInterfaceEvents
    {
        public static event System.Action
            OnReadyInterfaceEnter,
            OnReadyInterfaceExit,
            OnRespawnInterfaceEnter,
            OnRespawnInterfaceExit,
            OnOverInterfaceEnter,
            OnOverInterfaceExit;

        public static void TriggerReadyEnterEvent() => OnReadyInterfaceEnter?.Invoke();
        public static void TriggerReadyExitEvent() => OnReadyInterfaceExit?.Invoke();
        public static void TriggerRespawnEnterEvent() => OnRespawnInterfaceEnter?.Invoke();
        public static void TriggerRespawnExitEvent() => OnRespawnInterfaceExit?.Invoke();
        public static void TriggerOverEnterEvent() => OnOverInterfaceEnter?.Invoke();
        public static void TriggerOverExitEvent() => OnOverInterfaceExit?.Invoke();
    }
    public class UserInterfaceManager : MonoBehaviour
    {
        #region StaticMethods
        public static Vector2 CurrentResolution()
        {
            return new Vector2(Screen.width, Screen.height);
        }
        public static Image InstantiateImage(Color color)
        {
            Canvas canvas = new GameObject("MaskCanvas").AddComponent<Canvas>();
            canvas.gameObject.AddComponent<CanvasGroup>().alpha = 1;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (canvas.GetComponent<CanvasScaler>())
            {
                canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            }
            else
            {
                canvas.gameObject.AddComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            }

            Image image = new GameObject("MaskImage").AddComponent<Image>();
            image.color = color;
            image.transform.SetParent(canvas.transform);
            image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            image.rectTransform.anchoredPosition = Vector2.zero;
            return image;
        }
        public static void ShowGUIText(string content, Color color, int size, Rect rect)
        {
            GUIStyle fontStyle = new GUIStyle();
            fontStyle.normal.textColor = color;
            fontStyle.fontSize = size;
            GUI.Label(rect, content, fontStyle);
        }
        #endregion

        public InterfaceBase ReadyInstance, 
                             RespawnInstance, 
                             OverInstance;

        private static InterfaceBase curInterface;
        private GameObject _eventSystem;

        private bool _checkpointed = false, 
                     _gamePassed = false,
                     _confirmdDisRespawn = false;

        private void OnEnable()
        {
            GameEvents.OnEnterLevel += Init;
            GameEvents.OnEnterLevel += LoadReadyInterface;
            GameEvents.OnGamePaused += LoadReadyInterface;
            GameEvents.OnGameOver += LoadOverInterface;

            CheckpointEvents.OnCheckpointCollected += SetCheckpointed;

            RespawnEvents.OnEndRespawn += LoadReadyInterface;

            PyramidTrigger.OnEnterPyramidTrigger += SetGamePassed;

            UserInterfaceEvents.OnRespawnInterfaceExit += DisRespawnSetTrue;
            UserInterfaceEvents.OnRespawnInterfaceExit += LoadOverInterface;
        }
        private void OnDisable()
        {
            GameEvents.OnEnterLevel -= Init;
            GameEvents.OnEnterLevel -= LoadReadyInterface;
            GameEvents.OnGamePaused -= LoadReadyInterface;
            GameEvents.OnGameOver -= LoadOverInterface;

            CheckpointEvents.OnCheckpointCollected -= SetCheckpointed;

            RespawnEvents.OnEndRespawn -= LoadReadyInterface;

            PyramidTrigger.OnEnterPyramidTrigger -= SetGamePassed;

            UserInterfaceEvents.OnRespawnInterfaceExit -= DisRespawnSetTrue;
            UserInterfaceEvents.OnRespawnInterfaceExit -= LoadOverInterface;
        }
        private void SetCheckpointed() => _checkpointed = true;
        private void SetGamePassed() => _gamePassed = true;
        private void DisRespawnSetTrue() => _confirmdDisRespawn = true; 
        private void Init()
        {
            if (FindObjectOfType<EventSystem>()) _eventSystem = FindObjectOfType<EventSystem>().gameObject;

            if (_eventSystem)
                Destroy(_eventSystem.gameObject);

            _eventSystem = new GameObject("EventSystem");
            _eventSystem.AddComponent<EventSystem>();
            _eventSystem.AddComponent<StandaloneInputModule>();

            Debug.Log("EventSystemGenerated");
        }
        private void LoadReadyInterface()
        {
            if (!ReadyInstance) return;

            if (curInterface != null) curInterface.ExitInterface();

            curInterface = Instantiate(ReadyInstance.gameObject).GetComponent<InterfaceBase>();
            DontDestroyOnLoad(curInterface);
        }
        private void LoadRespawnInterface()
        {
            if (!RespawnInstance) return;

            curInterface = Instantiate(RespawnInstance.gameObject).GetComponent<InterfaceBase>();
            DontDestroyOnLoad(curInterface);
        }
        private void LoadOverInterface()
        {
            if(_checkpointed && !_confirmdDisRespawn && !_gamePassed) 
            {
                LoadRespawnInterface();
                return;
            }

            if (!OverInstance) return;

            curInterface = Instantiate(OverInstance.gameObject).GetComponent<InterfaceBase>();
            DontDestroyOnLoad(curInterface);
        }
    }
}
