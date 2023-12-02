using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RyuGiKen.Tools;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 音乐随机播放
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/音乐随机播放")]
    [RequireComponent(typeof(AudioSource))]
    public class MusicRandomPlay : MonoBehaviour
    {
        public enum PlayType
        {
            AutoRandomPlay,
            StartRandomPlayOnce,
            StartRandomLoopPlay,
            SecondRandomPlay
        }
        public PlayType playType;
        public AudioSource audioSource;
        public AudioClip[] otherClip;
        [Tooltip("切换键")] public KeyCode switchKey = KeyCode.PageDown;
        void Awake()
        {
            Reset();
        }
        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource)
                audioSource.loop = false;
            otherClip = otherClip.ClearNullItem();
        }
        private void OnEnable()
        {
            switch (playType)
            {
                case PlayType.AutoRandomPlay:
                case PlayType.StartRandomPlayOnce:
                case PlayType.StartRandomLoopPlay:
                    SwitchAndPlay();
                    break;
                case PlayType.SecondRandomPlay:
                    if (audioSource && audioSource.clip)
                        audioSource.Play();
                    else
                        SwitchAndPlay();
                    break;
            }
        }
        void Update()
        {
            if (audioSource)
            {
                if (Input.GetKeyDown(switchKey))
                {
                    SwitchAndPlay();
                    return;
                }
                if (!audioSource.isPlaying)
                {
                    switch (playType)
                    {
                        case PlayType.AutoRandomPlay:
                        case PlayType.SecondRandomPlay:
                            SwitchAndPlay();
                            break;
                        case PlayType.StartRandomPlayOnce:
                            break;
                        case PlayType.StartRandomLoopPlay:
                            audioSource?.Play();
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 切换音乐
        /// </summary>
        public void Switch()
        {
            if (otherClip.Length > 0)
            {
                audioSource.clip = otherClip.Remove(audioSource.clip).GetRandomItem(true);
            }
        }
        /// <summary>
        /// 切换音乐并播放
        /// </summary>
        [ContextMenu("切换音乐")]
        public void SwitchAndPlay()
        {
            Switch();
            audioSource?.Play();
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        [ContextMenu("停止播放")]
        public void StopPlay()
        {
            audioSource?.Stop();
        }
    }
}
namespace RyuGiKenEditor.Tools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(MusicRandomPlay), true)]
    [CanEditMultipleObjects]
    public class MusicRandomPlayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[2];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "切换音乐并播放";
                    ButtonName[1] = "停止播放";
                    break;
                default:
                    ButtonName[0] = "Switch And Play";
                    ButtonName[1] = "Stop";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as MusicRandomPlay).SwitchAndPlay();
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                (target as MusicRandomPlay).StopPlay();
            }
        }
    }
#endif
}
