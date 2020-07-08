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
    /// <summary>
    /// 对象调整
    /// </summary>
    public static class ObjectAdjust
    {
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetActive(this GameObject[] GO, bool active)
        {
            foreach (var item in GO)
            {
                item.SetActive(active);
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetActive<T>(this T[] GO, bool active) where T : Component
        {
            foreach (var item in GO)
            {
                item.gameObject.SetActive(active);
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetActive(this List<GameObject> GO, bool active)
        {
            foreach (var item in GO)
            {
                item.SetActive(active);
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetActive<T>(this List<T> GO, bool active) where T : Component
        {
            foreach (var item in GO)
            {
                item.gameObject.SetActive(active);
            }
        }
    }
    /// <summary>
    /// 数值调整
    /// </summary>
    public static class ValueAdjust
    {
        /// <summary>
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，速度/s）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="NormalSpeed">速度/s</param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 A, Vector3 B, float step, Vector3 NormalSpeed)
        {
            if ((B - A).magnitude <= NormalSpeed.magnitude * step)
                return B;
            else if (NormalSpeed.magnitude == 0 || step == 0)
                return A;
            return new Vector3(Lerp(A.x, B.x, step, NormalSpeed.x), Lerp(A.y, B.y, step, NormalSpeed.y), Lerp(A.z, B.z, step, NormalSpeed.z));
        }
        /// <summary>
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，速度/s，超出差值范围时的速度/s，差值范围）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="NormalSpeed">速度/s</param>
        /// <param name="PlusSpeed ">超出差值范围时的速度/s</param>
        /// <param name="setSpeedRange">差值范围</param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 A, Vector3 B, float step, Vector3 NormalSpeed, Vector3 PlusSpeed, float setSpeedRange = 0)
        {
            if ((B - A).magnitude <= NormalSpeed.magnitude * step)
                return B;
            else if (NormalSpeed.magnitude == 0 || step == 0)
                return A;
            return new Vector3(Lerp(A.x, B.x, step, NormalSpeed.x, PlusSpeed.x, setSpeedRange), Lerp(A.y, B.y, step, NormalSpeed.y, PlusSpeed.y, setSpeedRange), Lerp(A.z, B.z, step, NormalSpeed.z, PlusSpeed.z, setSpeedRange));
        }
        /// <summary>
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，速度/s，超出差值范围时的速度/s，差值范围）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="NormalSpeed">速度/s</param>
        /// <param name="PlusSpeed ">超出差值范围时的速度/s</param>
        /// <param name="setSpeedRange">差值范围</param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 A, Vector3 B, float step, float NormalSpeed = 1f, float PlusSpeed = 1f, float setSpeedRange = 0)
        {
            if ((B - A).magnitude <= NormalSpeed * step)
                return B;
            else if (NormalSpeed == 0 || step == 0)
                return A;
            Vector3 speed = (B - A).normalized * NormalSpeed;
            Vector3 plusSpeed = (B - A).normalized * PlusSpeed;
            return new Vector3(Lerp(A.x, B.x, step, speed.x, plusSpeed.x, setSpeedRange), Lerp(A.y, B.y, step, speed.y, plusSpeed.y, setSpeedRange), Lerp(A.z, B.z, step, speed.z, plusSpeed.z, setSpeedRange));
        }
        /// <summary>
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，速度/s）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="NormalSpeed">速度/s</param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 A, Vector2 B, float step, Vector2 NormalSpeed)
        {
            if ((B - A).magnitude <= NormalSpeed.magnitude * step)
                return B;
            else if (NormalSpeed.magnitude == 0 || step == 0)
                return A;
            return new Vector2(Lerp(A.x, B.x, step, NormalSpeed.x), Lerp(A.y, B.y, step, NormalSpeed.y));
        }
        /// <summary>
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，速度/s，超出差值范围时的速度/s，差值范围）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="NormalSpeed">速度/s</param>
        /// <param name="PlusSpeed ">超出差值范围时的速度/s</param>
        /// <param name="setSpeedRange">差值范围</param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 A, Vector2 B, float step, Vector2 NormalSpeed, Vector2 PlusSpeed, float setSpeedRange = 0)
        {
            if ((B - A).magnitude <= NormalSpeed.magnitude * step)
                return B;
            else if (NormalSpeed.magnitude == 0 || step == 0)
                return A;
            return new Vector2(Lerp(A.x, B.x, step, NormalSpeed.x, PlusSpeed.x, setSpeedRange), Lerp(A.y, B.y, step, NormalSpeed.y, PlusSpeed.y, setSpeedRange));
        }
        /// <summary>
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，速度/s，超出差值范围时的速度/s，差值范围）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="NormalSpeed">速度/s</param>
        /// <param name="PlusSpeed ">超出差值范围时的速度/s</param>
        /// <param name="setSpeedRange">差值范围</param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 A, Vector2 B, float step, float NormalSpeed = 1f, float PlusSpeed = 1f, float setSpeedRange = 0)
        {
            if ((B - A).magnitude <= NormalSpeed * step)
                return B;
            else if (NormalSpeed == 0 || step == 0)
                return A;
            Vector2 speed = (B - A).normalized * NormalSpeed;
            Vector2 plusSpeed = (B - A).normalized * PlusSpeed;
            return new Vector2(Lerp(A.x, B.x, step, speed.x, plusSpeed.x, setSpeedRange), Lerp(A.y, B.y, step, speed.y, plusSpeed.y, setSpeedRange));
        }
        /// <summary>
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，速度/s，超出差值范围时的速度/s，差值范围）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="NormalSpeed">速度/s</param>
        /// <param name="PlusSpeed ">超出差值范围时的速度/s</param>
        /// <param name="setSpeedRange">差值范围</param>
        /// <returns></returns>
        public static float Lerp(float A, float B, float step, float NormalSpeed = 1f, float PlusSpeed = 1f, float setSpeedRange = 0)
        {
            if (NormalSpeed == 0 || step == 0)
                return A;
            float speed = Mathf.Abs(NormalSpeed);
            if (setSpeedRange != 0)
            {
                if (PlusSpeed == 0)
                    PlusSpeed = speed;
                setSpeedRange = Mathf.Abs(setSpeedRange) * 0.5f;
                if (A > B + setSpeedRange || A < B - setSpeedRange)
                    speed = Mathf.Abs(PlusSpeed);
            }
            if (A > B + speed * step)
                A -= speed * step;
            else if (A < B - speed * step)
                A += speed * step;
            else
                A = B;
            return A;
        }
        /// <summary>
        /// 输出y=(k*（x + b）^2) + a；
        /// </summary>
        /// <param name="X"></param>
        /// <param name="k"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static float Quadratic(float X, float k, float b, float a)
        {
            return (X + b) * (X + b) * k + a;
        }
        /// <summary>
        /// 输出y=√（（x+b）/ k）；
        /// </summary>
        /// <param name="X"></param>
        /// <param name="b"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static float Square(float X, float b, float k)
        {
            return Mathf.Sqrt(Mathf.Abs((X + b) / k));
        }
        /// <summary>
        /// 判定是否在一定误差范围内约等于目标值（当前值，目标值，误差范围）
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="TargetValue">目标值</param>
        /// <param name="ErrorRange">误差范围</param>
        /// <returns></returns>
        public static bool JudgeRange(this float CurrentValue, float TargetValue, float ErrorRange)
        {
            if (ErrorRange < 0)
                ErrorRange = -ErrorRange;
            if (CurrentValue >= (TargetValue - ErrorRange * 0.5f) && CurrentValue <= (TargetValue + ErrorRange * 0.5f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判定是否在一定误差范围内约等于目标值（当前值，目标值，误差范围）
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="TargetValue">目标值</param>
        /// <param name="ErrorRange">误差范围</param>
        /// <returns></returns>
        public static bool JudgeRange(this Vector2 CurrentValue, Vector2 TargetValue, Vector2 ErrorRange)
        {
            if (JudgeRange(CurrentValue.x, TargetValue.x, ErrorRange.x) && JudgeRange(CurrentValue.y, TargetValue.y, ErrorRange.y))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 判定是否在一定误差范围内约等于目标值（当前值，目标值，误差范围）
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="TargetValue">目标值</param>
        /// <param name="ErrorRange">误差范围</param>
        /// <returns></returns>
        public static bool JudgeRange(this Vector3 CurrentValue, Vector2 TargetValue, Vector2 ErrorRange)
        {
            if (JudgeRange(CurrentValue.x, TargetValue.x, ErrorRange.x) && JudgeRange(CurrentValue.y, TargetValue.y, ErrorRange.y))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 判定是否在一定误差范围内约等于目标值（当前值，目标值，误差范围）
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="TargetValue">目标值</param>
        /// <param name="ErrorRange">误差范围</param>
        /// <returns></returns>
        public static bool JudgeRange(this Vector3 CurrentValue, Vector3 TargetValue, Vector3 ErrorRange)
        {
            if (JudgeRange(CurrentValue.x, TargetValue.x, ErrorRange.x) && JudgeRange(CurrentValue.y, TargetValue.y, ErrorRange.y) && JudgeRange(CurrentValue.z, TargetValue.z, ErrorRange.z))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 精确到小数点后几位（值，位数）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public static float Round(this float value, int digits)
        {
            //return Mathf.Round(value * Mathf.Pow(10, digits)) * Mathf.Pow(0.1f, digits);
            return float.Parse(value.ToString("f" + digits));
        }
        /// <summary>
        /// 精确到小数点后几位（值，位数）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public static Vector2 Round(this Vector2 value, int digits)
        {
            return new Vector2(Round(value.x, digits), Round(value.y, digits));
        }
        /// <summary>
        /// 精确到小数点后几位（值，位数）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public static Vector3 Round(this Vector3 value, int digits)
        {
            return new Vector3(Round(value.x, digits), Round(value.y, digits), Round(value.z, digits));
        }
        /// <summary>
        /// 调整循环范围(当前值，最小值，最大值，循环周期)
        /// </summary>
        /// <param name="num">当前值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="period">循环周期</param>
        /// <returns></returns>
        public static float SetRange(float num, float min, float max, float period)
        {
            float numAdjusted = num;
            if (num > max)
                numAdjusted -= period;
            else if (num < min)
                numAdjusted += period;
            return numAdjusted;
        }
        /// <summary>
        /// 直角坐标转换成极坐标系 Vector2(角度（0，360）, 距离)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector2 RectToPolar(Vector2 pos)
        {
            float Distance = 0, Angle = 0;
            Distance = Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y);
            if (Distance == 0)
            { Angle = 0; }
            else
            { Angle = Mathf.Atan2(pos.y, pos.x) / Mathf.Deg2Rad; }

            while (Angle < 0)
            { Angle += 360; }

            return new Vector2(Angle, Distance);
        }
        /// <summary>
        /// 极坐标转换成直角坐标系
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        public static Vector2 PolarToRect(float angle, float dis)
        {
            Vector2 rectpos;
            rectpos.x = dis * Mathf.Cos(angle * Mathf.Deg2Rad);
            rectpos.y = dis * Mathf.Sin(angle * Mathf.Deg2Rad);
            return rectpos;
        }
        /// <summary>
        /// x在[a，b]范围输出[0，1]，n=1为递增，n=-1为递减；
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Progress(float x, float n, float a, float b)//(参数，递增减系数正负1，x最小值，x最大值)
        {
            float temp;
            if (a == b)
            { return 0; }
            else if (a > b)
            {
                temp = a;
                a = b;
                b = temp;
            }
            x = Mathf.Clamp(x, a, b);
            if (n < 0)
            {
                return (x - b) / (a - b);//输出[-1,0]
            }
            else
            {
                return (x - a) / (b - a);//输出[0,1]
            }
        }
        /// <summary>
        /// 输出"00:00:00"
        /// </summary>
        /// <param name="time">秒</param>
        /// <param name="digit">位数</param>
        /// <param name="ShowMillisecond">显示毫秒</param>
        /// <returns></returns>
        public static string ShowTime(float time, int digit = 3, bool ShowMillisecond = false)
        {
            string sTime = "";
            string sign = "";
            if (time < 0)
            {
                time = Mathf.Abs(time);
                sign = "-";
            }
            switch (digit)
            {
                default:
                case 1:
                    sTime = time.ToString("0");
                    break;
                case 2:
                    sTime = ((int)(time / 60)).ToString(time / 60 < 100 ? "D2" : "") + ":" + ((int)(time % 60)).ToString("D2");
                    break;
                case 3:
                    sTime = ((int)(time / 3600)).ToString(time / 3600 < 100 ? "D2" : "") + ":" + ((int)(time / 60) % 60).ToString("D2") + ":" + ((int)(time % 60)).ToString("D2");
                    break;
                case 4:
                    sTime = ((int)(time / (3600 * 24))).ToString("D2") + ":" + ((int)(time / 3600) % 60).ToString("D2") + ":" + ((int)(time / 60) % 60).ToString("D2") + ":" + ((int)(time % 60)).ToString("D2");
                    break;
            }
            if (ShowMillisecond)
            {
                sTime += ":" + ((int)((time - (int)time) * 1000)).ToString("D3");
            }
            return sign + sTime;
        }
        /// <summary>
        /// 汉字整数数字显示（最大正负9999）；
        /// </summary>
        public static string NumShowCN(float N)//汉字数字显示
        {
            float num;
            string NumZ = "";
            string RoundD = "";//汉字数字千位
            string RoundC = "";//汉字数字百位
            string RoundB = "";//汉字数字十位
            string RoundA = "";//汉字数字个位
            int D = 0;//数字千位
            int C = 0;//数字百位
            int B = 0;//数字十位
            int A = 0;//数字个位

            if (N < 0)
            { num = -N; }
            else
            { num = N; }
            if (num < 1)
            {
                RoundD = "";
                RoundC = "";
                RoundB = "";
                RoundA = "零";
            }
            else if (num > 9999)
            {
                RoundD = "九千";
                RoundC = "九百";
                RoundB = "九十";
                RoundA = "九";
            }
            else
            {
                //数字千位
                if (num > 999)
                {
                    D = Mathf.FloorToInt(num / 1000);
                    switch (D)
                    {
                        case 1: RoundD = "一千"; break;
                        case 2: RoundD = "二千"; break;
                        case 3: RoundD = "三千"; break;
                        case 4: RoundD = "四千"; break;
                        case 5: RoundD = "五千"; break;
                        case 6: RoundD = "六千"; break;
                        case 7: RoundD = "七千"; break;
                        case 8: RoundD = "八千"; break;
                        case 9: RoundD = "九千"; break;
                        default: RoundD = ""; break;
                    }
                }
                //数字百位
                if (num > 99)
                {
                    C = Mathf.FloorToInt((num - D * 1000) / 100);
                    switch (C)
                    {
                        case 1: RoundC = "一百"; break;
                        case 2: RoundC = "二百"; break;
                        case 3: RoundC = "三百"; break;
                        case 4: RoundC = "四百"; break;
                        case 5: RoundC = "五百"; break;
                        case 6: RoundC = "六百"; break;
                        case 7: RoundC = "七百"; break;
                        case 8: RoundC = "八百"; break;
                        case 9: RoundC = "九百"; break;
                        default: RoundC = ""; break;
                    }
                }
                //数字十位
                if (num > 9)
                {
                    B = Mathf.FloorToInt((num - D * 1000 - C * 100) / 10);
                    switch (B)
                    {
                        case 1: RoundB = "一十"; break;
                        case 2: RoundB = "二十"; break;
                        case 3: RoundB = "三十"; break;
                        case 4: RoundB = "四十"; break;
                        case 5: RoundB = "五十"; break;
                        case 6: RoundB = "六十"; break;
                        case 7: RoundB = "七十"; break;
                        case 8: RoundB = "八十"; break;
                        case 9: RoundB = "九十"; break;
                        default: RoundB = ""; break;
                    }
                }
                //数字个位
                A = Mathf.FloorToInt(num - D * 1000 - C * 100 - B * 10);
                switch (A)
                {
                    case 1: RoundA = "一"; break;
                    case 2: RoundA = "二"; break;
                    case 3: RoundA = "三"; break;
                    case 4: RoundA = "四"; break;
                    case 5: RoundA = "五"; break;
                    case 6: RoundA = "六"; break;
                    case 7: RoundA = "七"; break;
                    case 8: RoundA = "八"; break;
                    case 9: RoundA = "九"; break;
                    default: RoundA = ""; break;
                }

                if (D == 0 && C == 0 && B == 1)
                {
                    RoundB = "十";
                }
                else if (C > 0 && B == 0 && A > 0)
                {
                    RoundB = "零";
                }
                else if (D > 0 && C == 0 && B > 0)
                {
                    RoundC = "零";
                }
                else if (D > 0 && C == 0 && B == 0 && A > 0)
                {
                    RoundC = "零";
                }
            }
            NumZ = RoundD + RoundC + RoundB + RoundA;
            if (N < 0)
                NumZ = "负" + NumZ;

            //Debug.Log(NumZ);
            return NumZ;
        }
    }
    /// <summary>
    /// 颜色（HSV模式）
    /// </summary>
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
    /// <summary>
    /// 颜色调整
    /// </summary>
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
