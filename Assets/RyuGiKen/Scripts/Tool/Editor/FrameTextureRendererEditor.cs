using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(FrameTextureRenderer))]
    public class FrameTextureRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[2];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "开始渲染";
                    ButtonName[1] = "停止渲染";
                    break;
                default:
                    ButtonName[0] = "Start Render";
                    ButtonName[1] = "Stop Render";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as FrameTextureRenderer).StartRender();
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                (target as FrameTextureRenderer).StopRender();
            }
        }
    }
}