using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
/// <summary>
/// 全局静态参数
/// </summary>
public static partial class GameParameter
{
    /// <summary>
    /// 控制[-1，1]
    /// </summary>
    public static BiaxValue m_BiaxValue;
    /// <summary>
    /// 触发按键
    /// </summary>
    public static bool Fire;
}
/// <summary>
/// 控制模式
/// </summary>
public enum ControlType
{
    /// <summary>
    /// 键盘
    /// </summary>
    Keyboard,
    /// <summary>
    /// 鼠标
    /// </summary>
    Mouse,
    /// <summary>
    /// 摇杆
    /// </summary>
    Joystick,
}
