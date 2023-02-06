using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Localization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace RyuGiKen.Localization
{
    /// <summary>
    /// 多语言切换
    /// </summary>
    public class LocalizationSwitcher : MonoBehaviour
    {
        public GamesLanguage language;
        public bool StartSet = true;
        public bool Seted = false;
        public LocalizationConfigurationBase configuration;
        public bool SetColorClearWhenNoSprite = true;
        //[HideInInspector]
        public Component[] components;
        [HideInInspector] public bool showComponentGroup = true;
        [HideInInspector][Range(0, 2)] public int hideBlank = 2;
        protected void Awake()
        {
            Seted = false;
        }
        protected void Start()
        {
            if (StartSet)
                UpdateLanguage();
        }
        protected void LateUpdate()
        {
            if (!Seted)
                UpdateLanguage();
        }
        /// <summary>
        /// 系统语言转游戏语言
        /// </summary>
        /// <param name="systemLanguage"></param>
        /// <returns></returns>
        public static GamesLanguage SystemLanguageToGamesLanguage(SystemLanguage systemLanguage)
        {
            GamesLanguage result;
            switch (systemLanguage)
            {
                default:
                case SystemLanguage.English:
                    result = GamesLanguage.English;
                    break;
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    result = GamesLanguage.Chinese;
                    break;
            }
            return result;
        }
        public virtual void UpdateLanguage()
        {
            if (language == GamesLanguage.Auto)
            {
                language = SystemLanguageToGamesLanguage(Application.systemLanguage);
            }
            UpdateLanguage(configuration, language);
        }
        public void UpdateLanguage(GamesLanguage language)
        {
            this.language = language;
            UpdateLanguage(configuration, language);
        }
        public void UpdateLanguage(LocalizationConfigurationBase configuration, GamesLanguage language)
        {
            if (configuration == null)
            {
                Debug.Log("无配置文件");
                return;
            }
            for (int i = 0; i < components.Length; i++)
            {
                if (i < components.Length && components[i])
                {
                    if (configuration is LocalizationConfiguration)
                    {
                        if (i < (configuration as LocalizationConfiguration).configurations.items.Count)
                        {
                            Localization objects = (configuration as LocalizationConfiguration).configurations.items[i];

                            if (components[i].TryGetComponent(out Text text))
                                text.text = objects.Find(language);
                            else if (components[i].TryGetComponent(out InputField input))
                                input.text = objects.Find(language);
                            else if (components[i].TryGetComponent(out AudioSource audio))
                                audio.clip = objects.Find(language);
                            else if (components[i].TryGetComponent(out Image image))
                            {
                                Sprite sprite = objects.Find(language);
                                image.sprite = sprite;
                                if (SetColorClearWhenNoSprite && !sprite)
                                    image.color = Color.clear;
                            }
                            else if (components[i].TryGetComponent(out RawImage rawImage))
                                rawImage.texture = objects.Find(language);
                            else if (components[i].TryGetComponent(out SpriteRenderer spriteRenderer))
                            {
                                Sprite sprite = objects.Find(language);
                                spriteRenderer.sprite = sprite;
                                if (SetColorClearWhenNoSprite && !sprite)
                                    spriteRenderer.color = Color.clear;
                            }
                            else if (components[i].TryGetComponent(out MeshRenderer meshRenderer))
                                meshRenderer.material.mainTexture = objects.Find(language);
                            else if (components[i].TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
                                skinnedMeshRenderer.material.mainTexture = objects.Find(language);
                        }
                    }
                    else if (configuration is LocalizationStringConfiguration)
                    {
                        if (i < (configuration as LocalizationStringConfiguration).configurations.items.Count)
                        {
                            LocalizationString objects2 = (configuration as LocalizationStringConfiguration).configurations.items[i];

                            if (components[i].TryGetComponent(out Text text))
                                text.text = objects2.Find(language);
                            else if (components[i].TryGetComponent(out InputField input))
                                input.text = objects2.Find(language);
                        }
                    }
                }
            }
            Seted = true;
        }
        public LocalizationItem TryGetLocalization(int index)
        {
            return Extension.TryGetLocalization(this, index);
        }
        public LocalizationItem TryGetLocalization(GamesLanguage language, int index)
        {
            return Extension.TryGetLocalization(this, language, index);
        }
        public string TryGetLocalizationString(int index, string exception)
        {
            return Extension.TryGetLocalizationString(this, index, exception);
        }
        public string TryGetLocalizationString(GamesLanguage language, int index, string exception)
        {
            return Extension.TryGetLocalizationString(this, language, index, exception);
        }
        public string TryGetLocalizationStringFormat(int index, string exception, params object[] args)
        {
            return Extension.TryGetLocalizationStringFormat(this, index, exception, args);
        }
        public string TryGetLocalizationStringFormat(GamesLanguage language, int index, string exception, params object[] args)
        {
            return Extension.TryGetLocalizationStringFormat(this, language, index, exception, args);
        }
    }
    public static partial class Extension
    {
        public static int GetItemCount(this LocalizationConfigurationBase configuration)
        {
            int result = 0;
            if (configuration is null) { }
            else if (configuration is LocalizationConfiguration)
            {
                try
                {
                    result = (configuration as LocalizationConfiguration).configurations.items.Count;
                }
                catch { }
            }
            else if (configuration is LocalizationStringConfiguration)
            {
                try
                {
                    result = (configuration as LocalizationStringConfiguration).configurations.items.Count;
                }
                catch { }
            }
            return result;
        }
        public static LocalizationItem TryGetLocalization(this LocalizationSwitcher switcher, int index)
        {
            if (!switcher || switcher.configuration == null || switcher.language == GamesLanguage.Auto || index < 0)
                return null;
            if (switcher.configuration is LocalizationConfiguration)
            {
                return (switcher.configuration as LocalizationConfiguration).GetLocalization(switcher.language, index);
            }
            return null;
        }
        public static LocalizationItem TryGetLocalization(this LocalizationSwitcher switcher, GamesLanguage language, int index)
        {
            if (!switcher || switcher.configuration == null || language == GamesLanguage.Auto || index < 0)
                return null;
            if (switcher.configuration is LocalizationConfiguration)
            {
                return (switcher.configuration as LocalizationConfiguration).GetLocalization(language, index);
            }
            return null;
        }
        public static string TryGetLocalizationString(this LocalizationSwitcher switcher, int index, string exception)
        {
            if (!switcher || switcher.configuration == null || switcher.language == GamesLanguage.Auto || index < 0)
                return exception;
            return switcher.configuration.TryGetLocalizationString(switcher.language, index, exception);
        }
        public static string TryGetLocalizationString(this LocalizationSwitcher switcher, GamesLanguage language, int index, string exception)
        {
            if (!switcher || switcher.configuration == null || language == GamesLanguage.Auto || index < 0)
                return exception;
            return switcher.configuration.TryGetLocalizationString(language, index, exception);
        }
        public static string TryGetLocalizationStringFormat(this LocalizationSwitcher switcher, int index, string exception, params object[] args)
        {
            if (!switcher || switcher.configuration == null || switcher.language == GamesLanguage.Auto || index < 0 || args == null || args.Length < 1)
                return exception;
            return switcher.configuration.TryGetLocalizationStringFormat(switcher.language, index, exception, args);
        }
        public static string TryGetLocalizationStringFormat(this LocalizationSwitcher switcher, GamesLanguage language, int index, string exception, params object[] args)
        {
            if (!switcher || switcher.configuration == null || language == GamesLanguage.Auto || index < 0 || args == null || args.Length < 1)
                return exception;
            return switcher.configuration.TryGetLocalizationStringFormat(language, index, exception, args);
        }
    }
}
namespace RyuGiKenEditor.Localization
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LocalizationSwitcher), true)]
    public class LocalizationSwitcherEditor : Editor
    {
        protected SerializedProperty components;
        protected SerializedProperty showComponentGroup;
        protected SerializedProperty hideBlank;
        private void OnEnable()
        {
            components = serializedObject.FindProperty("components");
            showComponentGroup = serializedObject.FindProperty("showComponentGroup");
            hideBlank = serializedObject.FindProperty("hideBlank");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            string[] Name = new string[3];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Name[0] = "显示翻译";
                    Name[1] = "隐藏空值";
                    Name[2] = "未设置";
                    break;
                default:
                    Name[0] = "Show Localization";
                    Name[1] = "Hide Blank";
                    Name[2] = "Not Seted";
                    break;
            }
            showComponentGroup.boolValue = EditorGUILayout.Foldout(showComponentGroup.boolValue, Name[0]);
            if (showComponentGroup.boolValue)
            {
                LocalizationConfigurationBase configuration = (target as LocalizationSwitcher).configuration;
                int MaxCount = configuration.GetItemCount();

                GamesLanguage language = (target as LocalizationSwitcher).language;
                if (language == GamesLanguage.Auto)
                    language = LocalizationSwitcher.SystemLanguageToGamesLanguage(Application.systemLanguage);

                hideBlank.intValue = EditorGUILayout.IntSlider(Name[1], hideBlank.intValue, 0, 2);
                int indent = EditorGUI.indentLevel;
                for (int i = 0; i < components.arraySize; i++)
                {
                    if (i < Mathf.Min(components.arraySize, MaxCount))
                    {
                        Object component = components.GetArrayElementAtIndex(i).objectReferenceValue;
                        if (hideBlank.intValue == 2 && !component)
                            continue;
                        EditorGUILayout.ToggleLeft(i.ToString() + (component ? "" : ("（" + Name[2] + "）")), component);
                        if (hideBlank.intValue == 1 && !component)
                            continue;
                        EditorGUI.indentLevel += 3;
                        components.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField(component, typeof(Component), true);

                        if (configuration is LocalizationConfiguration)
                        {
                            LocalizationConfiguration temp = configuration as LocalizationConfiguration;
                            LocalizationItem item = temp.configurations.GetItem(i, (int)(language - 1));
                            ObjectType type = item.type;

                            switch (type)
                            {
                                case ObjectType.String:
                                    EditorGUILayout.DelayedTextField(item.str);
                                    break;
                                case ObjectType.StringMultiLine:
                                    EditorGUILayout.TextArea(item.str);
                                    break;
                                case ObjectType.AudioClip:
                                    EditorGUILayout.ObjectField(item.audioClip, typeof(AudioClip), false);
                                    break;
                                case ObjectType.Sprite:
                                    EditorGUILayout.ObjectField(item.sprite, typeof(Sprite), false);
                                    break;
                                case ObjectType.Texture:
                                    EditorGUILayout.ObjectField(item.texture, typeof(Texture), false);
                                    break;
                                case ObjectType.OtherObject:
                                    EditorGUILayout.ObjectField(item.otherObject, typeof(Object), false);
                                    break;
                            }
                        }
                        else if (configuration is LocalizationStringConfiguration)
                        {
                            LocalizationStringConfiguration temp = configuration as LocalizationStringConfiguration;
                            LocalizationStringItem item = temp.configurations.GetItem(i, (int)(language - 1));
                            ObjectType type = item.multiLineString ? ObjectType.StringMultiLine : ObjectType.String;
                            switch (type)
                            {
                                case ObjectType.String:
                                    EditorGUILayout.DelayedTextField(item.str);
                                    break;
                                case ObjectType.StringMultiLine:
                                    EditorGUILayout.TextArea(item.str);
                                    break;
                            }
                        }
                    }
                    EditorGUI.indentLevel = indent;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
