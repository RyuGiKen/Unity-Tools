using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// Transfrom属性复制
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TransformPropertySync : MonoBehaviour
    {
        public Transform Target;

        [Tooltip("局部位置")] public bool localPosition;
        [Tooltip("全局位置")] public bool position;

        [Tooltip("局部旋转")] public bool localEulerAngles;
        [Tooltip("全局旋转")] public bool rotation;
        [Tooltip("局部旋转")] public bool localRotation;

        [Tooltip("局部缩放")] public bool localScale;
        [Tooltip("全局缩放")] public bool lossyScale;

        [Tooltip("锚点位置")] public bool anchoredPosition;
        [Tooltip("大小")] public bool sizeDelta;

        private void Update()
        {
            if (localPosition && position)
            {
                localPosition = false;
            }
            if ((localEulerAngles && rotation) || (rotation && localRotation) || (localEulerAngles && localRotation))
            {
                localEulerAngles = false;
                localRotation = false;
            }
            if (localScale && lossyScale)
            {
                localScale = false;
            }
        }
        private void LateUpdate()
        {
            if (!Target)
                return;

            if (localPosition)
                this.transform.localPosition = Target.localPosition;
            if (position)
                this.transform.position = Target.position;
            if (localEulerAngles)
                this.transform.localEulerAngles = Target.localEulerAngles;
            if (rotation)
                this.transform.rotation = Target.rotation;
            if (localRotation)
                this.transform.localRotation = Target.localRotation;
            if (localScale)
                this.transform.localScale = Target.localScale;
            if (lossyScale)
                this.transform.SetGlobalScale(Target.lossyScale);
            if (Target.TryGetComponent<RectTransform>(out RectTransform targetRect) && this.TryGetComponent<RectTransform>(out RectTransform thisRect))
            {
                if (anchoredPosition)
                    thisRect.anchoredPosition3D = targetRect.anchoredPosition3D;
                if (sizeDelta)
                    thisRect.sizeDelta = targetRect.rect.size;
            }
        }
    }
}
