﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using RyuGiKen.Tools;
namespace RyuGiKenEditor.Tools
{
    /// <summary>
    /// 导出前检测版本号
    /// </summary>
    public class CheckVersionOnBuild
    {
        [DidReloadScripts]
        static void OnScriptsEditOver()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OverridesBuildPlayer);
        }
        static void OverridesBuildPlayer(BuildPlayerOptions buildPlayerOptions)
        {
            if (CheckVersionInput(buildPlayerOptions.options))
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
        }
        static bool CheckVersionInput(BuildOptions buildOptions)
        {
            string input = RyuGiKen.Tools.Version.SetVersionNumber(Application.version);
            if ((buildOptions & BuildOptions.Development) == BuildOptions.Development)
            {
                switch (Version.SignType)//前缀
                {
                    case Version.VersionSignType.None:
                    case Version.VersionSignType.SamePrefix:
                        break;
                    case Version.VersionSignType.BothPrefix:
                    case Version.VersionSignType.BothPostfix:
                        input = input.Replace(Version.ReleaseSign, Version.DebugSign);
                        break;
                    case Version.VersionSignType.SamePrefixAndOnlyDebugPostfix:
                    case Version.VersionSignType.OnlyDebugPostfix:
                        input = input.Replace(Version.DebugSign, "");
                        break;
                }
            }
            string str = "构建前请确定版本号\n设置版本号为：" + Application.version + "\n";
            if (PlayerSettings.bundleVersion != input)
            {
                str += "被矫正为：" + input + "\n";
            }
            str += "确实使用此版本号输出吗？";
            int result = 0;
            /*if (PlayerSettings.bundleVersion != input)
            {
                result = EditorUtility.DisplayDialogComplex("提示", str, "确定", "不矫正", "取消");

            }
            else*/
            {
                result = EditorUtility.DisplayDialog("提示", str, "确定", "取消") ? 0 : 1;
            }
            switch (result)
            {
                case 0:
                    PlayerSettings.bundleVersion = input;
                    break;
                case 1:
                    return false;
                    break;
                case 2:
                    break;
            }
            return true;
        }
    }
}
