using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 抖动
    /// </summary>
    [AddComponentMenu("RyuGiKen/震动")]
    public class Shake : MonoBehaviour
    {
        [Tooltip("启动播放")] public bool playOnAwake;
        [Tooltip("初始位置")] [SerializeField] Vector3 originPos;
        [Tooltip("刷新目标位置")] [SerializeField] Vector3 tarPos;
        [Tooltip("刷新计时")] float mTime;
        [Tooltip("刷新时间")] public float maxTime;
        [Tooltip("刷新距离")] public float distance;
        [Tooltip("抖动中")] public bool playing;
        [Tooltip("归位中")] bool stopping = false;
        void Start()
        {
            originPos = this.transform.localPosition;
            playing = playOnAwake;
        }
        void Update()
        {
            if (maxTime < 0)
            { maxTime = -maxTime; }
            if (playing && maxTime > 0 && distance != 0)
            {
                mTime += Time.deltaTime;
                if (mTime >= maxTime && !stopping)
                {
                    tarPos = new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), Random.Range(-distance, distance));
                    mTime = 0;
                }
                if (stopping)
                {
                    if ((this.transform.localPosition - originPos).magnitude < 0.01f)
                    {
                        playing = false;
                        stopping = false;
                        this.transform.localPosition = originPos;
                    }
                    tarPos = originPos;
                }
                this.transform.localPosition += new Vector3(tarPos.x - this.transform.localPosition.x, tarPos.y - this.transform.localPosition.y, tarPos.z - this.transform.localPosition.z) * Time.deltaTime / maxTime;
            }
        }
        /// <summary>
        /// 开始抖动
        /// </summary>
        public void Play()
        {
            playing = true;
            mTime = 0;
        }
        /// <summary>
        /// 停止抖动、归位
        /// </summary>
        public void StopAndReturn()
        {
            if (playing)
                stopping = true;
            mTime = 0;
        }
        /// <summary>
        /// 停止抖动、不归位
        /// </summary>
        public void Stop()
        {
            playing = false;
            mTime = 0;
        }
        /// <summary>
        /// 停止抖动、立刻归位
        /// </summary>
        public void StopAndReturnImmediately()
        {
            playing = false;
            mTime = 0;
            this.transform.localPosition = originPos;
        }
        /// <summary>
        /// 抖动一定时间后停止、立刻归位
        /// </summary>
        /// <param name="time"></param>
        public void PlayAndStopReturnImmediatelyWait(float time)
        {
            playing = true;
            mTime = 0;
            StopAllCoroutines();
            StartCoroutine(WaitForStopAndReturnImmediately(time));
        }
        IEnumerator WaitForStopAndReturnImmediately(float time)
        {
            yield return new WaitForSeconds(time);
            StopAndReturnImmediately();
        }
        /// <summary>
        /// 抖动一定时间后停止、归位
        /// </summary>
        /// <param name="time"></param>
        public void PlayAndStopReturnWait(float time)
        {
            playing = true;
            mTime = 0;
            StopAllCoroutines();
            StartCoroutine(WaitForStopAndReturn(time));
        }
        IEnumerator WaitForStopAndReturn(float time)
        {
            yield return new WaitForSeconds(time);
            StopAndReturn();
        }
        /// <summary>
        /// 抖动一定时间后停止、不归位
        /// </summary>
        /// <param name="time"></param>
        public void PlayAndStopWait(float time)
        {
            playing = true;
            mTime = 0;
            StopAllCoroutines();
            StartCoroutine(WaitForStop(time));
        }
        IEnumerator WaitForStop(float time)
        {
            yield return new WaitForSeconds(time);
            Stop();
        }
    }
}
