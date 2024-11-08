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
    /// 触发按键
    /// </summary>
    public static bool Fire;
    /// <summary>
    /// 触发计数
    /// </summary>
    public static uint TriggerCount;
    /// <summary>
    /// 成功计数
    /// </summary>
    public static uint SuccessCount;
    /// <summary>
    /// 失败计数
    /// </summary>
    public static uint FailureCount;
    /// <summary>
    /// 控制[-1，1]
    /// </summary>
    public static BiaxValue m_BiaxValue;
    /// <summary>
    /// 触发按键支持
    /// </summary>
    public static TriggerMode FireMode = TriggerMode.UseFire;
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
/// <summary>
/// 触发模式
/// </summary>
public enum TriggerMode
{
    /// <summary>
    /// 不支持触发按键
    /// </summary>
    NoNeedFire,
    /// <summary>
    /// 需要触发键
    /// </summary>
    UseFire,
    /// <summary>
    /// 自动触发
    /// </summary>
    AutoFire,
}
