using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 电池信息
    /// </summary>
    [DisallowMultipleComponent]
    public class BatteryState : MonoBehaviour
    {
        static float timer = 0;
        static float RefreshTime = 5;
        public static string output;
        [SerializeField] BatteryStatus batteryStatus;
        [SerializeField] float batteryLevel;
        public static void SetRefreshTime(float time)
        {
            RefreshTime = time;
            timer = 0;
        }
        void LateUpdate()
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= RefreshTime)
            {
                timer = 0;
                UpdateBatteryState(out batteryStatus, out batteryLevel);
                string name = "";
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                    case SystemLanguage.ChineseTraditional:
                        name = "电池：";
                        break;
                    default:
                        name = "Battery：";
                        break;
                }
                output = name + batteryStatus + " " + (batteryLevel < 0 ? "-1" : (batteryLevel * 100 + " %"));
            }
        }
        public static void UpdateBatteryState(out BatteryStatus batteryStatus, out float batteryLevel)
        {
            batteryStatus = SystemInfo.batteryStatus;
            batteryLevel = SystemInfo.batteryLevel;
        }
    }
}
