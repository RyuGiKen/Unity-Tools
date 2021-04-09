﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 旋转
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/自动旋转")]
    public class AutoRoll : MonoBehaviour
    {
        enum UpdateMode
        {
            Update,
            LateUpdate,
            FixedUpdate,
        }
        [SerializeField] Space space = Space.Self;
        [SerializeField] UpdateMode m_UpdateMode;
        [Tooltip("启动时随机方向")] [SerializeField] bool RandomStartAngle = false;
        [Tooltip("自动旋转")] public bool autoRoll;
        [Tooltip("旋转方向")] public Vector3 angle;
        [Tooltip("旋转速度")] public float rollSpeed;
        [Tooltip("自动缩放")] public bool autoScale;
        //Vector3 originScale;
        //public Vector2 scale;//[min-x,max-y]
        [Tooltip("最小缩放")] public Vector3 minScale;
        [Tooltip("最大缩放")] public Vector3 maxScale;
        [Tooltip("缩放速度")] public float scaleSpeed;
        float scaleValue;
        private void OnEnable()
        {
            if (RandomStartAngle)
            {
                this.transform.Rotate(angle.normalized * 360 * Random.value, space);
            }
        }
        void Start()
        {
            //originScale = this.transform.localScale;
        }
        private void Update()
        {
            if (m_UpdateMode == UpdateMode.Update)
            {
                Refresh(Time.deltaTime);
            }
        }
        private void LateUpdate()
        {
            if (m_UpdateMode == UpdateMode.LateUpdate)
            {
                Refresh(Time.deltaTime);
            }
        }
        private void FixedUpdate()
        {
            if (m_UpdateMode == UpdateMode.FixedUpdate)
            {
                Refresh(Time.fixedDeltaTime);
            }
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="step">步长</param>
        void Refresh(float step)
        {
            if (autoRoll)
            {
                this.transform.Rotate(angle.normalized * rollSpeed * step, space);
            }
            if (autoScale)
            {
                //this.transform.localScale = Vector3.one * (Mathf.PingPong(Time.time * scaleSpeed, scale.y - scale.x) + scale.x);
                scaleValue = Mathf.PingPong(Time.time * scaleSpeed, 1);
                this.transform.localScale = new Vector3((maxScale.x - minScale.x) * scaleValue + minScale.x, (maxScale.y - minScale.y) * scaleValue + minScale.y, (maxScale.z - minScale.z) * scaleValue + minScale.z);
            }
        }
    }
}
