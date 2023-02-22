using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 彩色滑动条
    /// </summary>
    [AddComponentMenu("RyuGiKen/彩色滑动条")]
    public class ColorSlider : Slider
    {
        [System.Serializable]
        public struct ImageGradientGroup
        {
            public bool enable;
            public Graphic graphic;
            public Gradient gradient;

            public void Evaluate(float value)
            {
                if (enable && graphic && graphic.isActiveAndEnabled && gradient != null)
                {
                    graphic.color = gradient.Evaluate(value);
                }
            }
        }
        public List<ImageGradientGroup> groups;
        protected override void Update()
        {
            base.Update();
            for (int i = groups.Count - 1; i >= 0; i--)
            {
                groups[i].Evaluate(value / maxValue);
            }
        }
    }
}
namespace RyuGiKenEditor.Tools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ColorSlider))]
    public class ColorSliderEditor : SliderEditor
    {
        protected SerializedProperty groups;
        protected override void OnEnable()
        {
            base.OnEnable();
            groups = serializedObject.FindProperty("groups");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(groups);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
