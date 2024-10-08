﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Tools;
using RyuGiKen.Localization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace RyuGiKen.Localization
{
    /// <summary>
    /// 多语言配置父类
    /// </summary>
    public abstract class LocalizationConfigurationBase : ScriptableObject
    {
        public bool SupportMultiLine;
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
    [System.Serializable]
    public class MultiArrayLocalizationString : MultiArrayExtension<LocalizationStringItem>
    {
        public new List<LocalizationString> items;
        public MultiArrayLocalizationString(LocalizationStringItem[][] array)
        {
            this.items = new List<LocalizationString>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new LocalizationString(array[i]));
        }
        public MultiArrayLocalizationString(ReorderableList<LocalizationStringItem>[] array)
        {
            this.items = new List<LocalizationString>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new LocalizationString(array[i].ToArray()));
        }
        public MultiArrayLocalizationString(LocalizationString[] array)
        {
            this.items = new List<LocalizationString>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayLocalizationString(List<ReorderableList<LocalizationStringItem>> list)
        {
            this.items = new List<LocalizationString>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new LocalizationString(list[i].ToArray()));
        }
        public MultiArrayLocalizationString(List<LocalizationString> list) { this.items = list; }
        public override LocalizationStringItem GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override LocalizationStringItem GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [System.Serializable]
    public class LocalizationString : ReorderableList<LocalizationStringItem>
    {
        /// <summary>
        /// 多行
        /// </summary>
        public bool multiLineString;
        public LocalizationString() { this.items = new List<LocalizationStringItem>(); }
        public LocalizationString(LocalizationStringItem[] array) { this.items = array != null ? array.ToList() : new List<LocalizationStringItem>(); }
        public LocalizationString(List<LocalizationStringItem> list) { this.items = list; }
    }
    [System.Serializable]
    public class LocalizationStringItem
    {
        /// <summary>
        /// 语言
        /// </summary>
        public GamesLanguage language;
        /// <summary>
        /// 多行
        /// </summary>
        public bool multiLineString;
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
        public static string TryGetLocalizationStringFormat(this LocalizationConfigurationBase configuration, GamesLanguage language, int index, string exception, params object[] args)
        {
            if (configuration == null || language == GamesLanguage.Auto || index < 0 || args == null || args.Length < 1)
                return exception;
            if (configuration is LocalizationConfiguration)
            {
                try
                {
                    return (configuration as LocalizationConfiguration).TryGetLocalizationStringFormat(language, index, exception, args);
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
                    return (configuration as LocalizationStringConfiguration).TryGetLocalizationStringFormat(language, index, exception, args);
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
        public static string TryGetLocalizationStringFormat(this LocalizationStringConfiguration configuration, GamesLanguage language, int index, string exception, params object[] args)
        {
            if (configuration == null || language == GamesLanguage.Auto || index < 0 || args == null || args.Length < 1)
                return exception;
            try
            {
                return configuration.configurations.TryGetLocalizationStringFormat(language, index, exception, args);
            }
            catch
            {
                return exception;
            }
        }
        public static string TryGetLocalizationStringFormat(this MultiArrayLocalizationString array, GamesLanguage language, int index, string exception, params object[] args)
        {
            if (array == null || language == GamesLanguage.Auto || index < 0 || args == null || args.Length < 1)
                return exception;

            return array.TryGetLocalizationString(language, index, exception).StringFormat(args);
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
}
namespace RyuGiKenEditor.Localization
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LocalizationStringItem))]
    public class LocalizationStringItemPropertyDrawer : PropertyDrawer
    {
        protected static float LineHeight { get { return EditorGUIUtility.singleLineHeight; } }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (LineHeight - 3) * (CheckStringLine(property) - 1);
        }
        public static int CheckStringLine(SerializedProperty property)
        {
            if ((property.type != nameof(LocalizationItem) && property.FindPropertyRelative("multiLineString").boolValue) || (property.type == nameof(LocalizationItem) && (ObjectType)property.FindPropertyRelative("type")?.enumValueIndex == ObjectType.StringMultiLine))
            {
                string str = property.FindPropertyRelative("str").stringValue;
                int line = (str.Length - str.ReplaceAny("\n\r", "").Length + 1).Clamp(1);
                return line;
            }
            return 1;
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
        /// <param name="multiLine"></param>
        public static void DrawContent(Rect rect, SerializedProperty property, bool showType, float startX = float.NaN, bool? multiLine = null)
        {
            if (!float.IsNaN(startX))
            {
                rect.width += (rect.x - startX);
                rect.x = startX;
            }
            float width = (rect.width / 4f).Clamp(60);
            Rect languageRect = new Rect(rect.x, rect.y, width, LineHeight);
            Rect multiLineRect = new Rect(languageRect.xMax, rect.y, (multiLine == null ? 20 : 0), LineHeight);
            Rect valueRect = new Rect(multiLineRect.xMax, rect.y, rect.width - width - multiLineRect.width, rect.height);
            if (multiLine == null)
                multiLine = property.FindPropertyRelative("multiLineString").boolValue = EditorGUI.Toggle(multiLineRect, property.FindPropertyRelative("multiLineString").boolValue);
            valueRect.height = multiLine == true ? rect.height : LineHeight;

            GamesLanguage language = (GamesLanguage)property.FindPropertyRelative("language").enumValueIndex;
            language = (GamesLanguage)EditorGUI.EnumPopup(languageRect, language, EditorStyles.popup);
            property.FindPropertyRelative("language").enumValueIndex = (int)language;

            if (multiLine == true)
                property.FindPropertyRelative("str").stringValue = EditorGUI.TextArea(valueRect, property.FindPropertyRelative("str").stringValue, EditorStyles.textArea);
            else
                property.FindPropertyRelative("str").stringValue = EditorGUI.DelayedTextField(valueRect, property.FindPropertyRelative("str").stringValue, EditorStyles.textField);
        }
    }
    [CustomPropertyDrawer(typeof(LocalizationString))]
    public class LocalizationStringPropertyDrawer : ReorderableListPropertyDrawer
    {
        protected override ReorderableList CreateReorderableList(SerializedProperty property, SerializedProperty listProperty)
        {
            return CreateReorderableList(property, listProperty, false);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetReorderableList(property).GetHeight() + minSpacing + EditorGUIUtility.singleLineHeight;
        }
        protected virtual string[] GetNames()
        {
            string[] Name = new string[3];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Name[0] = "多行";
                    break;
                default:
                    Name[0] = "MultiLine";
                    break;
            }
            return Name;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect multiLineRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            bool multiLine = property.FindPropertyRelative("multiLineString").boolValue = EditorGUI.Foldout(multiLineRect, property.FindPropertyRelative("multiLineString").boolValue, GetNames()[0]);
            var list = GetReorderableList(property);
            var listProperty = property.FindPropertyRelative("items");
            var height = 0f;
            for (var i = 0; i < listProperty.arraySize; i++)
            {
                height = Mathf.Max(height, GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
                listProperty.GetArrayElementAtIndex(i).FindPropertyRelative("multiLineString").boolValue = multiLine;
            }
            list.elementHeight = height;
            position.y += EditorGUIUtility.singleLineHeight;
            list.DoList(position);
            //list.DoLayoutList();
        }
        protected override void DrawListItems(Rect rect, SerializedProperty property)
        {
            LocalizationStringItemPropertyDrawer.DrawContent(rect, property, false, float.NaN, property.FindPropertyRelative("multiLineString").boolValue);
            //EditorGUI.PropertyField(rect, property, true);
        }
    }
    [CustomEditor(typeof(LocalizationConfigurationBase), true)]
    public class LocalizationConfigurationBaseEditor : Editor
    {
        protected SerializedProperty Configurations;
        protected SerializedProperty[] Items;
        protected SerializedProperty SupportMultiLine;
        protected SerializedProperty Comments;
        protected bool EditComments = false;
        protected const int MinCount = 1;
        protected static int LanguageCount = 2;
        protected void OnEnable()
        {
            LanguageCount = ValueAdjust.GetEnumLength(typeof(GamesLanguage));
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
            SupportMultiLine = serializedObject.FindProperty("SupportMultiLine");
            Comments = serializedObject.FindProperty("comments");
            SerializedProperty items = Configurations.FindPropertyRelative("items");
            if (items.arraySize != items.arraySize.Clamp(MinCount))
                items.arraySize = items.arraySize.Clamp(MinCount);
            Items = new SerializedProperty[items.arraySize];
            for (int i = 0; i < items.arraySize; i++)
            {
                Items[i] = items.GetArrayElementAtIndex(i).FindPropertyRelative("items");
            }
            MultiLineSupport(items);
        }
        /// <summary>
        /// 多行输入支持
        /// </summary>
        /// <param name="items"></param>
        protected virtual void MultiLineSupport(SerializedProperty items)
        {
            SupportMultiLine = serializedObject.FindProperty("SupportMultiLine");
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
                        if (i == LanguageCount - 1)
                        {
                            language.enumValueIndex = 0;
                            ClearItem(property.GetArrayElementAtIndex(i));
                        }
                        else
                            language.enumValueIndex = i.Clamp(0, LanguageCount) + 1;
                    }
                }
            }
        }
        protected virtual void ClearItem(SerializedProperty item)
        {
            if (item != null && item.type == nameof(LocalizationStringItem))
            {
                //item.FindPropertyRelative("multiLineString").boolValue = false;
                item.FindPropertyRelative("str").stringValue = "";
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
                    //item.FindPropertyRelative("multiLineString").boolValue = false;
                    item.FindPropertyRelative("str").stringValue = "";
                }
            }
        }
    }
    [CustomEditor(typeof(LocalizationStringConfiguration), true)]
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
        protected override void MultiLineSupport(SerializedProperty items)
        {
            SupportMultiLine = serializedObject.FindProperty("SupportMultiLine");
            for (int i = 0; i < (items.isArray ? items.arraySize : 0); i++)
                if (!SupportMultiLine.boolValue)
                    items.GetArrayElementAtIndex(i).FindPropertyRelative("multiLineString").boolValue = false;
            if (!SupportMultiLine.boolValue)
                for (int i = 0; i < Items.Length; i++)
                {
                    for (int j = 0; j < (Items[i].isArray ? Items[i].arraySize : 0); j++)
                    {
                        SerializedProperty item = Items[i].GetArrayElementAtIndex(j);
                        item.FindPropertyRelative("str").stringValue = item.FindPropertyRelative("str").stringValue.ReplaceAny("\n\r", "");
                    }
                }
        }
    }
#endif
}
