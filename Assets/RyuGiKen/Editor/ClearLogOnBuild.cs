using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
/// <summary>
/// 构建时清空日志
/// </summary>
public class ClearLogOnBuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get; private set; }
    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("构建时清空日志");
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);
        try
        {
            File.WriteAllText(Application.streamingAssetsPath + "/Log.txt", string.Empty, Encoding.UTF8);
            string temp = "";
            for (int i = 9; i > 0; i--)
            {
                string fileName = string.Format("{0}/Log{1}.txt", Application.streamingAssetsPath, i.ToString("D2"));
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    temp += "\n移除 " + fileName;
                }
            }
            if (!string.IsNullOrWhiteSpace(temp))
                Debug.Log(temp);
        }
        catch { }
        try
        {
            File.WriteAllText(Application.streamingAssetsPath + "/FpsRecord.txt", string.Empty, Encoding.UTF8);
        }
        catch { }
        try
        {
            string ScreenShotPath = new DirectoryInfo(Application.dataPath).Parent.FullName + "/ScreenShot";
            foreach (FileInfo file in RyuGiKen.GetFile.GetFileInfoAll(ScreenShotPath))
            {
                file.Delete();
            }
            new DirectoryInfo(ScreenShotPath).Delete();
        }
        catch { }
    }
}