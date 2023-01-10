using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Tools;
using WindowsAPI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RyuGiKen.Tools
{
    [DisallowMultipleComponent]
    //[RequireComponent(typeof(Text))]
    public class TextReplaceSpacing : MonoBehaviour
    {
        public const string ChineseSpacing = "\u3000";
        public Text m_Text;
        [TextArea(1, 30)] public string TempString;
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
    [CustomEditor(typeof(TextReplaceSpacing))]
    public class TextReplaceSpacingEditor : Editor
    {
        SerializedProperty TempString;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] Names = new string[10];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Names[0] = "中文空格";
                    Names[1] = "英文空格";
                    Names[2] = "全部{0}替换成{1}";
                    Names[3] = "移除\\n";
                    Names[4] = "转换为大写";
                    Names[5] = "转换为小写";
                    Names[6] = "转换为全角";
                    Names[7] = "转换为半角";
                    Names[8] = "转换为繁体";
                    Names[9] = "转换为简体";
                    break;
                default:
                    Names[0] = "Spacing (Chinese)";
                    Names[1] = "Spacing (English)";
                    Names[2] = "Replace All {0} to {1}";
                    Names[3] = "Remove \\n";
                    Names[4] = "Convert to Uppercase";
                    Names[5] = "Convert to Lowercase";
                    Names[6] = "Convert to Full Width";
                    Names[7] = "Convert to Half Width";
                    Names[8] = "Convert to Traditional";
                    Names[9] = "Convert to Simplified";
                    break;
            }
            EditorGUILayout.TextField(Names[0], TextReplaceSpacing.ChineseSpacing);
            EditorGUILayout.TextField(Names[1], " ");
            TempString = serializedObject.FindProperty("TempString");
            Text m_Text = (target as TextReplaceSpacing).m_Text;
            if (m_Text)
            {
                TempString.stringValue = m_Text.text;
            }
            string temp = TempString.stringValue;
            if (GUILayout.Button(string.Format(Names[2], Names[1], Names[0])))//替换为中文空格
            {
                temp = temp.Replace(" ", TextReplaceSpacing.ChineseSpacing);
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(string.Format(Names[2], Names[0], Names[1])))//替换为英文空格
            {
                temp = temp.Replace(TextReplaceSpacing.ChineseSpacing, " ");
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(string.Format(Names[2], @"\r", @"\n")))//\r替换为\n
            {
                temp = temp.Replace("\r\n", "\n").Replace("\n\r", "\n");
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(Names[3]))//移除\n
            {
                temp = temp.ReplaceAny("\r\n", "");
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(Names[4]))//转换为大写
            {
                temp = temp.ToUpper();
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(Names[5]))//转换为小写
            {
                temp = temp.ToLower();
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(Names[6]))//转换为全角
            {
                temp = temp.ToFullWidthCharacters();
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(Names[7]))//转换为半角
            {
                temp = temp.ToHalfWidthCharacters();
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(Names[8]))//转换为繁体
            {
                temp = ChineseConverter.ToTraditional(temp);
                SetText(m_Text, temp);
            }
            if (GUILayout.Button(Names[9]))//转换为简体
            {
                temp = ChineseConverter.ToSimplified(temp);
                SetText(m_Text, temp);
            }
            serializedObject.ApplyModifiedProperties();
        }
        void SetText(Text m_Text, string str)
        {
            if (m_Text)
            {
                m_Text.SetText(str);
                Canvas.ForceUpdateCanvases();
            }
            if((target as TextReplaceSpacing).TryGetComponent(out PanelTextArea panelTextArea))
            {
                panelTextArea.Input = str;
            }
            TempString.stringValue = str;
        }
    }
#endif
}