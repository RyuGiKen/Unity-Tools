using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Tools;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace RyuGiKen.Localization
{
    /// <summary>
    /// 多语言配置父类
    /// </summary>
    public class LocalizationConfigurationBase : ScriptableObject
    {
        //public MultiArrayLocalizationString configurations;
        [HideInInspector] public string comments;
    }
    /// <summary>
    /// 文本多语言配置
    /// </summary>
    [CreateAssetMenu(fileName = "Localization String Configuration", menuName = "Localization String Configuration")]
    [System.Serializable]
    public class LocalizationStringConfiguration : LocalizationConfigurationBase
    {
        public MultiArrayLocalizationString configurations;
    }
    [System.Serializable] public class MultiArrayLocalizationString : MultiArray<LocalizationStringItem> { public new List<LocalizationString> items; }
    [System.Serializable] public class LocalizationString : ReorderableList<LocalizationStringItem> { }
    [System.Serializable]
    public class LocalizationStringItem
    {
        /// <summary>
        /// 语言
        /// </summary>
        public GamesLanguage language;
        //public bool mutiLineString;
        /// <summary>
        /// 文本
        /// </summary>
        public string str;

        public static implicit operator string(LocalizationStringItem value) { return value != null ? value.str : null; }
    }
    public static partial class Extension
    {
        private static bool OutIndex(int index, int IndexLength)
        {
            return index < 0 || index >= IndexLength;
        }
        public static LocalizationStringItem GetLocalization(this LocalizationStringConfiguration configuration, GamesLanguage language, int index)
        {
            if (configuration == null || configuration.configurations == null || configuration.configurations.items == null || OutIndex(index, configuration.configurations.items.Count))
                return null;
            return GetLocalization(configuration.configurations, language, index);
        }
        public static LocalizationStringItem GetLocalization(this MultiArrayLocalizationString configurations, GamesLanguage language, int index)
        {
            if (configurations == null || configurations.items == null || OutIndex(index, configurations.items.Count))
                return null;
            for (int i = 0; i < configurations.items[index].items.Count; i++)
            {
                if (configurations.items[index].items[i] != null && language == configurations.items[index].items[i].language)
                {
                    return configurations.items[index].items[i];
                }
            }
            return null;
        }
        public static string GetLocalizationString(this LocalizationConfigurationBase configuration, GamesLanguage language, int index)
        {
            if (configuration == null)
                return null;
            return configuration.TryGetLocalizationString(language, index, null);
        }
        public static string GetLocalizationString(this LocalizationStringConfiguration configuration, GamesLanguage language, int index)
        {
            if (configuration == null)
                return null;
            return configuration.configurations.GetLocalizationString(language, index);
        }
        public static string GetLocalizationString(this MultiArrayLocalizationString array, GamesLanguage language, int index)
        {
            LocalizationStringItem item = array.GetLocalization(language, index);
            if (item == null)
                return null;
            else
                return item;
        }
        public static string TryGetLocalizationString(this LocalizationConfigurationBase configuration, GamesLanguage language, int index, string exception)
        {
            if (configuration == null || language == GamesLanguage.Auto || index < 0)
                return exception;
            if (configuration is LocalizationConfiguration)
            {
                try
                {
                    return (configuration as LocalizationConfiguration).TryGetLocalizationString(language, index, exception);
                }
                catch
                {
                    return exception;
                }
            }
            else if (configuration is LocalizationStringConfiguration)
            {
                try
                {
                    return (configuration as LocalizationStringConfiguration).TryGetLocalizationString(language, index, exception);
                }
                catch
                {
                    return exception;
                }
            }
            else
            {
                return exception;
            }
        }
        public static string TryGetLocalizationString(this LocalizationStringConfiguration configuration, GamesLanguage language, int index, string exception)
        {
            if (configuration == null || language == GamesLanguage.Auto || index < 0)
                return exception;
            try
            {
                return configuration.configurations.TryGetLocalizationString(language, index, exception);
            }
            catch
            {
                return exception;
            }
        }
        public static string TryGetLocalizationString(this MultiArrayLocalizationString array, GamesLanguage language, int index, string exception)
        {
            if (array == null || language == GamesLanguage.Auto || index < 0)
                return exception;
            LocalizationStringItem item = array.GetLocalization(language, index);
            if (item == null)
                return exception;
            else
                return item;
        }
        public static LocalizationStringItem Find(this LocalizationStringItem[] array, GamesLanguage language)
        {
            if (array == null || array.Length < 1)
                return null;

            foreach (LocalizationStringItem item in array)
            {
                if (item.language == language)
                {
                    return item;
                }
            }
            return null;
        }
        public static LocalizationStringItem Find(this List<LocalizationStringItem> list, GamesLanguage language)
        {
            if (list == null || list.Count < 1)
                return null;

            //return list.ToArray().Find(language);
            foreach (LocalizationStringItem item in list)
            {
                if (item.language == language)
                {
                    return item;
                }
            }
            return null;
        }
        public static LocalizationStringItem Find(this LocalizationString localization, GamesLanguage language)
        {
            if (localization == null || localization.items == null || localization.items.Count < 1)
                return null;

            return localization.items.Find(language);
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LocalizationStringItem))]
    public class LocalizationStringItemPropertyDrawer : PropertyDrawer
    {
        protected static float LineHeight { get { return EditorGUIUtility.singleLineHeight; } }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            DrawContent(position, property, true);
            EditorGUI.indentLevel = indent;
        }
        /// <summary>
        /// 内容绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="showType"></param>
        /// <param name="startX"></param>
        public static void DrawContent(Rect rect, SerializedProperty property, bool showType, float startX = float.NaN)
        {
            if (!float.IsNaN(startX))
            {
                rect.width += (rect.x - startX);
                rect.x = startX;
            }
            float width = (rect.width / 4f).Clamp(60);
            Rect languageRect = new Rect(rect.x, rect.y, width, rect.height);
            Rect valueRect = new Rect(languageRect.x + languageRect.width, rect.y, rect.width - languageRect.width, LineHeight);

            GamesLanguage language = (GamesLanguage)property.FindPropertyRelative("language").enumValueIndex;
            language = (GamesLanguage)EditorGUI.EnumPopup(languageRect, language, EditorStyles.popup);
            property.FindPropertyRelative("language").enumValueIndex = (int)language;

            property.FindPropertyRelative("str").stringValue = EditorGUI.DelayedTextField(valueRect, property.FindPropertyRelative("str").stringValue, EditorStyles.textField);
        }
    }
    [CustomPropertyDrawer(typeof(LocalizationString))]
    public class LocalizationStringPropertyDrawer : ReorderableListPropertyDrawer
    {
        protected override ReorderableList CreateReorderableList(SerializedProperty property, SerializedProperty listProperty)
        {
            bool Draggable = property.FindPropertyRelative("Draggable").boolValue;
            return new ReorderableList(property.serializedObject, listProperty, Draggable, true, false, false);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetReorderableList(property).GetHeight() + minSpacing;// + EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = GetReorderableList(property);
            var listProperty = property.FindPropertyRelative("items");
            var height = 0f;
            for (var i = 0; i < listProperty.arraySize; i++)
            {
                height = Mathf.Max(height, GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
            }
            list.elementHeight = height;
            list.DoList(position);
            //list.DoLayoutList();
        }
        protected override void DrawListItems(Rect rect, SerializedProperty property)
        {
            LocalizationStringItemPropertyDrawer.DrawContent(rect, property, false);
            //EditorGUI.PropertyField(rect, property, true);
        }
    }
    [CustomEditor(typeof(LocalizationConfigurationBase))]
    public class LocalizationConfigurationBaseEditor : Editor
    {
        protected SerializedProperty Configurations;
        protected SerializedProperty[] Items;
        protected SerializedProperty Comments;
        protected bool EditComments = false;
        protected const int MinCount = 1;
        protected static int LanguageCount = 2;
        protected void OnEnable()
        {
            LanguageCount = ValueAdjust.GetEnumLength(typeof(GamesLanguage)) - 1;
        }
        protected virtual string[] GetNames()
        {
            string[] Name = new string[3];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Name[0] = "修改备注";
                    Name[1] = "清空冗余";
                    Name[2] = "清空全部";
                    break;
                default:
                    Name[0] = "Comments";
                    Name[1] = "Clear Redundan";
                    Name[2] = "Clear All";
                    break;
            }
            return Name;
        }
        /// <summary>
        /// 限制数量
        /// </summary>
        protected virtual void LimitCount()
        {
            Configurations = serializedObject.FindProperty("configurations");
            Comments = serializedObject.FindProperty("comments");
            SerializedProperty items = Configurations.FindPropertyRelative("items");
            if (items.arraySize != items.arraySize.Clamp(MinCount))
                items.arraySize = items.arraySize.Clamp(MinCount);
            Items = new SerializedProperty[items.arraySize];
            for (int i = 0; i < items.arraySize; i++)
            {
                Items[i] = items.GetArrayElementAtIndex(i).FindPropertyRelative("items");
            }
        }
        /// <summary>
        /// 限制语言
        /// </summary>
        protected virtual void LimitLanguage()
        {
            foreach (SerializedProperty property in Items)
            {
                if (property != null)
                {
                    if (property.arraySize != LanguageCount)
                        property.arraySize = LanguageCount;
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        SerializedProperty language = property.GetArrayElementAtIndex(i).FindPropertyRelative("language");
                        //LanguageCount = language.enumDisplayNames.Length - 1;
                        language.enumValueIndex = i.Clamp(0, LanguageCount) + 1;
                    }
                }
            }
        }
        /// <summary>
        /// 清空
        /// </summary>
        protected virtual void Clear(bool all)
        {
            //Debug.Log(Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                //Debug.Log(i);
                for (int j = 0; j < (Items[i].isArray ? Items[i].arraySize : 0); j++)
                {
                    SerializedProperty item = Items[i].GetArrayElementAtIndex(j);
                    //item.FindPropertyRelative("mutiLineString").boolValue = false;
                    item.FindPropertyRelative("str").stringValue = "";
                }
            }
        }
    }
    [CustomEditor(typeof(LocalizationStringConfiguration))]
    public class LocalizationStringConfigurationEditor : LocalizationConfigurationBaseEditor
    {
        public override void OnInspectorGUI()
        {
            string[] Name = GetNames();
            LimitCount();
            LimitLanguage();
            EditComments = EditorGUILayout.Foldout(EditComments, Name[0]);
            if (EditComments)
            {
                Comments.stringValue = EditorGUILayout.TextArea(Comments.stringValue);
            }
            else
            {
                EditorGUILayout.LabelField(Comments.stringValue, EditorStyles.textArea, GUILayout.ExpandHeight(true));
            }
            if (GUILayout.Button(Name[2]))
            {
                Clear(true);
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
#endif
}
