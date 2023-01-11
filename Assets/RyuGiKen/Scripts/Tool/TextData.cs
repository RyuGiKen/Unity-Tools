using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Tools;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RyuGiKen.Tools
{
    [DisallowMultipleComponent]
    //[ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class TextData : MonoBehaviour
    {
        public Text m_Text;
        void Awake()
        {
            m_Text = this.GetComponent<Text>();
        }
        void Reset()
        {
            m_Text = this.GetComponent<Text>();
        }
    }
}
namespace RyuGiKenEditor.Tools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(TextData))]
    public class TextDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Text m_Text = (target as TextData).m_Text;
            if (!m_Text)
                return;

            EditorGUILayout.LabelField("preferredWidth", m_Text.preferredWidth.ToString());
            EditorGUILayout.LabelField("preferredHeight", m_Text.preferredHeight.ToString());

            EditorGUILayout.LabelField("fontSizeUsedForBestFit", string.Format("{0}  /  {1}(ForLayout)", m_Text.cachedTextGenerator.fontSizeUsedForBestFit, m_Text.cachedTextGeneratorForLayout.fontSizeUsedForBestFit));
            EditorGUILayout.LabelField("characters", string.Format("{0}  /  {1}(ForLayout)", m_Text.cachedTextGenerator.characters.Count, m_Text.cachedTextGeneratorForLayout.characters.Count));
            EditorGUILayout.LabelField("characterCount", string.Format("{0}  /  {1}(ForLayout)", m_Text.cachedTextGenerator.characterCountVisible, m_Text.cachedTextGeneratorForLayout.characterCountVisible));

            EditorGUILayout.LabelField("verts", string.Format("{0}  /  {1}(ForLayout)", m_Text.cachedTextGenerator.verts.Count, m_Text.cachedTextGeneratorForLayout.verts.Count));
            EditorGUILayout.LabelField("lines", string.Format("{0}  /  {1}(ForLayout)", m_Text.cachedTextGenerator.lines.Count, m_Text.cachedTextGeneratorForLayout.lines.Count));
            EditorGUILayout.LabelField("lineCount", string.Format("{0}  /  {1}(ForLayout)", m_Text.cachedTextGenerator.lineCount, m_Text.cachedTextGeneratorForLayout.lineCount));

            EditorGUILayout.RectField("rectExtents", m_Text.cachedTextGenerator.rectExtents);
            EditorGUILayout.RectField("rectExtents(ForLayout)", m_Text.cachedTextGeneratorForLayout.rectExtents);
        }
    }
#endif
}
