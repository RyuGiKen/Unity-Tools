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
        public Color color;
        public Color color2;
        [SerializeField] float time = 0;
        [Tooltip("切换颜色时间")] public float SwitchColorTime = 0;
        [SerializeField] int direction = 1;
        void Start()
        {
            if (randomColour)
                color = ColorAdjust.ConvertHsvToRgb(Random.Range(0, 360), Random.Range(0, 1f), 1);
            if (renderers.Count < 1)
                renderers.Add(this.GetComponent<Renderer>());
            if (graphics.Count < 1)
                graphics.Add(this.GetComponent<Graphic>());
            SetColor(color);
        }
        void Update()
        {
            if (SwitchColorTime > 0 && renderers.Count > 0)
            {
                time += direction * Time.deltaTime;
                if (time <= 0)//(renderers[0].material.color == color)
                {
                    if (randomColour)
                        color2 = ColorAdjust.ConvertHsvToRgb(Random.Range(0, 360), Random.Range(0, 1f), 1);
                    direction = 1;
                }
                else if (time >= SwitchColorTime)//(renderers[0].material.color == color2)
                {
                    if (randomColour)
                        color = ColorAdjust.ConvertHsvToRgb(Random.Range(0, 360), Random.Range(0, 1f), 1);
                    direction = -1;
                }
                else
                {
                    Color tempColor = Color.Lerp(color, color2, time / SwitchColorTime);
                    SetColor(tempColor, mode);
                }
            }
        }
        public void SetColor(Color m_color)
        {
            for (int i = 0; i < renderers.Count; i++)
                if (renderers[i])
                {
                    if (string.IsNullOrWhiteSpace(ColorNameInShader))
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
                    if (string.IsNullOrWhiteSpace(ColorNameInShader))
                    {
                        renderers[i].material.color = ColorAdjust.AdjustColor(renderers[i].material.color, m_color, mode);
                    }
                    else
                    {
                        Color tempColor = renderers[i].material.GetColor(ColorNameInShader);
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
