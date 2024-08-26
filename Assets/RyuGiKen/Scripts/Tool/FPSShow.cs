using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RyuGiKen;
using RyuGiKen.Tools;
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
        [Tooltip("显示画质等级")] public bool ShowQualityLevel = true;
        [Tooltip("显示分辨率")] public bool ShowResolution = false;
        [Tooltip("显示刷新率")] public bool ShowRefreshRate = false;
        [Tooltip("打印平均帧率日志")] public bool FPSLog = false;

        [Header("根据帧数自动切换画质")]
        [Tooltip("根据帧数自动切换画质")] public bool autoAdjustQualityLevel = true;
        [Tooltip("切换时间")] public int autoAdjustQualityLevelTime = 60;
        [Tooltip("帧率超范围计时")] static float deltaTime = 0;
        internal static float autoAdjustQualityLevelTimer { get { return deltaTime; } }
        [Tooltip("帧率范围")] public ValueRange adjustFPSRange = new Vector2(24, 59);
        [Tooltip("限制帧率")] public int LockFrameRate = 60;
        internal static long BuildSize = 0;
        /// <summary>
        /// 切换画质
        /// </summary>
        public MyEvent OnChangeQualityLevel;
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
            OnChangeQualityLevel = ChangeQualityLevel;

            string xmlData = GetFile.LoadXmlData("AutoAdjustQualityLevel", Application.streamingAssetsPath + "/Setting.xml", "Data", true);
            if (!string.IsNullOrWhiteSpace(xmlData))
            {
                if (xmlData.ToBoolean())
                    autoAdjustQualityLevel = true;
                else if (xmlData.ContainIgnoreCase("False") || xmlData.ContainIgnoreCase("No") || xmlData == "0")
                    autoAdjustQualityLevel = false;
            }
            /*xmlData = GetFile.LoadXmlData("BuildSize", Application.dataPath + "/BuildData.xml", "Root", true);
            if (!string.IsNullOrWhiteSpace(xmlData))
            {
                int size = xmlData.ToInteger(-1);
                if (size > 0)
                    BuildSize = size;
            }*/
            xmlData = GetFile.LoadXmlData("AutoAdjustQualityLevelTime", Application.streamingAssetsPath + "/Setting.xml", "Data", true);
            if (!string.IsNullOrWhiteSpace(xmlData))
            {
                int time = xmlData.ToInteger(-1);
                if (time > 0)
                    autoAdjustQualityLevelTime = time;
            }
            /*else if (!Application.isEditor && BuildSize > 0)
            {
                ValueAdjust.ConvertSize(BuildSize, 2, out double size, out string unit);
                autoAdjustQualityLevelTime = (20 + (size / 10f).ToInteger()).Clamp(30);
            }*/
            if (autoAdjustQualityLevel)
            {
                xmlData = GetFile.LoadXmlData("AutoAdjustQualityLevelRange", Application.streamingAssetsPath + "/Setting.xml", "Data", true);
                if (!string.IsNullOrWhiteSpace(xmlData))
                {
                    ValueRange range = xmlData.ToVector2();
                    if (range.Length > 1)
                        adjustFPSRange = range;
                }
                if (adjustFPSRange.Length < 1)
                    autoAdjustQualityLevel = false;
            }
        }
        /// <summary>
        /// 切换画质
        /// </summary>
        void ChangeQualityLevel()
        {
            /*switch (QualitySettings.GetQualityLevel())
            {
                default:
                    break;
            }*/
        }
        private void Reset()
        {
            if (!FPSText)
                FPSText = this.GetComponent<Text>();
        }
        private void Start()
        {
            RefreshTargetFPS();
        }
        /// <summary>
        /// 设置目标帧率
        /// </summary>
        /// <param name="num"></param>
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
            if (Input.GetKeyUp(KeyCode.F3))//按F3限制帧数为30;
            {
                RefreshTargetFPS(30);
                QualitySettings.vSyncCount = 2;
            }
            if (Input.GetKeyUp(KeyCode.F4))//按F4限制帧数为60;
            {
                RefreshTargetFPS(60);
                QualitySettings.vSyncCount = 1;
            }
            if (Input.GetKeyUp(KeyCode.F5))//按F5不限制帧数;
            {
                Application.targetFrameRate = -1;
                QualitySettings.vSyncCount = 0;
            }
            if (FPSText)
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
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPaused)
                return;
#endif
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
                FPSText.text = Output(ShowQualityLevel, ShowResolution, ShowRefreshRate);
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
        }
        /// <summary>
        /// 输出显示
        /// </summary>
        /// <param name="QualityLevel">画质等级</param>
        /// <param name="Resolution">分辨率</param>
        /// <param name="RefreshRate">刷新率</param>
        /// <returns></returns>
        public string Output(bool QualityLevel, bool Resolution, bool RefreshRate)
        {
            string result = string.Format("FPS：{0} ", m_FPS.ToString("F0"));
            if (QualityLevel)
                result += string.Format(" L{0} ", QualitySettings.GetQualityLevel());
            if (Resolution)
                result += DisplayData.GetResolution(RefreshRate);
            return result;
        }
        /// <summary>
        /// 设置画质等级
        /// </summary>
        /// <param name="level"></param>
        public static void SetQualityLevel(int level = 0)
        {
            deltaTime = 0;
            QualitySettings.vSyncCount = 0;
            QualitySettings.SetQualityLevel(level, false);
            if (instance && instance.OnChangeQualityLevel != null)
                instance.OnChangeQualityLevel();
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
            if (instance && instance.OnChangeQualityLevel != null)
                instance.OnChangeQualityLevel();
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
}
namespace RyuGiKenEditor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(FPSShow), true)]
    public class FPSShowEditor : Editor
    {
        bool ShowInspector = false;
        SerializedProperty hide;
        SerializedProperty UnscaledDeltaTime;
        SerializedProperty FPSLog;
        SerializedProperty ShowQualityLevel;
        SerializedProperty ShowResolution;
        SerializedProperty ShowRefreshRate;
        SerializedProperty LockFrameRate;
        SerializedProperty autoAdjustQualityLevel;
        SerializedProperty autoAdjustQualityLevelTime;
        SerializedProperty adjustFPSRange;
        void OnEnable()
        {
            hide = serializedObject.FindProperty("hide");
            UnscaledDeltaTime = serializedObject.FindProperty("UnscaledDeltaTime");
            FPSLog = serializedObject.FindProperty("FPSLog");
            ShowQualityLevel = serializedObject.FindProperty("ShowQualityLevel");
            ShowResolution = serializedObject.FindProperty("ShowResolution");
            ShowRefreshRate = serializedObject.FindProperty("ShowRefreshRate");
            LockFrameRate = serializedObject.FindProperty("LockFrameRate");
            autoAdjustQualityLevel = serializedObject.FindProperty("autoAdjustQualityLevel");
            autoAdjustQualityLevelTime = serializedObject.FindProperty("autoAdjustQualityLevelTime");
            adjustFPSRange = serializedObject.FindProperty("adjustFPSRange");
        }
        public override void OnInspectorGUI()
        {
            string[] Name = new string[12];
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
                    Name[5] = "显示画质等级";
                    Name[6] = "显示分辨率";
                    Name[7] = "显示刷新率";
                    Name[8] = "限制帧率";
                    Name[9] = "根据帧数自动切换画质";
                    Name[10] = "切换时间";
                    Name[11] = "最大帧率范围";
                    break;
                default:
                    Name[0] = "Show Inspector";
                    Name[1] = "Display";
                    Name[2] = "FPS：";
                    Name[3] = "Unscaled DeltaTime";
                    Name[4] = "Log";
                    Name[5] = "Show QualityLevel";
                    Name[6] = "Show Resolution";
                    Name[7] = "Show RefreshRate";
                    Name[8] = "Limit FrameRate";
                    Name[9] = "Auto Adjust QualityLevel";
                    Name[10] = "Switch Time";
                    Name[11] = "FrameRate Range";
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
                ShowQualityLevel.boolValue = EditorGUILayout.Toggle(Name[5], ShowQualityLevel.boolValue);
                ShowResolution.boolValue = EditorGUILayout.Toggle(Name[6], ShowResolution.boolValue);
                ShowRefreshRate.boolValue = EditorGUILayout.Toggle(Name[7], ShowRefreshRate.boolValue);
                LockFrameRate.intValue = EditorGUILayout.DelayedIntField(Name[8], LockFrameRate.intValue.Clamp(-1));
                autoAdjustQualityLevel.boolValue = EditorGUILayout.ToggleLeft(Name[9], autoAdjustQualityLevel.boolValue);
                if (fps.autoAdjustQualityLevel)
                {
                    autoAdjustQualityLevelTime.intValue = EditorGUILayout.DelayedIntField(Name[10], autoAdjustQualityLevelTime.intValue.Clamp(0));
                    SerializedProperty range = adjustFPSRange.FindPropertyRelative("range");
                    Vector2 temp = EditorGUILayout.Vector2Field(Name[11], range.vector2Value).Clamp(Vector2.one, Vector2.one * 600);
                    range.vector2Value = temp.ToInteger();
                    EditorGUILayout.Slider(FPSShow.autoAdjustQualityLevelTimer.Abs(), 0, fps.autoAdjustQualityLevelTime);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
