using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 对象类型
    /// </summary>
    public enum ObjectType
    {
        String,
        AudioClip,
        Sprite,
        Texture,
        OtherObject,
    }
    /// <summary>
    /// 语言配置
    /// </summary>
    [CreateAssetMenu(fileName = "LanguageSetting", menuName = "LanguageSetting")]
    [System.Serializable]
    public class LanguageSetting : ScriptableObject
    {
        /// <summary>
        /// 从其他配置文件覆盖
        /// </summary>
        public bool OverWriteFromOther;
        /// <summary>
        /// 覆盖来源
        /// </summary>
        public LanguageSetting from;
        public GamesLanguage language;
        public LanguageSettingItem[] items;
    }
    [System.Serializable]
    public class LanguageSettingItem
    {
        public ObjectType type;
        public bool mutiLineString;
        public string str;
        public AudioClip audioClip;
        public Sprite sprite;
        public Texture texture;
        public Object otherObject;

        public static implicit operator string(LanguageSettingItem value) { return (value != null && value.type == ObjectType.String) ? value.str : null; }
        public static implicit operator AudioClip(LanguageSettingItem value) { return (value != null && value.type == ObjectType.AudioClip) ? value.audioClip : null; }
        public static implicit operator Sprite(LanguageSettingItem value) { return (value != null && value.type == ObjectType.Sprite) ? value.sprite : null; }
        public static implicit operator Texture(LanguageSettingItem value) { return (value != null && value.type == ObjectType.Texture) ? value.texture : null; }
        public static implicit operator Object(LanguageSettingItem value) { return (value != null && value.type == ObjectType.OtherObject) ? value.otherObject : null; }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(LanguageSetting), true)]
    [CanEditMultipleObjects]
    public class LanguageSettingEditor : Editor
    {
        SerializedProperty OverWriteFromOther;
        SerializedProperty From;
        SerializedProperty Language;
        SerializedProperty Items;
        void OnEnable()
        {
            OverWriteFromOther = serializedObject.FindProperty("OverWriteFromOther");
            From = serializedObject.FindProperty("from");
            Language = serializedObject.FindProperty("language");
            Items = serializedObject.FindProperty("items");
        }
        public override void OnInspectorGUI()
        {
            string[] Name = new string[10];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Name[0] = "覆盖";
                    Name[1] = "原始";
                    Name[2] = "来源";
                    Name[3] = "语言";
                    Name[4] = "数量";
                    Name[5] = "清空冗余";
                    Name[6] = "清空全部";
                    Name[7] = "复制";
                    Name[8] = "覆盖空项";
                    Name[9] = "覆盖";
                    break;
                default:
                    Name[0] = "OverWriteFromOther";
                    Name[1] = "Primitive";
                    Name[2] = "From";
                    Name[3] = "Language";
                    Name[4] = "Count";
                    Name[5] = "Clear Redundan";
                    Name[6] = "Clear All";
                    Name[7] = "Copy";
                    Name[8] = "Replace Null";
                    Name[9] = "Cover";
                    break;
            }
            int Selections = 0;
            foreach (Object obj in Selection.objects)
            {
                if (obj && obj is LanguageSetting)
                    Selections++;
            }
            if (Selections > 1)
            {
                int MaxCount = 0;
                List<SerializedObject> objs = new List<SerializedObject>();
                foreach (Object obj in Selection.objects)
                {
                    if (obj && obj is LanguageSetting)
                    {
                        int temp = (obj as LanguageSetting).items.Length;
                        if (temp > MaxCount)
                            MaxCount = temp;

                        EditorGUILayout.EnumPopup(Name[3], (obj as LanguageSetting).language);

                        SerializedObject serializedObj = new SerializedObject(obj as LanguageSetting);
                        objs.Add(serializedObj);
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                for (int i = 0; i < MaxCount; i++)
                {
                    List<SerializedProperty> items = new List<SerializedProperty>();
                    foreach (SerializedObject serializedObj in objs)
                    {
                        //items.Add((Selection.objects[j] as LanguageSetting).items[i]);
                        if (i < serializedObj.FindProperty("items").arraySize)
                            items.Add(serializedObj.FindProperty("items").GetArrayElementAtIndex(i));
                    }
                    bool SameType = SameTypeFromDifferentSetting(items.ToArray());
                    for (int j = 0; j < items.Count; j++)
                    {
                        LanguageSettingItemPropertyDrawer.DrawContent(items[j], (j == 0 && SameType), false, j == 0 ? (i.ToString("D" + Items.arraySize.ToString().Length)) : " ");
                    }
                    EditorGUILayout.Space();
                }
                foreach (SerializedObject serializedObj in objs)
                {
                    serializedObj.ApplyModifiedProperties();
                }
            }
            else
            {
                OverWriteFromOther.boolValue = EditorGUILayout.Toggle(OverWriteFromOther.boolValue ? Name[0] : Name[1], OverWriteFromOther.boolValue);
                LanguageSetting setting = null;
                if (OverWriteFromOther.boolValue)
                {
                    setting = (LanguageSetting)EditorGUILayout.ObjectField(Name[2], From.objectReferenceValue, typeof(LanguageSetting), false);
                    From.objectReferenceValue = setting;
                }
                GamesLanguage OverWriteLanguage = (GamesLanguage)EditorGUILayout.EnumPopup(Name[3], (GamesLanguage)Language.enumValueIndex);
                if (GUILayout.Button(Name[5]))
                {
                    Clear(false);
                }
                if (GUILayout.Button(Name[6]))
                {
                    Clear(true);
                }
                if (!OverWriteFromOther.boolValue)
                {
                    Language.enumValueIndex = (int)OverWriteLanguage;
                    if (OverWriteLanguage != GamesLanguage.Auto)
                    {
                        //base.OnInspectorGUI();
                        EditorGUILayout.Space();
                        int count = EditorGUILayout.DelayedIntField(Name[4], Items.arraySize).Clamp(1);
                        Items.arraySize = count;
                        for (int i = 0; i < Items.arraySize; i++)
                        {
                            SerializedProperty item = Items.GetArrayElementAtIndex(i);
                            //int line = LanguageSettingItemPropertyDrawer.CheckStringLine(item);
                            LanguageSettingItemPropertyDrawer.DrawContent(item, true, true, i.ToString("D" + Items.arraySize.ToString().Length));
                            EditorGUILayout.Space();
                        }
                    }
                    EditorGUILayout.Space();
                }
                else
                {
                    if (setting && setting.language != OverWriteLanguage && setting.language != GamesLanguage.Auto && OverWriteLanguage != GamesLanguage.Auto)
                    {
                        EditorGUILayout.Space();
                        if (GUILayout.Button(Name[7]))
                        {
                            CopyAll(setting);
                        }
                        if (GUILayout.Button(Name[8]))
                        {
                            ReplaceNull(setting);
                        }
                        EditorGUILayout.Space();
                        Language.enumValueIndex = (int)OverWriteLanguage;
                        //Rect rect = EditorGUILayout.GetControlRect(true, 18f, EditorStyles.objectField);
                        Items.arraySize = setting.items.Length;
                        for (int i = 0; i < Items.arraySize; i++)
                        {
                            SerializedProperty item = Items.GetArrayElementAtIndex(i);
                            //int line = LanguageSettingItemPropertyDrawer.CheckStringLine(item);
                            item.FindPropertyRelative("type").enumValueIndex = (int)setting.items[i].type;
                            item.FindPropertyRelative("mutiLineString").boolValue = setting.items[i].mutiLineString;
                            LanguageSettingItemPropertyDrawer.DrawContent(item, true, false, i.ToString("D" + Items.arraySize.ToString().Length));
                            if (GUILayout.Button(Name[9], GUILayout.MaxWidth(75)))
                            {
                                Cover(item, setting.items[i]);
                            }
                            EditorGUILayout.Space();
                        }
                        EditorGUILayout.Space();
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
        bool SameTypeFromDifferentSetting(params SerializedProperty[] items)
        {
            if (items.Length <= 1)
                return false;
            ObjectType temp = (ObjectType)items[0].FindPropertyRelative("type").enumValueIndex;
            for (int j = 1; j < items.Length; j++)
            {
                if ((ObjectType)items[j].FindPropertyRelative("type").enumValueIndex != temp)
                    return false;
            }
            return true;
        }
        bool SameTypeFromDifferentSetting(params LanguageSettingItem[] items)
        {
            if (items.Length <= 1)
                return false;
            ObjectType? temp = items[0]?.type;
            for (int j = 1; j < items.Length; j++)
            {
                if (items[j].type != temp)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 清空
        /// </summary>
        void Clear(bool all)
        {
            for (int i = 0; i < Items.arraySize; i++)
            {
                SerializedProperty item = Items.GetArrayElementAtIndex(i);
                if (all)
                {
                    item.FindPropertyRelative("type").enumValueIndex = 0;
                    item.FindPropertyRelative("mutiLineString").boolValue = false;
                    item.FindPropertyRelative("str").stringValue = "";
                    item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                    item.FindPropertyRelative("sprite").objectReferenceValue = null;
                    item.FindPropertyRelative("texture").objectReferenceValue = null;
                    item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                }
                else
                {
                    switch ((ObjectType)item.FindPropertyRelative("type").enumValueIndex)
                    {
                        case ObjectType.String:
                            //item.FindPropertyRelative("mutiLineString").boolValue = false;
                            //item.FindPropertyRelative("str").stringValue = "";
                            item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                            item.FindPropertyRelative("sprite").objectReferenceValue = null;
                            item.FindPropertyRelative("texture").objectReferenceValue = null;
                            item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                            break;
                        case ObjectType.AudioClip:
                            //item.FindPropertyRelative("mutiLineString").boolValue = false;
                            item.FindPropertyRelative("str").stringValue = "";
                            //item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                            item.FindPropertyRelative("sprite").objectReferenceValue = null;
                            item.FindPropertyRelative("texture").objectReferenceValue = null;
                            item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                            break;
                        case ObjectType.Sprite:
                            item.FindPropertyRelative("mutiLineString").boolValue = false;
                            item.FindPropertyRelative("str").stringValue = "";
                            item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                            //item.FindPropertyRelative("sprite").objectReferenceValue = null;
                            item.FindPropertyRelative("texture").objectReferenceValue = null;
                            item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                            break;
                        case ObjectType.Texture:
                            item.FindPropertyRelative("mutiLineString").boolValue = false;
                            item.FindPropertyRelative("str").stringValue = "";
                            item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                            item.FindPropertyRelative("sprite").objectReferenceValue = null;
                            //item.FindPropertyRelative("texture").objectReferenceValue = null;
                            item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                            break;
                        case ObjectType.OtherObject:
                            item.FindPropertyRelative("mutiLineString").boolValue = false;
                            item.FindPropertyRelative("str").stringValue = "";
                            item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                            item.FindPropertyRelative("sprite").objectReferenceValue = null;
                            item.FindPropertyRelative("texture").objectReferenceValue = null;
                            //item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="setting"></param>
        void CopyAll(LanguageSetting setting)
        {
            if (setting)
            {
                Items.arraySize = setting.items.Length;
                for (int i = 0; i < Items.arraySize; i++)
                {
                    SerializedProperty item = Items.GetArrayElementAtIndex(i);
                    item.FindPropertyRelative("type").enumValueIndex = (int)setting.items[i].type;
                    item.FindPropertyRelative("mutiLineString").boolValue = setting.items[i].mutiLineString;
                    item.FindPropertyRelative("str").stringValue = setting.items[i].str;
                    item.FindPropertyRelative("audioClip").objectReferenceValue = setting.items[i].audioClip;
                    item.FindPropertyRelative("sprite").objectReferenceValue = setting.items[i].sprite;
                    item.FindPropertyRelative("texture").objectReferenceValue = setting.items[i].texture;
                    item.FindPropertyRelative("otherObject").objectReferenceValue = setting.items[i].otherObject;
                }
            }
        }
        /// <summary>
        /// 覆盖空项
        /// </summary>
        /// <param name="setting"></param>
        void ReplaceNull(LanguageSetting setting)
        {
            if (setting)
            {
                Items.arraySize = setting.items.Length;
                for (int i = 0; i < Items.arraySize; i++)
                {
                    SerializedProperty item = Items.GetArrayElementAtIndex(i);
                    item.FindPropertyRelative("type").enumValueIndex = (int)setting.items[i].type;
                    item.FindPropertyRelative("mutiLineString").boolValue = setting.items[i].mutiLineString;
                    if (string.IsNullOrWhiteSpace(item.FindPropertyRelative("str").stringValue))
                        item.FindPropertyRelative("str").stringValue = setting.items[i].str;
                    if (!item.FindPropertyRelative("audioClip").objectReferenceValue)
                        item.FindPropertyRelative("audioClip").objectReferenceValue = setting.items[i].audioClip;
                    if (!item.FindPropertyRelative("sprite").objectReferenceValue)
                        item.FindPropertyRelative("sprite").objectReferenceValue = setting.items[i].sprite;
                    if (!item.FindPropertyRelative("texture").objectReferenceValue)
                        item.FindPropertyRelative("texture").objectReferenceValue = setting.items[i].texture;
                    if (!item.FindPropertyRelative("otherObject").objectReferenceValue)
                        item.FindPropertyRelative("otherObject").objectReferenceValue = setting.items[i].otherObject;
                }
            }
        }
        /// <summary>
        /// 覆盖
        /// </summary>
        /// <param name="item"></param>
        /// <param name="setting"></param>
        void Cover(SerializedProperty item, LanguageSettingItem setting)
        {
            item.FindPropertyRelative("type").enumValueIndex = (int)setting.type;
            item.FindPropertyRelative("mutiLineString").boolValue = setting.mutiLineString;
            item.FindPropertyRelative("str").stringValue = setting.str;
            item.FindPropertyRelative("audioClip").objectReferenceValue = setting.audioClip;
            item.FindPropertyRelative("sprite").objectReferenceValue = setting.sprite;
            item.FindPropertyRelative("texture").objectReferenceValue = setting.texture;
            item.FindPropertyRelative("otherObject").objectReferenceValue = setting.otherObject;
        }
    }
    [CustomPropertyDrawer(typeof(LanguageSettingItem))]
    public class LanguageSettingItemPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// 行高
        /// </summary>
        private static float LineHeight { get { return EditorGUIUtility.singleLineHeight; } }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + LineHeight * (CheckStringLine(property) - 1);
        }
        public static int CheckStringLine(SerializedProperty property)
        {
            if ((ObjectType)property.FindPropertyRelative("type").enumValueIndex == ObjectType.String)
            {
                int line = 1;
                bool mutiLine = property.FindPropertyRelative("mutiLineString").boolValue;
                if (mutiLine)
                {
                    string str = property.FindPropertyRelative("str").stringValue;
                    line = (str.Length - str.Replace("\n", "").Length + 1).Clamp(1);
                }
                return line + 1;
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
        public static void DrawContent(Rect rect, SerializedProperty property, bool canChangeType)
        {
            float width = canChangeType ? (rect.width / 3f) : 0;
            Rect typeRect = new Rect(rect.x, rect.y, width, rect.height);
            Rect valueRect;
            ObjectType type = (ObjectType)property.FindPropertyRelative("type").enumValueIndex;
            if (canChangeType)
            {
                type = (ObjectType)EditorGUI.EnumPopup(typeRect, type, EditorStyles.popup);
                property.FindPropertyRelative("type").enumValueIndex = (int)type;
            }
            switch (type)
            {
                case ObjectType.String:
                    bool mutiLine = EditorGUI.Toggle(new Rect(typeRect.x + typeRect.width + 20, typeRect.y, width, LineHeight), property.FindPropertyRelative("mutiLineString").boolValue);
                    property.FindPropertyRelative("mutiLineString").boolValue = mutiLine;
                    valueRect = new Rect(rect.x, rect.y + LineHeight, rect.width, rect.height - LineHeight);
                    if (mutiLine)
                        property.FindPropertyRelative("str").stringValue = EditorGUI.TextArea(valueRect, property.FindPropertyRelative("str").stringValue, EditorStyles.textArea);
                    else
                        property.FindPropertyRelative("str").stringValue = EditorGUI.TextField(valueRect, property.FindPropertyRelative("str").stringValue, EditorStyles.textField);
                    break;
                default:
                    if (canChangeType)
                        valueRect = new Rect(typeRect.x + typeRect.width + 20, rect.y, rect.width - width - 20, LineHeight);
                    else
                        valueRect = new Rect(typeRect.x, rect.y, rect.width, LineHeight);
                    switch (type)
                    {
                        case ObjectType.AudioClip:
                            EditorGUI.ObjectField(valueRect, property.FindPropertyRelative("audioClip"), typeof(AudioClip), GUIContent.none);
                            break;
                        case ObjectType.Sprite:
                            EditorGUI.ObjectField(valueRect, property.FindPropertyRelative("sprite"), typeof(Sprite), GUIContent.none);
                            break;
                        case ObjectType.Texture:
                            EditorGUI.ObjectField(valueRect, property.FindPropertyRelative("texture"), typeof(Texture), GUIContent.none);
                            break;
                        case ObjectType.OtherObject:
                            EditorGUI.ObjectField(valueRect, property.FindPropertyRelative("otherObject"), GUIContent.none);
                            break;
                    }
                    break;
            }
        }
        public static void DrawContent(SerializedProperty property, bool showType, bool canChangeType, string label)
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 2;
            ObjectType type = (ObjectType)property.FindPropertyRelative("type").enumValueIndex;
            if (showType)
                type = (ObjectType)EditorGUILayout.EnumPopup(label, type, EditorStyles.popup);
            else if (!string.IsNullOrWhiteSpace(label))
                EditorGUILayout.LabelField(label);
            if (canChangeType)
                property.FindPropertyRelative("type").enumValueIndex = (int)type;

            EditorGUI.indentLevel += 3;
            switch (type)
            {
                case ObjectType.String:
                    if (showType)
                        property.FindPropertyRelative("mutiLineString").boolValue = EditorGUILayout.Toggle(property.FindPropertyRelative("mutiLineString").boolValue);
                    bool mutiLine = property.FindPropertyRelative("mutiLineString").boolValue;
                    if (mutiLine)
                        property.FindPropertyRelative("str").stringValue = EditorGUILayout.TextArea(property.FindPropertyRelative("str").stringValue, EditorStyles.textArea, GUILayout.ExpandHeight(true));
                    else
                        property.FindPropertyRelative("str").stringValue = EditorGUILayout.TextField(property.FindPropertyRelative("str").stringValue, EditorStyles.textField);
                    break;
                default:
                    switch (type)
                    {
                        case ObjectType.AudioClip:
                            EditorGUILayout.ObjectField(property.FindPropertyRelative("audioClip"), typeof(AudioClip), new GUIContent(""));
                            break;
                        case ObjectType.Sprite:
                            EditorGUILayout.ObjectField(property.FindPropertyRelative("sprite"), typeof(Sprite), new GUIContent(""));
                            break;
                        case ObjectType.Texture:
                            EditorGUILayout.ObjectField(property.FindPropertyRelative("texture"), typeof(Texture), new GUIContent(""));
                            break;
                        case ObjectType.OtherObject:
                            EditorGUILayout.ObjectField(property.FindPropertyRelative("otherObject"), new GUIContent(""));
                            break;
                    }
                    break;
            }
            EditorGUI.indentLevel = indent;
        }
        public static void DrawContent(LanguageSettingItem property, bool showType, string label)
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 2;
            if (showType)
                EditorGUILayout.EnumPopup(label, property.type, EditorStyles.popup);
            else if (!string.IsNullOrWhiteSpace(label))
                EditorGUILayout.LabelField(label);

            EditorGUI.indentLevel += 3;
            switch (property.type)
            {
                case ObjectType.String:
                    if (property.mutiLineString)
                        property.str = EditorGUILayout.TextArea(property.str, EditorStyles.textArea, GUILayout.ExpandHeight(true));
                    else
                        property.str = EditorGUILayout.TextField(property.str, EditorStyles.textField);
                    break;
                default:
                    switch (property.type)
                    {
                        case ObjectType.AudioClip:
                            EditorGUILayout.ObjectField(property.audioClip, typeof(AudioClip), false);
                            break;
                        case ObjectType.Sprite:
                            EditorGUILayout.ObjectField(property.sprite, typeof(Sprite), false);
                            break;
                        case ObjectType.Texture:
                            EditorGUILayout.ObjectField(property.texture, typeof(Texture), false);
                            break;
                        case ObjectType.OtherObject:
                            EditorGUILayout.ObjectField(property.otherObject, typeof(Object), false);
                            break;
                    }
                    break;
            }
            EditorGUI.indentLevel = indent;
        }
    }
    public static partial class Extension
    {
        public static string GetText(this LanguageSetting setting, int index, string exception)
        {
            try
            {
                return setting.items[index].type == ObjectType.String ? setting.items[index].str : null;
            }
            catch
            {
                return exception;
            }
        }
        public static LanguageSetting Find(this LanguageSetting[] array, GamesLanguage language)
        {
            if (array == null || array.Length < 1)
                return null;

            foreach (LanguageSetting item in array)
            {
                if (item.language == language)
                {
                    return item;
                }
            }
            return null;
        }
        public static LanguageSetting Find(this List<LanguageSetting> list, GamesLanguage language)
        {
            if (list == null || list.Count < 1)
                return null;

            foreach (LanguageSetting item in list)
            {
                if (item.language == language)
                {
                    return item;
                }
            }
            return null;
        }
    }
#endif
}
