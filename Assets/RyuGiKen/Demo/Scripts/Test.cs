using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Tools;
public class Test : MonoBehaviour
{
    public static Test instance;
    public Text[] m_Text;
    public InputField[] ResolutionInput;
    public Dropdown FullScreenToggle;
    public bool PrintOnScreen;
    public List<string> PrintData;
    [SerializeField] LineRenderer[] lineRenderers;
    public DrawData drawer;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        ShowSystemInfo();
        Invoke(nameof(UpdateResolutionInput), 2f);
        SmoothingLines();
    }

    void Update()
    {
        m_Text[1].text = ValueAdjust.ShowTime(Time.time);
        drawer.RecordData = ValueAdjust.ToList(FPSShow.Data_FPS.ToArray());
    }
    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Alpha1))//切换主屏
        {
            if (SetDisplay.instance)
                SetDisplay.instance.SetPosition(0, 0);
            //SwitchDisplay(1);
            Invoke(nameof(UpdateResolutionInput), 0.5f);
        }
        if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Alpha2))//切换副屏
        {
            if (SetDisplay.instance)
                SetDisplay.instance.SetPosition(int.Parse(ResolutionInput[2].text), int.Parse(ResolutionInput[3].text));
            //SwitchDisplay(2);
            Invoke(nameof(UpdateResolutionInput), 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ShowSystemInfo();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowScreenInfo();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            PrintOnScreen = !PrintOnScreen;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SmoothingLines();
        }

        m_Text[0].text = "";
        if (PrintOnScreen)
        {
            if (PrintData.Count > 10)
            {
                for (int i = PrintData.Count - 10; i < PrintData.Count; i++)
                    m_Text[0].text += "\n" + PrintData[i];
            }
            else
            {
                for (int i = 0; i < PrintData.Count; i++)
                    m_Text[0].text += "\n" + PrintData[i];
            }
            if (PrintData.Count > 30)
                for (int i = 0; i < 20; i++)
                    PrintData.RemoveAt(0);
        }
        else
        {
            PrintData.Clear();
        }
    }
    void ShowSystemInfo()
    {
        string systemInfo = ""
//+ " deviceModel(设备的模型或模式): " + SystemInfo.deviceModel + " \r\n"
//+ " deviceType(设备类型): " + SystemInfo.deviceType + " \r\n"
//+ " deviceUniqueIdentifier(设备的唯一标识符): " + SystemInfo.deviceUniqueIdentifier + " \r\n"
+ " graphicsDeviceName(显卡的名称)： " + SystemInfo.graphicsDeviceName + "  ID： " + SystemInfo.graphicsDeviceID + " \r\n"
//+ " graphicsDeviceType(显卡的类型): " + SystemInfo.graphicsDeviceType + " \r\n"
//+ " graphicsDeviceVendor(显卡的供应商)： " + SystemInfo.graphicsDeviceVendor + "  ID： " + SystemInfo.graphicsDeviceVendorID + " \r\n"
//+ " graphicsDeviceVersion(显卡的类型和版本): " + SystemInfo.graphicsDeviceVersion + " \r\n"
//+ " graphicsMemorySize(显存大小): " + SystemInfo.graphicsMemorySize + " \r\n"
//+ " graphicsMultiThreaded(是否支持多线程渲染): " + SystemInfo.graphicsMultiThreaded + " \r\n"
//+ " graphicsShaderLevel(显卡着色器的级别): " + SystemInfo.graphicsShaderLevel + " \r\n"
//+ " maxTextureSize(支持的最大纹理大小): " + SystemInfo.maxTextureSize + " \r\n"
//+ " npotSupport(GPU支持的NPOT纹理): " + SystemInfo.npotSupport + " \r\n"
//+ " operatingSystem(操作系统的版本名称): " + SystemInfo.operatingSystem + " \r\n"
//+ " operatingSystemFamily(操作系统系列): " + SystemInfo.operatingSystemFamily + " \r\n"
//+ " processorCount(当前处理器的数量): " + SystemInfo.processorCount + " \r\n"
//+ " processorFrequency(处理器的频率)： " + SystemInfo.processorFrequency + " \r\n"
+ " processorType(处理器的名称)： " + SystemInfo.processorType + " \r\n"
//+ " supportedRenderTargetCount(支持渲染多少目标纹理): " + SystemInfo.supportedRenderTargetCount + " \r\n"
+ " systemMemorySize(系统内存大小)： " + SystemInfo.systemMemorySize + " \r\n"
+ " displays(显示器数)： " + Display.displays.Length + " \r\n";

        PrintData.Add(systemInfo);
        UpdateResolutionInput();
        Debug_T.Log(systemInfo);
        ShowScreenInfo();
    }
    void ShowScreenInfo()
    {
        string Info = ""
            + " renderingWidth&Height(窗口宽高)： " + Display.main.renderingWidth + " x " + Display.main.renderingHeight + " \r\n"
            + " systemWidth&Height(屏幕实际宽高)： " + Display.main.systemWidth + " x " + Display.main.systemHeight + " \r\n"
            + " currentResolution(屏幕绘制分辨率)： " + Screen.currentResolution + " \r\n";
        PrintData.Add(Info);
        UpdateResolutionInput();
        Debug_T.Log(Info);
    }
    void SwitchDisplay(int index)
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera c in cameras)
        {
            c.targetDisplay = index;
        }
        Canvas[] canvas = FindObjectsOfType<Canvas>();
        foreach (Canvas c in canvas)
        {
            c.targetDisplay = index;
        }
    }
    void UpdateResolutionInput()
    {
        ResolutionInput[0].text = Display.main.renderingWidth.ToString();
        ResolutionInput[1].text = Display.main.renderingHeight.ToString();
        FullScreenToggle.value = (int)Screen.fullScreenMode;
    }
    /// <summary>
    /// 设置分辨率
    /// </summary>
    public void SetResolution()
    {
        int Width = -1;
        int Height = -1;
        int X = -1;
        int Y = -1;
        if (int.TryParse(ResolutionInput[0].text, out Width) && int.TryParse(ResolutionInput[1].text, out Height))
        {
            if (Width > 0 && Height > 0)
            {                
                if (int.TryParse(ResolutionInput[2].text, out X) && int.TryParse(ResolutionInput[3].text, out Y))
                {
                    if (X >= 0 && Y >= 0)
                        Display.main.SetParams(Width, Height, X, Y);
                    else
                        Display.main.SetRenderingResolution(Width, Height);
                }
                else
                Screen.SetResolution(Width, Height, (FullScreenMode)FullScreenToggle.value);
                Invoke(nameof(UpdateResolutionInput), 0.5f);
                ShowScreenInfo();
            }
        }
    }
    void SmoothingLines()
    {
        float[][] result = new float[lineRenderers.Length][];
        int count = 100;
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            result[i] = new float[count];
            lineRenderers[i].positionCount = count;
        }
        for (int i = 0; i < count; i++)
        {
            lineRenderers[0].SetPosition(i, new Vector3(i, Random.Range(0f, 10f), 0));
            result[0][i] = lineRenderers[0].GetPosition(i).y;
        }

        int size = 10;

        result[1] = ValueAdjust.Smoothing(result[0], 1);
        result[2] = ValueAdjust.Smoothing(ValueAdjust.Smoothing(result[0], 1), 1);
        result[3] = ValueAdjust.Smoothing(result[0], 5, size, true);
        float[] index = new float[(count - 1) * size + 1];
        for (int i = 0; i < (count - 1) * size + 1; i++)
        {
            index[i] = i * 1f / size;
        }
        result[4] = ValueAdjust.Smoothing(result[0], 5, size);

        lineRenderers[3].positionCount = (count - 1) * size + 1;
        for (int j = 1; j < 5; j++)
            for (int i = 0; i < count; i++)
            {
                lineRenderers[j].SetPosition(i, new Vector3(i, result[j][i], 0/*-j*/));
            }
        for (int i = 0; i < 5; i++)
            Debug.Log("Ave" + i + "  " + ValueAdjust.GetAverage(result[i]));
        //int k = 0;
        for (int i = 0; i < (count - 1) * size + 1; i++)
        {
            lineRenderers[3].SetPosition(i, new Vector3(i * 1f / size, result[3][i], 0));
            //if (Mathf.RoundToInt(result4[i].x) == result4[i].x)
            //{
            //    lineRenderers[4].SetPosition(k, new Vector3(result4[i].x, result4[i].y, -4));
            //    k++;
            //}
        }
    }
}
