using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
/// <summary>
/// RyuGiKen's Tools
/// </summary>
namespace RyuGiKen
{
    public struct HSVColor
    {
        [Tooltip("色调[0，360]")] public float Hue;
        [Tooltip("饱和度[0，1]")] public float Saturation;
        [Tooltip("明度[0，1]")] public float Value;
        [Tooltip("透明度[0，1]")] public float alpha;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="H">色调[0，360]</param>
        /// <param name="S">饱和度[0，1]</param>
        /// <param name="V">明度[0，1]</param>
        /// <param name="a">透明度[0，1]</param>
        public HSVColor(float H, float S, float V, float a = 1)
        {
            this.Hue = H;
            this.Saturation = S;
            this.Value = V;
            this.alpha = a;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public HSVColor(Color color)
        {
            ColorAdjust.ConvertRgbToHsv(color.r, color.g, color.b, color.a, out this.Hue, out this.Saturation, out this.Value, out this.alpha);
        }
        public Color ToColor()
        {
            return ColorAdjust.ConvertHsvToRgb(Hue, Saturation, Value, alpha);
        }
        public Vector3 ToVector3()
        {
            return new Vector3(Hue, Saturation, Value);
        }
        public Vector4 ToVector4()
        {
            return new Vector4(Hue, Saturation, Value, alpha);
        }
        public override string ToString()
        {
            return ToVector4().ToString();
        }
    }
    public static class ColorAdjust
    {
        /// <summary>
        /// 三维坐标转颜色
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color ToColor(Vector3 value)
        {
            return new Color(value.x, value.y, value.z);
        }
        /// <summary>
        /// 四维坐标转颜色
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color ToColor(Vector4 value)
        {
            return new Color(value.x, value.y, value.z, value.w);
        }
        /// <summary>
        /// 更改颜色色相（颜色，指定量百分比系数[0，1]）
        /// </summary>
        /// <param name="RGBA">颜色</param>
        /// <param name="percent">指定量百分比系数[0，1]</param>
        /// <returns></returns>
        public static Color ColorHueChange(Color RGBA, float percent)
        {
            HSVColor temp = ConvertRgbToHsv(RGBA);
            temp.Hue = percent * 360;
            if (temp.Hue > 360)
                temp.Hue = 0;
            else if (temp.Hue < 0)
                temp.Hue = 0;

            Color Changed = ConvertHsvToRgb(temp);
            return Changed;
        }
        /// <summary>
        /// 更改颜色饱和度（颜色，指定量百分比系数[0，1]）
        /// </summary>
        /// <param name="RGBA"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Color ColorSaturationChange(Color RGBA, float percent)//更改颜色饱和度（颜色，指定量百分比系数[0，1]）
        {
            HSVColor temp = ConvertRgbToHsv(RGBA);
            temp.Saturation = percent;
            if (temp.Saturation > 1)
                temp.Saturation = 1;
            else if (temp.Saturation < 0)
                temp.Saturation = 0;

            Color Changed = ConvertHsvToRgb(temp);
            return Changed;
        }
        /// <summary>
        /// 更改颜色明度（颜色，指定量百分比系数[0，1]）
        /// </summary>
        /// <param name="RGBA"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Color ColorValueChange(Color RGBA, float percent)//更改颜色明度（颜色，指定量百分比系数[0，1]）
        {
            HSVColor temp = ConvertRgbToHsv(RGBA);
            temp.Value = percent;
            if (temp.Value > 1)
                temp.Value = 1;
            else if (temp.Value < 0)
                temp.Value = 0;

            Color Changed = ConvertHsvToRgb(temp);
            return Changed;
        }
        /// <summary>
        /// 更改颜色透明度（颜色，指定量百分比系数[0，1]）
        /// </summary>
        /// <param name="RGBA"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Color ColorAlphaChange(Color RGBA, float percent)//更改颜色透明度（颜色，指定量百分比系数[0，1]）
        {
            return new Color(RGBA.r, RGBA.g, RGBA.b, percent);
        }
        /// <summary>
        /// RGB转换HSV
        /// </summary>
        /// <param name="r">红[0，1]</param>
        /// <param name="g">绿[0，1]</param>
        /// <param name="b">蓝[0，1]</param>
        /// <param name="alpha">透明度[0，1]</param>
        /// <param name="H">色调[0，360]</param>
        /// <param name="S">饱和度[0，1]</param>
        /// <param name="V">明度[0，1]</param>
        /// <param name="A">透明度[0，1]</param>
        public static void ConvertRgbToHsv(float r, float g, float b, float alpha, out float H, out float S, out float V, out float A)
        {
            float delta, min;
            float h = 0, s, v;

            min = Mathf.Min(Mathf.Min(r, g), b);
            v = Mathf.Max(Mathf.Max(r, g), b);
            delta = v - min;

            if (v == 0)
                s = 0;
            else
                s = delta / v;

            if (s == 0)
                h = 360;
            else
            {
                if (r == v)
                    h = (g - b) / delta;
                else if (g == v)
                    h = 2 + (b - r) / delta;
                else if (b == v)
                    h = 4 + (r - g) / delta;

                h *= 60;
                if (h <= 0.0)
                    h += 360;
            }
            H = h;
            S = s;
            V = v;
            A = alpha;
        }
        /// <summary>
        /// RGB转换HSV
        /// </summary>
        /// <param name="color"></param>
        /// <param name="H">色调[0，360]</param>
        /// <param name="S">饱和度[0，1]</param>
        /// <param name="V">明度[0，1]</param>
        /// <param name="A">透明度[0，1]</param>
        public static void ConvertRgbToHsv(Color color, out float H, out float S, out float V, out float A)
        {
            ConvertRgbToHsv(color.r, color.g, color.b, color.a, out H, out S, out V, out A);
        }
        /// <summary>
        /// RGB转换HSV
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="alpha"></param>
        /// <returns>（360，1，1，1）</returns>
        public static HSVColor ConvertRgbToHsv(float r, float g, float b, float alpha = 1f)
        {
            float H, S, V, A;
            ConvertRgbToHsv(r, g, b, alpha, out H, out S, out V, out A);
            return new HSVColor(H, S, V, A);
        }
        /// <summary>
        /// RGB转换HSV
        /// </summary>
        /// <param name="RGB"></param>
        /// <returns>(360，1，1，1)</returns>
        public static HSVColor ConvertRgbToHsv(Color RGB)
        {
            HSVColor HSV = ConvertRgbToHsv(RGB.r, RGB.g, RGB.b, RGB.a);
            return HSV;
        }
        /// <summary>
        ///  HSV转换RGBA（360，1，1，1）
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color ConvertHsvToRgb(float h, float s, float v, float alpha = 1f)
        {
            float r = 0, g = 0, b = 0;
            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                int i;
                float f, p, q, t;

                if (h == 360)
                    h = 0;
                else
                    h = h / 60;

                i = (int)(h);
                f = h - i;

                p = v * (1.0f - s);
                q = v * (1.0f - (s * f));
                t = v * (1.0f - (s * (1.0f - f)));

                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;
                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }
            return new Color(r, g, b, alpha);
        }
        /// <summary>
        /// HSV转换RGBA（360，1，1，1）
        /// </summary>
        /// <param name="HSV"></param>
        /// <returns></returns>
        public static Color ConvertHsvToRgb(HSVColor HSV)
        {
            Color RGB = ConvertHsvToRgb(HSV.Hue, HSV.Saturation, HSV.Value, HSV.alpha);
            return RGB;
        }
    }
}
