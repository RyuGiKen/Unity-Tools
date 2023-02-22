using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RyuGiKen.Tools
{
    /// <summary>
    /// ����Ӧ��С
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("RyuGiKen/����Ӧ��С")]
    public class AdaptiveSize : MonoBehaviour
    {
        public enum Mode
        {
            /// <summary>
            /// ����
            /// </summary>
            Min,
            /// <summary>
            /// ����
            /// </summary>
            Max
        }
        public Mode mode = Mode.Min;
        public RectTransform rectTransform;
        public RectTransform parent;
        public Vector2 NormalSize;
        private void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
            parent = this.transform.parent?.GetComponent<RectTransform>();
        }
        void Update()
        {
            if (parent && rectTransform)
            {
                if (NormalSize != Vector2.zero)
                {
                    if (float.IsNaN(rectTransform.sizeDelta.x) || float.IsNaN(rectTransform.sizeDelta.y) || rectTransform.sizeDelta.x <= 0 || rectTransform.sizeDelta.y <= 0)
                        rectTransform.sizeDelta = NormalSize;
                }
                Vector2 rect = ValueAdjust.ProportionalScaling(NormalSize, parent.rect.size, mode == Mode.Min);
                rectTransform.sizeDelta = new Vector2(rect.x.Clamp(1), rect.y.Clamp(1));
            }
        }
    }
}