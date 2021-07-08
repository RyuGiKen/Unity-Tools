using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 抖动
    /// </summary>
    [AddComponentMenu("RyuGiKen/抖动")]
    public class Shake : MonoBehaviour
    {
        [Tooltip("启动播放")] public bool playOnAwake;
        [SerializeField] Space space = Space.Self;
        public bool RandomX = true;
        public bool RandomY = true;
        public bool RandomZ = true;
        [Tooltip("初始位置")] [SerializeField] Vector3 originPosition;
        [Tooltip("初始位置")] [SerializeField] Vector3 originLocalPosition;
        [Tooltip("刷新目标位置")] [SerializeField] Vector3 tarPos;
        [Tooltip("刷新计时")] float mTime;
        [Tooltip("刷新时间")] public float maxTime;
        [Tooltip("刷新距离")] public float distance;
        [Tooltip("到达距离阈值")] public float reachDistance = 0.01f;
        [Tooltip("抖动中")] public bool playing;
        [Tooltip("归位中")] bool stopping = false;
        void Start()
        {
            originPosition = this.transform.position;
            originLocalPosition = this.transform.localPosition;
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
                    tarPos = new Vector3(RandomX ? Random.Range(-distance, distance) : 0, RandomY ? Random.Range(-distance, distance) : 0, RandomZ ? Random.Range(-distance, distance) : 0);
                    if (tarPos.magnitude > distance.Abs())
                        tarPos = tarPos.normalized * distance.Abs();
                    mTime = 0;
                }
                if (stopping)
                {
                    switch (space)
                    {
                        default:
                        case Space.Self:
                            if ((this.transform.localPosition - originLocalPosition).magnitude < reachDistance)
                            {
                                playing = false;
                                stopping = false;
                                this.transform.localPosition = originLocalPosition;
                            }
                            tarPos = originLocalPosition;
                            break;
                        case Space.World:
                            if ((this.transform.position - originPosition).magnitude < reachDistance)
                            {
                                playing = false;
                                stopping = false;
                                this.transform.position = originPosition;
                            }
                            tarPos = Vector3.zero;
                            break;
                    }
                }
                switch (space)
                {
                    default:
                    case Space.Self:
                        this.transform.localPosition += new Vector3(tarPos.x - this.transform.localPosition.x, tarPos.y - this.transform.localPosition.y, tarPos.z - this.transform.localPosition.z) * Time.deltaTime / maxTime;
                        break;
                    case Space.World:
                        this.transform.position += new Vector3((originPosition + tarPos).x - this.transform.position.x, (originPosition + tarPos).y - this.transform.position.y, (originPosition + tarPos).z - this.transform.position.z) * Time.deltaTime / maxTime;
                        break;
                }
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
            switch (space)
            {
                default:
                case Space.Self:
                    this.transform.localPosition = originLocalPosition;
                    break;
                case Space.World:
                    this.transform.position = originPosition;
                    break;
            }
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
