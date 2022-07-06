using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RyuGiKen.Tools
{
    /// <summary>
    /// FPS显示，限制，画质切换
    /// </summary>
    [AddComponentMenu("RyuGiKen/FPS显示")]
    [DisallowMultipleComponent]
    public class FPSShow : MonoBehaviour
    {
        public static FPSShow instance;
        [Tooltip("隐藏")] public bool hide = true;//默认隐藏
        public Text FPSText;
        float passTime;
        public float m_FPS;
        int FrameCount;
        [Tooltip("统计时间")] public float fixTime = 1;
        [Tooltip("切换按键")] public KeyCode hideKey = KeyCode.F7;
        [Tooltip("不受时间缩放影响")] public bool UnscaledDeltaTime = true;
        [Tooltip("打印平均帧率日志")] public bool FPSLog = false;
        [Header("根据帧数自动切换画质(连续10s)")]
        [Tooltip("根据帧数自动切换画质")] public bool autoAdjustQualityLevel = true;
        [Tooltip("切换时间")] public int autoAdjustQualityLevelTime = 30;
        [Tooltip("帧率超范围计时")] static float deltaTime = 0;
        internal static float autoAdjustQualityLevelTimer { get { return deltaTime; } }
        [Tooltip("帧率范围")] public ValueRange adjustFPSRange = new Vector2(25, 55);
        [Tooltip("限制帧率")] public int LockFrameRate = 60;
        private void Awake()
        {
            if (FPSText == null)
            { FPSText = GetComponent<Text>(); }
            if (gameObject.activeInHierarchy)
                if (!instance) instance = this;
            if (instance != this)
                Destroy(this.gameObject);
            QualitySettings.vSyncCount = 0;
            RefreshTargetFPS();
        }
        private void Reset()
        {
            if (FPSText == null)
            { FPSText = GetComponent<Text>(); }
        }
        private void Start()
        {
            RefreshTargetFPS();
        }
        public static void RefreshTargetFPS(int num = -1)
        {
            if (num > 0)
            {
                if (instance)
                {
                    Application.targetFrameRate = instance.LockFrameRate = num;
                }
                else
                {
                    Application.targetFrameRate = num;
                }
            }
            else if (instance)
            {
                Application.targetFrameRate = instance.LockFrameRate;
            }
        }
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F1))//按F1降低画质
            {
                AdjustQualityLevel(-1);
            }
            if (Input.GetKeyUp(KeyCode.F2))//按F2提高画质
            {
                AdjustQualityLevel(1);
            }
            if (Input.GetKeyUp(KeyCode.F3))//按F3限制帧数为60;
            {
                RefreshTargetFPS();
            }
            if (Input.GetKeyUp(KeyCode.F4))//按F4不限制帧数;
            {
                Application.targetFrameRate = -1;
            }
            if (FPSText != null)
            {
                if (Input.GetKeyDown(hideKey))
                {
                    hide = !hide;
                    if (!hide)
                        FPSLog = true;
                }
                FPSText.enabled = !hide;
                ShowFPS();
            }
            if (autoAdjustQualityLevel && m_FPS < adjustFPSRange.MinValue && Time.timeSinceLevelLoad > 10)//自动降低画质
            {
                if (deltaTime <= 0)
                    deltaTime -= Time.unscaledDeltaTime;
                else
                    deltaTime = 0;
                if (deltaTime <= -autoAdjustQualityLevelTime.Clamp(0))
                {
                    deltaTime = 0;
                    AdjustQualityLevel(-1);
                }
            }
            else if (autoAdjustQualityLevel && m_FPS > adjustFPSRange.MaxValue && Time.timeSinceLevelLoad > 1)//自动提高画质
            {
                if (deltaTime >= 0)
                    deltaTime += Time.unscaledDeltaTime;
                else
                    deltaTime = 0;
                if (deltaTime >= autoAdjustQualityLevelTime.Clamp(0))
                {
                    deltaTime = 0;
                    AdjustQualityLevel(1);
                }
            }
            else if (deltaTime.Abs() > 1)
                deltaTime = 0;
        }
        /// <summary>
        /// FPS计数
        /// </summary>
        void ShowFPS()
        {
            FrameCount += 1;
            passTime += (UnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime);

            if (passTime > fixTime)
            {
                m_FPS = (float)Math.Round(FrameCount / passTime);
                FPSText.text = "FPS  " + m_FPS.ToString("F0") + "  L" + QualitySettings.GetQualityLevel().ToString();
                passTime = 0.0f;
                FrameCount = 0;
                if (FPSLog)
                {
                    try
                    {
                        Data_FPS.Add(m_FPS);
                    }
                    catch { }
                }
            }
            //FPSText.text = "FPS  " + Mathf.RoundToInt(1.0f / deltaTime).ToString();
        }
        /// <summary>
        /// 设置画质等级
        /// </summary>
        /// <param name="level"></param>
        public static void SetQualityLevel(int level = 0)
        {
            deltaTime = 0;
            QualitySettings.vSyncCount = 0;
            QualitySettings.SetQualityLevel(level);
        }
        /// <summary>
        /// 调整画质等级
        /// </summary>
        /// <param name="type">大于0增加，小于0减少</param>
        public static void AdjustQualityLevel(int type = 0)
        {
            deltaTime = 0;
            QualitySettings.vSyncCount = 0;
            if (type > 0)
            {
                QualitySettings.IncreaseLevel(false);
            }
            else if (type < 0)
            {
                QualitySettings.DecreaseLevel(false);
            }
        }
        public static string Path = Application.streamingAssetsPath;
        static StreamWriter sw;
        [Tooltip("记录")] public static List<float> Data_FPS = new List<float>();
        public static void Log()
        {
            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
                sw = new StreamWriter(Path + "/FpsRecord.txt", true, System.Text.Encoding.UTF8);
                for (int i = 0; i < 2; i++)
                    if (Data_FPS.Count > 2)
                        Data_FPS.RemoveAt(0);
                string info = "[FPS]" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " \t" + Data_FPS.Count + "平均：" + ValueAdjust.GetAverage(Data_FPS.ToArray()).ToString("F2");
                //Debug.Log(ValueAdjust.PrintArray(Data_FPS.ToArray()));
                Debug.Log(info);
                //开始写入
                sw.WriteLine(info + "\r\n");
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
            }
            catch { }
        }
        private void OnDisable()
        {
            if (Data_FPS.Count > 1 && FPSLog)
                Log();
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(FPSShow))]
    public class FPSShowEditor : Editor
    {
        bool ShowInspector = false;
        SerializedProperty hide;
        SerializedProperty UnscaledDeltaTime;
        SerializedProperty FPSLog;
        SerializedProperty LockFrameRate;
        SerializedProperty autoAdjustQualityLevel;
        SerializedProperty autoAdjustQualityLevelTime;
        SerializedProperty adjustFPSRange;
        void OnEnable()
        {
            hide = serializedObject.FindProperty("hide");
            UnscaledDeltaTime = serializedObject.FindProperty("UnscaledDeltaTime");
            FPSLog = serializedObject.FindProperty("FPSLog");
            LockFrameRate = serializedObject.FindProperty("LockFrameRate");
            autoAdjustQualityLevel = serializedObject.FindProperty("autoAdjustQualityLevel");
            autoAdjustQualityLevelTime = serializedObject.FindProperty("autoAdjustQualityLevelTime");
            adjustFPSRange = serializedObject.FindProperty("adjustFPSRange");
        }
        public override void OnInspectorGUI()
        {
            string[] Name = new string[9];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Name[0] = "显示值";
                    Name[1] = "显示";
                    Name[2] = "FPS：";
                    Name[3] = "不受时间缩放影响";
                    Name[4] = "打印平均帧率日志";
                    Name[5] = "限制帧率";
                    Name[6] = "根据帧数自动切换画质(连续10s)";
                    Name[7] = "切换时间";
                    Name[8] = "最大帧率范围";
                    break;
                default:
                    Name[0] = "Show Inspector";
                    Name[1] = "Display";
                    Name[2] = "FPS：";
                    Name[3] = "Unscaled DeltaTime";
                    Name[4] = "Log";
                    Name[5] = "Limit FrameRate";
                    Name[6] = "Auto Adjust QualityLevel";
                    Name[7] = "Switch Time";
                    Name[8] = "FrameRate Range";
                    break;
            }
            ShowInspector = EditorGUILayout.Foldout(ShowInspector, Name[0]);
            serializedObject.Update();
            if (ShowInspector)
            {
                base.OnInspectorGUI();
            }
            else
            {
                FPSShow fps = target as FPSShow;
                hide.boolValue = !EditorGUILayout.ToggleLeft(Name[1], !hide.boolValue);
                EditorGUILayout.IntField(Name[2], (int)fps.m_FPS);
                UnscaledDeltaTime.boolValue = EditorGUILayout.Toggle(Name[3], UnscaledDeltaTime.boolValue);
                FPSLog.boolValue = EditorGUILayout.Toggle(Name[4], FPSLog.boolValue);
                LockFrameRate.intValue = EditorGUILayout.DelayedIntField(Name[5], LockFrameRate.intValue.Clamp(-1));
                autoAdjustQualityLevel.boolValue = EditorGUILayout.ToggleLeft(Name[6], autoAdjustQualityLevel.boolValue);
                if (fps.autoAdjustQualityLevel)
                {
                    autoAdjustQualityLevelTime.intValue = EditorGUILayout.DelayedIntField(Name[7], autoAdjustQualityLevelTime.intValue.Clamp(0));
                    SerializedProperty range = adjustFPSRange.FindPropertyRelative("range");
                    Vector2 temp = EditorGUILayout.Vector2Field(Name[8], range.vector2Value).Clamp(Vector2.one, Vector2.one * 600);
                    range.vector2Value = new Vector2Int(temp.x.ToInteger(), temp.y.ToInteger());
                    EditorGUILayout.Slider(FPSShow.autoAdjustQualityLevelTimer, 0, fps.autoAdjustQualityLevelTime);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
