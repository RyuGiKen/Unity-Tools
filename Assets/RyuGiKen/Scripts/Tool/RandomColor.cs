using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 颜色闪烁
    /// </summary>
    [AddComponentMenu("RyuGiKen/颜色闪烁")]
    public class RandomColor : MonoBehaviour
    {
        [Tooltip("Shader中的颜色属性名")] [SerializeField] string ColorNameInShader = "_EmissionColor";
        [SerializeField] List<Renderer> renderers = new List<Renderer>();
        [SerializeField] List<Graphic> graphics = new List<Graphic>();
        public ColorAdjust.ColorAdjustMode mode = ColorAdjust.ColorAdjustMode.RGBA;
        public bool randomColour = true;
        Color tempColor;
        public Color color;
        public Color color2;
        public Gradient gradient;
        public Color disableColor;
        [SerializeField] float time = 0;
        [Tooltip("切换颜色时间")] public float SwitchColorTime = 0;
        [SerializeField][Range(-1, 1)] int direction = 1;
        public bool BackToBlack;
        void Start()
        {
            if (randomColour)
                color = ColorAdjust.ConvertHsvToRgb(Random.Range(0, 360), Random.Range(0, 1f), 1);
            if (renderers.Count < 1)
                renderers.Add(this.GetComponent<Renderer>());
            if (graphics.Count < 1)
                graphics.Add(this.GetComponent<Graphic>());
            tempColor = color;
            SetColor(color);
        }
        void Update()
        {
            if (SwitchColorTime > 0 && renderers.Count > 0)
            {
                time += direction * Time.deltaTime;
                if (time < 0)//(renderers[0].material.color == color)
                {
                    time = 0;
                    if (randomColour)
                    {
                        color2 = ColorAdjust.ConvertHsvToRgb(Random.Range(0, 360), Random.Range(0, 1f), 1, color2.a);
                    }
                    //if (!BackToBlack)
                    direction = 1;
                }
                else if (time > SwitchColorTime)//(renderers[0].material.color == color2)
                {
                    time = SwitchColorTime;
                    if (randomColour)
                    {
                        color = ColorAdjust.ConvertHsvToRgb(Random.Range(0, 360), Random.Range(0, 1f), 1, color.a);
                    }
                    //if (!BackToBlack)
                    direction = -1;
                }
                else
                {
                    tempColor = Color.Lerp(tempColor, BackToBlack ? disableColor : Color.Lerp(color, color2, time / SwitchColorTime), Time.deltaTime);
                    SetColor(tempColor, mode);
                }
            }
        }
        private void LateUpdate()
        {
            gradient.colorKeys = new GradientColorKey[2] { new GradientColorKey(color, 0), new GradientColorKey(color2, 1) };
            gradient.alphaKeys = new GradientAlphaKey[2] { new GradientAlphaKey(color.a, 0), new GradientAlphaKey(color2.a, 1) };
        }
        public void SetColor(Color m_color)
        {
            for (int i = 0; i < renderers.Count; i++)
                if (renderers[i])
                {
                    if (renderers[i] is SpriteRenderer)
                        ((SpriteRenderer)renderers[i]).color = m_color;
                    else if (renderers[i] is LineRenderer)
                        ((LineRenderer)renderers[i]).startColor = ((LineRenderer)renderers[i]).endColor = m_color;
                    else if (renderers[i] is TrailRenderer)
                        ((TrailRenderer)renderers[i]).startColor = ((TrailRenderer)renderers[i]).endColor = m_color;
                    else if (string.IsNullOrWhiteSpace(ColorNameInShader))
                        renderers[i].material.color = m_color;
                    else
                        renderers[i].material.SetColor(ColorNameInShader, m_color);
                }
            for (int i = 0; i < graphics.Count; i++)
                if (graphics[i])
                {
                    graphics[i].color = m_color;
                }
        }
        public void SetColor(Color m_color, ColorAdjust.ColorAdjustMode mode)
        {
            for (int i = 0; i < renderers.Count; i++)
                if (renderers[i])
                {
                    Color tempColor = disableColor;
                    if (renderers[i] is SpriteRenderer)
                    {
                        tempColor = ((SpriteRenderer)renderers[i]).color;
                        ((SpriteRenderer)renderers[i]).color = ColorAdjust.AdjustColor(tempColor, m_color, mode);
                    }
                    else if (renderers[i] is LineRenderer)
                    {
                        tempColor = ((LineRenderer)renderers[i]).startColor;
                        ((LineRenderer)renderers[i]).startColor = ColorAdjust.AdjustColor(tempColor, m_color, mode);
                        tempColor = ((LineRenderer)renderers[i]).endColor;
                        ((LineRenderer)renderers[i]).endColor = ColorAdjust.AdjustColor(tempColor, m_color, mode);
                    }
                    else if (renderers[i] is TrailRenderer)
                    {
                        tempColor = ((TrailRenderer)renderers[i]).startColor;
                        ((TrailRenderer)renderers[i]).startColor = ColorAdjust.AdjustColor(tempColor, m_color, mode);
                        tempColor = ((TrailRenderer)renderers[i]).endColor;
                        ((TrailRenderer)renderers[i]).endColor = ColorAdjust.AdjustColor(tempColor, m_color, mode);
                    }
                    else if (string.IsNullOrWhiteSpace(ColorNameInShader))
                    {
                        tempColor = renderers[i].material.color;
                        renderers[i].material.color = ColorAdjust.AdjustColor(tempColor, m_color, mode);
                    }
                    else
                    {
                        tempColor = renderers[i].material.GetColor(ColorNameInShader);
                        renderers[i].material.SetColor(ColorNameInShader, ColorAdjust.AdjustColor(tempColor, m_color, mode));
                    }
                }
            for (int i = 0; i < graphics.Count; i++)
                if (graphics[i])
                {
                    graphics[i].color = ColorAdjust.AdjustColor(graphics[i].color, m_color, mode);
                }
        }
        public void ForceSetColor1()
        {
            SetColor(color);
        }
        public void ForceSetColor2()
        {
            SetColor(color2);
        }
    }
}
