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
        AudioSource audioSource;
        public AudioClip[] otherClip;
        public bool canPlay = true;
        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        void Update()
        {
            if (audioSource != null && canPlay)
                if (otherClip.Length > 0 && (!audioSource.isPlaying || Input.GetKeyUp(KeyCode.PageDown) || Input.GetKeyUp(KeyCode.PageUp)))//切换音乐
                {
                    int randomNum = Random.Range(0, otherClip.Length - 1);
                    if (Input.GetKeyUp(KeyCode.PageUp))
                        randomNum--;
                    if (Input.GetKeyUp(KeyCode.PageDown))
                        randomNum++;
                    if (randomNum < 0)
                        randomNum = otherClip.Length - 1;
                    else if (randomNum >= otherClip.Length)
                        randomNum = 0;
                    audioSource.clip = otherClip[randomNum]; audioSource.Play();
                }
        }
    }
}
