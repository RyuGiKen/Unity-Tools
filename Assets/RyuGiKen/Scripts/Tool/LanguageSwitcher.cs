using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 多语言切换
    /// </summary>
    public class LanguageSwitcher : MonoBehaviour
    {
        public GamesLanguage language;
        public bool StartSet = true;
        public bool Seted = false;
        public LanguageSetting[] LanguageSettings;
        public LanguageSetting UseSetting;
        public Component[] components;
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
        public LanguageSetting GetSetting(GamesLanguage language)
        {
            return LanguageSettings.Find(language);
        }
        public virtual void UpdateLanguage()
        {
            if (language == GamesLanguage.Auto)
            {
                language = SystemLanguageToGamesLanguage(Application.systemLanguage);
            }
            UpdateLanguage(language);
        }
        public void UpdateLanguage(GamesLanguage language)
        {
            UseSetting = GetSetting(language);
            UpdateLanguage(UseSetting);
        }
        public void UpdateLanguage(LanguageSetting objects)
        {
            if (objects == null)
            {
                Debug.Log("找不到" + language.ToString() + "的配置");
                return;
            }
            for (int i = 0; i < components.Length; i++)
            {
                if (i < components.Length && components[i] && i < objects.items.Length)
                {
                    if (components[i].TryGetComponent(out Text text))
                        text.text = objects.items[i];
                    else if (components[i].TryGetComponent(out InputField input))
                        input.text = objects.items[i];
                    else if (components[i].TryGetComponent(out AudioSource audio))
                        audio.clip = objects.items[i];
                    else if (components[i].TryGetComponent(out Image image))
                        image.sprite = objects.items[i];
                    else if (components[i].TryGetComponent(out RawImage rawImage))
                        rawImage.texture = objects.items[i];
                    else if (components[i].TryGetComponent(out SpriteRenderer spriteRenderer))
                        spriteRenderer.sprite = objects.items[i];
                    else if (components[i].TryGetComponent(out MeshRenderer meshRenderer))
                        meshRenderer.material.mainTexture = objects.items[i];
                    else if (components[i].TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
                        skinnedMeshRenderer.material.mainTexture = objects.items[i];
                }
            }
            Seted = true;
        }
    }
}
