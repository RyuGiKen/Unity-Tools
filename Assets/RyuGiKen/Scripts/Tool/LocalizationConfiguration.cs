using System.Collections;
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
    /// 复合多语言配置
    /// </summary>
    [CreateAssetMenu(fileName = "Localization Configuration", menuName = "Localization Configuration")]
    [System.Serializable]
    public class LocalizationConfiguration : LocalizationConfigurationBase
    {
        public MultiArrayLocalization configurations;
        //public LanguageSetting[] setting = new LanguageSetting[3];
    }
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
        StringMultiLine,
    }
    [System.Serializable]
    public class MultiArrayLocalization : MultiArrayExtension<LocalizationItem>
    {
        public new List<Localization> items;
        public MultiArrayLocalization(LocalizationItem[][] array)
        {
            this.items = new List<Localization>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new Localization(array[i]));
        }
        public MultiArrayLocalization(ReorderableList<LocalizationItem>[] array)
        {
            this.items = new List<Localization>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new Localization(array[i].ToArray()));
        }
        public MultiArrayLocalization(Localization[] array)
        {
            this.items = new List<Localization>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayLocalization(List<ReorderableList<LocalizationItem>> list)
        {
            this.items = new List<Localization>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new Localization(list[i].ToArray()));
        }
        public MultiArrayLocalization(List<Localization> list) { this.items = list; }
        public override LocalizationItem GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override LocalizationItem GetItem(int index1, int index2)
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
    /// <summary>
    /// 复合多语言参数
    /// </summary>
    [System.Serializable]
    public class Localization : ReorderableList<LocalizationItem>
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public ObjectType type;
        /// <summary>
        /// 设置类型
        /// </summary>
        /// <param name="type"></param>
        public void SetType(ObjectType type)
        {
            this.type = type;
            foreach (LocalizationItem item in items)
            {
                if (item != null)
                    item.type = type;
            }
        }
        public Localization() { this.items = new List<LocalizationItem>(); }
        public Localization(LocalizationItem[] array) { this.items = array != null ? array.ToList() : new List<LocalizationItem>(); }
        public Localization(LocalizationStringItem[] array)
        {
            this.items = new List<LocalizationItem>();
            if (array?.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    items.Add(array[i].ToLocalizationItem());
                }
            }
        }
        public Localization(List<LocalizationItem> list) { this.items = list; }
        public static implicit operator Localization(LocalizationString value)
        {
            Localization result = null;
            if (value != null)
            {
                result = new Localization(value.items.ToArray());
            }
            return result;
        }
        public static implicit operator LocalizationString(Localization value)
        {
            LocalizationString result = null;
            if (value != null)
            {
                result = new LocalizationString();
                for (int i = 0; i < value.items.Count; i++)
                {
                    result.items.Add(value.items[i].ToLocalizationItem());
                }
            }
            return result;
        }
    }
    /// <summary>
    /// 复合多语言参数项
    /// </summary>
    [System.Serializable]
    public class LocalizationItem : LocalizationStringItem
    {
        //private new bool multiLineString;
        //public GamesLanguage language;
        /// <summary>
        /// 对象类型
        /// </summary>
        public ObjectType type;
        //public string str;
        public AudioClip audioClip;
        public Sprite sprite;
        public Texture texture;
        public Object otherObject;

        public static implicit operator string(LocalizationItem value) { return (value != null && (value.type == ObjectType.String || value.type == ObjectType.StringMultiLine)) ? value.str : null; }
        public static implicit operator AudioClip(LocalizationItem value) { return (value != null && value.type == ObjectType.AudioClip) ? value.audioClip : null; }
        public static implicit operator Sprite(LocalizationItem value) { return (value != null && value.type == ObjectType.Sprite) ? value.sprite : null; }
        public static implicit operator Texture(LocalizationItem value) { return (value != null && value.type == ObjectType.Texture) ? value.texture : null; }
        public static implicit operator Object(LocalizationItem value) { return (value != null && value.type == ObjectType.OtherObject) ? value.otherObject : null; }
    }
    public static partial class Extension
    {
        public static LocalizationItem ToLocalizationItem(this LocalizationStringItem value)
        {
            LocalizationItem result = null;
            if (value is LocalizationItem)
                return (LocalizationItem)value;
            if (value != null)
            {
                result = new LocalizationItem();
                result.type = value?.multiLineString == true ? ObjectType.StringMultiLine : ObjectType.String;
                result.str = value?.str;
            }
            return result;
        }
        public static LocalizationStringItem ToLocalizationStringItem(this LocalizationItem value)
        {
            LocalizationStringItem result = null;
            if (value != null)
            {
                result = new LocalizationStringItem();
                result.multiLineString = value?.type == ObjectType.StringMultiLine;
                result.str = value?.str;
            }
            return result;
        }
        public static LocalizationItem GetLocalization(this LocalizationConfiguration configuration, GamesLanguage language, int index)
        {
            if (configuration == null || configuration.configurations == null || configuration.configurations.items == null || OutIndex(index, configuration.configurations.items.Count))
                return null;
            return GetLocalization(configuration.configurations, language, index);
        }
        public static LocalizationItem GetLocalization(this MultiArrayLocalization configurations, GamesLanguage language, int index)
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
        public static string GetLocalizationString(this LocalizationConfiguration configuration, GamesLanguage language, int index)
        {
            if (configuration == null)
                return null;
            return configuration.configurations.GetLocalizationString(language, index);
        }
        public static string GetLocalizationString(this MultiArrayLocalization array, GamesLanguage language, int index)
        {
            LocalizationItem item = array.GetLocalization(language, index);
            if (item == null || (item.type != ObjectType.String && item.type != ObjectType.StringMultiLine))
                return null;
            else
                return item;
        }
        public static string TryGetLocalizationString(this LocalizationConfiguration configuration, GamesLanguage language, int index, string exception)
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
        public static string TryGetLocalizationString(this MultiArrayLocalization array, GamesLanguage language, int index, string exception)
        {
            if (array == null || language == GamesLanguage.Auto || index < 0)
                return exception;
            LocalizationItem item = array.GetLocalization(language, index);
            if (item == null || (item.type != ObjectType.String && item.type != ObjectType.StringMultiLine))
                return exception;
            else
                return item;
        }
        public static string TryGetLocalizationStringFormat(this LocalizationConfiguration configuration, GamesLanguage language, int index, string exception, params object[] args)
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
        public static string TryGetLocalizationStringFormat(this MultiArrayLocalization array, GamesLanguage language, int index, string exception, params object[] args)
        {
            if (array == null || language == GamesLanguage.Auto || index < 0 || args == null || args.Length < 1)
                return exception;
            string temp = array.TryGetLocalizationString(language, index, exception);
            return temp.StringFormat(args);
            //LocalizationItem item = array.GetLocalization(language, index);
            //return ((item == null || item.type != ObjectType.String) ? exception : item.str).StringFormat(args);
        }
        public static LocalizationItem Find(this LocalizationItem[] array, GamesLanguage language)
        {
            if (array == null || array.Length < 1)
                return null;

            foreach (LocalizationItem item in array)
            {
                if (item.language == language)
                {
                    return item;
                }
            }
            return null;
        }
        public static LocalizationItem Find(this List<LocalizationItem> list, GamesLanguage language)
        {
            if (list == null || list.Count < 1)
                return null;

            //return list.ToArray().Find(language);
            foreach (LocalizationItem item in list)
            {
                if (item.language == language)
                {
                    return item;
                }
            }
            return null;
        }
        public static LocalizationItem Find(this Localization localization, GamesLanguage language)
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
    [CustomPropertyDrawer(typeof(LocalizationItem))]
    public class LocalizationItemPropertyDrawer : LocalizationStringItemPropertyDrawer
    {
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
        public static new void DrawContent(Rect rect, SerializedProperty property, bool showType, float startX = float.NaN, bool? multiLine = null)
        {
            if (!float.IsNaN(startX))
            {
                rect.width += (rect.x - startX);
                rect.x = startX;
            }
            ObjectType type = (ObjectType)property.FindPropertyRelative("type").enumValueIndex;
            switch (type)
            {
                default:
                case ObjectType.String:
                    multiLine = false;
                    break;
                case ObjectType.StringMultiLine:
                    multiLine = true;
                    break;
            }
            float width = showType ? (rect.width / 4f).Clamp(60, 100) : (rect.width / 4f).Clamp(60);
            Rect typeRect = new Rect(rect.x, rect.y, showType ? width : 0, LineHeight);
            Rect languageRect = new Rect(rect.x + typeRect.width, rect.y, width, LineHeight);
            Rect multiLineRect = new Rect(languageRect.xMax, rect.y, (multiLine == null ? 20 : 0), LineHeight);
            Rect valueRect = new Rect(multiLineRect.xMax, rect.y, rect.width - width - (showType ? width : 0) - multiLineRect.width, rect.height);
            //if (multiLine == null)
            //    multiLine = property.FindPropertyRelative("multiLineString").boolValue = EditorGUI.Toggle(multiLineRect, property.FindPropertyRelative("multiLineString").boolValue);
            valueRect.height = multiLine == true ? rect.height : LineHeight;
            GamesLanguage language = (GamesLanguage)property.FindPropertyRelative("language").enumValueIndex;
            if (showType)
            {
                type = (ObjectType)EditorGUI.EnumPopup(typeRect, type, EditorStyles.popup);
                property.FindPropertyRelative("type").enumValueIndex = (int)type;
            }
            language = (GamesLanguage)EditorGUI.EnumPopup(languageRect, language, EditorStyles.popup);
            property.FindPropertyRelative("language").enumValueIndex = (int)language;
            switch (type)
            {
                case ObjectType.String:
                    property.FindPropertyRelative("str").stringValue = EditorGUI.DelayedTextField(valueRect, property.FindPropertyRelative("str").stringValue, EditorStyles.textField);
                    break;
                case ObjectType.StringMultiLine:
                    property.FindPropertyRelative("str").stringValue = EditorGUI.TextArea(valueRect, property.FindPropertyRelative("str").stringValue, EditorStyles.textArea);
                    break;
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
        }
    }
    [CustomPropertyDrawer(typeof(RyuGiKen.Localization.Localization), true)]
    public class LocalizationPropertyDrawer : LocalizationStringPropertyDrawer
    {
        //protected override ReorderableList CreateReorderableList(SerializedProperty property, SerializedProperty listProperty)
        //{
        //    return CreateReorderableList(property, listProperty, false);
        //}
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float width = (position.width / 3f).Clamp(80);
            Rect typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            ObjectType type = (ObjectType)property.FindPropertyRelative("type").enumValueIndex;
            type = (ObjectType)EditorGUI.EnumPopup(typeRect, new GUIContent("Type"), type, EditorStyles.popup);
            SetType(property, type);

            var list = GetReorderableList(property);
            var listProperty = property.FindPropertyRelative("items");
            var height = 0f;
            for (var i = 0; i < listProperty.arraySize; i++)
            {
                height = Mathf.Max(height, GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
            }
            list.elementHeight = height;
            position.y += EditorGUIUtility.singleLineHeight;
            list.DoList(position);
            //list.DoLayoutList();
        }
        protected override void DrawListItems(Rect rect, SerializedProperty property)
        {
            LocalizationItemPropertyDrawer.DrawContent(rect, property, false, float.NaN, property.FindPropertyRelative("type").enumValueIndex == (int)ObjectType.StringMultiLine);
            //EditorGUI.PropertyField(rect, property, true);
        }
        /// <summary>
        /// 设置类型
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        void SetType(SerializedProperty property, ObjectType type)
        {
            property.FindPropertyRelative("type").enumValueIndex = (int)type;
            for (int i = 0; i < property.FindPropertyRelative("items").arraySize; i++)
            {
                property.FindPropertyRelative("items").GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex = (int)type;
            }
        }
    }
    [CustomEditor(typeof(LocalizationConfiguration))]
    public class LocalizationConfigurationEditor : LocalizationConfigurationBaseEditor
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
            if (GUILayout.Button(Name[1]))
            {
                Clear(false);
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
            {
                SerializedProperty item = items.GetArrayElementAtIndex(i);
                if (!SupportMultiLine.boolValue && item.FindPropertyRelative("type").enumValueIndex == (int)ObjectType.StringMultiLine)
                    item.FindPropertyRelative("type").enumValueIndex = (int)ObjectType.String;
            }
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
        protected override void Clear(bool all)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                for (int j = 0; j < (Items[i].isArray ? Items[i].arraySize : 0); j++)
                {
                    SerializedProperty item = Items[i].GetArrayElementAtIndex(j);
                    if (all)
                    {
                        item.FindPropertyRelative("type").enumValueIndex = 0;
                        //item.FindPropertyRelative("multiLineString").boolValue = false;
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
                            case ObjectType.StringMultiLine:
                                //item.FindPropertyRelative("multiLineString").boolValue = false;
                                //item.FindPropertyRelative("str").stringValue = "";
                                item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                                item.FindPropertyRelative("sprite").objectReferenceValue = null;
                                item.FindPropertyRelative("texture").objectReferenceValue = null;
                                item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                                break;
                            case ObjectType.AudioClip:
                                //item.FindPropertyRelative("multiLineString").boolValue = false;
                                item.FindPropertyRelative("str").stringValue = "";
                                //item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                                item.FindPropertyRelative("sprite").objectReferenceValue = null;
                                item.FindPropertyRelative("texture").objectReferenceValue = null;
                                item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                                break;
                            case ObjectType.Sprite:
                                //item.FindPropertyRelative("multiLineString").boolValue = false;
                                item.FindPropertyRelative("str").stringValue = "";
                                item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                                //item.FindPropertyRelative("sprite").objectReferenceValue = null;
                                item.FindPropertyRelative("texture").objectReferenceValue = null;
                                item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                                break;
                            case ObjectType.Texture:
                                //item.FindPropertyRelative("multiLineString").boolValue = false;
                                item.FindPropertyRelative("str").stringValue = "";
                                item.FindPropertyRelative("audioClip").objectReferenceValue = null;
                                item.FindPropertyRelative("sprite").objectReferenceValue = null;
                                //item.FindPropertyRelative("texture").objectReferenceValue = null;
                                item.FindPropertyRelative("otherObject").objectReferenceValue = null;
                                break;
                            case ObjectType.OtherObject:
                                //item.FindPropertyRelative("multiLineString").boolValue = false;
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
        }
    }
#endif
}
