using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        }
        private void OnEnable()
        {
            Switch();
        }
        void Update()
        {
            if (audioSource != null && (!audioSource.isPlaying || Input.GetKeyDown(switchKey)))//切换音乐
            {
                Switch();
            }
        }
        /// <summary>
        /// 切换音乐
        /// </summary>
        [ContextMenu("切换音乐")]
        public void Switch()
        {
            if (otherClip.Length > 0)
            {
                audioSource.clip = otherClip.GetRandomItem(true);
                audioSource.Play();
            }
        }
    }
}
