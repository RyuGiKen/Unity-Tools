using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Xml;
#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
#endif
/// <summary>
/// RyuGiKen's Tools
/// <para>
/// git@github.com:RyuGiKen/Unity-Tools.git
/// </para>
/// </summary>
namespace RyuGiKen
{
    public delegate void MyEvent();
    /// <summary>
    /// 获取文件
    /// </summary>
    public static class GetFile
    {
        /// <summary>
        /// 读取配置参数
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static XmlDocument LoadXmlData(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;
            XmlDocument xmlDoc = null;
            XmlReader reader = null;
            try
            {
                if (File.Exists(filePath))
                {
                    xmlDoc = new XmlDocument();
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreComments = true;//忽略注释
                    reader = XmlReader.Create(filePath, settings);
                    xmlDoc.Load(reader);
                }
            }
            catch { }
            if (reader != null)
                reader.Close();
            return xmlDoc;
        }
        /// <summary>
        /// 读取配置参数
        /// </summary>
        /// <param name="NodeName"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="RootNodeName">根节点</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns></returns>
        public static string LoadXmlData(string NodeName, string filePath, string RootNodeName, bool IgnoreCase = false)
        {
            return LoadXmlData(new string[] { NodeName }, filePath, RootNodeName, IgnoreCase);
        }
        /// <summary>
        /// 读取配置参数
        /// </summary>
        /// <param name="NodeName"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="RootNodeName">根节点</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns></returns>
        public static string LoadXmlData(string[] NodeName, string filePath, string RootNodeName, bool IgnoreCase = false)
        {
            if (NodeName == null || NodeName.Length < 1 || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(RootNodeName))
                return null;
            string result = null;
            XmlReader reader = null;
            try
            {
                if (File.Exists(filePath))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreComments = true;//忽略注释
                    reader = XmlReader.Create(filePath, settings);
                    xmlDoc.Load(reader);
                    XmlNodeList node = xmlDoc.SelectSingleNode(RootNodeName).ChildNodes;
                    foreach (XmlElement x1 in node)
                    {
                        foreach (string nodeName in NodeName)
                        {
                            if (!string.IsNullOrWhiteSpace(nodeName) && (x1.Name == nodeName || (IgnoreCase && x1.Name.ContainIgnoreCase(nodeName))))
                            {
                                result = x1.InnerText;
                                break;
                            }
                        }
                    }
                }
            }
            catch { }
            if (reader != null)
                reader.Close();
            return result;
        }
        /// <summary>
        /// 读取配置参数
        /// </summary>
        /// <param name="NodeName"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="RootNodeName">根节点</param>
        /// <param name="data"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        public static void LoadXmlData(string NodeName, string filePath, string RootNodeName, out string[] data, bool IgnoreCase = false)
        {
            LoadXmlData(new string[] { NodeName }, filePath, RootNodeName, out data, IgnoreCase);
        }
        /// <summary>
        /// 读取配置参数
        /// </summary>
        /// <param name="NodeName"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="RootNodeName">根节点</param>
        /// <param name="data"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns></returns>
        public static void LoadXmlData(string[] NodeName, string filePath, string RootNodeName, out string[] data, bool IgnoreCase = false)
        {
            if (NodeName == null || NodeName.Length < 1 || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(RootNodeName))
            {
                data = null;
                return;
            }
            List<string> result = new List<string>();
            XmlReader reader = null;
            try
            {
                if (File.Exists(filePath))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreComments = true;//忽略注释
                    reader = XmlReader.Create(filePath, settings);
                    xmlDoc.Load(reader);
                    XmlNodeList node = xmlDoc.SelectSingleNode(RootNodeName).ChildNodes;
                    foreach (XmlElement x1 in node)
                    {
                        foreach (string nodeName in NodeName)
                        {
                            if (!string.IsNullOrWhiteSpace(nodeName) && (x1.Name == nodeName || (IgnoreCase && x1.Name.ContainIgnoreCase(nodeName))))
                            {
                                result.Add(x1.InnerText);
                                //break;
                            }
                        }
                    }
                }
            }
            catch { }
            if (reader != null)
                reader.Close();
            data = result.ToArray();
        }
        /// <summary>
        /// 找出文件名相似的文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="data">比较结果</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns></returns>
        public static List<FileInfo> CompareFilesNameSimilarityRatio(this FileInfo[] files, bool IgnoreCase, out List<List<Tuple<FileInfo, Vector4>>> data, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            if (files == null || files.Length < 1)
            {
                data = new List<List<Tuple<FileInfo, Vector4>>>();
                return null;
            }
            List<FileInfo> result = GetFile.CompareFilesNameSimilarityRatio(files.ToList(), IgnoreCase, out data, prefixDelimiter, exclude1, exclude2, delimiter);
            return result;
        }
        /// <summary>
        /// 找出文件名相似的文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns></returns>
        public static List<FileInfo> CompareFilesNameSimilarityRatio(this FileInfo[] files, bool IgnoreCase, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            if (files == null || files.Length < 1)
                return null;
            List<FileInfo> result = GetFile.CompareFilesNameSimilarityRatio(files.ToList(), IgnoreCase, out List<List<Tuple<FileInfo, Vector4>>> data, prefixDelimiter, exclude1, exclude2, delimiter);
            return result;
        }
        /// <summary>
        /// 找出文件名相似的文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns></returns>
        public static List<FileInfo> CompareFilesNameSimilarityRatio(this List<FileInfo> files, bool IgnoreCase, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            if (files == null || files.Count < 1)
                return null;
            List<FileInfo> result = GetFile.CompareFilesNameSimilarityRatio(files, IgnoreCase, out List<List<Tuple<FileInfo, Vector4>>> data, prefixDelimiter, exclude1, exclude2, delimiter);
            return result;
        }
        /// <summary>
        /// 找出文件名相似的文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="data">比较结果</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns></returns>
        public static List<FileInfo> CompareFilesNameSimilarityRatio(this List<FileInfo> files, bool IgnoreCase, out List<List<Tuple<FileInfo, Vector4>>> data, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            data = new List<List<Tuple<FileInfo, Vector4>>>();
            if (files == null || files.Count < 1)
                return null;
            List<FileInfo> result = new List<FileInfo>();
            for (int i = 0; i < files.Count; i++)
            {
                List<Tuple<FileInfo, Vector4>> temp = new List<Tuple<FileInfo, Vector4>>();
                temp.Add(new Tuple<FileInfo, Vector4>(files[i], Vector4.one));
                for (int j = 0; j < files.Count; j++)
                {
                    if (i == j)
                        continue;
                    GetFile.CompareFileNameSimilarityRatio(files[i], files[j], IgnoreCase, out Vector4 radio, prefixDelimiter, exclude1, exclude2, delimiter);
                    if ((radio.x > 0.5f && radio.y > 0.5f && radio.z > 0.5f && radio.w > 0.5f) ||
                        ((radio.x >= 0.7f || radio.y >= 0.7f || radio.z >= 0.7f || radio.w >= 0.7f) && radio.w > 0) ||
                        (radio.z > 0.8f))
                    {
                        temp.Add(new Tuple<FileInfo, Vector4>(files[j], radio));
                        //Count++;
                    }
                }
                Thread.Sleep(0);
                if (temp.Count > 1)
                {
                    data.Add(temp);
                    for (int k = 0; k < temp.Count; k++)
                    {
                        result.Add(temp[k].Item1);
                        files.Remove(temp[k].Item1);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 找出目录中文件名相似的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="data">比较结果</param>
        /// <param name="types">文件类型</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns></returns>
        public static List<FileInfo> CompareFilesNameSimilarityRatio(string path, bool IgnoreCase, out List<List<Tuple<FileInfo, Vector4>>> data, string[] types = null, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            List<FileInfo> files = GetFile.GetFileInfoAll(path, types);
            if (files == null || files.Count < 1)
            {
                data = new List<List<Tuple<FileInfo, Vector4>>>();
                return null;
            }
            List<FileInfo> result = GetFile.CompareFilesNameSimilarityRatio(files, IgnoreCase, out data, prefixDelimiter, exclude1, exclude2, delimiter);
            return result;
        }
        /// <summary>
        /// 找出目录中文件名相似的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="types">文件类型</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns></returns>
        public static List<FileInfo> CompareFilesNameSimilarityRatio(string path, bool IgnoreCase, string[] types = null, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            List<FileInfo> result = GetFile.CompareFilesNameSimilarityRatio(path, IgnoreCase, out List<List<Tuple<FileInfo, Vector4>>> data, types, prefixDelimiter, exclude1, exclude2, delimiter);
            return result;
        }
        /// <summary>
        /// 找出两个目录中文件名相似的文件
        /// </summary>
        /// <param name="path1">路径</param>
        /// <param name="path2">路径</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="data">比较结果</param>
        /// <param name="types">文件类型</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns></returns>
        public static List<FileInfo> CompareFilesNameSimilarityRatio(string path1, string path2, bool IgnoreCase, out List<List<Tuple<FileInfo, Vector4>>> data, string[] types = null, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            data = new List<List<Tuple<FileInfo, Vector4>>>();
            if (string.IsNullOrWhiteSpace(path1) || string.IsNullOrWhiteSpace(path2))
                return null;

            List<FileInfo> files = GetFile.GetFileInfoAll(path1, types).AddList(GetFile.GetFileInfoAll(path2, types));
            if (files == null || files.Count < 1)
                return null;
            List<FileInfo> result = GetFile.CompareFilesNameSimilarityRatio(files, IgnoreCase, out data, prefixDelimiter, exclude1, exclude2, delimiter);
            return result;
        }
        /// <summary>
        /// 找出两个目录中文件名相似的文件
        /// </summary>
        /// <param name="path1">路径</param>
        /// <param name="path2">路径</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="types">文件类型</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns></returns>
        public static List<FileInfo> CompareFilesNameSimilarityRatio(string path1, string path2, bool IgnoreCase, string[] types = null, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            if (string.IsNullOrWhiteSpace(path1) || string.IsNullOrWhiteSpace(path2))
                return null;

            List<FileInfo> result = GetFile.CompareFilesNameSimilarityRatio(path1, path2, IgnoreCase, out List<List<Tuple<FileInfo, Vector4>>> data, types, prefixDelimiter, exclude1, exclude2, delimiter);
            return result;
        }
        /// <summary>
        /// 比较文件名相似度
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns>[0，1]</returns>
        public static float CompareFileNameSimilarityRatio(this FileInfo file1, FileInfo file2, bool IgnoreCase, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            GetFile.CompareFileNameSimilarityRatio(file1, file2, IgnoreCase, out Vector4 Ratio, prefixDelimiter, exclude1, exclude2, delimiter);
            ValueAdjust.FindMinAndMax(new float[] { Ratio.x, Ratio.y, Ratio.z, Ratio.w }, out float min, out float max);
            return max;
        }
        /// <summary>
        /// 比较文件名相似度
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="Ratio"></param>
        /// <param name="prefixDelimiter">前缀分割符</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <param name="delimiter">组合分割符</param>
        /// <returns>[0，1]</returns>
        public static void CompareFileNameSimilarityRatio(this FileInfo file1, FileInfo file2, bool IgnoreCase, out Vector4 Ratio, string prefixDelimiter = null, string[] exclude1 = null, string exclude2 = null, string delimiter = null)
        {
            string name1 = file1.GetFileNameWithOutType();
            string name2 = file2.GetFileNameWithOutType();
            if (!string.IsNullOrWhiteSpace(prefixDelimiter))//移除前缀
            {
                if (name1.IndexOf(prefixDelimiter) >= 0)
                    name1 = name1.Remove(0, name1.IndexOf(prefixDelimiter) + prefixDelimiter.Length);
                if (name2.IndexOf(prefixDelimiter) >= 0)
                    name2 = name2.Remove(0, name2.IndexOf(prefixDelimiter) + prefixDelimiter.Length);
            }
            Ratio.x = ValueAdjust.GetCompoundSimilarityRatio(name1, name2, IgnoreCase, exclude1, exclude2);//按组合比较相似度
            if (exclude1 != null)//前置过滤
            {
                for (int i = 0; i < exclude1.Length; i++)
                {
                    if (string.IsNullOrEmpty(exclude1[i]))
                        continue;
                    if (IgnoreCase)
                    {
                        name1 = name1.ToLower().Replace(exclude1[i].ToLower(), "");
                        name2 = name2.ToLower().Replace(exclude1[i].ToLower(), "");
                    }
                    else
                    {
                        name1 = name1.Replace(exclude1[i], "");
                        name2 = name2.Replace(exclude1[i], "");
                    }
                }
            }
            //移除括号内容
            if (string.IsNullOrWhiteSpace(delimiter))
                delimiter = "~~～〜\"“”()（）[]【】「」『』<>《》";
            string temp1 = name1.Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0].ReplaceAny(exclude2, "");
            string temp2 = name2.Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0].ReplaceAny(exclude2, "");
            Ratio.y = ValueAdjust.GetSequenceSimilarityRatio(temp1, temp2, IgnoreCase);
            //后置过滤
            name1 = name1.ReplaceAny(exclude2, "");
            name2 = name2.ReplaceAny(exclude2, "");
            Ratio.z = ValueAdjust.GetSimilarityRatio(name1, name2, IgnoreCase);
            Ratio.w = ValueAdjust.GetSequenceSimilarityRatio(name1, name2, IgnoreCase);
        }
        /// <summary>
        /// 找出两个目录中文件名不同的文件
        /// </summary>
        /// <param name="path1">路径</param>
        /// <param name="path2">路径</param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="types">文件类型</param>
        /// <returns></returns>
        public static FileInfo[] FindDifferenceCompareFilesName(string path1, string path2, bool IgnoreCase, string[] types = null)
        {
            if (string.IsNullOrWhiteSpace(path1) || string.IsNullOrWhiteSpace(path2))
                return null;

            List<FileInfo> files1 = GetFile.GetFileInfoAll(path1, types);
            List<FileInfo> files2 = GetFile.GetFileInfoAll(path2, types);

            if (files1 == null || files1.Count < 1 || files2 == null || files2.Count < 1)
                return null;

            List<FileInfo> result = new List<FileInfo>();
            for (int k = 0; k < 2; k++)
            {
                int count1 = k == 0 ? files1.Count : files2.Count;
                int count2 = k == 0 ? files2.Count : files1.Count;
                List<FileInfo> data1 = k == 0 ? files1 : files2;
                List<FileInfo> data2 = k == 0 ? files2 : files1;
                for (int i = 0; i < count1; i++)
                {
                    string name1 = data1[i].GetFileNameWithOutType();
                    bool NoSame = true;
                    for (int j = 0; j < count2; j++)
                    {
                        string name2 = data2[j].GetFileNameWithOutType();
                        //if (ValueAdjust.GetSequenceSimilarityRatio(name1, name2, IgnoreCase) > 0.9f)
                        if (IgnoreCase && name1.ToLower() == name2.ToLower())
                        {
                            NoSame = false;
                            break;
                        }
                        else if (!IgnoreCase && name1 == name2)
                        {
                            NoSame = false;
                            break;
                        }
                    }
                    if (NoSame)
                        result.Add(data1[i]);
                    Thread.Sleep(0);
                }
            }
            return result.ToArray().ClearRepeatingItem();
        }
        /// <summary>
        /// 文件名更换前缀
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="oldPrefix">原前缀</param>
        /// <param name="newPrefix">新前缀</param>
        /// <param name="OnlyReplace">仅替换原前缀</param>
        /// <returns></returns>
        public static string RenamePrefix(string fileName, string oldPrefix = "", string newPrefix = "", bool OnlyReplace = false)
        {
            if (string.IsNullOrWhiteSpace(fileName) || oldPrefix == null || newPrefix == null)
                return null;
            string newName = fileName;
            bool hasOldPrefix = false;
            if (!string.IsNullOrEmpty(oldPrefix))
            {
                int i = newName.IndexOf(oldPrefix, StringComparison.OrdinalIgnoreCase);
                if (i == 0)
                {
                    hasOldPrefix = true;
                    newName = newName.Remove(0, oldPrefix.Length);
                }
            }
            if (!string.IsNullOrEmpty(newPrefix))
            {
                int i = newName.IndexOf(newPrefix, StringComparison.OrdinalIgnoreCase);
                if (i != 0 && (hasOldPrefix || !OnlyReplace))
                {
                    newName = newName.Insert(0, newPrefix);
                }
            }
            return newName;
        }
        /// <summary>
        /// 文件名更换前缀
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="oldPrefix">原前缀</param>
        /// <param name="newPrefix">新前缀</param>
        /// <param name="OnlyReplace">仅替换原前缀</param>
        public static void FileRenamePrefix(this FileSystemInfo file, string oldPrefix = "", string newPrefix = "", bool OnlyReplace = false)
        {
            if (file == null)
                return;
            string newName = RenamePrefix(file.Name, oldPrefix, newPrefix, OnlyReplace);
            if (newName != file.Name && !string.IsNullOrWhiteSpace(newName))
                file.FileRename(newName);
        }
        /// <summary>
        /// 文件名更换前缀
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="oldPrefix">原前缀</param>
        /// <param name="newPrefix">新前缀</param>
        /// <param name="OnlyReplace">仅替换原前缀</param>
        public static void FilesRenamePrefix(this FileSystemInfo[] files, string oldPrefix = "", string newPrefix = "", bool OnlyReplace = false)
        {
            if (files == null || files.Length < 1)
                return;
            foreach (FileInfo file in files)
            {
                file.FileRenamePrefix(oldPrefix, newPrefix, OnlyReplace);
            }
        }
        /// <summary>
        /// 文件名更换前缀
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="oldPrefix">原前缀</param>
        /// <param name="newPrefix">新前缀</param>
        /// <param name="OnlyReplace">仅替换原前缀</param>
        public static void FilesRenamePrefix(this List<FileSystemInfo> files, string oldPrefix = "", string newPrefix = "", bool OnlyReplace = false)
        {
            if (files == null || files.Count < 1)
                return;
            foreach (FileInfo file in files)
            {
                file.FileRenamePrefix(oldPrefix, newPrefix, OnlyReplace);
            }
        }
        /// <summary>
        /// 文件名更换后缀
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="oldPostfix">原后缀</param>
        /// <param name="newPostfix">新后缀</param>
        /// <param name="OnlyReplace">仅替换原后缀</param>
        /// <returns></returns>
        public static string RenamePostfix(string fileName, string oldPostfix = "", string newPostfix = "", bool OnlyReplace = false)
        {
            if (string.IsNullOrWhiteSpace(fileName) || oldPostfix == null || newPostfix == null)
                return null;
            GetFileNameAndType(fileName, out string newName, out string type);
            bool hasOldPostfix = false;

            if (!string.IsNullOrEmpty(oldPostfix))
            {
                int i = newName.IndexOf(oldPostfix, StringComparison.OrdinalIgnoreCase);
                if (i >= newName.Length - newPostfix.Length - 1)
                {
                    hasOldPostfix = true;
                    newName = newName.Remove(i, oldPostfix.Length);
                }
            }
            if (!string.IsNullOrEmpty(newPostfix))
            {
                int i = newName.IndexOf(newPostfix, StringComparison.OrdinalIgnoreCase);
                if (i < newName.Length - newPostfix.Length - 1 && (hasOldPostfix || !OnlyReplace))
                {
                    newName = newName.Insert((newName.Length).Clamp(0), newPostfix);
                }
            }
            return newName + (type.Length < 1 ? "" : ("." + type));
        }
        /// <summary>
        /// 文件名更换后缀
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="oldPostfix">原后缀</param>
        /// <param name="newPostfix">新后缀</param>
        /// <param name="OnlyReplace">仅替换原后缀</param>
        public static void FileRenamePostfix(this FileSystemInfo file, string oldPostfix = "", string newPostfix = "", bool OnlyReplace = false)
        {
            if (file == null)
                return;
            string newName = RenamePostfix(file.Name, oldPostfix, newPostfix, OnlyReplace);
            if (newName != file.Name && !string.IsNullOrWhiteSpace(newName))
                file.FileRename(newName);
        }
        /// <summary>
        /// 文件名更换后缀
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="oldPostfix">原后缀</param>
        /// <param name="newPostfix">新后缀</param>
        /// <param name="OnlyReplace">仅替换原后缀</param>
        public static void FilesRenamePostfix(this FileSystemInfo[] files, string oldPostfix = "", string newPostfix = "", bool OnlyReplace = false)
        {
            if (files == null || files.Length < 1)
                return;
            foreach (FileInfo file in files)
            {
                file.FileRenamePostfix(oldPostfix, newPostfix, OnlyReplace);
            }
        }
        /// <summary>
        /// 文件名更换后缀
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="oldPostfix">原后缀</param>
        /// <param name="newPostfix">新后缀</param>
        /// <param name="OnlyReplace">仅替换原后缀</param>
        public static void FilesRenamePostfix(this List<FileSystemInfo> files, string oldPostfix = "", string newPostfix = "", bool OnlyReplace = false)
        {
            if (files == null || files.Count < 1)
                return;
            foreach (FileInfo file in files)
            {
                file.FileRenamePostfix(oldPostfix, newPostfix, OnlyReplace);
            }
        }
        /// <summary>
        /// 文件名更换前后缀
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="oldPrefix">原前缀</param>
        /// <param name="newPrefix">新前缀</param>
        /// <param name="oldPostfix">原后缀</param>
        /// <param name="newPostfix">新后缀</param>
        /// <param name="OnlyReplace">仅替换原前后缀</param>
        /// <returns></returns>
        public static string RenamePrefixPostfix(string fileName, string oldPrefix = "", string newPrefix = "", string oldPostfix = "", string newPostfix = "", bool OnlyReplace = false)
        {
            return RenamePostfix(RenamePrefix(fileName, oldPrefix, newPrefix, OnlyReplace), oldPostfix, newPostfix, OnlyReplace);
        }
        /// <summary>
        /// 文件名更换前后缀
        /// </summary>
        /// <param name="oldPrefix">原前缀</param>
        /// <param name="newPrefix">新前缀</param>
        /// <param name="oldPostfix">原后缀</param>
        /// <param name="newPostfix">新后缀</param>
        /// <param name="OnlyReplace">仅替换原前后缀</param>
        public static void FileRenamePrefixPostfix(this FileSystemInfo file, string oldPrefix = "", string newPrefix = "", string oldPostfix = "", string newPostfix = "", bool OnlyReplace = false)
        {
            if (file == null)
                return;
            string newName = RenamePrefixPostfix(file.Name, oldPrefix, newPrefix, oldPostfix, newPostfix, OnlyReplace);
            if (newName != file.Name && !string.IsNullOrWhiteSpace(newName))
                file.FileRename(newName);
        }
        /// <summary>
        /// 文件名更换前后缀
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="oldPostfix">原后缀</param>
        /// <param name="newPostfix">新后缀</param>
        /// <param name="OnlyReplace">仅替换原后缀</param>
        public static void FilesRenamePrefixPostfix(this FileSystemInfo[] files, string oldPrefix = "", string newPrefix = "", string oldPostfix = "", string newPostfix = "", bool OnlyReplace = false)
        {
            if (files == null || files.Length < 1)
                return;
            foreach (FileInfo file in files)
            {
                file.FileRenamePrefixPostfix(oldPrefix, newPrefix, oldPostfix, newPostfix, OnlyReplace);
            }
        }
        /// <summary>
        /// 文件名更换前后缀
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="oldPostfix">原后缀</param>
        /// <param name="newPostfix">新后缀</param>
        /// <param name="OnlyReplace">仅替换原后缀</param>
        public static void FilesRenamePrefixPostfix(this List<FileSystemInfo> files, string oldPrefix = "", string newPrefix = "", string oldPostfix = "", string newPostfix = "", bool OnlyReplace = false)
        {
            if (files == null || files.Count < 1)
                return;
            foreach (FileInfo file in files)
            {
                file.FileRenamePrefixPostfix(oldPrefix, newPrefix, oldPostfix, newPostfix, OnlyReplace);
            }
        }
        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="newName">新名称</param>
        public static void FileRename<T>(this T file, string newName) where T : FileSystemInfo
        {
            if (file == null || string.IsNullOrWhiteSpace(newName))
                return;
            try
            {
                if (newName != file.Name)
                {
                    if (file is FileInfo)
                        (file as FileInfo).MoveTo((file as FileInfo).DirectoryName + "\\" + newName);
                    else if (file is DirectoryInfo)
                        (file as DirectoryInfo).MoveTo((file as DirectoryInfo).Parent.FullName + "\\" + newName);
                }
            }
            catch { }
        }
        /// <summary>
        /// 文件批量重命名
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="newNames">新名称</param>
        public static void FilesRename(this FileSystemInfo[] files, string[] newNames)
        {
            if (files == null || files.Length < 1 || newNames == null || newNames.Length < 1)
                return;
            for (int i = 0; i < Math.Min(files.Length, newNames.Length); i++)
            {
                files[i].FileRename(newNames[i]);
            }
        }
        /// <summary>
        /// 文件批量重命名
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="newNames">新名称</param>
        public static void FilesRename(this List<FileSystemInfo> files, string[] newNames)
        {
            if (files == null || files.Count < 1 || newNames == null || newNames.Length < 1)
                return;
            for (int i = 0; i < Math.Min(files.Count, newNames.Length); i++)
            {
                files[i].FileRename(newNames[i]);
            }
        }
        /// <summary>
        /// 文件列表反向排序
        /// </summary>
        /// <param name="files"></param>
        public static void SortFilesReverse(this List<FileInfo> files)
        {
            if (files.Count > 0)
                files.Sort(delegate (FileInfo row02, FileInfo row01)
                {
                    int result = row01.Name.CompareTo(row02.Name);
                    if (result == 0)
                    {
                        int result2 = row01.FullName.CompareTo(row02.FullName);
                        return result2;
                    }
                    return result;
                });
        }
        /// <summary>
        /// 文件列表排序
        /// </summary>
        /// <param name="files"></param>
        public static void SortFiles(this List<FileInfo> files)
        {
            if (files.Count > 0)
                files.Sort(delegate (FileInfo row01, FileInfo row02)
                {
                    int result = row01.Name.CompareTo(row02.Name);
                    if (result == 0)
                    {
                        int result2 = row01.FullName.CompareTo(row02.FullName);
                        return result2;
                    }
                    return result;
                });
        }
        /// <summary>
        /// 获得指定路径下所有子目录文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfoAll(string path, string type = "")
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            List<List<FileInfo>> files = new List<List<FileInfo>>();
            files.Add(GetFileInfos(path, type));
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                files.Add(GetFileInfoAll(d.FullName, type));
            }
            List<FileInfo> result = ValueAdjust.ListAddition(files);
            return result;
        }
        /// <summary>
        /// 获得指定路径下所有子目录文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="types">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfoAll(string path, string[] types)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            List<List<FileInfo>> files = new List<List<FileInfo>>();
            files.Add(GetFileInfos(path, types));
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                files.Add(GetFileInfoAll(d.FullName, types));
            }
            List<FileInfo> result = ValueAdjust.ListAddition(files);
            return result;
        }
        /// <summary>
        /// 获得指定路径下所有子目录文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="types">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfoAllWithOutType(string path, string[] types)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            List<List<FileInfo>> files = new List<List<FileInfo>>();
            files.Add(GetFileInfosWithOutType(path, types));
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                files.Add(GetFileInfoAllWithOutType(d.FullName, types));
            }
            List<FileInfo> result = ValueAdjust.ListAddition(files);
            return result;
        }
        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfos(string path, string type = "")
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();
            DirectoryInfo root = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in root.GetFiles())
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    files.Add(file);
                }
                else if (file.JudgeFileType(type))
                {
                    files.Add(file);
                }
            }
            return files;
        }
        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="types">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfos(string path, string[] types)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            if (types == null || types.Length < 1)
                return GetFileInfos(path);

            DirectoryInfo root = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in root.GetFiles())
            {
                foreach (string type in types)
                {
                    if (file.JudgeFileType(type))
                    {
                        files.Add(file);
                    }
                }
            }
            return files;
        }
        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfosWithOutType(string path, string type)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            if (string.IsNullOrWhiteSpace(type))
                return GetFileInfos(path);

            DirectoryInfo root = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in root.GetFiles())
            {
                if (file.JudgeFileType(type))
                    files.Add(file);
            }
            return files;
        }
        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="types">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfosWithOutType(string path, string[] types)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Directory.GetCurrentDirectory();

            if (types == null || types.Length < 1)
                return GetFileInfos(path);

            DirectoryInfo root = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in root.GetFiles())
            {
                bool EnabledFileType = true;
                foreach (string type in types)
                {
                    if (string.IsNullOrWhiteSpace(type))
                        continue;
                    if (file.JudgeFileType(type))
                    {
                        EnabledFileType = false;
                    }
                }
                if (EnabledFileType)
                    files.Add(file);
            }
            return files;
        }
        /// <summary>
        /// 获得文件名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string[] GetFileName<T>(this T[] files) where T : FileSystemInfo
        {
            if (files == null || files.Length < 1)
                return null;
            List<string> fileNames = new List<string>();
            foreach (T file in files)
            {
                fileNames.Add(file.Name);
            }
            return fileNames.ToArray();
        }
        /// <summary>
        /// 获得文件名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string[] GetFileName<T>(this List<T> files) where T : FileSystemInfo
        {
            if (files == null || files.Count < 1)
                return null;
            return files.ToArray().GetFileName();
        }
        /// <summary>
        /// 获得文件路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string[] GetFileFullName<T>(this T[] files) where T : FileSystemInfo
        {
            if (files == null || files.Length < 1)
                return null;
            List<string> fileNames = new List<string>();
            foreach (T file in files)
            {
                fileNames.Add(file.FullName);
            }
            return fileNames.ToArray();
        }
        /// <summary>
        /// 获得文件路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string[] GetFileFullName<T>(this List<T> files) where T : FileSystemInfo
        {
            if (files == null || files.Count < 1)
                return null;
            return files.ToArray().GetFileFullName();
        }
        /// <summary>
        /// 获得文件名
        /// </summary>
        /// <returns></returns>
        public static string GetFileNameWithOutType(this FileInfo file)
        {
            if (file == null)
                return null;
            int TypeIndex = file.Name.LastIndexOf('.');
            string newName = file.Name;
            if (TypeIndex >= 0)
            {
                newName = newName.Remove(TypeIndex);
            }
            return newName;
        }
        /// <summary>
        /// 获得文件名
        /// </summary>
        /// <returns></returns>
        public static string GetFileNameWithOutType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;
            int TypeIndex = fileName.LastIndexOf('.');
            string newName = fileName;
            if (TypeIndex >= 0)
            {
                newName = newName.Remove(TypeIndex);
            }
            return newName;
        }
        /// <summary>
        /// 获得文件名和类型
        /// </summary>
        /// <returns></returns>
        public static void GetFileNameAndType(this FileInfo file, out string name, out string type)
        {
            name = type = "";
            if (file == null)
                return;
            int TypeIndex = file.Name.LastIndexOf('.');
            type = "";
            name = file.Name;
            if (TypeIndex >= 0)
            {
                type = file.Name.Substring(TypeIndex).Replace(".", "");
                name = name.Remove(TypeIndex);
            }
        }
        /// <summary>
        /// 获得文件名和类型
        /// </summary>
        /// <returns></returns>
        public static void GetFileNameAndType(string fileName, out string name, out string type)
        {
            name = type = "";
            if (string.IsNullOrWhiteSpace(fileName))
                return;
            int TypeIndex = fileName.LastIndexOf('.');
            type = "";
            name = fileName;
            if (TypeIndex >= 0)
            {
                type = fileName.Substring(TypeIndex).Replace(".", "");
                name = name.Remove(TypeIndex);
            }
        }
        /// <summary>
        /// 获得文件类型
        /// </summary>
        /// <returns></returns>
        public static string GetFileType(this FileInfo file)
        {
            if (file == null)
                return null;
            int TypeIndex = file.Name.LastIndexOf('.');
            string type = "";
            if (TypeIndex >= 0)
            {
                type = file.Name.Substring(TypeIndex).Replace(".", "");
            }
            return type;
        }
        /// <summary>
        /// 获得文件类型
        /// </summary>
        /// <returns></returns>
        public static string GetFileType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;
            int TypeIndex = fileName.LastIndexOf('.');
            string type = "";
            if (TypeIndex >= 0)
            {
                type = fileName.Substring(TypeIndex).Replace(".", "");
            }
            return type;
        }
        /// <summary>
        /// 获得文件类型
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string[] GetFileType(this FileInfo[] files)
        {
            if (files == null || files.Length < 1)
                return null;
            List<string> fileTypes = new List<string>();
            foreach (FileInfo file in files)
            {
                fileTypes.Add(file.GetFileType());
            }
            return fileTypes.ToArray();
        }
        /// <summary>
        /// 获得文件类型
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string[] GetFileType(this List<FileInfo> files)
        {
            if (files == null || files.Count < 1)
                return null;
            return files.ToArray().GetFileType();
        }
        /// <summary>
        /// 从文件名判断类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool JudgeFileType(string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
                return false;
            try
            {
                string FileTypeName = name.Substring(name.Length - type.Length - 1);
                return FileTypeName.LastIndexOf("." + type, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 从文件名判断类型
        /// </summary>
        /// <param name="file"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool JudgeFileType(this FileInfo file, string type)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.Name) || string.IsNullOrWhiteSpace(type))
                return false;
            return JudgeFileType(file.Name, type);
        }
    }
    /// <summary>
    /// 对象调整
    /// </summary>
    public static class ObjectAdjust
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 获取对象所有子对象
        /// </summary>
        /// <param name="GO"></param>
        /// <returns></returns>
        public static GameObject[] GetChildren(this GameObject GO)
        {
            List<GameObject> result = new List<GameObject>();
            if (GO.transform.childCount > 0)
                foreach (Transform child in GO.transform)
                {
                    result.Add(child.gameObject);
                }
            return ValueAdjust.ToArray(result.ClearRepeatingItem());
        }
        /// <summary>
        /// 获取对象所有子对象
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform[] GetChildren(this Transform transform)
        {
            List<Transform> result = new List<Transform>();
            if (transform.childCount > 0)
                foreach (Transform child in transform)
                {
                    result.Add(child);
                }
            return ValueAdjust.ToArray(result.ClearRepeatingItem());
        }
        /// <summary>
        /// 获取对象所有子孙
        /// </summary>
        /// <param name="GO"></param>
        /// <returns></returns>
        public static GameObject[] GetDescendants(this GameObject GO)
        {
            List<GameObject> result = new List<GameObject>();
            result.AddList(GO.GetChildren().ToList());
            foreach (Transform child in GO.transform)
            {
                result.AddList(child.gameObject.GetDescendants().ToList());
            }
            return ValueAdjust.ToArray(result.ClearRepeatingItem());
        }
        /// <summary>
        /// 获取对象所有子孙
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform[] GetDescendants(this Transform transform)
        {
            List<Transform> result = new List<Transform>();
            result.AddList(transform.GetChildren().ToList());
            foreach (Transform child in transform)
            {
                result.AddList(child.GetDescendants().ToList());
            }
            return ValueAdjust.ToArray(result.ClearRepeatingItem());
        }
        /// <summary>
        /// Transform复制
        /// </summary>
        /// <param name="Get">读取对象</param>
        /// <param name="Set">设置对象</param>
        /// <param name="localPosition">是否更新局部位置</param>
        /// <param name="position">是否更新全局位置</param>
        /// <param name="localEulerAngles">是否更新局部旋转</param>
        /// <param name="rotation">是否更新全局旋转</param>
        /// <param name="localScale">是否更新局部缩放</param>
        /// <param name="lossyScale">是否更新全局缩放</param>
        public static void Sync(Transform Get, Transform Set, bool localPosition, bool position, bool localEulerAngles, bool rotation, bool localScale, bool lossyScale)
        {
            if (!Get || !Set)
                return;

            if (localPosition)
                Set.localPosition = Get.localPosition;
            if (position)
                Set.position = Get.position;
            if (localEulerAngles)
                Set.localEulerAngles = Get.localEulerAngles;
            if (rotation)
                Set.rotation = Get.rotation;
            if (localScale)
                Set.localScale = Get.localScale;
            if (lossyScale)
            {
                SetGlobalScale(Set, Get.lossyScale);
            }
        }
        /// <summary>
        /// 设置全局缩放
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="lossyScale"></param>
        public static void SetGlobalScale(this Transform transform, Vector3 lossyScale)
        {
            if (transform)
            {
                if (transform.parent)
                    transform.localScale = new Vector3(lossyScale.x / transform.parent.lossyScale.x, lossyScale.y / transform.parent.lossyScale.y, lossyScale.z / transform.parent.lossyScale.z);
                else
                    transform.localScale = lossyScale;
            }
        }
        /// <summary>
        /// 重设父对象和位置角度缩放
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent">父对象</param>
        public static void SetParentReset(this Transform transform, Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }
        /// <summary>
        /// 重设父对象和位置角度
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent">父对象</param>
        /// <param name="ResetLocalPosition"></param>
        /// <param name="ResetLocalEulerAngles"></param>
        public static void SetParentReset(this Transform transform, Transform parent, Vector3 ResetLocalPosition, Vector3 ResetLocalEulerAngles)
        {
            transform.SetParent(parent);
            transform.localPosition = ResetLocalPosition;
            transform.localEulerAngles = ResetLocalEulerAngles;
        }
        /// <summary>
        /// 重设父对象和位置角度缩放
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent">父对象</param>
        /// <param name="ResetLocalPosition"></param>
        /// <param name="ResetLocalEulerAngles"></param>
        /// <param name="ResetLocalScale"></param>
        public static void SetParentReset(this Transform transform, Transform parent, Vector3 ResetLocalPosition, Vector3 ResetLocalEulerAngles, Vector3 ResetLocalScale)
        {
            transform.SetParent(parent);
            transform.localPosition = ResetLocalPosition;
            transform.localEulerAngles = ResetLocalEulerAngles;
            transform.localScale = ResetLocalScale;
        }
        /// <summary>
        /// 重设父对象和位置
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent">父对象</param>
        /// <param name="ResetLocalPosition"></param>
        public static void SetParentResetPosition(this Transform transform, Transform parent, Vector3 ResetLocalPosition)
        {
            transform.SetParent(parent);
            transform.localPosition = ResetLocalPosition;
        }
        /// <summary>
        /// 重设父对象和角度
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent">父对象</param>
        /// <param name="ResetLocalEulerAngles"></param>
        public static void SetParentResetRotation(this Transform transform, Transform parent, Vector3 ResetLocalEulerAngles)
        {
            transform.SetParent(parent);
            transform.localEulerAngles = ResetLocalEulerAngles;
        }
        /// <summary>
        /// 重设父对象和角度
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent">父对象</param>
        /// <param name="ResetLocalRotation"></param>
        public static void SetParentResetRotation(this Transform transform, Transform parent, Quaternion ResetLocalRotation)
        {
            transform.SetParent(parent);
            transform.localRotation = ResetLocalRotation;
        }
        /// <summary>
        /// 重设父对象和缩放
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parent">父对象</param>
        /// <param name="ResetLocalScale"></param>
        public static void SetParentResetScale(this Transform transform, Transform parent, Vector3 ResetLocalScale)
        {
            transform.SetParent(parent);
            transform.localScale = ResetLocalScale;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="GO"></param>
        public static void Destroy<T>(this T[] GO) where T : Object
        {
            foreach (var item in GO)
            {
                if (Application.isPlaying)
                    Object.Destroy(item);
                else
                    Object.DestroyImmediate(item);
            }
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="GO"></param>
        public static void Destroy<T>(this List<T> GO) where T : Object
        {
            foreach (var item in GO)
            {
                if (Application.isPlaying)
                    Object.Destroy(item);
                else
                    Object.DestroyImmediate(item);
            }
        }
        /// <summary>
        /// GameObject设置活动状态
        /// </summary>
        /// <param name="component"></param>
        /// <param name="active"></param>
        public static void SetActive(this Component component, bool active)
        {
            try
            {
                component.gameObject.SetActive(active);
            }
            catch { }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetActive(this GameObject[] GO, bool active)
        {
            foreach (var item in GO)
            {
                if (item)
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
                if (item)
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
                if (item)
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
                if (item)
                    item.gameObject.SetActive(active);
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetEnable(this Collider[] GO, bool active)
        {
            foreach (var item in GO)
            {
                if (item)
                    item.enabled = active;
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetEnable(this List<Collider> GO, bool active)
        {
            foreach (var item in GO)
            {
                if (item)
                    item.enabled = active;
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetEnable(this Renderer[] GO, bool active)
        {
            foreach (var item in GO)
            {
                if (item)
                    item.enabled = active;
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetEnable(this List<Renderer> GO, bool active)
        {
            foreach (var item in GO)
            {
                if (item)
                    item.enabled = active;
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetEnable<T>(this T[] GO, bool active) where T : Behaviour
        {
            foreach (var item in GO)
            {
                if (item)
                    item.enabled = active;
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void SetEnable<T>(this List<T> GO, bool active) where T : Behaviour
        {
            foreach (var item in GO)
            {
                if (item)
                    item.enabled = active;
            }
        }
        /// <summary>
        /// 数组转列表，转组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this GameObject[] array) where T : Component
        {
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
                if (array[i] && array[i].GetComponent<T>())
                    list.Add(array[i].GetComponent<T>());
                else
                    list.Add(null);
            return list;
        }
        /// <summary>
        /// 列表转数组，转组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this List<GameObject> list) where T : Component
        {
            T[] array = new T[list.Count];
            for (int i = 0; i < array.Length; i++)
                if (list[i] && list[i].GetComponent<T>())
                    array[i] = list[i].GetComponent<T>();
            return array;
        }
        /// <summary>
        /// 转组件
        /// </summary>
        /// <param name="m_List"></param>
        /// <returns></returns>
        public static List<T> ToComponent<T>(this List<Component> m_List) where T : Component
        {
            List<T> list = new List<T>();
            for (int i = 0; i < m_List.Count; i++)
                if (m_List[i] && m_List[i].GetComponent<T>())
                    list.Add(m_List[i].GetComponent<T>());
                else
                    list.Add(null);
            return list;
        }
        /// <summary>
        /// 转组件
        /// </summary>
        /// <param name="m_Array"></param>
        /// <returns></returns>
        public static T[] ToComponent<T>(this Component[] m_Array) where T : Component
        {
            T[] array = new T[m_Array.Length];
            for (int i = 0; i < array.Length; i++)
                if (m_Array[i] && m_Array[i].GetComponent<T>())
                    array[i] = m_Array[i].GetComponent<T>();
            return array;
        }
        /// <summary>
        /// 转对象
        /// </summary>
        /// <param name="m_List"></param>
        /// <returns></returns>
        public static List<GameObject> ToGameObject<T>(this List<T> m_List) where T : Component
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < m_List.Count; i++)
                if (m_List[i] && m_List[i].gameObject)
                    list.Add(m_List[i].gameObject);
                else
                    list.Add(null);
            return list;
        }
        /// <summary>
        /// 转对象
        /// </summary>
        /// <param name="m_Array"></param>
        /// <returns></returns>
        public static GameObject[] ToGameObject<T>(this T[] m_Array) where T : Component
        {
            GameObject[] array = new GameObject[m_Array.Length];
            for (int i = 0; i < array.Length; i++)
                if (m_Array[i] && m_Array[i].gameObject)
                    array[i] = m_Array[i].gameObject;
            return array;
        }
        /// <summary>
        /// 获取所有子对象的材质
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="UseSharedMaterials"></param>
        /// <returns></returns>
        public static Material[] GetAllMaterials(this Transform transform, bool UseSharedMaterials = false)
        {
            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>().ClearRepeatingItem();
            List<Material> materials = new List<Material>();
            foreach (Renderer renderer in renderers)
            {
                if (UseSharedMaterials)
                    materials.AddList(renderer.sharedMaterials.ToList());
                else
                    materials.AddList(renderer.materials.ToList());
            }
            return materials.ClearRepeatingItem().ToArray();
        }
        /// <summary>
        /// 批量设置材质
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static void SetMaterials(this Material[] materials, Material material)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                try
                {
                    materials[i] = material;
                }
                catch { }
            }
        }
        /// <summary>
        /// 批量设置材质颜色
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="color"></param>
        /// <param name="ColorShaderName"></param>
        public static void SetMaterialsColor(this Material[] materials, Color color, string ColorShaderName)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(ColorShaderName))
                    {
                        materials[i].color = color;
                        materials[i].SetColor("_BaseColor", color);
                    }
                    else
                    {
                        materials[i].SetColor(ColorShaderName, color);
                    }
                }
                catch { }
            }
        }
#endif
    }
#if !UNITY_EDITOR && !UNITY_STANDALONE
    public static class Random
    {
        static int RandomSeed { set; get; }
        /// <summary>
        /// 返回[0，1]范围的小数
        /// </summary>
        public static float value { get { return (new System.Random(RandomSeed++).Next((int)Math.Pow(10, 8))) * (float)Math.Pow(0.1f, 8); } }
        /// <summary>
        /// 返回[min，max)范围的整数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(int min, int max)
        {
            int Min, Max;
            if (min > max)
            {
                Min = max;
                Max = min;
            }
            else
            {
                Min = min;
                Max = max;
            }
            return new System.Random(RandomSeed++).Next(Max - Min) + Min;
        }
        /// <summary>
        /// 返回[min，max)范围的小数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Range(float min, float max)
        {
            float Min, Max;
            if (min > max)
            {
                Min = max;
                Max = min;
            }
            else
            {
                Min = min;
                Max = max;
            }
            return Random.value * (Max - Min) + Min;
        }
    }
    public class Object
    {
        public static implicit operator bool(Object exists) { return exists != null; }
    }
    public static class Mathf
    {
        public static float Rad2Deg = 360f / (float)(Math.PI * 2);
        public static float Deg2Rad = (float)(Math.PI * 2) / 360f;
        /// <summary>
        /// 限制值范围[0，1]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp01(float value)
        {
            return Mathf.Clamp(value, 0, 1f);
        }
        /// <summary>
        /// 限制值范围
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }
        /// <summary>
        /// 限制值范围
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }
        /// <summary>
        /// 返回小于或等于指定小数的最大整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FloorToInt(float value)
        {
            return (int)Math.Floor(value);
        }
        /// <summary>
        /// 返回大于或等于指定小数的最小整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CeilToInt(float value)
        {
            return (int)Math.Ceiling(value);
        }
    }
    public struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;
        public Color(float Red, float Green, float Blue, float Alpha = 1)
        {
            this.r = Red;
            this.g = Green;
            this.b = Blue;
            this.a = Alpha;
        }
    }
    public struct Vector2
    {
        public float x;
        public float y;
        public static Vector2 zero = new Vector2(0, 0);
        public static Vector2 one = new Vector2(1, 1);
        public Vector2(float X = 0, float Y = 0)
        {
            this.x = X;
            this.y = Y;
        }
        public static implicit operator Vector3(Vector2 v) { return new Vector3(v.x, v.y, 0); }
        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }
        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }
    }
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public static Vector3 zero = new Vector3(0, 0, 0);
        public static Vector3 one = new Vector3(1, 1, 1);
        public Vector3(float X = 0, float Y = 0, float Z = 0)
        {
            this.x = X;
            this.y = Y;
            this.z = Z;
        }
        public static implicit operator Vector2(Vector3 v) { return new Vector2(v.x, v.y); }
        public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z + b.z); }
    }
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
    /// <summary>
    /// 多维数组
    /// </summary>
    [System.Serializable]
    public class MultiArray
    {
        [System.Serializable]
        public struct SecondArray
        {
            public Component[] items;
            public int Length
            {
                get
                {
                    return items == null ? 0 : items.Length;
                }
            }
        }
        public SecondArray[] items;
        public Component GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public Component GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default(Component);
            }
        }
        public Component[] AllItems
        {
            get
            {
                List<Component> result = new List<Component>();
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].Length > 0)
                        result.AddList(items[i].items.ToList());
                }
                return result.ToArray();
            }
        }
        public Component this[int index]
        {
            get
            {
                try
                {
                    return AllItems[index];
                }
                catch
                {
                    return default(Component);
                }
            }
        }
        public int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    result += items[i].Length;
                }
                return result;
            }
        }
    }
#endif
    /// <summary>
    /// 速度类型
    /// </summary>
    public enum SpeedType
    {
        /// <summary>
        /// 米每秒
        /// </summary>
        MPS,
        /// <summary>
        /// 千米每小时
        /// </summary>
        KPH,
        /// <summary>
        /// 英里每小时
        /// </summary>
        MPH,
    }
    /// <summary>
    /// 散布模式
    /// </summary>
    public enum IntersperseMode
    {
        /// <summary>
        /// 平均
        /// </summary>
        Average = 0,
        /// <summary>
        /// 集中中心
        /// </summary>
        Centre = 1,
        /// <summary>
        /// 集中边缘
        /// </summary>
        Edge = 2,
        /// <summary>
        /// 偏离中心与边缘
        /// </summary>
        NotCentreAndEdge = 3,
        /// <summary>
        /// 集中中心与边缘
        /// </summary>
        CentreAndEdge = 4
    }
    /// <summary>
    /// 数值调整
    /// </summary>
    public static class ValueAdjust
    {
        /// <summary>
        /// 转速转角速度
        /// </summary>
        /// <param name="RPM">转速(圈每分)</param>
        /// <param name="SpeedFactor">减速比</param>
        /// <returns>角速度(度每秒)</returns>
        public static float RPMtoSpeed(float RPM, float SpeedFactor)
        {
            if (float.IsNaN(SpeedFactor) || SpeedFactor <= 0)
                return 0;
            return RPM * (6f / SpeedFactor);
        }
        /// <summary>
        /// 角速度转转速
        /// </summary>
        /// <param name="AngleSpeed">角速度(度每秒)</param>
        /// <param name="SpeedFactor">减速比</param>
        /// <returns>转速(圈每分)</returns>
        public static float SpeedtoRPM(float AngleSpeed, float SpeedFactor)
        {
            if (float.IsNaN(SpeedFactor) || SpeedFactor <= 0)
                return 0;
            return AngleSpeed / (6f / SpeedFactor);
        }
        /// <summary>
        /// 转换速度
        /// </summary>
        /// <param name="type">原单位</param>
        /// <param name="TargetType">目标单位</param>
        /// <param name="value">原数值</param>
        /// <returns></returns>
        public static float ConvertSpeed(SpeedType type, SpeedType TargetType, float value)
        {
            float result = 0;
            switch (type)
            {
                case SpeedType.MPH:
                    switch (TargetType)
                    {
                        case SpeedType.MPH:
                            result = value;
                            break;
                        case SpeedType.KPH:
                            result = value * 1.609344f;
                            break;
                        case SpeedType.MPS:
                            result = value / 2.23693629f;
                            break;
                    }
                    break;
                case SpeedType.KPH:
                    switch (TargetType)
                    {
                        case SpeedType.MPH:
                            result = value * 0.6213711f;
                            break;
                        case SpeedType.KPH:
                            result = value;
                            break;
                        case SpeedType.MPS:
                            result = value / 3.6f;
                            break;
                    }
                    break;
                case SpeedType.MPS:
                    switch (TargetType)
                    {
                        case SpeedType.MPH:
                            result = value * 2.23693629f;
                            break;
                        case SpeedType.KPH:
                            result = value * 3.6f;
                            break;
                        case SpeedType.MPS:
                            result = value;
                            break;
                    }
                    break;
            }
            return result;
        }
#if UNITY_STANDALONE || UNITY_EDITOR
        /// <summary>
        /// 四元数转四维坐标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector4 ToVector4(this Quaternion value)
        {
            return new Vector4(value.x, value.y, value.z, value.w);
        }
        /// <summary>
        /// 二维坐标转四维坐标
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueZ"></param>
        /// <param name="valueW"></param>
        /// <returns></returns>
        public static Vector4 ToVector4(this Vector2 value, float valueZ = 0, float valueW = 0)
        {
            return new Vector4(value.x, value.y, valueZ, valueW);
        }
        /// <summary>
        /// 二维坐标数组转四维坐标数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueZ"></param>
        /// <param name="valueW"></param>
        /// <returns></returns>
        public static Vector4[] ToVector4(this Vector2[] value, float valueZ = 0, float valueW = 0)
        {
            Vector4[] result = new Vector4[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = value[i].ToVector4(valueZ, valueW);
            }
            return result;
        }
        /// <summary>
        /// 三维坐标转四维坐标
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueW"></param>
        /// <returns></returns>
        public static Vector4 ToVector4(this Vector3 value, float valueW = 0)
        {
            return new Vector4(value.x, value.y, valueW);
        }
        /// <summary>
        /// 三维坐标数组转四维坐标数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueW"></param>
        /// <returns></returns>
        public static Vector4[] ToVector4(this Vector3[] value, float valueW = 0)
        {
            Vector4[] result = new Vector4[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = value[i].ToVector4(valueW);
            }
            return result;
        }
        /// <summary>
        /// x,y,z,w数组转四维坐标数组
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static Vector4[] ToVector4(float[] x, float[] y, float[] z, float[] w)
        {
            FindMinAndMax(new int[] { x.Length, y.Length, z.Length, w.Length }, out int min, out int max);
            Vector4[] result = new Vector4[min];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Vector4(x[i], y[i], z[i], w[i]);
            }
            return result;
        }
#endif
        /// <summary>
        /// 二维坐标转三维坐标
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueZ"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this Vector2 value, float valueZ = 0)
        {
            return new Vector3(value.x, value.y, valueZ);
        }
        /// <summary>
        /// 二维坐标数组转三维坐标数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueZ"></param>
        /// <returns></returns>
        public static Vector3[] ToVector3(this Vector2[] value, float valueZ = 0)
        {
            Vector3[] result = new Vector3[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = value[i].ToVector3(valueZ);
            }
            return result;
        }
        /// <summary>
        /// x,y,z数组转三维坐标数组
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3[] ToVector3(float[] x, float[] y, float[] z)
        {
            Vector3[] result = new Vector3[Math.Min(Math.Min(x.Length, y.Length), z.Length)];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Vector3(x[i], y[i], z[i]);
            }
            return result;
        }
        /// <summary>
        /// 三维坐标转二维坐标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }
        /// <summary>
        /// x,y数组转二维坐标数组
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2[] ToVector2(float[] x, float[] y)
        {
            Vector2[] result = new Vector2[Math.Min(x.Length, y.Length)];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Vector2(x[i], y[i]);
            }
            return result;
        }
        /// <summary>
        /// 三维坐标数组转二维坐标数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueZ"></param>
        /// <returns></returns>
        public static Vector2[] ToVector2(this Vector3[] value)
        {
            Vector2[] result = new Vector2[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = value[i].ToVector2();
            }
            return result;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 全局方向转局部角度
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction">全局方向</param>
        /// <returns></returns>
        public static Vector3 DirectionToLocalEulerAngles(this Transform transform, Vector3 direction)
        {
            Vector3 LocalDirection = transform.InverseTransformDirection(direction);//局部方向
            Vector3 LocalEulerAngles = Vector3.zero;
            LocalEulerAngles.x = Mathf.Atan2(LocalDirection.y, LocalDirection.z) / Mathf.PI * 180;
            LocalEulerAngles.y = Mathf.Atan2(LocalDirection.x, LocalDirection.z) / Mathf.PI * 180;
            return LocalEulerAngles;
        }
        /// <summary>
        /// 方向转旋转
        /// </summary>
        /// <param name="direction">方向</param>
        /// <returns></returns>
        public static Quaternion DirectionToRotation(this Vector3 direction)
        {
            //return Quaternion.FromToRotation(Vector3.forward, direction);
            return Quaternion.LookRotation(direction);
        }
        /// <summary>
        /// 方向转旋转
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="direction">上方向</param>
        /// <returns></returns>
        public static Quaternion DirectionToRotation(this Vector3 direction, Vector3 up)
        {
            //return Quaternion.FromToRotation(Vector3.forward, direction);
            return Quaternion.LookRotation(direction, up);
        }
        /// <summary>
        /// 旋转转方向
        /// </summary>
        /// <param name="rotation">旋转</param>
        /// <returns></returns>
        public static Vector3 RotationToDirection(this Quaternion rotation)
        {
            return rotation * Vector3.forward;
            //Vector3 Angle = rotation.eulerAngles;
            //return Quaternion.AngleAxis(Angle.x, Vector3.right) * Quaternion.AngleAxis(Angle.y, Vector3.up) * Vector3.forward;
        }
        /// <summary>
        /// 直线旋转球形插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t">[0,1]</param>
        /// <returns></returns>
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
        {
            return Vector3.Slerp(a.RotationToDirection(), b.RotationToDirection(), t).DirectionToRotation();
        }
        /// <summary>
        /// 直线旋转球形插值
        /// </summary>
        /// <param name="a">欧拉角</param>
        /// <param name="b">欧拉角</param>
        /// <param name="t">[0,1]</param>
        /// <returns>欧拉角</returns>
        public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
        {
            return ValueAdjust.Slerp(Quaternion.Euler(a), Quaternion.Euler(b), t).eulerAngles;
        }
#endif
        /// <summary>
        /// 二维坐标数组批量设置单轴值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">XY</param>
        /// <returns></returns>
        public static Vector2[] SetVectorValue(Vector2[] array, int[] value, string type)
        {
            Vector2[] result = new Vector2[array.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = array[i];
                if (i < value.Length)
                {
                    switch (type)
                    {
                        case "x":
                        case "X":
                            result[i].x = value[i];
                            break;
                        case "y":
                        case "Y":
                            result[i].y = value[i];
                            break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 二维坐标数组批量设置单轴值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">XY</param>
        /// <returns></returns>
        public static Vector2[] SetVectorValue(Vector2[] array, float[] value, string type)
        {
            Vector2[] result = new Vector2[array.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = array[i];
                if (i < value.Length)
                {
                    switch (type)
                    {
                        case "x":
                        case "X":
                            result[i].x = value[i];
                            break;
                        case "y":
                        case "Y":
                            result[i].y = value[i];
                            break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 三维坐标数组批量设置单轴值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">XYZ</param>
        /// <returns></returns>
        public static Vector3[] SetVectorValue(Vector3[] array, int[] value, string type)
        {
            Vector3[] result = new Vector3[array.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = array[i];
                if (i < value.Length)
                {
                    switch (type)
                    {
                        case "x":
                        case "X":
                            result[i].x = value[i];
                            break;
                        case "y":
                        case "Y":
                            result[i].y = value[i];
                            break;
                        case "z":
                        case "Z":
                            result[i].z = value[i];
                            break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 三维坐标数组批量设置单轴值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">XYZ</param>
        /// <returns></returns>
        public static Vector3[] SetVectorValue(Vector3[] array, float[] value, string type)
        {
            Vector3[] result = new Vector3[array.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = array[i];
                if (i < value.Length)
                {
                    switch (type)
                    {
                        case "x":
                        case "X":
                            result[i].x = value[i];
                            break;
                        case "y":
                        case "Y":
                            result[i].y = value[i];
                            break;
                        case "z":
                        case "Z":
                            result[i].z = value[i];
                            break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 二维坐标数组转单轴值数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">XY</param>
        /// <returns></returns>
        public static float[] GetVectorValue(this Vector2[] value, string type)
        {
            float[] result = new float[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                switch (type)
                {
                    case "x":
                    case "X":
                        result[i] = value[i].x;
                        break;
                    case "y":
                    case "Y":
                        result[i] = value[i].y;
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 三维坐标数组转单轴值数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">XYZ</param>
        /// <returns></returns>
        public static float[] GetVectorValue(this Vector3[] value, string type)
        {
            float[] result = new float[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                switch (type)
                {
                    case "x":
                    case "X":
                        result[i] = value[i].x;
                        break;
                    case "y":
                    case "Y":
                        result[i] = value[i].y;
                        break;
                    case "z":
                    case "Z":
                        result[i] = value[i].z;
                        break;
                }
            }
            return result;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 重复项置空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> SetRepeatingItemNull<T>(this List<T> list) where T : Object
        {
            if (list == null || list.Count < 1)
                return null;
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (i != j && list[i] && list[j] && list[i] == list[j])
                        list[i] = null;
                }
            }
            return list;
        }
        /// <summary>
        /// 清空重复项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> ClearRepeatingItem<T>(this List<T> list) where T : Object
        {
            if (list == null || list.Count < 1)
                return null;
            return ValueAdjust.ClearNullItem(list.SetRepeatingItemNull());
        }
#endif
        /// <summary>
        /// 清空重复项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] ClearRepeatingItem<T>(this T[] array)
        {
            if (array == null || array.Length < 1)
                return null;
            T[] temp = array.ClearNullItem();
            if (temp == null || temp.Length < 1)
                return null;
            List<T> result = new List<T>();
            result.Add(temp[0]);
            for (int i = 1; i < temp.Length; i++)
            {
                bool Repeating = false;
                for (int j = 0; j < result.Count; j++)
                {
                    if (array is Object[])
                    {
                        if ((temp[i] as Object) == (result[j] as Object))
                        {
                            Repeating = true;
                            break;
                        }
                    }
                    else if (temp[i].Equals(result[j]))
                    {
                        Repeating = true;
                        break;
                    }
                }
                if (!Repeating)
                    result.Add(temp[i]);
            }
            return result.ToArray();
        }
        /// <summary>
        /// 排除指定项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, T[] exclude)
        {
            if (array == null || array.Length < 1)
                return null;
            if (exclude == null)
                return array;
            List<T> result = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                bool Repeating = false;
                for (int j = 0; j < exclude.Length; j++)
                {
                    if (array is Object[])
                    {
                        if ((array[i] as Object) == (exclude[j] as Object))
                        {
                            Repeating = true;
                            break;
                        }
                    }
                    else if (array[i].Equals(exclude[j]))
                    {
                        Repeating = true;
                        break;
                    }
                }
                if (!Repeating)
                    result.Add(array[i]);
            }
            return result.ToArray();
        }
        /// <summary>
        /// 排除指定项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, T exclude)
        {
            if (array == null || array.Length < 1)
                return null;
            if (exclude == null)
                return array;
            List<T> result = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array is Object[])
                {
                    if ((array[i] as Object) != (exclude as Object))
                    {
                        result.Add(array[i]);
                    }
                }
                else if (!array[i].Equals(exclude))
                {
                    result.Add(array[i]);
                }
            }
            return result.ToArray();
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 清空空项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="changeSelf">改变自身</param>
        /// <returns></returns>
        public static List<T> ClearNullItem<T>(this List<T> list, bool changeSelf = true) where T : Object
        {
            if (list == null || list.Count < 1)
                return null;
            List<T> result = new List<T>(list.Count);
            if (!changeSelf)
            {
                for (int i = 0; i < list.Count; i++)
                    result[i] = list[i];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (result[i] == null)
                        result.RemoveAt(i);
                }
                return result;
            }
            else
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i] == null)
                        list.RemoveAt(i);
                }
                return list;
            }
        }
#endif
        /// <summary>
        /// 清空空项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] ClearNullItem<T>(this T[] array)
        {
            if (array == null || array.Length < 1)
                return null;
            List<T> result = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array is Object[])
                {
                    if (array[i] as Object)
                        result.Add(array[i]);
                }
                else if (array is string[])
                {
                    if (!string.IsNullOrEmpty(array[i] as string))
                        result.Add(array[i]);
                }
                else if (array[i] != null)
                    result.Add(array[i]);
            }
            return result.ToArray();
        }
        /// <summary>
        /// 计算字符串编辑距离
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns></returns>
        public static int CompareStrSimilarity(this string str, string target, bool IgnoreCase)
        {
            int[][] d; // 矩阵
            int n = str.Length;
            int m = target.Length;
            int i; // 遍历str的
            int j; // 遍历target的
            char ch1; // str的
            char ch2; // target的
            int temp; // 记录相同字符,在某个矩阵位置值的增量,不是0就是1
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            d = ValueAdjust.ConvertArray(new int[n + 1, m + 1]);
            for (i = 0; i <= n; i++)
            {
                // 初始化第一列
                d[i][0] = i;
            }

            for (j = 0; j <= m; j++)
            {
                // 初始化第一行
                d[0][j] = j;
            }

            for (i = 1; i <= n; i++)
            {
                // 遍历str
                ch1 = str[i - 1];
                // 去匹配target
                for (j = 1; j <= m; j++)
                {
                    ch2 = target[j - 1];
                    if (IgnoreCase)
                    {
                        temp = ValueAdjust.CompareIgnoreCase(ch1, ch2) ? 0 : 1;
                    }
                    else
                    {
                        temp = ch1 == ch2 ? 0 : 1;
                    }
                    // 左边+1,上边+1, 左上角+temp取最小
                    d[i][j] = Math.Min(Math.Min(d[i - 1][j] + 1, d[i][j - 1] + 1), d[i - 1][j - 1] + temp);
                }
            }
            return d[n][m];
        }
        /// <summary>
        /// 比较字符串相似度（编辑距离比较）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>[0，1]</returns>
        public static float GetSimilarityRatio(this string str, string target, bool IgnoreCase)
        {
            if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(target))
                return 1;
            else if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(target))
                return 0;
            float result = 0;
            if (IgnoreCase)
            {
                str = WindowsAPI.ChineseConverter.ToSimplified(str);
                target = WindowsAPI.ChineseConverter.ToSimplified(target);
            }
            if (Math.Max(str.Length, target.Length) == 0)
            {
                result = 1;
            }
            else
            {
                result = 1 - CompareStrSimilarity(str, target, IgnoreCase) * 1f / Math.Max(str.Length, target.Length);
            }
            return result;
        }
        /// <summary>
        /// 比较字符串相似度（构成比较）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>[0，1]</returns>
        public static float GetFormSimilarityRatio(this string str, string target, bool IgnoreCase)
        {
            if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(target))
                return 1;
            else if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(target))
                return 0;
            int length = Math.Max(str.Length, target.Length);
            int result = 0;
            if (IgnoreCase)
            {
                str = WindowsAPI.ChineseConverter.ToSimplified(str);
                target = WindowsAPI.ChineseConverter.ToSimplified(target);
            }
            for (int i = 0; i < length; i++)
            {
                if (i < str.Length && i < target.Length)
                {
                    if (IgnoreCase && str[i].CompareIgnoreCase(target[i]))
                        result++;
                    else if (!IgnoreCase && str[i] == target[i])
                        result++;
                }
            }
            return result * 1f / length;
        }
        /// <summary>
        /// 比较字符串相似度（顺序比较）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <returns>[0，1]</returns>
        public static float GetSequenceSimilarityRatio(this string str, string target, bool IgnoreCase)
        {
            if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(target))
                return 1;
            else if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(target))
                return 0;
            int length = Math.Max(str.Length, target.Length);
            int result = 0;
            if (IgnoreCase)
            {
                str = WindowsAPI.ChineseConverter.ToSimplified(str);
                target = WindowsAPI.ChineseConverter.ToSimplified(target);
            }
            for (int i = 0; i < length; i++)
            {
                if (i < str.Length && i < target.Length)
                {
                    if (IgnoreCase && str[i].CompareIgnoreCase(target[i]))
                        result++;
                    else if (!IgnoreCase && str[i] == target[i])
                        result++;
                    else
                        break;
                }
            }
            return result * 1f / length;
        }
        /// <summary>
        /// 比较字符串相似度（组合比较）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <param name="IgnoreCase">忽略大小写</param>
        /// <param name="exclude1">前置过滤字符串</param>
        /// <param name="exclude2">后置过滤字符</param>
        /// <returns>[0，1]</returns>
        public static float GetCompoundSimilarityRatio(this string str, string target, bool IgnoreCase, string[] exclude1 = null, string exclude2 = null)
        {
            if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(target))
                return 1;
            else if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(target))
                return 0;
            if (IgnoreCase)
            {
                str = WindowsAPI.ChineseConverter.ToSimplified(str);
                target = WindowsAPI.ChineseConverter.ToSimplified(target);
            }
            if (exclude1 != null && exclude1.Length > 0)
            {
                for (int i = 0; i < exclude1.Length; i++)
                {
                    if (string.IsNullOrEmpty(exclude1[i]))
                        continue;
                    if (IgnoreCase)
                    {
                        str = str.ToLower().Replace(exclude1[i].ToLower(), "");
                        target = target.ToLower().Replace(exclude1[i].ToLower(), "");
                    }
                    else
                    {
                        str = str.Replace(exclude1[i], "");
                        target = target.Replace(exclude1[i], "");
                    }
                }
            }

            string[] temp1 = str.SplitNumOrAlphabet(exclude2);
            string[] temp2 = target.SplitNumOrAlphabet(exclude2);
            //Debug.Log(ValueAdjust.PrintArray(temp1, true) + "\n" + ValueAdjust.PrintArray(temp2, true));
            int length = Math.Max(temp1.Length, temp2.Length);
            float result = 0;
            if (temp1.Length == 1 && temp2.Length == 1)
            {
                result = temp1[0].GetSequenceSimilarityRatio(temp2[0], IgnoreCase);
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    if (i < temp1.Length && i < temp2.Length)
                    {
                        result += temp1[i].GetSequenceSimilarityRatio(temp2[i], IgnoreCase);
                        //if (IgnoreCase && temp1[i].ToLower() == temp2[i].ToLower())
                        //    result++;
                        //else if (!IgnoreCase && temp1[i] == temp2[i])
                        //    result++;
                    }
                }
            }
            return result / length;
        }
        /// <summary>
        /// 字符串批量替换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string ReplaceAny(this string str, string from, string to)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(from))
                return str;
            string result = str;
            foreach (char c in from)
            {
                result = result.Replace(c.ToString(), to);
            }
            return result;
        }
        /// <summary>
        /// 字符串批量替换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string ReplaceAny(this string str, char[] from, string to)
        {
            if (string.IsNullOrEmpty(str) || from == null || from.Length < 1)
                return str;
            string result = str;
            foreach (char c in from)
            {
                result = result.Replace(c.ToString(), to);
            }
            return result;
        }
        /// <summary>
        /// 路径分离
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] SplitPath(string path)
        {
            string[] result = null;
            string[] temp1 = path.Split('\\');
            List<List<string>> temp2 = new List<List<string>>();
            for (int i = 0; i < temp1.Length; i++)
            {
                string[] temp3 = temp1[i].Split('/');
                temp2.Add(temp3.ToList());
            }
            result = temp2.ListAddition().ToArray().ClearNullItem<string>();
            return result;
        }
        /// <summary>
        /// 检查路径输入
        /// </summary>
        /// <param name="path">输入</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="createPath">如无则创建目录</param>
        /// <returns>输出</returns>
        public static string CheckFilePath(string path, string fileType, bool createPath)
        {
            return CheckPathInAssets(path, fileType, createPath, out string OutputPath, out string OutputFileName);
        }
        /// <summary>
        /// 检查路径输入
        /// </summary>
        /// <param name="path">输入</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="createPath">如无则创建目录</param>
        /// <param name="OutputPath">输出路径</param>
        /// <param name="OutputFileName">输出文件名</param>
        /// <returns>输出</returns>
        public static string CheckFilePath(string path, string fileType, bool createPath, out string OutputPath, out string OutputFileName)
        {
            string OutputFullName = "";
            OutputPath = "";
            OutputFileName = "";

            List<string> tempString = ValueAdjust.SplitPath(path).ToList();
            for (int i = 0; i < tempString.Count; i++)
            {
                if (i >= tempString.Count - 1)
                {
                    if (path[path.Length - 1] == '/' || path[path.Length - 1] == '\\')
                    {
                        OutputPath += tempString[i] + "\\";
                        OutputFileName = "";
                    }
                    else
                    {
                        OutputFileName = tempString[i];
                    }
                    if (tempString[tempString.Count - 1].LastIndexOf("." + fileType, StringComparison.OrdinalIgnoreCase) < 0)
                        OutputFileName += "." + fileType;
                    break;
                }
                else
                    OutputPath += tempString[i] + "\\";
            }
            OutputFullName = OutputPath + OutputFileName;
            //Debug.Log(OutputFullName);

            if (!Directory.Exists(OutputPath) && createPath)
                Directory.CreateDirectory(OutputPath);

            return OutputFullName;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 检查路径输入（工程根目录）
        /// </summary>
        /// <param name="path">输入</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="createPath">如无则创建目录</param>
        /// <returns>输出</returns>
        public static string CheckPathInRoot(string path, string fileType, bool createPath)
        {
            return CheckPathInRoot(path, fileType, createPath, out string OutputPath, out string OutputFileName);
        }
        /// <summary>
        /// 检查路径输入（工程根目录）
        /// </summary>
        /// <param name="path">输入</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="createPath">如无则创建目录</param>
        /// <param name="OutputPath">输出路径</param>
        /// <param name="OutputFileName">输出文件名</param>
        /// <returns>输出</returns>
        public static string CheckPathInRoot(string path, string fileType, bool createPath, out string OutputPath, out string OutputFileName)
        {
            string OutputFullName = "";
            OutputPath = "";
            OutputFileName = "";

            List<string> tempString = ValueAdjust.SplitPath(path).ToList();

            for (int i = 0; i < tempString.Count; i++)
            {
                if (i >= tempString.Count - 1)
                {
                    OutputFileName = tempString[i];
                    if (tempString[tempString.Count - 1].LastIndexOf("." + fileType, StringComparison.OrdinalIgnoreCase) < 0)
                        OutputFileName += "." + fileType;
                    break;
                }
                else
                    OutputPath += tempString[i] + "\\";
            }
            OutputFullName = OutputPath + OutputFileName;
            //Debug.Log(OutputFullName);

            if (createPath)
            {
                if (path.IndexOf(':') == 1)
                {
                    if (!Directory.Exists(OutputPath))
                        Directory.CreateDirectory(OutputPath);
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(Application.dataPath).Parent;
                    if (!Directory.Exists(directory.FullName + "/" + OutputPath))
                        Directory.CreateDirectory(directory.FullName + "/" + OutputPath);
                }
            }
            return OutputFullName;
        }
        /// <summary>
        /// 检查路径输入（Assets文件夹内）
        /// </summary>
        /// <param name="path">输入</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="createPath">如无则创建目录</param>
        /// <returns>输出</returns>
        public static string CheckPathInAssets(string path, string fileType, bool createPath)
        {
            return CheckPathInAssets(path, fileType, createPath, out string OutputPath, out string OutputFileName);
        }
        /// <summary>
        /// 检查路径输入（Assets文件夹内）
        /// </summary>
        /// <param name="path">输入</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="createPath">如无则创建目录</param>
        /// <param name="OutputPath">输出路径</param>
        /// <param name="OutputFileName">输出文件名</param>
        /// <returns>输出</returns>
        public static string CheckPathInAssets(string path, string fileType, bool createPath, out string OutputPath, out string OutputFileName)
        {
            string OutputFullName = "";
            OutputPath = "";
            OutputFileName = "";

            List<string> tempString = ValueAdjust.SplitPath(path).ToList();
            if (tempString[0] != "Assets" && path.IndexOf(':') != 1)
                tempString.Insert(0, "Assets");

            for (int i = 0; i < tempString.Count; i++)
            {
                if (i >= tempString.Count - 1)
                {
                    OutputFileName = tempString[i];
                    if (tempString[tempString.Count - 1].LastIndexOf("." + fileType, StringComparison.OrdinalIgnoreCase) < 0)
                        OutputFileName += "." + fileType;
                    break;
                }
                else
                    OutputPath += tempString[i] + "\\";
            }
            OutputFullName = OutputPath + OutputFileName;
            //Debug.Log(OutputFullName);

            if (createPath)
            {
                if (path.IndexOf(':') == 1)
                {
                    if (!Directory.Exists(OutputPath))
                        Directory.CreateDirectory(OutputPath);
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(Application.dataPath).Parent;
                    if (!Directory.Exists(directory.FullName + "/" + OutputPath))
                        Directory.CreateDirectory(directory.FullName + "/" + OutputPath);
                }
            }
            return OutputFullName;
        }
#endif
        /// <summary>
        /// 列表相加。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> ListAddition<T>(this List<List<T>> lists)
        {
            List<T> resultList = new List<T>();
            for (int i = 0; i < lists.Count; i++)
            {
                for (int j = 0; j < lists[i].Count; j++)
                {
                    resultList.Add(lists[i][j]);
                }
            }
            return resultList;
        }
        /// <summary>
        /// 列表相加。补充在后。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list01"></param>
        /// <param name="list02"></param>
        /// <returns></returns>
        public static List<T> ListAddition<T>(List<T> list01, List<T> list02)
        {
            List<T> resultList = new List<T>();
            for (int i = 0; i < list01.Count; i++)
            {
                resultList.Add(list01[i]);
            }
            for (int i = 0; i < list02.Count; i++)
            {
                resultList.Add(list02[i]);
            }
            return resultList;
        }
        /// <summary>
        /// 数组增加项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] Add<T>(this T[] array, T item)
        {
            T[] result = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i];
            }
            result[array.Length] = item;
            return result;
        }
        /// <summary>
        /// 列表相加。补充在后。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list01"></param>
        /// <param name="list02"></param>
        /// <returns></returns>
        public static List<T> AddList<T>(this List<T> list01, List<T> list02)
        {
            if (list01 == null || list02 == null)
                return null;
            for (int i = 0; i < list02.Count; i++)
            {
                list01.Add(list02[i]);
            }
            return list01;
        }
        /// <summary>
        /// 数组转列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this T[] array)
        {
            if (array == null || array.Length < 1)
                return null;
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
                list.Add(array[i]);
            return list;
        }
        /// <summary>
        /// 数组转列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<List<T>> ToList<T>(this T[][] array)
        {
            if (array == null || array.Length < 1)
                return null;
            List<List<T>> list = new List<List<T>>();
            for (int i = 0; i < array.Length; i++)
            {
                list[i] = new List<T>();
                if (array[i] != null)
                    for (int j = 0; j < array[i].Length; j++)
                        list[i].Add(array[i][j]);
            }
            return list;
        }
        /// <summary>
        /// 个别转列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> ToListOne<T>(this T obj)
        {
            return new List<T>() { obj };
        }
        /// <summary>
        /// 个别转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T[] ToArrayOne<T>(this T obj)
        {
            return new T[] { obj };
        }
        /// <summary>
        /// 列表转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this List<T> list)
        {
            if (list == null || list.Count < 1)
                return null;
            return list.ToArray();
        }
        /// <summary>
        /// 列表转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[][] ToArray<T>(this List<List<T>> list)
        {
            if (list == null || list.Count < 1)
                return null;
            T[][] result = new T[list.Count][];
            for (int i = 0; i < result.Length; i++)
            {
                if (list[i] != null)
                    result[i] = list[i].ToArray();
            }
            return result;
        }
        /// <summary>
        /// 数组ToString
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string[] ToStrings<T>(this T[] array)
        {
            if (array == null)
                return null;
            string[] result = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToString();
            }
            return result;
        }
        /// <summary>
        /// 列表ToString
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string[] ToStrings<T>(this List<T> list)
        {
            if (list == null)
                return null;
            string[] result = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                result[i] = list[i].ToString();
            }
            return result;
        }
        /// <summary>
        /// 数组统一赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        public static void SetArrayAll<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
        /// <summary>
        /// 输出统一赋值的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Length">长度</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T[] SetArrayAll<T>(uint Length = 1, T value = default(T))
        {
            T[] result = new T[Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = value;
            }
            return result;
        }
        /// <summary>
        /// 列表统一赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        public static void SetListAll<T>(this List<T> list, T value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = value;
            }
        }
        /// <summary>
        /// 输出统一赋值的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Length">长度</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static List<T> SetListAll<T>(uint Length = 1, T value = default(T))
        {
            List<T> result = new List<T>();
            for (int i = 0; i < Length; i++)
            {
                result.Add(value);
            }
            return result;
        }
        /// <summary>
        /// 乱序排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="changeSelf">改变原参数</param>
        /// <returns></returns>
        public static List<T> RandomSort<T>(this List<T> list, bool changeSelf = true)
        {
            if (list == null || list.Count < 1)
                return null;
            List<T> result = new List<T>(list);
            if (!changeSelf)
            {
                for (int i = 0; i < list.Count; i++)
                    result[i] = list[i];
            }
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                    if (RandomBoolean() && i != j)
                    {
                        if (changeSelf)
                        {
                            T temp = list[i];
                            list[i] = list[j];
                            list[j] = temp;
                        }
                        else
                        {
                            result[i] = list[j];
                            result[j] = list[i];
                        }
                    }
            }
            if (changeSelf)
                return list;
            else
                return result;
        }
        /// <summary>
        /// 乱序排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="changeSelf">改变原参数</param>
        /// <returns></returns>
        public static T[] RandomSort<T>(this T[] array, bool changeSelf = true)
        {
            if (array == null || array.Length < 1)
                return null;
            T[] result = new T[array.Length];
            if (!changeSelf)
            {
                for (int i = 0; i < array.Length; i++)
                    result[i] = array[i];
            }
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array.Length; j++)
                    if (RandomBoolean() && i != j)
                    {
                        if (changeSelf)
                            Exchange(array[i], array[j], out array[i], out array[j]);
                        else
                            Exchange(array[i], array[j], out result[i], out result[j]);
                    }
            }
            if (changeSelf)
                return array;
            else
                return result;
        }
        /// <summary>
        /// 随机抽取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="excludeNull">排除空项</param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this T[] array, bool excludeNull = false)
        {
            if (array == null || array.Length < 1)
                return default(T);
            T result;
            if (excludeNull)
            {
                T[] temp = array.ClearNullItem();
                if (temp.Length < 1)
                    return default(T);
                result = temp[Random.Range(0, temp.Length)];
            }
            else
            {
                result = array[Random.Range(0, array.Length)];
            }
            return result;
        }
        /// <summary>
        /// 随机抽取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="excludeNull">排除空项</param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this List<T> list, bool excludeNull = false)
        {
            if (list == null || list.Count < 1)
                return default(T);
            T result;
            if (excludeNull)
            {
                T[] temp = list.ToArray().ClearNullItem();
                if (temp.Length < 1)
                    return default(T);
                result = temp[Random.Range(0, temp.Length)];
            }
            else
            {
                result = list[Random.Range(0, list.Count)];
            }
            return result;
        }
        /// <summary>
        /// 随机多选多(数组，取值量，是否重复)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="count">取值量</param>
        /// <param name="repetition">是否重复</param>
        /// <returns></returns>
        public static T[] GetRandomValues<T>(T[] array, int count = 1, bool repetition = false)
        {
            count = Clamp(count, 1, array.Length);
            T[] temp = new T[array.Length];
            T[] result = new T[count];
            int end = array.Length - 1;
            for (int i = 0; i < array.Length; i++)
            {
                temp[i] = array[i];
            }
            for (int i = 0; i < count; i++)
            {
                int random = Random.Range(0, end + 1);
                result[i] = temp[random];
                if (!repetition)
                {
                    //将区间最后一个数赋值到取到的数上,取值区间-1，取走的值不能被再取。
                    temp[random] = temp[end];
                    end--;
                }
            }
            //Debug.Log(PrintArray(result));
            return result;
        }
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[][] ConvertArray<T>(this T[,] array)
        {
            if (array == null || array.Length < 1)
                return null;
            T[][] result = new T[array.GetLength(0)][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new T[array.GetLength(1)];
                for (int j = 0; j < result[i].Length; j++)
                {
                    result[i][j] = array[i, j];
                }
            }
            return result;
        }
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[,] ConvertArray<T>(this T[][] array)
        {
            if (array == null || array.Length < 1)
                return null;
            int maxLength = -1;
            foreach (T[] item in array)
            {
                if (item != null && item.Length > maxLength)
                    maxLength = item.Length;
            }
            if (maxLength < 1)
                return null;
            T[,] result = new T[array.Length, maxLength];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                    for (int j = 0; j < maxLength; j++)
                    {
                        result[i, j] = array[i][j];
                    }
            }
            return result;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <returns></returns>
        public static Component[][] ConvertArray(this MultiArray multiArray)
        {
            if (multiArray == null || multiArray.items == null || multiArray.Length < 1)
                return null;
            Component[][] result = new Component[multiArray.items.Length][];
            for (int i = 0; i < multiArray.items.Length; i++)
            {
                result[i] = new Component[multiArray.items[i].Length];
                for (int j = 0; j < result[i].Length; j++)
                {
                    result[i][j] = multiArray.GetItem(i, j);
                }
            }
            return result;
        }
#endif
        /// <summary>
        /// 打印数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="newline">换行</param>
        /// <returns></returns>
        public static string PrintArray<T>(this T[] array, bool newline = false)
        {
            if (array == null || array.Length < 1)
                return "";
            string str = "";
            for (int i = 0; i < array.Length; i++)
            {
                str += "  [" + i + "] ";
                if (array[i] != null)
                    str += array[i].ToString();
                if (newline)
                    str += "\n";
            }
            return str;
        }
        /// <summary>
        /// 打印数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string PrintArray<T>(this T[][] array)
        {
            if (array == null || array.Length < 1)
                return "";
            string str = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                    for (int j = 0; j < array[i].Length; j++)
                    {
                        str += "  [" + i + "][" + j + "] ";
                        if (array[i][j] != null)
                            str += array[i][j].ToString();
                    }
                str += "\n";
            }
            return str;
        }
        /// <summary>
        /// 打印数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string PrintArray<T>(this T[,] array)
        {
            if (array == null || array.Length < 1)
                return "";
            string str = "";
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    str += "  [" + i + "][" + j + "] ";
                    if (array[i, j] != null)
                        str += array[i, j].ToString();
                }
                str += "\n";
            }
            return str;
        }
        /// <summary>
        /// 打印列表元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="newline">换行</param>
        /// <returns></returns>
        public static string PrintArray<T>(this List<T> list, bool newline = false)
        {
            if (list == null || list.Count < 1)
                return "";
            string str = "";
            for (int i = 0; i < list.Count; i++)
            {
                str += "  [" + i + "] ";
                if (list[i] != null)
                    str += list[i].ToString();
                if (newline)
                    str += "\n";
            }
            return str;
        }
        /// <summary>
        /// 打印列表元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string PrintArray<T>(this List<List<T>> list)
        {
            if (list == null || list.Count < 1)
                return "";
            string str = "";
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                    for (int j = 0; j < list[i].Count; j++)
                    {
                        str += "  [" + i + "][" + j + "] ";
                        if (list[i][j] != null)
                            str += list[i][j].ToString();
                    }
                str += "\n";
            }
            return str;
        }
        /// <summary>
        /// 生成序列
        /// </summary>
        /// <param name="Format">序列格式
        /// <para>以大于小于号限制序列范围</para>
        /// </param>
        /// <param name="StartIndex">序列开始序号</param>
        /// <param name="Count">序列长度</param>
        /// <returns></returns>
        public static string[] RenameSequence(string Format, int StartIndex, uint Count)
        {
            if (string.IsNullOrWhiteSpace(Format))
                return null;
            if (Count < 1)
                return null;
            string[] format = Format.Split(new char[] { '<', '>' });
            if (format == null || format.Length != 3)
                return null;
            uint SequenceLength = (uint)format[1].Length;

            string[] result = RenameSequence(format[0], format[1], SequenceLength, StartIndex, Count);
            return result;
        }
        /// <summary>
        /// 生成序列
        /// </summary>
        /// <param name="Prefix">前缀</param>
        /// <param name="Postfix">后缀</param>
        /// <param name="SequenceLength">序列位数</param>
        /// <param name="StartIndex">序列开始序号</param>
        /// <param name="Count">序列长度</param>
        /// <returns></returns>
        public static string[] RenameSequence(string Prefix, string Postfix, uint SequenceLength, int StartIndex, uint Count)
        {
            if (string.IsNullOrEmpty(Prefix))
                Prefix = "";
            if (string.IsNullOrEmpty(Postfix))
                Postfix = "";
            if (Count < 1)
                return null;
            string[] result = new string[Count];
            string[] sequence = new string[Count];
            int curIndex = StartIndex;
            for (int i = 0; i < Count; i++)
            {
                sequence[i] = (curIndex++).ToString("D" + SequenceLength.ToString());
                result[i] = Prefix + sequence[i] + Postfix;
            }
            return result;
        }
        /// <summary>
        /// 生成序列
        /// </summary>
        /// <param name="Prefix">前缀</param>
        /// <param name="Postfix">后缀</param>
        /// <param name="SequenceLength">序列位数</param>
        /// <param name="StartIndex">序列开始序号</param>
        /// <param name="Count">序列长度</param>
        /// <returns></returns>
        public static string[] RenameSequence(string[] Prefix, string[] Postfix, uint SequenceLength, int StartIndex, uint Count)
        {
            if (Prefix == null)
                Prefix = new string[0];
            if (Postfix == null)
                Postfix = new string[0];
            if (Count < 1)
                return null;
            string[] result = new string[Count];
            string[] sequence = new string[Count];
            int curIndex = StartIndex;
            for (int i = 0; i < Count; i++)
            {
                sequence[i] = (curIndex++).ToString("D" + SequenceLength.ToString());
                result[i] = (i < Prefix.Length ? Prefix[i] : "") + sequence[i] + (i < Postfix.Length ? Postfix[i] : "");
            }
            return result;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 生成序列
        /// </summary>
        /// <param name="Prefix">前缀</param>
        /// <param name="Postfix">后缀</param>
        /// <param name="SequenceLength">序列位数</param>
        /// <param name="SequenceRange">序列范围</param>
        /// <returns></returns>
        public static string[] RenameSequence(string Prefix, string Postfix, uint SequenceLength, Vector2Int SequenceRange)
        {
            FindMinAndMax(SequenceRange.x, SequenceRange.y, out int min, out int max);
            return RenameSequence(Prefix, Postfix, SequenceLength, min, (max - min).ToUInteger());
        }
        /// <summary>
        /// 生成序列
        /// </summary>
        /// <param name="Prefix">前缀</param>
        /// <param name="Postfix">后缀</param>
        /// <param name="SequenceLength">序列位数</param>
        /// <param name="SequenceRange">序列范围</param>
        /// <returns></returns>
        public static string[] RenameSequence(string[] Prefix, string[] Postfix, uint SequenceLength, Vector2Int SequenceRange)
        {
            FindMinAndMax(SequenceRange.x, SequenceRange.y, out int min, out int max);
            return RenameSequence(Prefix, Postfix, SequenceLength, min, (max - min).ToUInteger());
        }
        /// <summary>
        /// 生成序列
        /// </summary>
        /// <param name="Format">序列格式</param>
        /// <param name="SequenceRange">序列范围</param>
        /// <returns></returns>
        public static string[] RenameSequence(string Format, Vector2Int SequenceRange)
        {
            FindMinAndMax(SequenceRange.x, SequenceRange.y, out int min, out int max);
            return RenameSequence(Format, min, (max - min).ToUInteger());
        }
#endif
        /// <summary>
        /// 序列重命名
        /// </summary>
        /// <param name="Names">名称</param>
        /// <param name="SequenceLength_Old">原序列长度</param>
        /// <param name="SequenceLength">序列长度</param>
        /// <param name="StartIndex">序列开始序号</param>
        /// <returns></returns>
        public static string[] RenameSequence(string[] Names, uint SequenceLength_Old, uint SequenceLength, int StartIndex)
        {
            if (Names == null || Names.Length < 1)
                return null;
            string[] Prefix = new string[Names.Length];
            string[] Postfix = new string[Names.Length];
            List<string[]> temp = new List<string[]>();
            for (int i = 0; i < Names.Length; i++)
            {
                temp.Add(Names[i].SplitNum(SequenceLength_Old));
            }
            for (int i = 0; i < temp.Count; i++)
            {
                int j = 0;
                for (; j < temp[i].Length; j++)
                {
                    if (int.TryParse(temp[i][j], out int num))
                    {
                        break;
                    }
                }
                StringBuilder stringBuilder = new StringBuilder();
                for (int k = 0; k < j; k++)
                {
                    stringBuilder.Append(temp[i][k]);
                }
                Prefix[i] = stringBuilder.ToString();
                stringBuilder.Clear();
                for (int k = j + 1; k < temp[i].Length; k++)
                {
                    stringBuilder.Append(temp[i][k]);
                }
                Postfix[i] = stringBuilder.ToString();
            }
            return RenameSequence(Prefix, Postfix, SequenceLength, StartIndex, Names.Length.ToUInteger());
        }
        /// <summary>
        /// 字符串按格式组合
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator">间隔</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static string Concat(this string[] str, string separator = null, string[] format = null)
        {
            if (str == null)
                return null;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (string.IsNullOrEmpty(str[i]))
                    continue;
                string temp = "";
                if (int.TryParse(str[i], out int num))
                {
                    try
                    {
                        temp = num.ToString((format != null && format.Length > i) ? format[i] : "");
                    }
                    catch
                    {
                        temp = str[i];
                    }
                }
                else if (double.TryParse(str[i], out double num2))
                {
                    try
                    {
                        temp = num2.ToString((format != null && format.Length > i) ? format[i] : "");
                    }
                    catch
                    {
                        temp = str[i];
                    }
                }
                else
                {
                    temp = str[i];
                }
                if (i != 0 && !string.IsNullOrEmpty(separator))
                    builder.Append(separator);
                builder.Append(temp);
            }
            return builder.ToString();
        }
        /// <summary>
        /// 分离混合的字符串和数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitNum(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            char[] chars = str.ToCharArray();
            bool[] maybeNum = new bool[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                //if (chars[i].ToString().IndexOfAny("0123456789".ToCharArray()) >= 0)
                //    maybeNum[i] = true;
                if (char.IsNumber(chars[i]))
                    maybeNum[i] = true;
            }
            List<string> result = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                builder.Clear();
                for (int j = i; j <= chars.Length; j++)
                {
                    if (j < chars.Length && (i == j || maybeNum[i] == maybeNum[j]))
                    {
                        builder.Append(chars[j]);
                    }
                    else
                    {
                        i = j - 1;
                        break;
                    }
                }
                result.Add(builder.ToString());
            }
            return result.ToArray();
        }
        /// <summary>
        /// 分离混合的字符串和数字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="MinNumCount">最少数字长度</param>
        /// <returns></returns>
        public static string[] SplitNum(this string str, uint MinNumCount)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            if (str.Length <= MinNumCount)
                return new string[] { str };
            string[] result = ValueAdjust.SplitNum(str);
            for (int i = result.Length - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(result[i]))
                    continue;
                int count = result[i].CountSequenceNumInString();
                if ((count != 0 && count < MinNumCount) || (count == 0 && result[i].Length < MinNumCount))
                {
                    if (i == 0 && result.Length > 1)
                    {
                        if (int.TryParse(result[i + 1], out int num))
                        {
                            continue;
                        }
                        else
                        {
                            result[i + 1] = result[i] + result[i + 1];
                        }
                    }
                    else
                    {
                        if (int.TryParse(result[i - 1], out int num) && result[i - 1].CountSequenceNumInString() >= MinNumCount)
                        {
                            continue;
                        }
                        result[i - 1] = result[i - 1] + result[i];
                    }
                    result[i] = null;
                }
            }
            return result.ClearNullItem();
        }
        /// <summary>
        /// 分离混合的字符串和数字和字母
        /// </summary>
        /// <param name="str"></param>
        /// <param name="excludeSpace">排除空格</param>
        /// <returns></returns>
        public static string[] SplitNumAndAlphabet(this string str, bool excludeSpace)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            char[] chars = str.ToCharArray();
            bool?[] maybeNum = new bool?[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                //if (chars[i].ToString().IndexOfAny("0123456789".ToCharArray()) >= 0)
                //    maybeNum[i] = true;
                if (char.IsNumber(chars[i]))
                    maybeNum[i] = true;
                else if (char.IsLetter(chars[i]))
                    maybeNum[i] = false;
            }
            List<string> result = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                builder.Clear();
                for (int j = i; j <= chars.Length; j++)
                {
                    if (j < chars.Length && (i == j || maybeNum[i] == maybeNum[j]))
                    {
                        builder.Append(chars[j]);
                    }
                    else
                    {
                        i = j - 1;
                        break;
                    }
                }
                string temp = null;
                if (excludeSpace)
                {
                    temp = builder.ToString().Replace(" ", "");
                    if (!string.IsNullOrWhiteSpace(temp))
                        result.Add(temp);
                }
                else
                {
                    temp = builder.ToString();
                    result.Add(temp);

                }
            }
            return result.ToArray();
        }
        /// <summary>
        /// 分离混合的字符串和数字或字母
        /// </summary>
        /// <param name="str"></param>
        /// <param name="exclude">过滤</param>
        /// <returns></returns>
        public static string[] SplitNumOrAlphabet(this string str, string exclude = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            char[] chars = str.ToCharArray();
            bool[] maybeNum = new bool[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                //if (chars[i].ToString().IndexOfAny("0123456789".ToCharArray()) >= 0)
                //    maybeNum[i] = true;
                if (char.IsLetterOrDigit(chars[i]))
                    maybeNum[i] = true;
            }
            List<string> result = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                builder.Clear();
                for (int j = i; j <= chars.Length; j++)
                {
                    if (j < chars.Length && (i == j || maybeNum[i] == maybeNum[j]))
                    {
                        builder.Append(chars[j]);
                    }
                    else
                    {
                        i = j - 1;
                        break;
                    }
                }
                string temp = null;
                if (!string.IsNullOrEmpty(exclude))
                {
                    temp = builder.ToString();
                    temp = temp.ReplaceAny(exclude, "");
                    if (!string.IsNullOrWhiteSpace(temp))
                        result.Add(temp);
                }
                else
                {
                    temp = builder.ToString();
                    result.Add(temp);
                }
            }
            return result.ToArray();
        }
        /// <summary>
        /// 分离混合的字符串和数字或字母
        /// </summary>
        /// <param name="str"></param>
        /// <param name="exclude">过滤</param>
        /// <returns></returns>
        public static string[] SplitNumOrAlphabet(this string str, string[] exclude = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            char[] chars = str.ToCharArray();
            bool[] maybeNum = new bool[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                //if (chars[i].ToString().IndexOfAny("0123456789".ToCharArray()) >= 0)
                //    maybeNum[i] = true;
                if (char.IsLetterOrDigit(chars[i]))
                    maybeNum[i] = true;
            }
            List<string> result = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                builder.Clear();
                for (int j = i; j <= chars.Length; j++)
                {
                    if (j < chars.Length && (i == j || maybeNum[i] == maybeNum[j]))
                    {
                        builder.Append(chars[j]);
                    }
                    else
                    {
                        i = j - 1;
                        break;
                    }
                }
                string temp = null;
                if (exclude != null && exclude.Length > 0)
                {
                    temp = builder.ToString();
                    foreach (string ex in exclude)
                    {
                        if (!string.IsNullOrEmpty(ex))
                            temp = temp.Replace(ex, "");
                    }
                    if (!string.IsNullOrWhiteSpace(temp))
                        result.Add(temp);
                }
                else
                {
                    temp = builder.ToString();
                    result.Add(temp);
                }
            }
            return result.ToArray();
        }
        /// <summary>
        /// 统计字符串中特定字符个数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CountCharInString(this string str, char c)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            int result = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == c)
                {
                    result++;
                }
            }
            return result;
        }
        /// <summary>
        /// 统计字符串中连续特定字符个数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CountSequenceCharInString(this string str, char c)
        {
            return CountSequenceCharInString(str, c, out int[] length);
        }
        /// <summary>
        /// 统计字符串中连续特定字符个数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CountSequenceCharInString(this string str, char c, out int[] length)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                length = new int[] { 0 };
                return 0;
            }
            List<int> result = new List<int>();
            bool lastIsC = false;
            int temp = 0;
            for (int i = 0; i < str.Length; i++)
            {
                bool isC = str[i] == c;
                if (isC)
                {
                    temp++;
                }
                else
                {
                    if (isC != lastIsC)
                    {
                        result.Add(temp);
                        temp = 0;
                    }
                }
                lastIsC = isC;
            }
            if (temp != 0)
                result.Add(temp);
            length = result.ToArray();
            ValueAdjust.FindMinAndMax(result, out int min, out int max);
            return max;
        }
        /// <summary>
        /// 字符串是否全为特定字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsFullChar(this string str, char c)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != c)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 移除字符串中连续特定字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ClearSequenceCharInString(this string str, char c)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.IndexOf(c) < 0 || str.Length < 2)
                return str;
            string[] temp = new string[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                temp[i] = str[i].ToString();
            }
            for (int i = 0; i < temp.Length; i++)
            {
                if (i >= temp.Length - 1)
                {
                    if (str[i] == c && str[i - 1] == c)
                        temp[i] = "";
                }
                else if (str[i] == c && str[i + 1] == c)
                    temp[i] = "";
                else if (i > 0 && str[i] == c && str[i - 1] == c)
                    temp[i] = "";
            }
            return string.Concat(temp);
        }
        /// <summary>
        /// 统计字符串中数字个数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CountNumInString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            int result = 0;
            byte[] tempbyte = System.Text.Encoding.ASCII.GetBytes(str);
            for (int i = 0; i < str.Length; i++)
            {
                if ((tempbyte[i] >= 48) && (tempbyte[i] <= 57))
                {
                    result++;
                }
            }
            return result;
        }
        /// <summary>
        /// 统计数组中指定项个数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int CountItem<T>(this T[] array, T item)
        {
            if (array == null || array.Length < 1)
                return 0;
            int result = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array is Object[] && (array[i] as Object) == (item as Object))
                {
                    result++;
                }
                else if (array[i].Equals(item))
                {
                    result++;
                }
            }
            return result;
        }
        /// <summary>
        /// 统计列表中指定项个数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int CountItem<T>(this List<T> list, T item)
        {
            if (list == null || list.Count < 1)
                return 0;
            int result = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list is Object[] && (list[i] as Object) == (item as Object))
                {
                    result++;
                }
                else if (list[i].Equals(item))
                {
                    result++;
                }
            }
            return result;
        }
        /// <summary>
        /// 统计字符串中连续数字长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CountSequenceNumInString(this string str)
        {
            return CountSequenceNumInString(str, out int[] length);
        }
        /// <summary>
        /// 统计字符串中连续数字长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int CountSequenceNumInString(this string str, out int[] length)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                length = new int[] { 0 };
                return 0;
            }
            List<int> result = new List<int>();
            byte[] tempbyte = System.Text.Encoding.ASCII.GetBytes(str);
            bool lastIsNum = false;
            int temp = 0;
            for (int i = 0; i < str.Length; i++)
            {
                bool isNum = (tempbyte[i] >= 48) && (tempbyte[i] <= 57);
                if (isNum)
                {
                    temp++;
                }
                else
                {
                    if (isNum != lastIsNum)
                    {
                        result.Add(temp);
                        temp = 0;
                    }
                }
                lastIsNum = isNum;
            }
            if (temp != 0)
                result.Add(temp);
            length = result.ToArray();
            ValueAdjust.FindMinAndMax(result, out int min, out int max);
            return max;
        }
        /// <summary>
        /// 找出字符串中第一个数字的序号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int FindIndexOfNumInString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            return str.IndexOfAny("0123456789".ToCharArray());
        }
        /// <summary>
        /// 统计字符串中字母个数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CountLetterInString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            int result = 0;
            byte[] tempbyte = System.Text.Encoding.ASCII.GetBytes(str);
            for (int i = 0; i < str.Length; i++)
            {
                if ((tempbyte[i] >= 65) && (tempbyte[i] <= 90) || ((tempbyte[i] >= 97) && (tempbyte[i] <= 122)))
                {
                    result++;
                }
            }
            return result;
        }
        /// <summary>
        /// 找出字符串中第一个字母的序号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int FindIndexOfLetterInString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            //return str.IndexOfAny("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray());
            byte[] tempbyte = System.Text.Encoding.ASCII.GetBytes(str);
            for (int i = 0; i < str.Length; i++)
            {
                byte by = tempbyte[i];
                if ((by >= 65) && (by <= 90) || ((by >= 97) && (by <= 122)))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 找出字符串中第一个汉字的序号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int FindIndexOfChineseCharacterInString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].IsChineseCharacter())
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 是否汉字
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsChineseCharacter(this char c)
        {
            if (c >= 0x4e00 && c <= 0x9fbb)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 输出文本占位宽度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int GetTextLength(this string str, int size)
        {
            int result = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].IsChineseCharacter())
                    result += 2 * size;
                else
                    result += size;
            }
            return result;
        }
        /// <summary>
        /// 是否相同（忽略大小写）
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool CompareIgnoreCase(this char c1, char c2)
        {
            return c1 == char.ToLower(c2) || c1 == char.ToUpper(c2);
        }
        /// <summary>
        /// 是否包含指定字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Contain(this string source, string value)
        {
            if (string.IsNullOrEmpty(source))
                return false;
            return (source.IndexOf(value, StringComparison.Ordinal) >= 0);
        }
        /// <summary>
        /// 是否包含指定字符串（忽略大小写）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainIgnoreCase(this string source, string value)
        {
            if (string.IsNullOrEmpty(source))
                return false;
            return (source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0);
        }
#if UNITY_EDITOR || UNITY_STANDALONE
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
#endif
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
            if (NormalSpeed == 0 || step == 0 || A.IsNaN() || B.IsNaN() || step.IsNaN() || NormalSpeed.IsNaN())
                return A;
            float speed = Math.Abs(NormalSpeed);
            if (setSpeedRange != 0)
            {
                if (PlusSpeed == 0 || PlusSpeed.IsNaN())
                    PlusSpeed = speed;
                setSpeedRange = Math.Abs(setSpeedRange) * 0.5f;
                if (A > B + setSpeedRange || A < B - setSpeedRange)
                    speed = Math.Abs(PlusSpeed);
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
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，速度/s，超出差值范围时的速度/s，差值范围）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="NormalSpeed">速度/s</param>
        /// <param name="PlusSpeed ">超出差值范围时的速度/s</param>
        /// <param name="setSpeedRange">差值范围</param>
        /// <returns></returns>
        public static double Lerp(double A, double B, double step, double NormalSpeed = 1f, double PlusSpeed = 1f, double setSpeedRange = 0)
        {
            if (NormalSpeed == 0 || step == 0 || A.IsNaN() || B.IsNaN() || step.IsNaN() || NormalSpeed.IsNaN())
                return A;
            double speed = Math.Abs(NormalSpeed);
            if (setSpeedRange != 0)
            {
                if (PlusSpeed == 0 || PlusSpeed.IsNaN())
                    PlusSpeed = speed;
                setSpeedRange = Math.Abs(setSpeedRange) * 0.5f;
                if (A > B + setSpeedRange || A < B - setSpeedRange)
                    speed = Math.Abs(PlusSpeed);
            }
            if (A > B + speed * step)
                A -= speed * step;
            else if (A < B - speed * step)
                A += speed * step;
            else
                A = B;
            return A;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，(点，速度/s)）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="PointSpeeds">(点，速度/s)</param>
        /// <param name="SpeedFactor">速度系数</param>
        /// <returns></returns>
        public static float Lerp(float A, float B, float step, Vector2[] PointSpeeds, float SpeedFactor = 1)
        {
            if (step == 0 || PointSpeeds.Length < 1)
                return A;
            List<float> X = PointSpeeds.GetVectorValue("X").ToList();
            List<float> Y = PointSpeeds.GetVectorValue("Y").ToList();
            List<Vector2> PointSpeeds2 = new List<Vector2>();
            PointSpeeds2.Add(new Vector2(X[0], Y[0]));
            for (int i = 1; i < PointSpeeds.Length; i++)//移除重复点
            {
                bool Repeating = false;
                for (int j = 0; j < PointSpeeds2.Count; j++)
                {
                    if (X[i].Equals(PointSpeeds2[j].x))
                    {
                        Repeating = true;
                        break;
                    }
                }
                if (!Repeating)
                    PointSpeeds2.Add(new Vector2(X[i], Y[i]));
            }
            if (PointSpeeds2.Count > 0)//按点值排序
            {
                PointSpeeds2.Sort(delegate (Vector2 item01, Vector2 item02)
                {
                    int result = item01.x.CompareTo(item02.x);
                    return result;
                });
            }
            float speed = float.NaN;
            for (int i = 1; i < PointSpeeds2.Count; i++)
            {
                if (B >= PointSpeeds2[i - 1].x && B <= PointSpeeds2[i].x)//最接近B的两点
                {
                    speed = SpeedFactor * Math.Abs(Mathf.Lerp(PointSpeeds2[i - 1].y, PointSpeeds2[i].y, ToPercent01(B, PointSpeeds2[i - 1].x, PointSpeeds2[i].x)));//根据插值计算速度
                }
            }
            if (float.IsNaN(speed) || speed == 0)
                return A;

            if (A > B + speed * step)
                A -= speed * step;
            else if (A < B - speed * step)
                A += speed * step;
            else
                A = B;
            return A;
        }
#endif
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
            return (float)Math.Sqrt(Math.Abs((X + b) / k));
        }
        /// <summary>
        /// 是否非数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsNaN(this float num)
        {
            return float.IsNaN(num);
        }
        /// <summary>
        /// 是否非数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsNaN(this double num)
        {
            return double.IsNaN(num);
        }
        /// <summary>
        /// 字符串转浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static double ToDouble(this string num, double FailValue = 0)
        {
            double result = FailValue;
            double.TryParse(num, out result);
            return result;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double ToDouble(this int num)
        {
            return num;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double ToDouble(this uint num)
        {
            return num;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double ToDouble(this long num)
        {
            return num;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double ToDouble(this decimal num)
        {
            return (double)num;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double ToDouble(this float num)
        {
            return num;
        }
        /// <summary>
        /// 字符串转浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static double[] ToDouble(this string[] num, double FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            double[] result = new double[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDouble(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double[] ToDouble(this int[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            double[] result = new double[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDouble();
            }
            return result;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double[] ToDouble(this uint[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            double[] result = new double[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDouble();
            }
            return result;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double[] ToDouble(this long[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            double[] result = new double[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDouble();
            }
            return result;
        }
        /// <summary>
        /// 转双精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double[] ToDouble(this decimal[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            double[] result = new double[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDouble();
            }
            return result;
        }
        /// <summary>
        /// 转双精度浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double[] ToDouble(this float[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            double[] result = new double[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDouble();
            }
            return result;
        }
        /// <summary>
        /// 字符串转浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string num, decimal FailValue = 0)
        {
            decimal result = FailValue;
            decimal.TryParse(num, out result);
            return result;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this int num)
        {
            return num;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this uint num)
        {
            return num;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this long num)
        {
            return num;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this double num)
        {
            return (decimal)num;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this float num)
        {
            return (decimal)num;
        }
        /// <summary>
        /// 字符串转浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static decimal[] ToDecimal(this string[] num, decimal FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            decimal[] result = new decimal[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDecimal(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal[] ToDecimal(this int[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            decimal[] result = new decimal[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDecimal();
            }
            return result;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal[] ToDecimal(this uint[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            decimal[] result = new decimal[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDecimal();
            }
            return result;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal[] ToDecimal(this long[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            decimal[] result = new decimal[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDecimal();
            }
            return result;
        }
        /// <summary>
        /// 转高精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal[] ToDecimal(this double[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            decimal[] result = new decimal[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDecimal();
            }
            return result;
        }
        /// <summary>
        /// 转高精度浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal[] ToDecima(this float[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            decimal[] result = new decimal[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToDecimal();
            }
            return result;
        }
        /// <summary>
        /// 字符串转浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static float ToFloat(this string num, float FailValue = 0)
        {
            float result = FailValue;
            float.TryParse(num, out result);
            return result;
        }
        /// <summary>
        /// 转单精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float ToFloat(this int num)
        {
            return num;
        }
        /// <summary>
        /// 转单精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float ToFloat(this uint num)
        {
            return num;
        }
        /// <summary>
        /// 转单精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float ToFloat(this long num)
        {
            return num;
        }
        /// <summary>
        /// 转单精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float ToFloat(this decimal num)
        {
            return (float)num;
        }
        /// <summary>
        /// 转单精度浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static float ToFloat(this double num, float FailValue = 0)
        {
            float result = FailValue;
            try
            {
                result = (float)num;
            }
            catch { }
            return result;
        }
        /// <summary>
        /// 字符串转浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static float[] ToFloat(this string[] num, float FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            float[] result = new float[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToFloat(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转单精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float[] ToFloat(this int[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            float[] result = new float[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToFloat();
            }
            return result;
        }
        /// <summary>
        /// 转单精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float[] ToFloat(this uint[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            float[] result = new float[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToFloat();
            }
            return result;
        }
        /// <summary>
        /// 转单精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float[] ToFloat(this long[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            float[] result = new float[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToFloat();
            }
            return result;
        }
        /// <summary>
        /// 转单精度浮点数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static float[] ToFloat(this decimal[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            float[] result = new float[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToFloat();
            }
            return result;
        }
        /// <summary>
        /// 转单精度浮点数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static float[] ToFloat(this double[] num, float FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            float[] result = new float[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToFloat(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 字符串转整数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static int ToInteger(this string num, int FailValue = 0)
        {
            int result = FailValue;
            int.TryParse(num, out result);
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static int ToInteger(this float num, int FailValue = 0)
        {
            return float.IsNaN(num) ? FailValue : (int)Round(num);
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static int ToInteger(this double num, int FailValue = 0)
        {
            return double.IsNaN(num) ? FailValue : (int)Round(num);
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int ToInteger(this decimal num)
        {
            return (int)Round(num);
        }
        /// <summary>
        /// 字符串转整数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static int[] ToInteger(this string[] num, int FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            int[] result = new int[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static int[] ToInteger(this float[] num, int FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            int[] result = new int[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static int[] ToInteger(this double[] num, int FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            int[] result = new int[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int[] ToInteger(this decimal[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            int[] result = new int[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger();
            }
            return result;
        }
        /// <summary>
        /// 字符串转整数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static uint ToUInteger(this string num, uint FailValue = 0)
        {
            uint result = FailValue;
            uint.TryParse(num, out result);
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static uint ToUInteger(this int num)
        {
            return (uint)(num.Clamp(0));
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static uint ToUInteger(this long num)
        {
            return (uint)(num.Clamp(0));
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static uint ToUInteger(this float num, uint FailValue = 0)
        {
            return float.IsNaN(num) ? FailValue : (uint)Round(num.Clamp(0));
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static uint ToUInteger(this double num, uint FailValue = 0)
        {
            return double.IsNaN(num) ? FailValue : (uint)Round(num.Clamp(0));
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static uint ToUInteger(this decimal num)
        {
            return (uint)Round(num.Clamp(0));
        }
        /// <summary>
        /// 字符串转整数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static uint[] ToUInteger(this string[] num, uint FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            uint[] result = new uint[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToUInteger(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static uint[] ToUInteger(this int[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            uint[] result = new uint[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToUInteger();
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static uint[] ToUInteger(this long[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            uint[] result = new uint[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToUInteger();
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static uint[] ToUInteger(this float[] num, uint FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            uint[] result = new uint[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToUInteger(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static uint[] ToUInteger(this double[] num, uint FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            uint[] result = new uint[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToUInteger(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static uint[] ToUInteger(this decimal[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            uint[] result = new uint[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToUInteger();
            }
            return result;
        }
        /// <summary>
        /// 字符串转整数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static long ToInteger64(this string num, long FailValue = 0)
        {
            long result = FailValue;
            long.TryParse(num, out result);
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long ToInteger64(this int num)
        {
            return num;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long ToInteger64(this uint num)
        {
            return num;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static long ToInteger64(this float num, long FailValue = 0)
        {
            return float.IsNaN(num) ? FailValue : (long)Round(num);
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static long ToInteger64(this double num, long FailValue = 0)
        {
            return double.IsNaN(num) ? FailValue : (long)Round(num);
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long ToInteger64(this decimal num)
        {
            return (long)Round(num);
        }
        /// <summary>
        /// 字符串转整数，失败为0
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static long[] ToInteger64(this string[] num, long FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            long[] result = new long[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger64(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long[] ToInteger64(this int[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            long[] result = new long[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger64();
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long[] ToInteger64(this uint[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            long[] result = new long[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger64();
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static long[] ToInteger64(this float[] num, long FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            long[] result = new long[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger64(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="FailValue">转换失败时的值</param>
        /// <returns></returns>
        public static long[] ToInteger64(this double[] num, long FailValue = 0)
        {
            if (num == null || num.Length < 1)
                return null;
            long[] result = new long[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger64(FailValue);
            }
            return result;
        }
        /// <summary>
        /// 转整数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long[] ToInteger64(this decimal[] num)
        {
            if (num == null || num.Length < 1)
                return null;
            long[] result = new long[num.Length];
            for (int i = 0; i < num.Length; i++)
            {
                result[i] = num[i].ToInteger64();
            }
            return result;
        }
        /// <summary>
        /// 取绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Abs(this int value)
        {
            return Math.Abs(value);
        }
        /// <summary>
        /// 取绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Abs(this long value)
        {
            return Math.Abs(value);
        }
        /// <summary>
        /// 取绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Abs(this float value)
        {
            return Math.Abs(value);
        }
        /// <summary>
        /// 取绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Abs(this double value)
        {
            return Math.Abs(value);
        }
        /// <summary>
        /// 取绝对值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal Abs(this decimal value)
        {
            return Math.Abs(value);
        }
        /// <summary>
        /// 限位。返回[0，1]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Clamp(float value)
        {
            float result = 0;
            if (value < 0)
                result = 0;
            else if (value > 1)
                result = 1;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回[0，1]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Clamp(double value)
        {
            double result = 0;
            if (value < 0)
                result = 0;
            else if (value > 1)
                result = 1;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Clamp(this float value, float min)
        {
            float result = 0;
            if (value < min)
                result = min;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Clamp(this double value, double min)
        {
            double result = 0;
            if (value < min)
                result = min;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Clamp(this int value, int min)
        {
            int result = 0;
            if (value < min)
                result = min;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint Clamp(this uint value, uint min)
        {
            uint result = 0;
            if (value < min)
                result = min;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Clamp(this long value, long min)
        {
            long result = 0;
            if (value < min)
                result = min;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal Clamp(this decimal value, decimal min)
        {
            decimal result = 0;
            if (value < min)
                result = min;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min且不大于max的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Clamp(this float value, float min, float max)
        {
            float result = 0;
            if (min > max)
                ValueAdjust.Exchange(min, max, out min, out max);
            if (value < min)
                result = min;
            else if (value > max)
                result = max;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min且不大于max的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Clamp(this double value, double min, double max)
        {
            double result = 0;
            if (min > max)
                ValueAdjust.Exchange(min, max, out min, out max);
            if (value < min)
                result = min;
            else if (value > max)
                result = max;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min且不大于max的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal Clamp(this decimal value, decimal min, decimal max)
        {
            decimal result = 0;
            if (min > max)
                ValueAdjust.Exchange(min, max, out min, out max);
            if (value < min)
                result = min;
            else if (value > max)
                result = max;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min且不大于max的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Clamp(this int value, int min, int max)
        {
            int result = 0;
            if (min > max)
                ValueAdjust.Exchange(min, max, out min, out max);
            if (value < min)
                result = min;
            else if (value > max)
                result = max;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min且不大于max的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint Clamp(this uint value, uint min, uint max)
        {
            uint result = 0;
            if (min > max)
                ValueAdjust.Exchange(min, max, out min, out max);
            if (value < min)
                result = min;
            else if (value > max)
                result = max;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min且不大于max的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Clamp(this long value, long min, long max)
        {
            long result = 0;
            if (min > max)
                ValueAdjust.Exchange(min, max, out min, out max);
            if (value < min)
                result = min;
            else if (value > max)
                result = max;
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 限位。返回不小于min且不大于max的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Clamp(this int value, float min, float max)
        {
            int result = 0;
            if (min > max)
                ValueAdjust.Exchange(min, max, out min, out max);
            if (value < min)
                result = Mathf.CeilToInt(min);
            else if (value > max)
                result = Mathf.FloorToInt(max);
            else
                result = value;
            return result;
        }
        /// <summary>
        /// 计算数字位数（取整）
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="outputMinus">输出符号</param>
        /// <returns></returns>
        public static int GetNumDigit(this float Num, bool outputMinus = false)
        {
            int size = 0;//位数
            float num = Math.Abs(Num);
            while (num >= 1)//获取位数
            {
                num /= 10;
                size++;
            }
            return size * (Num < 0 && outputMinus ? -1 : 1);
            //return Math.Abs((int)Num).ToString().Length * (Num < 0 ? -1 : 1);
        }
        /// <summary>
        /// 计算数字位数（取整）
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="outputMinus">输出符号</param>
        /// <returns></returns>
        public static int GetNumDigit(this double Num, bool outputMinus = false)
        {
            int size = 0;//位数
            double num = Math.Abs(Num);
            while (num >= 1)//获取位数
            {
                num /= 10;
                size++;
            }
            return size * (Num < 0 && outputMinus ? -1 : 1);
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(int A, int B, out int min, out int max)
        {
            min = Math.Min(A, B);
            max = Math.Max(A, B);
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(float A, float B, out float min, out float max)
        {
            min = Math.Min(A, B);
            max = Math.Max(A, B);
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(double A, double B, out double min, out double max)
        {
            min = Math.Min(A, B);
            max = Math.Max(A, B);
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(int[] array, out int min, out int max)
        {
            if (array == null || array.Length < 1)
            {
                min = max = 0;
                return;
            }
            min = int.MaxValue;
            max = int.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                    min = array[i];
                if (array[i] > max)
                    max = array[i];
            }
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(float[] array, out float min, out float max)
        {
            if (array == null || array.Length < 1)
            {
                min = max = float.NaN;
                return;
            }
            min = float.MaxValue;
            max = float.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                    min = array[i];
                if (array[i] > max)
                    max = array[i];
            }
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(double[] array, out double min, out double max)
        {
            if (array == null || array.Length < 1)
            {
                min = max = double.NaN;
                return;
            }
            min = double.MaxValue;
            max = double.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                    min = array[i];
                if (array[i] > max)
                    max = array[i];
            }
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="list"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(List<int> list, out int min, out int max)
        {
            if (list == null || list.Count < 1)
            {
                min = max = 0;
                return;
            }
            min = int.MaxValue;
            max = int.MinValue;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < min)
                    min = list[i];
                if (list[i] > max)
                    max = list[i];
            }
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="list"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(List<float> list, out float min, out float max)
        {
            if (list == null || list.Count < 1)
            {
                min = max = float.NaN;
                return;
            }
            min = float.MaxValue;
            max = float.MinValue;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < min)
                    min = list[i];
                if (list[i] > max)
                    max = list[i];
            }
        }
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="list"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void FindMinAndMax(List<double> list, out double min, out double max)
        {
            if (list == null || list.Count < 1)
            {
                min = max = double.NaN;
                return;
            }
            min = double.MaxValue;
            max = double.MinValue;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < min)
                    min = list[i];
                if (list[i] > max)
                    max = list[i];
            }
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="array"></param>
        public static Vector2Int FindMinAndMax(int[] array)
        {
            if (array == null || array.Length < 1)
                return Vector2Int.zero;
            Vector2Int result = new Vector2Int(int.MaxValue, int.MinValue);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < result.x)
                    result.x = array[i];
                if (array[i] > result.y)
                    result.y = array[i];
            }
            return result;
        }
#else
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="array"></param>
        public static Vector2 FindMinAndMax(int[] array)
        {
            if (array == null || array.Length < 1)
                return Vector2.zero;
            Vector2 result = new Vector2(int.MaxValue, int.MinValue);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < result.x)
                    result.x = array[i];
                if (array[i] > result.y)
                    result.y = array[i];
            }
            return result;
        }
#endif
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="array"></param>
        public static Vector2 FindMinAndMax(float[] array)
        {
            if (array == null || array.Length < 1)
                return new Vector2(float.NaN, float.NaN);
            Vector2 result = new Vector2(float.MaxValue, float.MinValue);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < result.x)
                    result.x = array[i];
                if (array[i] > result.y)
                    result.y = array[i];
            }
            return result;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="list"></param>
        public static Vector2Int FindMinAndMax(List<int> list)
        {
            if (list == null || list.Count < 1)
                return Vector2Int.zero;
            Vector2Int result = new Vector2Int(int.MaxValue, int.MinValue);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < result.x)
                    result.x = list[i];
                if (list[i] > result.y)
                    result.y = list[i];
            }
            return result;
        }
#else
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="list"></param>
        public static Vector2 FindMinAndMax(List<int> list)
        {
            if (list == null || list.Count < 1)
                return Vector2.zero;
            Vector2 result = new Vector2(int.MaxValue, int.MinValue);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < result.x)
                    result.x = list[i];
                if (list[i] > result.y)
                    result.y = list[i];
            }
            return result;
        }
#endif
        /// <summary>
        /// 找出最大最小值
        /// </summary>
        /// <param name="list"></param>
        public static Vector2 FindMinAndMax(List<float> list)
        {
            if (list == null || list.Count < 1)
                return new Vector2(float.NaN, float.NaN);
            Vector2 result = new Vector2(float.MaxValue, float.MinValue);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < result.x)
                    result.x = list[i];
                if (list[i] > result.y)
                    result.y = list[i];
            }
            return result;
        }
        /// <summary>
        /// 判定是否在范围内
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="MinValue">最小值</param>
        /// <param name="MaxValue">最大值</param>
        /// <returns></returns>
        public static bool InRange(this int CurrentValue, int MinValue, int MaxValue)
        {
            if (MinValue > MaxValue)
            {
                Exchange(MinValue, MaxValue, out MinValue, out MaxValue);
            }
            return CurrentValue >= MinValue && CurrentValue <= MaxValue;
        }
        /// <summary>
        /// 判定是否在范围内
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="MinValue">最小值</param>
        /// <param name="MaxValue">最大值</param>
        /// <returns></returns>
        public static bool InRange(this float CurrentValue, float MinValue, float MaxValue)
        {
            if (CurrentValue.IsNaN() || MinValue.IsNaN() || MaxValue.IsNaN())
                return false;
            if (MinValue > MaxValue)
            {
                Exchange(MinValue, MaxValue, out MinValue, out MaxValue);
            }
            return CurrentValue >= MinValue && CurrentValue <= MaxValue;
        }
        /// <summary>
        /// 判定是否在范围内
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="MinValue">最小值</param>
        /// <param name="MaxValue">最大值</param>
        /// <returns></returns>
        public static bool InRange(this double CurrentValue, double MinValue, double MaxValue)
        {
            if (CurrentValue.IsNaN() || MinValue.IsNaN() || MaxValue.IsNaN())
                return false;
            if (MinValue > MaxValue)
            {
                Exchange(MinValue, MaxValue, out MinValue, out MaxValue);
            }
            return CurrentValue >= MinValue && CurrentValue <= MaxValue;
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
            if (CurrentValue.IsNaN() || TargetValue.IsNaN() || ErrorRange.IsNaN())
                return false;
            if (ErrorRange < 0)
                ErrorRange = -ErrorRange;

            return CurrentValue >= (TargetValue - ErrorRange * 0.5f) && CurrentValue <= (TargetValue + ErrorRange * 0.5f);
        }
        /// <summary>
        /// 判定是否在一定误差范围内约等于目标值（当前值，目标值，误差范围）
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="TargetValue">目标值</param>
        /// <param name="ErrorRange">误差范围</param>
        /// <returns></returns>
        public static bool JudgeRange(this double CurrentValue, double TargetValue, double ErrorRange)
        {
            if (CurrentValue.IsNaN() || TargetValue.IsNaN() || ErrorRange.IsNaN())
                return false;
            if (ErrorRange < 0)
                ErrorRange = -ErrorRange;

            return CurrentValue >= (TargetValue - ErrorRange * 0.5f) && CurrentValue <= (TargetValue + ErrorRange * 0.5f);
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
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 点到直线距离
        /// </summary>
        /// <param name="point">点坐标</param>
        /// <param name="linePoint1">直线上一个点的坐标</param>
        /// <param name="linePoint2">直线上另一个点的坐标</param>
        /// <returns></returns>
        public static float DistanceToLine(Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
        {
            Vector2 vec1 = point - linePoint1;
            Vector2 vec2 = linePoint2 - linePoint1;
            Vector2 vecProj = Vector3.Project(vec1, vec2);
            float dis = (float)Math.Sqrt(Math.Pow(vec1.magnitude, 2) - Math.Pow(vecProj.magnitude, 2));
            return dis;
        }
        /// <summary>
        /// 点到直线距离
        /// </summary>
        /// <param name="point">点坐标</param>
        /// <param name="linePoint1">直线上一个点的坐标</param>
        /// <param name="linePoint2">直线上另一个点的坐标</param>
        /// <returns></returns>
        public static float DistanceToLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            Vector3 vec1 = point - linePoint1;
            Vector3 vec2 = linePoint2 - linePoint1;
            Vector3 vecProj = Vector3.Project(vec1, vec2);
            float dis = (float)Math.Sqrt(Math.Pow(vec1.magnitude, 2) - Math.Pow(vecProj.magnitude, 2));
            return dis;
        }
        /// <summary>
        /// 判断点位于线段上
        /// </summary>
        /// <param name="value">点</param>
        /// <param name="start">起点</param>
        /// <param name="end">重点</param>
        /// <param name="errorRange">误差半径</param>
        /// <returns></returns>
        public static bool InLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2, float errorRange)
        {
            bool result = false;
            if (DistanceToLine(point, linePoint1, linePoint2) <= Math.Abs(errorRange))
            {
                float dis1 = (linePoint1 - point).magnitude;
                float dis2 = (linePoint2 - point).magnitude;
                float Dis = (linePoint2 - linePoint1).magnitude;
                if (dis1 > dis2 ? (Dis > dis1) : (Dis > dis2))
                    result = true;
            }
            return result;
        }
        /// <summary>
        /// 点到平面距离，三点确定一平面  
        /// </summary>
        /// <param name="point"></param>
        /// <param name="surfacePoint1"></param>
        /// <param name="surfacePoint2"></param>
        /// <param name="surfacePoint3"></param>
        /// <returns></returns>
        public static float DistanceToPlane(Vector3 point, Vector3 surfacePoint1, Vector3 surfacePoint2, Vector3 surfacePoint3)
        {
            Plane plane = new Plane(surfacePoint1, surfacePoint2, surfacePoint3);
            return DistanceToPlane(point, plane);
        }
        /// <summary>
        /// 点到平面距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static float DistanceToPlane(Vector3 point, Plane plane)
        {
            return plane.GetDistanceToPoint(point);
        }
        /// <summary>
        /// 平面夹角
        /// </summary>
        /// <param name="surface1Point1"></param>
        /// <param name="surface1Point2"></param>
        /// <param name="surface1Point3"></param>
        /// <param name="surface2Point1"></param>
        /// <param name="surface2Point2"></param>
        /// <param name="surface2Point3"></param>
        /// <returns></returns>
        public static float PlaneAngle(Vector3 surface1Point1, Vector3 surface1Point2, Vector3 surface1Point3, Vector3 surface2Point1, Vector3 surface2Point2, Vector3 surface2Point3)
        {
            Plane plane1 = new Plane(surface1Point1, surface1Point2, surface1Point3);
            Plane plane2 = new Plane(surface2Point1, surface2Point2, surface2Point3);
            return PlaneAngle(plane1, plane2);
        }
        /// <summary>
        /// 平面夹角
        /// </summary>
        /// <param name="plane1"></param>
        /// <param name="plane2"></param>
        /// <returns></returns>
        public static float PlaneAngle(Plane plane1, Plane plane2)
        {
            return Vector3.Angle(plane1.normal, plane2.normal);
        }
#endif
        /// <summary>
        /// 精确到小数点后几位（值，位数）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public static float Round(this float value, int digits = 0)
        {
            return (float)Math.Round(value, digits);
            //return (float)(Math.Round(value * Math.Pow(10, digits)) * Math.Pow(0.1f, digits));
            //return float.Parse(value.ToString("f" + digits));
        }
        /// <summary>
        /// 精确到小数点后几位（值，位数）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public static double Round(this double value, int digits = 0)
        {
            return Math.Round(value, digits);
        }
        /// <summary>
        /// 精确到小数点后几位（值，位数）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public static decimal Round(this decimal value, int digits = 0)
        {
            return Math.Round(value, digits);
        }
        /// <summary>
        /// 精确到小数点后几位（值，位数）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public static Vector2 Round(this Vector2 value, int digits = 0)
        {
            return new Vector2(Round(value.x, digits), Round(value.y, digits));
        }
        /// <summary>
        /// 精确到小数点后几位（值，位数）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public static Vector3 Round(this Vector3 value, int digits = 0)
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
        public static double SetRange(double num, double min, double max, double period)
        {
            if (min == max)
                return min;
            if (min.IsNaN() || max.IsNaN() || num.IsNaN())
                return double.NaN;
            double numAdjusted = num;
            while (numAdjusted >= max)
            {
                numAdjusted -= period;
            }
            while (numAdjusted < min)
            {
                numAdjusted += period;
            }
            return numAdjusted;
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
            if (min == max)
                return min;
            if (min.IsNaN() || max.IsNaN() || num.IsNaN())
                return float.NaN;
            float numAdjusted = num;
            while (numAdjusted >= max)
            {
                numAdjusted -= period;
            }
            while (numAdjusted < min)
            {
                numAdjusted += period;
            }
            return numAdjusted;
        }
        /// <summary>
        /// 调整循环范围(当前值，最小值，最大值，循环周期)
        /// </summary>
        /// <param name="num">当前值</param>
        /// <param name="range">循环周期</param>
        /// <returns></returns>
        public static float SetRange(float num, Vector2 range)
        {
            if (range == null)
                return num;
            else
            {
                FindMinAndMax(range.x, range.y, out float min, out float max);
                return SetRange(num, min, max, max - min);
            }
        }
        /// <summary>
        /// 反循环
        /// </summary>
        /// <param name="curValue">当前值</param>
        /// <param name="lastValue">上一帧值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static double UnLoop(double curValue, double lastValue, double min, double max)
        {
            if (min == max)
                return min;
            if (min.IsNaN() || max.IsNaN() || curValue.IsNaN())
                return double.NaN;
            FindMinAndMax(min, max, out min, out max);
            double result = curValue;
            while (result < lastValue)
            {
                result += (max - min);
            }
            return result;
        }
        /// <summary>
        /// 反循环
        /// </summary>
        /// <param name="curValue">当前值</param>
        /// <param name="lastValue">上一帧值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static float UnLoop(float curValue, float lastValue, float min, float max)
        {
            if (min == max)
                return min;
            if (min.IsNaN() || max.IsNaN() || curValue.IsNaN())
                return float.NaN;
            FindMinAndMax(min, max, out min, out max);
            float result = curValue;
            while (result < lastValue)
            {
                result += (max - min);
            }
            return result;
        }
        /// <summary>
        /// 反循环
        /// </summary>
        /// <param name="curValue">当前值</param>
        /// <param name="lastValue">上一帧值</param>
        /// <param name="Range">循环周期</param>
        /// <returns></returns>
        public static float UnLoop(float curValue, float lastValue, Vector2 Range)
        {
            if (Range == null)
                return curValue;
            return UnLoop(curValue, lastValue, Range.x, Range.y);
        }
        /// <summary>
        /// 直角坐标转换成极坐标系 Vector2(角度（0，360）, 距离)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector2 RectToPolar(Vector2 pos)
        {
            float Distance = 0, Angle = 0;
            Distance = (float)Math.Sqrt(pos.x * pos.x + pos.y * pos.y);
            if (Distance == 0)
            { Angle = 0; }
            else
            { Angle = (float)Math.Atan2(pos.y, pos.x) / Mathf.Deg2Rad; }

            while (Angle < 0)
            { Angle += 360; }

            return new Vector2(Angle, Distance);
        }
        /// <summary>
        /// 直角坐标转换成极坐标系 Vector2(角度（0，360）, 距离)
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static Vector2 RectToPolar(float X, float Y)
        {
            return RectToPolar(new Vector2(X, Y));
        }
        /// <summary>
        /// 直角坐标转换成极坐标系 Vector2(角度（0，360）, 距离)
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Angle"></param>
        /// <param name="Distance"></param>
        /// <returns></returns>
        public static Vector2 RectToPolar(float X, float Y, out float Angle, out float Distance)
        {
            Vector2 Pos = RectToPolar(new Vector2(X, Y));
            Angle = Pos.x;
            Distance = Pos.y;
            return Pos;
        }
        /// <summary>
        /// 直角坐标转换成极坐标系 Vector2(角度（0，360）, 距离)
        /// </summary>
        /// <param name="XY"></param>
        /// <param name="Angle"></param>
        /// <param name="Distance"></param>
        /// <returns></returns>
        public static Vector2 RectToPolar(Vector2 XY, out float Angle, out float Distance)
        {
            Vector2 Pos = RectToPolar(XY);
            Angle = Pos.x;
            Distance = Pos.y;
            return Pos;
        }
        /// <summary>
        /// 极坐标转换成直角坐标系 Vector2(角度（0，360）, 距离)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector2 PolarToRect(Vector2 pos)
        {
            return PolarToRect(pos.x, pos.y);
        }
        /// <summary>
        /// 极坐标转换成直角坐标系(角度（0，360）, 距离)
        /// </summary>
        /// <param name="angle">角度（0，360）</param>
        /// <param name="dis">距离</param>
        /// <returns></returns>
        public static Vector2 PolarToRect(float angle, float dis = 1f)
        {
            Vector2 rectpos = new Vector2();
            rectpos.x = dis * (float)Math.Cos(angle * Mathf.Deg2Rad);
            rectpos.y = dis * (float)Math.Sin(angle * Mathf.Deg2Rad);
            return rectpos;
        }
        /// <summary>
        /// 极坐标转换成直角坐标系(角度（0，360）, 距离)
        /// </summary>
        /// <param name="angle">角度（0，360）</param>
        /// <param name="dis">距离</param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public static void PolarToRect(float angle, float dis, out float X, out float Y)
        {
            X = dis * (float)Math.Cos(angle * Mathf.Deg2Rad);
            Y = dis * (float)Math.Sin(angle * Mathf.Deg2Rad);
        }
        /// <summary>
        /// 圆形范围随机取值（半径，散布类型）
        /// </summary>
        /// <param name="size">半径</param>
        /// <param name="type">散布类型</param>
        /// <returns>直角坐标</returns>
        public static Vector2 RandomPolarPoint(float size, IntersperseMode type = IntersperseMode.Average)
        {
            return RandomPolarPoint(size, Vector2.zero, type);
        }
        /// <summary>
        /// 圆形范围随机取值（半径，散布类型）
        /// </summary>
        /// <param name="size">半径</param>
        /// <param name="Center">圆心</param>
        /// <param name="type">散布类型</param>
        /// <returns>直角坐标</returns>
        public static Vector2 RandomPolarPoint(float size, Vector2 Center, IntersperseMode type = IntersperseMode.Average)
        {
            float dis = 0;
            switch (type)
            {
                default:
                case IntersperseMode.Average:
                    dis = Random.Range(0, size);
                    break;
                case IntersperseMode.NotCentreAndEdge:
                    dis = (Random.Range(0, size) + Random.Range(0, size)) * 0.5f;
                    break;
                case IntersperseMode.Centre:
                    dis = Random.Range(0, 1f) * Random.Range(0, 1f) * size;
                    break;
                case IntersperseMode.Edge:
                    dis = (1 - Random.Range(0, 1f) * Random.Range(0, 1f)) * size;
                    break;
                case IntersperseMode.CentreAndEdge:
                    if (RandomBoolean())
                        dis = Random.Range(0, 1f) * Random.Range(0, 1f) * size;
                    else
                        dis = (1 - Random.Range(0, 1f) * Random.Range(0, 1f)) * size;
                    break;
            }
            Vector2 result = ValueAdjust.PolarToRect(Random.Range(0, 360f), dis);
            return result + Center;
        }
        /// <summary>
        /// 随机布尔值
        /// </summary>
        /// <param name="percent">为true的百分比</param>
        /// <returns></returns>
        public static bool RandomBoolean(float percent = 0.5f)
        {
            return Random.value < percent ? true : false;
        }
        /// <summary>
        /// 范围内随机取值
        /// </summary>
        /// <param name="Range"></param>
        /// <returns></returns>
        public static float RandomValueInRange(Vector2 Range, IntersperseMode type = IntersperseMode.Average)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Range == null)
                return float.NaN;
#endif
            if (Range.x == Range.y)
                return Range.x;
            return RandomValueInRange(Range.x, Range.y, type);
        }
        /// <summary>
        /// 范围内随机取值
        /// </summary>
        /// <param name="Range"></param>
        /// <returns></returns>
        public static float RandomValueInRange(float Min, float Max, IntersperseMode type = IntersperseMode.Average)
        {
            if (float.IsNaN(Min) || float.IsNaN(Max))
                return float.NaN;
            if (Min == Max)
                return Min;
            float result = 0;
            float min = Math.Min(Min, Max);
            float max = Math.Max(Min, Max);
            float middle = (min + max) * 0.5f;
            float dis = max - min;
            switch (type)
            {
                default:
                case IntersperseMode.Average:
                    result = Random.Range(min, max);
                    break;
                case IntersperseMode.NotCentreAndEdge:
                    result = ValueAdjust.PolarToRect(RandomBoolean() ? 0 : 180, (Random.Range(0, dis * 0.5f) + Random.Range(0, dis * 0.5f)) * 0.5f).x;
                    break;
                case IntersperseMode.Centre:
                    result = Random.Range(0, 1f) * Random.Range(0, 1f) * dis * 0.5f * (RandomBoolean() ? 1 : -1) + middle;
                    break;
                case IntersperseMode.Edge:
                    if (RandomBoolean())
                        result = Random.Range(0, 1f) * Random.Range(0, 1f) * dis * 0.5f + min;
                    else
                        result = max - Random.Range(0, 1f) * Random.Range(0, 1f) * dis * 0.5f;
                    break;
                case IntersperseMode.CentreAndEdge:
                    if (RandomBoolean())
                        result = Random.Range(0, 1f) * Random.Range(0, 1f) * dis * 0.5f * (RandomBoolean() ? 1 : -1) + middle;
                    else
                    {
                        if (RandomBoolean())
                            result = Random.Range(0, 1f) * Random.Range(0, 1f) * dis * 0.5f + min;
                        else
                            result = max - Random.Range(0, 1f) * Random.Range(0, 1f) * dis * 0.5f;
                    }
                    break;
            }
            return result;
        }
        /// <summary>
        /// 二维范围内随机取值
        /// </summary>
        /// <param name="RangeX"></param>
        /// <param name="RangeY"></param>
        /// <returns></returns>
        public static Vector2 RandomRectPointInRange(Vector2 RangeX, Vector2 RangeY, IntersperseMode type = IntersperseMode.Average)
        {
            Vector2 result = new Vector2();
            result.x = RandomValueInRange(RangeX, type);
            result.y = RandomValueInRange(RangeY, type);
            return result;
        }
        /// <summary>
        /// 三维范围内随机取值
        /// </summary>
        /// <param name="RangeX"></param>
        /// <param name="RangeY"></param>
        /// <param name="RangeZ"></param>
        /// <returns></returns>
        public static Vector3 RandomRectPointInRange(Vector2 RangeX, Vector2 RangeY, Vector2 RangeZ, IntersperseMode type = IntersperseMode.Average)
        {
            Vector3 result = new Vector3();
            result.x = RandomValueInRange(RangeX, type);
            result.y = RandomValueInRange(RangeY, type);
            result.z = RandomValueInRange(RangeZ, type);
            return result;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 平滑处理
        /// </summary>
        /// <param name="line">线段</param>
        /// <param name="type">类型</param>
        /// <param name="size">平滑范围</param>
        /// <param name="iterations">插值倍数（1为原数量）</param>
        /// <returns></returns>
        public static Vector3[] Smoothing(this LineRenderer line, Vector3 type, int size = 1, int iterations = 1)
        {
            if (!line || line.positionCount < 2)
                return null;
            if (type == null || type == Vector3.zero)
                type = Vector3.up;

            Vector3[] result = new Vector3[line.positionCount];
            line.GetPositions(result);
            result = Smoothing(result, type, size, iterations);
            line.positionCount = result.Length;
            for (int i = 0; i < result.Length; i++)
            {
                line.SetPosition(i, result[i]);
            }
            return result;
        }
        /// <summary>
        /// 平滑处理
        /// </summary>
        /// <param name="array">数据</param>
        /// <param name="type">类型</param>
        /// <param name="size">平滑范围</param>
        /// <param name="iterations">插值倍数（1为原数量）</param>
        /// <returns></returns>
        public static Vector2[] Smoothing(this Vector2[] array, Vector2 type, int size = 1, int iterations = 1)
        {
            if (array == null || array.Length < 2)
                return null;
            if (type == null || type == Vector2.zero)
                type = Vector2.up;

            float[][] temp = new float[2][];
            temp[0] = type.x != 0 ? array.GetVectorValue("X").Smoothing(size, iterations, true) : null;
            temp[1] = type.y != 0 ? array.GetVectorValue("Y").Smoothing(size, iterations, true) : null;
            Vector2[] result = new Vector2[(array.Length - 1) * iterations + 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Vector3();
                result[i].x = temp[0] == null ? 0 : temp[0][i];
                result[i].y = temp[1] == null ? 0 : temp[1][i];
            }
            return result;
        }
        /// <summary>
        /// 平滑处理
        /// </summary>
        /// <param name="array">数据</param>
        /// <param name="type">类型</param>
        /// <param name="size">平滑范围</param>
        /// <param name="iterations">插值倍数（1为原数量）</param>
        /// <returns></returns>
        public static Vector3[] Smoothing(this Vector3[] array, Vector3 type, int size = 1, int iterations = 1)
        {
            if (array == null || array.Length < 2)
                return null;
            if (type == null || type == Vector3.zero)
                type = Vector3.up;

            float[][] temp = new float[3][];
            temp[0] = type.x != 0 ? array.GetVectorValue("X").Smoothing(size, iterations, true) : null;
            temp[1] = type.y != 0 ? array.GetVectorValue("Y").Smoothing(size, iterations, true) : null;
            temp[2] = type.z != 0 ? array.GetVectorValue("Z").Smoothing(size, iterations, true) : null;
            Vector3[] result = new Vector3[(array.Length - 1) * iterations + 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Vector3();
                result[i].x = temp[0] == null ? 0 : temp[0][i];
                result[i].y = temp[1] == null ? 0 : temp[1][i];
                result[i].z = temp[2] == null ? 0 : temp[2][i];
            }
            return result;
        }
#endif
        /// <summary>
        /// 平滑处理
        /// </summary>
        /// <param name="array">数据</param>
        /// <param name="size">平滑范围</param>
        /// <param name="iterations">插值倍数（1为原数量）</param>
        /// <param name="outputIteration">输出插值扩充后的数据</param>
        /// <returns></returns>
        public static float[] Smoothing(this float[] array, int size = 1, int iterations = 1, bool outputIteration = false)
        {
            if (iterations < 1)
                iterations = 1;
            int count = (array.Length - 1) * iterations + 1;//扩充后数组长度
            float[] result = new float[array.Length];
            float[] resultLong = new float[count];
            if (iterations == 1)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    if (i == 0 || i == array.Length - 1)
                        result[i] = array[i];
                    else
                        result[i] = GetAverage(array, i, size);
                }
                return result;
            }
            else
            {
                float[] array2 = new float[count];//值
                //float[] index = new float[count];//缩短序号
                int lastIndex = 0;//上一个关键点序号
                for (int i = 0; i < count; i++)
                {
                    //index[i] = i * 1f / iterations;
                    if (i % iterations == 0)//关键点
                    {
                        lastIndex = i / iterations;
                        array2[i] = array[i / iterations];
                    }
                    else//插值
                    {
                        array2[i] = array[lastIndex] + ((array[lastIndex + 1] - array[lastIndex]) / iterations * (i % iterations));
                    }
                }
                resultLong = Smoothing(array2, size, 1);
                if (outputIteration)
                {
                    return resultLong;
                }
                else
                {
                    List<float> value = new List<float>();
                    for (int i = 0; i < count; i += iterations)
                    {
                        //if (Mathf.RoundToInt(resultLong[i].x) == resultLong[i].x)
                        //{
                        //    result[j] = resultLong[i];
                        //    j++;
                        //}
                        value.Add(resultLong[i]);
                    }
                    result = ToArray(value);
                    return result;
                }
            }
        }
        /// <summary>
        /// 交换AB值
        /// </summary>
        /// <param name="valueA">输入A值</param>
        /// <param name="valueB">输入B值</param>
        public static void Exchange<T>(ref T valueA, ref T valueB)
        {
            T temp = valueB;
            valueB = valueA;
            valueA = temp;
        }
        /// <summary>
        /// 交换AB值
        /// </summary>
        /// <param name="valueA">输入A值</param>
        /// <param name="valueB">输入B值</param>
        /// <param name="outputA">输出A值</param>
        /// <param name="outputB">输出B值</param>
        public static void Exchange<T>(T valueA, T valueB, out T outputA, out T outputB)
        {
            outputA = valueB;
            outputB = valueA;
        }
        /// <summary>
        /// 取连续数组内特定值在一定范围内的平均值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index">取值序号(-1为全部)</param>
        /// <param name="size">平滑范围</param>
        /// <param name="startIndex">开始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static float GetAverage(this float[] array, int index = -1, int size = 1, int startIndex = -1, int endIndex = -1)
        {
            float result = 0;
            float sum = 0;
            if (startIndex < 0 && endIndex < 0)
            {
                startIndex = 0;
                endIndex = array.Length - 1;
            }
            else
            {
                if (startIndex > endIndex)
                {
                    Exchange(startIndex, endIndex, out startIndex, out endIndex);
                }
                startIndex = Clamp(startIndex, 0, array.Length - 1);
                endIndex = Clamp(endIndex, 0, array.Length - 1);
            }
            if (index < 0)//全部求平均
            {
                for (int i = startIndex; i < endIndex + 1; i++)
                {
                    sum += array[i];
                }
                result = sum * 1f / (endIndex + 1 - startIndex);
            }
            else//指定序号附近
            {
                if (index < startIndex || index > endIndex)
                    return float.NaN;
                size = Math.Abs(size);
                int count = 0;
                for (int i = 0; i <= size; i++)
                {
                    if (i == 0)
                    {
                        sum += array[index];
                        count++;
                    }
                    else if (index - i >= startIndex && index + i <= endIndex)
                    {
                        sum += array[index - i] + array[index + i];
                        count += 2;
                    }
                }
                result = sum * 1f / count;
            }
            return result;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 取平均值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex">开始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static Vector2 GetAverage(this Vector2[] array, int startIndex = -1, int endIndex = -1)
        {
            return ToVector2(GetAverage(ToVector3(array), startIndex, endIndex));
        }
        /// <summary>
        /// 取平均值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex">开始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static Vector3 GetAverage(this Vector3[] array, int startIndex = -1, int endIndex = -1)
        {
            Vector3 Max = new Vector3();
            if (startIndex < 0 && endIndex < 0)
            {
                startIndex = 0;
                endIndex = array.Length - 1;
            }
            else
            {
                startIndex = Clamp(startIndex, 0, array.Length - 1);
                endIndex = Clamp(endIndex, 0, array.Length - 1);
                if (startIndex > endIndex)
                {
                    Exchange(startIndex, endIndex, out startIndex, out endIndex);
                }
            }
            for (int i = startIndex; i < endIndex + 1; i++)
            {
                Max += array[i];
            }
            return Max * 1f / (endIndex + 1 - startIndex);
        }
#endif
        /// <summary>
        /// 按权重映射
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float[] ToWeight(this int[] array)
        {
            return ToWeight(array.ToFloat());
        }
        /// <summary>
        /// 按权重映射
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<float> ToWeight(this List<int> list)
        {
            return ToWeight(list.ToArray()).ToList();
        }
        /// <summary>
        /// 按权重映射
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float[] ToWeight(this float[] array)
        {
            if (array == null || array.Length < 1)
                return null;
            float sum = 0;
            float[] result = new float[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > 0)
                    sum += array[i];
            }
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (array[i] > 0 ? array[i] : 0) / sum;
            }
            return result;
        }
        /// <summary>
        /// 按权重映射
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<float> ToWeight(this List<float> list)
        {
            return ToWeight(list.ToArray()).ToList();
        }
        /// <summary>
        /// 按权重映射
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] ToWeight(this double[] array)
        {
            if (array == null || array.Length < 1)
                return null;
            double sum = 0;
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > 0)
                    sum += array[i];
            }
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (array[i] > 0 ? array[i] : 0) / sum;
            }
            return result;
        }
        /// <summary>
        /// 按权重映射
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<double> ToWeight(this List<double> list)
        {
            return ToWeight(list.ToArray()).ToList();
        }
        /// <summary>
        /// 输出等权重映射范围
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static float[] ToWeightRange(uint count)
        {
            double[] temp = new double[count];
            temp.SetArrayAll(1);
            return ToWeightRange(temp).ToFloat();
        }
        /// <summary>
        /// 按权重映射范围
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float[] ToWeightRange(this int[] array)
        {
            return ToWeightRange(array.ToDouble()).ToFloat();
        }
        /// <summary>
        /// 按权重映射范围
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float[] ToWeightRange(this float[] array)
        {
            return ToWeightRange(array.ToDouble()).ToFloat();
        }
        /// <summary>
        /// 按权重映射范围
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] ToWeightRange(this double[] array)
        {
            if (array == null || array.Length < 1)
                return null;
            double sum = 0;
            double[] temp = new double[array.Length];
            double[] result = new double[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > 0)
                    sum += array[i];
            }
            for (int i = 0; i < array.Length; i++)
            {
                temp[i] = (array[i] > 0 ? array[i] : 0) / sum;
            }
            result[0] = 0;
            for (int i = 1; i < result.Length; i++)
            {
                result[i] = temp[i - 1] + result[i - 1];
            }
            return result;
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex">起始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static int Sum(this int[] array, int startIndex = 0, int endIndex = -1)
        {
            if (array == null || array.Length < 1)
                return 0;
            startIndex = startIndex.Clamp(0, array.Length - 1);
            if (!endIndex.InRange(0, array.Length - 1))
                endIndex = array.Length - 1;
            if (startIndex > endIndex)
            {
                Exchange(startIndex, endIndex, out startIndex, out endIndex);
            }
            int sum = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                sum += array[i];
            }
            return sum;
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="list"></param>
        /// <param name="startIndex">起始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static int Sum(this List<int> list, int startIndex = 0, int endIndex = -1)
        {
            if (list == null || list.Count < 1)
                return 0;
            return list.ToArray().Sum(startIndex, endIndex);
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex">起始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static float Sum(this float[] array, int startIndex = 0, int endIndex = -1)
        {
            if (array == null || array.Length < 1)
                return float.NaN;
            startIndex = startIndex.Clamp(0, array.Length - 1);
            if (!endIndex.InRange(0, array.Length - 1))
                endIndex = array.Length - 1;
            if (startIndex > endIndex)
            {
                Exchange(startIndex, endIndex, out startIndex, out endIndex);
            }
            float sum = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (!float.IsNaN(array[i]))
                    sum += array[i];
            }
            return sum;
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="list"></param>
        /// <param name="startIndex">起始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static float Sum(this List<float> list, int startIndex = 0, int endIndex = -1)
        {
            if (list == null || list.Count < 1)
                return float.NaN;
            return list.ToArray().Sum(startIndex, endIndex);
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex">起始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static double Sum(this double[] array, int startIndex = 0, int endIndex = -1)
        {
            if (array == null || array.Length < 1)
                return double.NaN;
            startIndex = startIndex.Clamp(0, array.Length - 1);
            if (!endIndex.InRange(0, array.Length - 1))
                endIndex = array.Length - 1;
            if (startIndex > endIndex)
            {
                Exchange(startIndex, endIndex, out startIndex, out endIndex);
            }
            double sum = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (!double.IsNaN(array[i]))
                    sum += array[i];
            }
            return sum;
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <param name="list"></param>
        /// <param name="startIndex">起始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static double Sum(this List<double> list, int startIndex = 0, int endIndex = -1)
        {
            if (list == null || list.Count < 1)
                return double.NaN;
            return list.ToArray().Sum(startIndex, endIndex);
        }
        /// <summary>
        /// x在[a，b]范围输出[-1，1]
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns>[-1，1]</returns>
        public static float ToPercentPlusMinus01(float value, float min, float max, float n = 1, bool limit = true)
        {
            return ToPercent01(value, min, max, n, limit) * 2 - 1;
        }
        /// <summary>
        /// x在[a，b]范围输出[-1，1]
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns>[-1，1]</returns>
        public static double ToPercentPlusMinus01(double value, double min, double max, float n = 1, bool limit = true)
        {
            return ToPercent01(value, min, max, n, limit) * 2 - 1;
        }
        /// <summary>
        /// x在[a，b]范围输出[0，1]，n=1为递增，n=-1为递减
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns>[0，1]</returns>
        public static float ToPercent01(float value, float min, float max, float n = 1, bool limit = true)
        {
            float result;
            if (float.IsNaN(value) || float.IsNaN(min) || float.IsNaN(max))
                return float.NaN;
            else if (min == max)
                return 0;
            else if (min > max)
            {
                Exchange(min, max, out min, out max);
            }
            if (n < 0)
            {
                result = (value - max) * 1f / (min - max);//输出[1,0]
            }
            else
            {
                result = (value - min) * 1f / (max - min);//输出[0,1]
            }
            if (limit)
                return Clamp(result);
            else
                return result;
        }
        /// <summary>
        /// x在[a，b]范围输出[0，1]，n=1为递增，n=-1为递减
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns>[0，1]</returns>
        public static double ToPercent01(double value, double min, double max, float n = 1, bool limit = true)
        {
            double result;
            if (double.IsNaN(value) || double.IsNaN(min) || double.IsNaN(max))
                return double.NaN;
            else if (min == max)
                return 0;
            else if (min > max)
            {
                Exchange(min, max, out min, out max);
            }
            if (n < 0)
            {
                result = (value - max) * 1f / (min - max);//输出[1,0]
            }
            else
            {
                result = (value - min) * 1f / (max - min);//输出[0,1]
            }
            if (limit)
                return Clamp(result);
            else
                return result;
        }
        /// <summary>
        /// 输出在范围内的连续数组
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static int[] ToRange(int min, int max)
        {
            if (min == max)
                return new int[] { min };
            int[] result = new int[max - min + 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = min + i;
            }
            return result;
        }
        /// <summary>
        /// 输出在范围内的等比变化数组
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="num">数组长度</param>
        /// <returns></returns>
        public static float[] ToRange(float min, float max, int num)
        {
            if (min == max)
                return new float[] { min };
            if (num < 2)
                num = 2;
            float[] result = new float[num];
            for (int i = 0; i < result.Length; i++)
            {
                //result[i] = MappingRange(i, 0, result.Length - 1, min, max);
                result[i] = min + (max - min) * i * 1f / (result.Length - 1);
            }
            return result;
        }
        /// <summary>
        /// 输出在范围内的等比变化数组
        /// </summary>
        /// <param name="range">参数最小值,最大值</param>
        /// <param name="num">数组长度</param>
        /// <returns></returns>
        public static float[] ToRange(Vector2 range, int num)
        {
            return ToRange(range.x, range.y, num);
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="range">参数最小值,最大值</param>
        /// <param name="OutputRange">输出最小值,最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns></returns>
        public static float MappingRange(float value, Vector2 range, Vector2 OutputRange, int n = 1, bool limit = true)
        {
            return MappingRange(value, range.x, range.y, OutputRange.x, OutputRange.y, n, limit);
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="range">参数范围节点</param>
        /// <param name="OutputMin">输出最小值</param>
        /// <param name="OutputMax">输出最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns></returns>
        public static float MappingRange(float value, float[] range, float OutputMin, float OutputMax, int n = 1, bool limit = true)
        {
            if (OutputMin.IsNaN() || OutputMax.IsNaN())
                return float.NaN;
            if (OutputMin > OutputMax)
                ValueAdjust.Exchange(OutputMin, OutputMax, out OutputMin, out OutputMax);
            if (range == null || range.Length < 2)
                return OutputMin;
            float result = float.NaN;
            for (int i = 0; i < range.Length; i++)
            {
                for (int j = 0; j < range.Length; j++)
                {
                    if (i == j)
                        continue;
                    if (i < j && range[i] > range[j])
                    {
                        Exchange(ref range[i], ref range[j]);
                    }
                }
            }
            float[] tempRange = ToWeightRange(range.Length.ToUInteger() - 1);
            for (int i = 0; i < tempRange.Length; i++)
            {
                tempRange[i] = OutputMin + tempRange[i] * (OutputMax - OutputMin);
            }
            for (int i = 1; i < range.Length; i++)
            {
                if ((i == range.Length - 1 && value >= range[i]) || (i == 1 && value <= range[0]) || (value >= range[i - 1]) && value <= range[i])
                {
                    result = MappingRange(value, range[i - 1], range[i], tempRange[i - 1], tempRange[i], n, limit);
                    break;
                }
            }
            if (limit)
                return result.Clamp(OutputMin, OutputMax);
            else
                return result;
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">参数最小值</param>
        /// <param name="max">参数最大值</param>
        /// <param name="OutputMin">输出最小值</param>
        /// <param name="OutputMax">输出最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns></returns>
        public static float MappingRange(float value, float min, float max, float OutputMin, float OutputMax, int n = 1, bool limit = true)
        {
            float result, percent;
            if (float.IsNaN(OutputMin) || float.IsNaN(OutputMax))
                return float.NaN;
            if (float.IsNaN(value) || float.IsNaN(min) || float.IsNaN(max) || min == max || OutputMin == OutputMax)
                return OutputMin;
            if (min > max)
            {
                ValueAdjust.Exchange(min, max, out min, out max);
            }
            if (OutputMin > OutputMax)
            {
                ValueAdjust.Exchange(OutputMin, OutputMax, out OutputMin, out OutputMax);
            }
            if (n < 0)
            {
                percent = (value - max) * 1f / (min - max);//输出[1,0]
            }
            else
            {
                percent = (value - min) * 1f / (max - min);//输出[0,1]
            }
            if (limit)
                result = ValueAdjust.Clamp(percent) * (OutputMax - OutputMin) + OutputMin;
            else
                result = percent * (OutputMax - OutputMin) + OutputMin;
            return result;
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">参数最小值</param>
        /// <param name="max">参数最大值</param>
        /// <param name="OutputMin">输出最小值</param>
        /// <param name="OutputMax">输出最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns></returns>
        public static double MappingRange(double value, double min, double max, double OutputMin, double OutputMax, int n = 1, bool limit = true)
        {
            double result, percent;
            if (double.IsNaN(OutputMin) || double.IsNaN(OutputMax))
                return double.NaN;
            if (double.IsNaN(value) || double.IsNaN(min) || double.IsNaN(max) || min == max || OutputMin == OutputMax)
                return OutputMin;
            if (min > max)
            {
                ValueAdjust.Exchange(min, max, out min, out max);
            }
            if (OutputMin > OutputMax)
            {
                ValueAdjust.Exchange(OutputMin, OutputMax, out OutputMin, out OutputMax);
            }
            if (n < 0)
            {
                percent = (value - max) * 1f / (min - max);//输出[1,0]
            }
            else
            {
                percent = (value - min) * 1f / (max - min);//输出[0,1]
            }
            if (limit)
                result = ValueAdjust.Clamp(percent) * (OutputMax - OutputMin) + OutputMin;
            else
                result = percent * (OutputMax - OutputMin) + OutputMin;
            return result;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">参数最小值</param>
        /// <param name="max">参数最大值</param>
        /// <param name="OutputMin">输出最小值</param>
        /// <param name="OutputMax">输出最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns></returns>
        public static Vector2 MappingRange(float value, float min, float max, Vector2 OutputMin, Vector2 OutputMax, int n = 1, bool limit = true)
        {
            Vector2 result;
            float percent;
            if (float.IsNaN(value) || float.IsNaN(min) || float.IsNaN(max) || min == max || OutputMin == OutputMax)
                return OutputMin;
            if (min > max)
            {
                ValueAdjust.Exchange(min, max, out min, out max);
            }
            if (n < 0)
            {
                percent = (value - max) * 1f / (min - max);//输出[1,0]
            }
            else
            {
                percent = (value - min) * 1f / (max - min);//输出[0,1]
            }
            if (limit)
                result = ValueAdjust.Clamp(percent) * (OutputMax - OutputMin) + OutputMin;
            else
                result = percent * (OutputMax - OutputMin) + OutputMin;
            return result;
        }
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">参数最小值</param>
        /// <param name="max">参数最大值</param>
        /// <param name="OutputMin">输出最小值</param>
        /// <param name="OutputMax">输出最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns></returns>
        public static Vector3 MappingRange(float value, float min, float max, Vector3 OutputMin, Vector3 OutputMax, int n = 1, bool limit = true)
        {
            Vector3 result;
            float percent;
            if (float.IsNaN(value) || float.IsNaN(min) || float.IsNaN(max) || min == max || OutputMin == OutputMax)
                return OutputMin;
            if (min > max)
            {
                ValueAdjust.Exchange(min, max, out min, out max);
            }
            if (n < 0)
            {
                percent = (value - max) * 1f / (min - max);//输出[1,0]
            }
            else
            {
                percent = (value - min) * 1f / (max - min);//输出[0,1]
            }
            if (limit)
                result = ValueAdjust.Clamp(percent) * (OutputMax - OutputMin) + OutputMin;
            else
                result = percent * (OutputMax - OutputMin) + OutputMin;
            return result;
        }
#endif
        /// <summary>
        /// 不对称范围映射
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">参数最小值</param>
        /// <param name="middle">参数中间值</param>
        /// <param name="max">参数最大值</param>
        /// <param name="OutputMin">输出最小值</param>
        /// <param name="OutputMiddle">输出中间值</param>
        /// <param name="OutputMax">输出最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns></returns>
        public static float MappingAsymmetryRange(float value, float min, float middle, float max, float OutputMin, float OutputMiddle, float OutputMax, int n = 1, bool limit = true)
        {
            if (float.IsNaN(value) || float.IsNaN(min) || float.IsNaN(max) || min == max || OutputMin == OutputMax)
                return OutputMin;
            if (float.IsNaN(OutputMin) || float.IsNaN(OutputMax))
                return float.NaN;
            if (min > max)
            {
                ValueAdjust.Exchange(min, max, out min, out max);
            }
            if (OutputMin > OutputMax)
            {
                ValueAdjust.Exchange(OutputMin, OutputMax, out OutputMin, out OutputMax);
            }
            middle = ValueAdjust.Clamp(middle, min, max);
            OutputMiddle = ValueAdjust.Clamp(OutputMiddle, OutputMin, OutputMax);
            if (limit)
            {
                if (value >= max)
                    return n < 0 ? OutputMin : OutputMax;
                else if (value <= min)
                    return n < 0 ? OutputMax : OutputMin;
            }
            if (value == middle)
                return OutputMiddle;
            else if (value < middle)
            {
                if (n < 0)
                    return MappingRange(value, min, middle, OutputMiddle, OutputMax, n, limit);
                else
                    return MappingRange(value, min, middle, OutputMin, OutputMiddle, n, limit);
            }
            else
            {
                if (n < 0)
                    return MappingRange(value, middle, max, OutputMin, OutputMiddle, n, limit);
                else
                    return MappingRange(value, middle, max, OutputMiddle, OutputMax, n, limit);
            }
        }
        /// <summary>
        /// 不对称范围映射
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="min">参数最小值</param>
        /// <param name="middle">参数中间值</param>
        /// <param name="max">参数最大值</param>
        /// <param name="OutputMin">输出最小值</param>
        /// <param name="OutputMiddle">输出中间值</param>
        /// <param name="OutputMax">输出最大值</param>
        /// <param name="n">n大于等于0为递增，n小于0为递减</param>
        /// <param name="limit">限制范围</param>
        /// <returns></returns>
        public static double MappingAsymmetryRange(double value, double min, double middle, double max, double OutputMin, double OutputMiddle, double OutputMax, int n = 1, bool limit = true)
        {
            if (double.IsNaN(value) || double.IsNaN(min) || double.IsNaN(max) || min == max || OutputMin == OutputMax)
                return OutputMin;
            if (double.IsNaN(OutputMin) || double.IsNaN(OutputMax))
                return float.NaN;
            if (min > max)
            {
                ValueAdjust.Exchange(min, max, out min, out max);
            }
            if (OutputMin > OutputMax)
            {
                ValueAdjust.Exchange(OutputMin, OutputMax, out OutputMin, out OutputMax);
            }
            middle = ValueAdjust.Clamp(middle, min, max);
            OutputMiddle = ValueAdjust.Clamp(OutputMiddle, OutputMin, OutputMax);
            if (limit)
            {
                if (value >= max)
                    return n < 0 ? OutputMin : OutputMax;
                else if (value <= min)
                    return n < 0 ? OutputMax : OutputMin;
            }
            if (value == middle)
                return OutputMiddle;
            else if (value < middle)
            {
                if (n < 0)
                    return MappingRange(value, min, middle, OutputMiddle, OutputMax, n, limit);
                else
                    return MappingRange(value, min, middle, OutputMin, OutputMiddle, n, limit);
            }
            else
            {
                if (n < 0)
                    return MappingRange(value, middle, max, OutputMin, OutputMiddle, n, limit);
                else
                    return MappingRange(value, middle, max, OutputMiddle, OutputMax, n, limit);
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
                time = Math.Abs(time);
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
        /// 汉字数字显示
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ToChineseDigit(this decimal N)
        {
            string result = "";
            string str = N.ToString("F29");
            int index = str.IndexOf('.');
            if (index < 0)
            {
                result = ToChineseDigit(N.ToInteger64());
            }
            else
            {
                string[] temp = str.Split('.');
                result = ToChineseDigit(temp[0].ToInteger64());
                string temp2 = str.Substring(index);
                while (temp2[temp2.Length - 1] == '0')
                {
                    temp2 = temp2.Remove(temp2.Length - 1);
                }
                for (int i = 0; i < temp2.Length; i++)
                {
                    result += ToChineseDigit(temp2[i]);
                }
            }
            return result;
        }
        /// <summary>
        /// 汉字数字显示
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ToChineseDigit(this double N)
        {
            if (N.IsNaN())
                return "";
            string result = "";
            string str = N.ToString("F16");
            int index = str.IndexOf('.');
            if (index < 0)
            {
                result = ToChineseDigit(N.ToInteger64());
            }
            else
            {
                string[] temp = str.Split('.');
                result = ToChineseDigit(temp[0].ToInteger64());
                string temp2 = str.Substring(index);
                while (temp2[temp2.Length - 1] == '0')
                {
                    temp2 = temp2.Remove(temp2.Length - 1);
                }
                for (int i = 0; i < temp2.Length; i++)
                {
                    result += ToChineseDigit(temp2[i]);
                }
            }
            return result;
        }
        /// <summary>
        /// 汉字数字显示
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ToChineseDigit(this float N)
        {
            if (N.IsNaN())
                return "";
            return ToChineseDigit(N.ToDouble());
        }
        /// <summary>
        /// 汉字数字转浮点数
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static double ChineseDigitToDouble(this string N)
        {
            if (string.IsNullOrWhiteSpace(N))
                return double.NaN;
            List<string> digit = new List<string>() { "零", "十", "百", "千", "万", "十万", "百万", "千万", "亿", "十亿", "百亿", "千亿", "万亿", "十万亿", "百万亿", "千万亿" };
            int MaxLength = digit.Count;
            bool negative = N[0] == '负';
            int index = N.IndexOf('点');
            string temp = index < 0 ? N.Replace("负", "") : N.Replace("负", "").Substring(0, index - 1);
            string result = "";

            if (temp == "零")
            {
                result = "0";
            }
            else if (digit.IndexOf(N) > 0)
            {
                result = Math.Pow(10, digit.IndexOf(N)).ToInteger64().ToString();
            }
            else
            {
                long num = 0;
                long numtemp = 0;
                int index2 = -1;
                for (int i = MaxLength - 1; i > 0; i--)
                {
                    index2 = temp.IndexOf(digit[i]);
                    if (index2 > 0)
                    {
                        numtemp = temp[index2 - 1].ChineseDigitToString().ToInteger() * Math.Pow(10, i).ToInteger64();
                        num += numtemp;
                        temp = temp.Substring(index2 + digit[i].Length);
                    }
                }
                if (temp.Replace("零", "").Length > 0)
                    numtemp = temp.Replace("零", "")[0].ChineseDigitToString().ToInteger64();
                else
                    numtemp = temp.ChineseDigitToString().ToInteger64();
                num += numtemp;
                result += num.ToString();
            }
            if (index >= 0)
            {
                string temp2 = N.Substring(index);
                for (int i = 0; i < temp2.Length; i++)
                {
                    result += temp2[i].ChineseDigitToString();
                }
            }
            return (negative ? -1 : 1) * result.ToDouble(double.NaN);
        }
        /// <summary>
        /// 汉字数字转浮点数
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static decimal ChineseDigitToDecimal(this string N)
        {
            if (string.IsNullOrWhiteSpace(N))
                return 0;
            List<string> digit = new List<string>() { "零", "十", "百", "千", "万", "十万", "百万", "千万", "亿", "十亿", "百亿", "千亿", "万亿", "十万亿", "百万亿", "千万亿" };
            int MaxLength = digit.Count;
            bool negative = N[0] == '负';
            int index = N.IndexOf('点');
            string temp = index < 0 ? N.Replace("负", "") : N.Replace("负", "").Substring(0, index - 1);
            string result = "";

            if (temp == "零")
            {
                result = "0";
            }
            else if (digit.IndexOf(N) > 0)
            {
                result = Math.Pow(10, digit.IndexOf(N)).ToInteger64().ToString();
            }
            else
            {
                long num = 0;
                long numtemp = 0;
                int index2 = -1;
                for (int i = MaxLength - 1; i > 0; i--)
                {
                    index2 = temp.IndexOf(digit[i]);
                    if (index2 > 0)
                    {
                        numtemp = temp[index2 - 1].ChineseDigitToString().ToInteger() * Math.Pow(10, i).ToInteger64();
                        num += numtemp;
                        temp = temp.Substring(index2 + digit[i].Length);
                    }
                }
                if (temp.Replace("零", "").Length > 0)
                    numtemp = temp.Replace("零", "")[0].ChineseDigitToString().ToInteger64();
                else
                    numtemp = temp.ChineseDigitToString().ToInteger64();
                num += numtemp;
                result += num.ToString();
            }
            if (index >= 0)
            {
                string temp2 = N.Substring(index);
                for (int i = 0; i < temp2.Length; i++)
                {
                    result += temp2[i].ChineseDigitToString();
                }
            }
            return (negative ? -1 : 1) * result.ToDecimal();
        }
        /// <summary>
        /// 汉字数字转浮点数
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static float ChineseDigitToFloat(this string N)
        {
            if (string.IsNullOrWhiteSpace(N))
                return float.NaN;
            return N.ChineseDigitToDouble().ToFloat();
        }
        /// <summary>
        /// 汉字数字转整数
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static int ChineseDigitToInteger(this string N)
        {
            if (string.IsNullOrWhiteSpace(N))
                return 0;
            return N.ChineseDigitToDouble().ToInteger();
        }
        /// <summary>
        /// 汉字数字转整数
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static long ChineseDigitToInteger64(this string N)
        {
            if (string.IsNullOrWhiteSpace(N))
                return 0;
            return N.ChineseDigitToDouble().ToInteger64();
        }
        /// <summary>
        /// 汉字数字转整数
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ChineseDigitToString(this char N)
        {
            string result = "";
            switch (N)
            {
                case '一': result += "1"; break;
                case '二': result += "2"; break;
                case '三': result += "3"; break;
                case '四': result += "4"; break;
                case '五': result += "5"; break;
                case '六': result += "6"; break;
                case '七': result += "7"; break;
                case '八': result += "8"; break;
                case '九': result += "9"; break;
                case '零': result += "0"; break;
                case '点': result += "."; break;
                case '负': result = "-"; break;
            }
            return result;
        }
        /// <summary>
        /// 汉字数字转整数（直接转换）
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ChineseDigitToString(this string N)
        {
            if (string.IsNullOrWhiteSpace(N))
                return "";
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < N.Length; i++)
            {
                stringBuilder.Append(N[i].ChineseDigitToString());
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 汉字数字显示
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ToChineseDigit(this string N)
        {
            if (string.IsNullOrWhiteSpace(N))
                return "";
            string result = "";
            int index = N.IndexOf('.');
            if (index < 0)
            {
                //for (int i = 0; i < N.Length; i++)
                //{
                //    result += ToChineseDigit(N[i]);
                //}
                result = ToChineseDigit(N.ToDouble(double.NaN).ToInteger64());
            }
            else
            {
                string[] temp = N.Split('.');
                result = ToChineseDigit(temp[0].ToInteger());
                string temp2 = N.Substring(index);
                for (int i = 0; i < temp2.Length; i++)
                {
                    result += ToChineseDigit(temp2[i]);
                }
            }
            return result;
        }
        /// <summary>
        /// 汉字数字显示（直接转换）
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ToChineseDigit(this char N)
        {
            string result = "";
            switch (N)
            {
                case '1': result = "一"; break;
                case '2': result = "二"; break;
                case '3': result = "三"; break;
                case '4': result = "四"; break;
                case '5': result = "五"; break;
                case '6': result = "六"; break;
                case '7': result = "七"; break;
                case '8': result = "八"; break;
                case '9': result = "九"; break;
                case '0': result = "零"; break;
                case '.': result = "点"; break;
                case '-': result = "负"; break;
            }
            return result;
        }
        /// <summary>
        /// 汉字数字整数显示
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ToChineseDigit(this int N)
        {
            return ToChineseDigit(N.ToInteger64());
        }
        /// <summary>
        /// 汉字数字整数显示
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string ToChineseDigit(this long N)
        {
            if (N == 0)
            {
                return "零";
            }
            else if (N > -10 && N < 10)
            {
                string temp = "";
                switch (N)
                {
                    case 1: temp = "一"; break;
                    case 2: temp = "二"; break;
                    case 3: temp = "三"; break;
                    case 4: temp = "四"; break;
                    case 5: temp = "五"; break;
                    case 6: temp = "六"; break;
                    case 7: temp = "七"; break;
                    case 8: temp = "八"; break;
                    case 9: temp = "九"; break;
                }
                return (N < 0 ? "负" : "") + temp;
            }
            string[] digit = new string[16] { "零", "十", "百", "千", "万", "十万", "百万", "千万", "亿", "十亿", "百亿", "千亿", "万亿", "十万亿", "百万亿", "千万亿" };
            int MaxLength = digit.Length;
            long num = Math.Abs(N);
            string[] DigitString = new string[MaxLength];
            DigitString.SetArrayAll("");
            int[] Digit = new int[MaxLength];
            Digit.SetArrayAll(0);

            if (GetNumDigit(num) > MaxLength)
            {
                DigitString[0] = ToChineseDigit(9);
                for (int i = 1; i < MaxLength; i++)
                {
                    DigitString[i] = ToChineseDigit(9) + digit[i];
                }
            }
            else
            {
                for (int i = MaxLength - 1; i >= 0; i--)
                {
                    if (i < GetNumDigit(num))
                    {
                        double temp = num;
                        for (int j = MaxLength - 1; j > i; j--)
                        {
                            temp -= Digit[j] * Math.Pow(10, j);
                        }
                        Digit[i] = Math.Floor(temp / Math.Pow(10, i)).ToInteger();
                    }
                    //if (Digit[i] != 0)
                    {
                        DigitString[i] = ToChineseDigit(Digit[i].ToString()[0]);
                        if (i > 0 && i < MaxLength && Digit[i] != 0)
                        {
                            DigitString[i] += digit[i];
                        }
                    }
                }
                /*bool[] isZero = new bool[MaxLength];
                for (int i = 0; i < MaxLength; i++)
                {
                    isZero[i] = (Digit[i] == 0);
                }
                bool temp2 = true;
                for (int i = 2; i < MaxLength; i++)
                {
                    if (!isZero[i])
                        temp2 = false;
                }
                if (temp2 && Digit[1] == 1)
                {
                    DigitString[1] = "十";
                }*/
                /*else if (!isZero[4] && isZero[3] && !isZero[2] && isZero[1] && !isZero[0])
                {
                    DigitString[3] = "零";
                    DigitString[1] = "零";
                }
                else if (!isZero[2] && isZero[1] && !isZero[0])
                {
                    DigitString[1] = "零";
                }
                else if ((!isZero[3] && isZero[2] && !isZero[1]) || (!isZero[3] && isZero[2] && isZero[1] && !isZero[0]))
                {
                    DigitString[2] = "零";
                }
                else if ((!isZero[4] && isZero[3] && isZero[2] && isZero[1] && !isZero[0]) || (!isZero[4] && isZero[3] && isZero[2] && !isZero[1]) || (!isZero[4] && isZero[3] && !isZero[2]))
                {
                    DigitString[3] = "零";
                }*/
                int firstIndex = 0;
                for (int i = MaxLength - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(DigitString[i]))
                    {
                        firstIndex = i;
                        break;
                    }
                }
                foreach (int i in new int[] { 1, 5, 9, 13 })
                {
                    if (Digit[i] == 1 && (firstIndex == i || Digit[i + 1] == 0))
                        DigitString[i] = digit[i];
                }
            }
            string result = (N < 0 ? "负" : "");
            for (int i = MaxLength - 1; i >= 0; i--)
            {
                result += DigitString[i];
            }
            while (result[0] == '零')
            {
                result = result.Substring(1);
            }
            while (result[result.Length - 1] == '零')
            {
                result = result.Remove(result.Length - 1);
            }
            while (result.Contain("零零"))
            {
                result = result.Replace("零零", "零");
            }
            return result;
        }
    }
    /// <summary>
    /// 颜色（HSV模式）
    /// </summary>
    public struct HSVColor
    {
        /// <summary>
        /// 色调[0，360]
        /// </summary>
        public float Hue;
        /// <summary>
        /// 饱和度[0，1]
        /// </summary>
        public float Saturation;
        /// <summary>
        /// 明度[0，1]
        /// </summary>
        public float Value;
        /// <summary>
        /// 透明度[0，1]
        /// </summary>
        public float alpha;
        /// <summary>
        /// 新建HSV颜色
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
        /// 新建HSV颜色
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
#if UNITY_EDITOR || UNITY_STANDALONE
        public Vector4 ToVector4()
        {
            return new Vector4(Hue, Saturation, Value, alpha);
        }
#endif
        public override string ToString()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return ToVector4().ToString();
#else
            return ("(" + Hue + "," + Saturation + "," + Value + "," + alpha + ")");
#endif
        }
        public static implicit operator Color(HSVColor v) { return v.ToColor(); }
    }
    /// <summary>
    /// 颜色调整
    /// </summary>
    public static class ColorAdjust
    {
        /// <summary>
        /// 颜色调整模式
        /// </summary>
        public enum ColorAdjustMode
        {
            /// <summary>
            /// 完全颜色
            /// </summary>
            RGBA,
            /// <summary>
            /// 忽略透明度
            /// </summary>
            RGB,
            /// <summary>
            /// 红
            /// </summary>
            Red,
            /// <summary>
            /// 绿
            /// </summary>
            Green,
            /// <summary>
            /// 蓝
            /// </summary>
            Blue,
            /// <summary>
            /// 透明度
            /// </summary>
            Alpha,
            /// <summary>
            /// 色相
            /// </summary>
            Hue,
            /// <summary>
            /// 饱和度
            /// </summary>
            Saturation,
            /// <summary>
            /// 明度
            /// </summary>
            Value
        }
        /// <summary>
        /// 调整颜色
        /// </summary>
        /// <param name="color">当前颜色</param>
        /// <param name="targetColor">目标颜色</param>
        /// <param name="mode">模式</param>
        /// <returns></returns>
        public static Color AdjustColor(this Color color, Color targetColor, ColorAdjustMode mode = ColorAdjustMode.RGBA)
        {
            Color result = color;
            switch (mode)
            {
                default:
                    switch (mode)
                    {
                        case ColorAdjustMode.RGBA:
                        case ColorAdjustMode.RGB:
                        case ColorAdjustMode.Red:
                            result.r = targetColor.r;
                            break;
                    }
                    switch (mode)
                    {
                        case ColorAdjustMode.RGBA:
                        case ColorAdjustMode.RGB:
                        case ColorAdjustMode.Green:
                            result.g = targetColor.g;
                            break;
                    }
                    switch (mode)
                    {
                        case ColorAdjustMode.RGBA:
                        case ColorAdjustMode.RGB:
                        case ColorAdjustMode.Blue:
                            result.b = targetColor.b;
                            break;
                    }
                    switch (mode)
                    {
                        case ColorAdjustMode.RGBA:
                        case ColorAdjustMode.Alpha:
                            result.a = targetColor.a;
                            break;
                    }
                    break;
                case ColorAdjustMode.Hue:
                    result = ColorHueChange(color, targetColor.ConvertRgbToHsv().Hue);
                    break;
                case ColorAdjustMode.Saturation:
                    result = ColorSaturationChange(color, targetColor.ConvertRgbToHsv().Saturation);
                    break;
                case ColorAdjustMode.Value:
                    result = ColorValueChange(color, targetColor.ConvertRgbToHsv().Value);
                    break;
            }
            return result;
        }
        /// <summary>
        /// 三维坐标转颜色
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color ToColor(Vector3 value)
        {
            return new Color(value.x, value.y, value.z);
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// 四维坐标转颜色
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color ToColor(Vector4 value)
        {
            return new Color(value.x, value.y, value.z, value.w);
        }
#endif
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
        public static Color ColorSaturationChange(Color RGBA, float percent)
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
        public static Color ColorValueChange(Color RGBA, float percent)
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
        public static Color ColorAlphaChange(Color RGBA, float percent)
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

            min = Math.Min(Math.Min(r, g), b);
            v = Math.Max(Math.Max(r, g), b);
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
        public static HSVColor ConvertRgbToHsv(this Color RGB)
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
    /// <summary>
    /// IP地址相关
    /// </summary>
    public static class IPInformation
    {
        /// <summary>
        /// IP地址类型
        /// </summary>
        public enum IPAddressType
        {
            IPv4, IPv6
        }
        /// <summary>
        /// 限制IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string LimitIPv4(string ip)
        {
            string[] str = ip.Trim(' ').Split('.');
            int[] num = new int[4];
            for (int i = 0; i < str.Length; i++)
            {
                if (i < 4)
                    int.TryParse(str[i].Length > 3 ? str[i].Remove(3) : str[i], out num[i]);//能转换整数才读取
            }
            for (int i = 0; i < num.Length; i++)
            {
                num[i] = ValueAdjust.Clamp(num[i], 0, 255);
            }
            string result = num[0] + "." + num[1] + "." + num[2] + "." + num[3];
            return result;
        }
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <param name="type">要获取的IP类型</param>
        /// <returns></returns>
        public static string GetIP(IPAddressType type = IPAddressType.IPv4)
        {
            return GetIP(out IPAddress address, type);
        }
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <param name="type">要获取的IP类型</param>
        /// <returns></returns>
        public static string GetIP(out IPAddress address, IPAddressType type = IPAddressType.IPv4)
        {
            GetIP(out IPAddress[] addresses, type);
            address = addresses.Length > 0 ? addresses[0] : null;
            return address == null ? null : address.ToString();
        }
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <param name="type">要获取的IP类型</param>
        /// <returns></returns>
        public static string[] GetIP(out IPAddress[] address, IPAddressType type = IPAddressType.IPv4)
        {
            address = null;
            if (type == IPAddressType.IPv6 && !Socket.OSSupportsIPv6)
            {
                return null;
            }

            List<IPAddress> output = new List<IPAddress>();

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
                NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

                if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        switch (type)
                        {
                            default:
                            case IPAddressType.IPv4:
                                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    output.Add(ip.Address);
                                }
                                break;
                            case IPAddressType.IPv6:
                                if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                {
                                    output.Add(ip.Address);
                                }
                                break;
                        }
                    }
                }
            }
            address = output.ToArray();
            return output.ToStrings();
        }
        ///<summary>
        /// 传入域名返回对应的IP地址
        ///</summary>
        ///<param name="domain">域名</param>
        ///<returns></returns>
        public static IPAddress GetIP(string domain)
        {
            domain = domain.Replace("http://", "").Replace("https://", "");
            IPHostEntry hostEntry = Dns.GetHostEntry(domain);
            IPEndPoint ipEndPoint = new IPEndPoint(hostEntry.AddressList[0], 0);
            return ipEndPoint.Address;
        }
    }
}