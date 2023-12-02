using System.Collections;
using System.Collections.Generic;
using RyuGiKen;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 方形碰撞体自适应大小
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class AdaptiveBoxCollider2DSize : MonoBehaviour
    {
        public BoxCollider2D collider;
        public RectTransform rectTransform;
        public SpriteRenderer spriteRenderer;
        private void Reset()
        {
            collider = this.GetComponent<BoxCollider2D>();
            rectTransform = this.GetComponent<RectTransform>();
            spriteRenderer = this.GetComponent<SpriteRenderer>();
        }
        void LateUpdate()
        {
            if (rectTransform && rectTransform.TryGetComponent(out Image image) && image.preserveAspect)
            {
                collider.size = ValueAdjust.ProportionalScaling(image.sprite.bounds.size, rectTransform.rect.size);
            }
            else if (rectTransform)
            {
                collider.size = rectTransform.rect.size;
            }
            else if (spriteRenderer)
            {
                collider.size = spriteRenderer.size;
            }
        }
    }
}
