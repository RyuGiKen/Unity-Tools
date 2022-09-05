using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 自适应大小
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("RyuGiKen/自适应大小")]
    public class AdaptiveSize : MonoBehaviour
    {
        public enum Mode
        {
            Min,
            Max
        }
        public Mode mode = Mode.Min;
        public RectTransform rectTransform;
        public RectTransform parent;
        private void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
            parent = this.transform.parent?.GetComponent<RectTransform>();
        }
        void Update()
        {
            if (parent && rectTransform)
            {
                if ((parent.rect.size.x > parent.rect.size.y && mode == Mode.Min) || (mode == Mode.Max && parent.rect.size.x < parent.rect.size.y))
                    rectTransform.sizeDelta = rectTransform.sizeDelta * parent.rect.size.y / rectTransform.rect.size.y;
                else
                    rectTransform.sizeDelta = rectTransform.sizeDelta * parent.rect.size.x / rectTransform.rect.size.x;
            }
        }
    }
}
