using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using DancingLineFanmade.Gameplay;
using DancingLineFanmade.Audio;
using DancingLineFanmade.UI;

namespace DancingLineFanmade.Debugging
{
    public class InLevelDebugLable : MonoBehaviour
    {
        public static InLevelDebugLable instance;

        [BoxGroup("DrawGUI")]
        public bool DrawGUI;

        public void OnGUI()
        {
            if (DrawGUI)
            {
                UserInterfaceManager.ShowGUIText("��ǰ��Ϸ״̬:" + GameController.curGameState, Color.black, 30, new Rect(50, 30, 200, 70));
                UserInterfaceManager.ShowGUIText("����" + Mathf.Abs(Physics.gravity.y).ToString(), Color.black, 30, new Rect(50, 70, 200, 70));
                UserInterfaceManager.ShowGUIText("Player�ٶ�" + Player.instance.DefaultPlayerSpeed.ToString(), Color.black, 30, new Rect(50, 110, 200, 70));
                UserInterfaceManager.ShowGUIText("Player����" + Mathf.Round(Player.instance.transform.eulerAngles.y).ToString(), Color.black, 30, new Rect(50, 150, 200, 70));
                UserInterfaceManager.ShowGUIText("��ǰStair״̬:" + ReadyStairManager.curStairState, Color.black, 30, new Rect(50, 230, 200, 70));
                UserInterfaceManager.ShowGUIText("��ǰStair:Stared:" + ReadyStairManager.started, Color.black, 30, new Rect(50, 270, 200, 70));
                
                UserInterfaceManager.ShowGUIText("PoniterOnUI:" + GameController.PointerOnUI, Color.black, 30, new Rect(50, 310, 200, 70));
                UserInterfaceManager.ShowGUIText("CurrentFps:" + (int)(1 / _deltaTime), Color.black, 30, new Rect(50, 350, 200, 70));
                UserInterfaceManager.ShowGUIText("��ǰSoundtrackTime:" + AudioManager.instance.CurrentLevelTime, Color.black, 30, new Rect(50, 390, 200, 70));
            }
        }
        private float _deltaTime = 0;
        private void Update()
        {
            if (!Application.isPlaying) return;

            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }
    }
}
