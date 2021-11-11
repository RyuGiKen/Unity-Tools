using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(ShowTransfromData))]
    public class ShowTransfromDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Transform transform = (target as ShowTransfromData).transform;

            if (transform)
            {
                GUILayout.Label("Posiition：\t\t " + transform.position);
                GUILayout.Label("localPosition：\t " + transform.localPosition);
                GUILayout.Label("eulerAngles：\t " + transform.eulerAngles);
                GUILayout.Label("localEulerAngles：\t " + transform.localEulerAngles);
                GUILayout.Label("rotation：\t\t " + transform.rotation);
                GUILayout.Label("localRotation：\t " + transform.localRotation);
                GUILayout.Label("lossyScale：\t " + transform.lossyScale);
                GUILayout.Label("localScale：\t " + transform.localScale);
                GUILayout.Space(10);
                GUILayout.Label("up：\t\t " + transform.up);
                GUILayout.Label("right：\t\t " + transform.right);
                GUILayout.Label("forward：\t\t " + transform.forward);
                if (transform is RectTransform)
                {
                    RectTransform rectTransform = transform as RectTransform;
                    GUILayout.Space(15);
                    GUILayout.Label("anchoredPosition：\t " + rectTransform.anchoredPosition);
                    GUILayout.Label("anchoredPosition3D:\t " + rectTransform.anchoredPosition3D);
                    GUILayout.Label("offsetMin：\t " + rectTransform.offsetMin);
                    GUILayout.Label("offsetMax：\t " + rectTransform.offsetMax);
                    GUILayout.Label("sizeDelta：\t " + rectTransform.sizeDelta);
                    GUILayout.Label("anchorMin：\t " + rectTransform.anchorMin);
                    GUILayout.Label("anchorMax：\t " + rectTransform.anchorMax);
                    GUILayout.Label("rect：\t\t " + rectTransform.rect.ToString().Replace("width", "\n\t\t width"));
                }
            }
        }
    }
}
