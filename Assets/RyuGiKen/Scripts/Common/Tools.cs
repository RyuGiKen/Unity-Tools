using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
#if UNITY_EDITOR || UNITY_STANDALONE
using WindowsAPI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
#endif
/// <summary>
/// RyuGiKen's Tools
/// </summary>
namespace RyuGiKen
{
    /// <summary>
    /// 获取文件
    /// </summary>
    public static class GetFile
    {
        /// <summary>
        /// 获得指定路径下所有子目录文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="Type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfoAll(string path, string Type = "")
        {
            if (path == null || path == "")
                path = Directory.GetCurrentDirectory();

            List<List<FileInfo>> files = new List<List<FileInfo>>();
            files.Add(GetFileInfos(path, Type));
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                files.Add(GetFileInfoAll(d.FullName, Type));
            }
            List<FileInfo> result = ValueAdjust.ListAddition(files);
            return result;
        }
        /// <summary>
        /// 获得指定路径下所有子目录文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="Type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfoAll(string path, string[] Type)
        {
            if (path == null || path == "")
                path = Directory.GetCurrentDirectory();

            List<List<FileInfo>> files = new List<List<FileInfo>>();
            files.Add(GetFileInfos(path, Type));
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                files.Add(GetFileInfoAll(d.FullName, Type));
            }
            List<FileInfo> result = ValueAdjust.ListAddition(files);
            return result;
        }
        /// <summary>
        /// 获得指定路径下所有子目录文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="Type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfoAllWithOutType(string path, string[] Type)
        {
            if (path == null || path == "")
                path = Directory.GetCurrentDirectory();

            List<List<FileInfo>> files = new List<List<FileInfo>>();
            files.Add(GetFileInfosWithOutType(path, Type));
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                files.Add(GetFileInfoAllWithOutType(d.FullName, Type));
            }
            List<FileInfo> result = ValueAdjust.ListAddition(files);
            return result;
        }
        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="Type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfos(string path, string Type = "")
        {
            DirectoryInfo root = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in root.GetFiles())
            {
                if (Type == "")
                {
                    files.Add(file);
                }
                else if (file.JudgeFileType(Type))
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
        /// <param name="Type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfos(string path, string[] Type)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in root.GetFiles())
            {
                foreach (string type in Type)
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
        /// <param name="Type">文件类型</param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfosWithOutType(string path, string[] Type)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in root.GetFiles())
            {
                bool EnabledFileType = true;
                foreach (string type in Type)
                {
                    if (type == "" && type == null)
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
        public static List<string> GetFileName<T>(this T[] files) where T : FileSystemInfo
        {
            List<string> fileNames = new List<string>();
            foreach (T file in files)
            {
                fileNames.Add(file.Name);
            }
            return fileNames;
        }
        /// <summary>
        /// 获得文件路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="files"></param>
        /// <returns></returns>
        public static List<string> GetFileFullName<T>(this T[] files) where T : FileSystemInfo
        {
            List<string> fileNames = new List<string>();
            foreach (T file in files)
            {
                fileNames.Add(file.FullName);
            }
            return fileNames;
        }
        /// <summary>
        /// 从文件名判断类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool JudgeFileType(string name, string Type)
        {
            if (name == null || name == "" || Type == null || Type == "")
                return false;
            string FileTypeName = name.Substring(name.Length - Type.Length - 1);
            return FileTypeName.LastIndexOf("." + Type, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        /// <summary>
        /// 从文件名判断类型
        /// </summary>
        /// <param name="file"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool JudgeFileType(this FileInfo file, string Type)
        {
            if (file == null || file.Name == "" || Type == null || Type == "")
                return false;
            string FileTypeName = file.Name.Substring(file.Name.Length - Type.Length - 1);
            return FileTypeName.LastIndexOf("." + Type, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
    /// <summary>
    /// 对象调整
    /// </summary>
    public static class ObjectAdjust
    {
#if UNITY_EDITOR || UNITY_STANDALONE
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
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void ColliderSetEnable<T>(this T[] GO, bool active) where T : Collider
        {
            foreach (var item in GO)
            {
                item.enabled = active;
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void ColliderSetEnable<T>(this List<T> GO, bool active) where T : Collider
        {
            foreach (var item in GO)
            {
                item.enabled = active;
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void BehaviourSetEnable<T>(this T[] GO, bool active) where T : Behaviour
        {
            foreach (var item in GO)
            {
                item.enabled = active;
            }
        }
        /// <summary>
        /// 批量设置活动状态
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="active">活动状态</param>
        public static void BehaviourSetEnable<T>(this List<T> GO, bool active) where T : Behaviour
        {
            foreach (var item in GO)
            {
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
                if (array[i].GetComponent<T>())
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
                if (list[i].GetComponent<T>())
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
                if (m_List[i].GetComponent<T>())
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
                if (m_Array[i].GetComponent<T>())
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
                if (m_List[i].gameObject)
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
                if (m_Array[i].gameObject)
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
                    if (string.IsNullOrEmpty(ColorShaderName))
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
    public class Vector2
    {
        public float x;
        public float y;
        public Vector2(float X = 0, float Y = 0)
        {
            this.x = X;
            this.y = Y;
        }
    }
    public class Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float X = 0, float Y = 0, float Z = 0)
        {
            this.x = X;
            this.y = Y;
            this.y = Z;
        }
    }
#endif
    /// <summary>
    /// 数值调整
    /// </summary>
    public static class ValueAdjust
    {
        /// <summary>
        /// 二维坐标转三维坐标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this Vector2 value)
        {
            return new Vector3(value.x, value.y);
        }
        /// <summary>
        /// 二维坐标数组转三维坐标数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3[] ToVector3(this Vector2[] value)
        {
            Vector3[] result = new Vector3[value.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = value[i].ToVector3();
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
        /// 清空重复项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> ClearRepeatingItem<T>(this List<T> list) where T : Object
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
            return ValueAdjust.ClearNullItem(list);
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
            List<T> result = new List<T>(array.Length);
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array is Object[])
                    {
                        if (array[i] as Object)
                            result.Add(array[i]);
                    }
                    else if (array is string)
                    {
                        if (!string.IsNullOrEmpty(array[i] as string))
                            result.Add(array[i]);
                    }
                    else if (array[i] != null)
                        result.Add(array[i]);
                }
                return result.ToArray();
            }
        }
        /// <summary>
        /// 列表相加。补充在后。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list01"></param>
        /// <param name="list02"></param>
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
        /// 列表相加。补充在后。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list01"></param>
        /// <param name="list02"></param>
        /// <returns></returns>
        public static List<T> AddList<T>(this List<T> list01, List<T> list02)
        {
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
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
                list.Add(array[i]);
            return list;
        }
        /// <summary>
        /// 列表转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this List<T> list)
        {
            return list.ToArray();
        }
        /// <summary>
        /// 数组ToString
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string[] ToStrings<T>(this T[] array)
        {
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
            string[] result = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                result[i] = list[i].ToString();
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
                    if (Random.value > 0.5f && i != j)
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
                    if (Random.value > 0.5f && i != j)
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
                result = temp[Random.Range(0, temp.Length - 1)];
            }
            else
            {
                result = array[Random.Range(0, array.Length - 1)];
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
                result = temp[Random.Range(0, temp.Length - 1)];
            }
            else
            {
                result = list[Random.Range(0, list.Count - 1)];
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
        /// 打印数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="newline">换行</param>
        /// <returns></returns>
        public static string PrintArray<T>(T[] array, bool newline = false)
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
        public static string PrintArray<T>(T[][] array)
        {
            string str = "";
            for (int i = 0; i < array.Length; i++)
            {
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
        /// 打印列表元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="newline">换行</param>
        /// <returns></returns>
        public static string PrintArray<T>(List<T> list, bool newline = false)
        {
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
        public static string PrintArray<T>(List<List<T>> list)
        {
            string str = "";
            for (int i = 0; i < list.Count; i++)
            {
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
        /// 找出字符串中第一个数字的序号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int FindIndexOfNumInString(this string number)
        {
            return number.IndexOfAny("0123456789".ToCharArray());
        }
        /// <summary>
        /// 找出字符串中第一个字母的序号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int FindIndexOfLetterInString(this string str)
        {
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
        /// 是否包含指定字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool Contain(this string source, string value)
        {
            return (source.IndexOf(value, StringComparison.Ordinal) >= 0);
        }
        /// <summary>
        /// 是否包含指定字符串（忽略大小写）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool ContainIgnoreCase(this string source, string value)
        {
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
            if (NormalSpeed == 0 || step == 0)
                return A;
            float speed = Math.Abs(NormalSpeed);
            if (setSpeedRange != 0)
            {
                if (PlusSpeed == 0)
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
        ///  A向B渐变（当前值，目标值，步长(Time.deltaTime或Time.unscaledDeltaTime)，(点，速度/s)）
        /// </summary>
        /// <param name="A">当前值</param>
        /// <param name="B">目标值</param>
        /// <param name="step">步长(Time.deltaTime或Time.unscaledDeltaTime)</param>
        /// <param name="PointSpeeds">(点，速度/s)</param>
        /// <returns></returns>
        public static float Lerp(float A, float B, float step, Vector2[] PointSpeeds)
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
                    speed = Math.Abs(Mathf.Lerp(PointSpeeds2[i - 1].y, PointSpeeds2[i].y, ToPercent01(B, PointSpeeds2[i - 1].x, PointSpeeds2[i].x)));//根据插值计算速度
                }
            }
            if (float.IsNaN(speed))
                return A;

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
            return (float)Math.Sqrt(Math.Abs((X + b) / k));
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
        /// 限位。返回不小于min且不大于max的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Clamp(this float value, float min, float max)
        {
            float result = 0;
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
            if (value < min)
                result = min;
            else if (value > max)
                result = max;
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
        /// 判定是否在范围内
        /// </summary>
        /// <param name="CurrentValue">当前值</param>
        /// <param name="MinValue"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public static bool InRange(this float CurrentValue, float MinValue, float MaxValue)
        {
            if (MinValue > MaxValue)
            {
                Exchange(MinValue, MaxValue, out MinValue, out MaxValue);
            }
            if (CurrentValue >= MinValue && CurrentValue <= MaxValue)
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
        /// 平滑处理
        /// </summary>
        /// <param name="array">数据</param>
        /// <param name="size">平滑范围</param>
        /// <param name="iterations">插值倍数（1为原数量）</param>
        /// <param name="outputIteration">输出插值扩充后的数据</param>
        /// <returns></returns>
        public static float[] Smoothing(float[] array, int size = 1, int iterations = 1, bool outputIteration = false)
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
        public static float GetAverage(float[] array, int index = -1, int size = 1, int startIndex = -1, int endIndex = -1)
        {
            float result = 0;
            float sum = 0;
            if (startIndex == -1 && endIndex == -1)
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
            if (index == -1)//全部求平均
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
        public static Vector2 GetAverage(Vector2[] array, int startIndex = -1, int endIndex = -1)
        {
            Vector2 Max = new Vector2();
            return ToVector2(GetAverage(ToVector3(array), startIndex, endIndex));
        }
        /// <summary>
        /// 取平均值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex">开始序号</param>
        /// <param name="endIndex">结束序号</param>
        /// <returns></returns>
        public static Vector3 GetAverage(Vector3[] array, int startIndex = -1, int endIndex = -1)
        {
            Vector3 Max = new Vector3();
            if (startIndex == -1 && endIndex == -1)
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
            if (min == max)
            { return 0; }
            else
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
            if (min == max)
            { return 0; }
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
        /// 汉字整数数字显示（最大正负99999）
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static string NumShowCN(this float N)//汉字数字显示
        {
            float num;
            string NumZ = "";//结果
            string RoundE = "";//汉字数字万位
            string RoundD = "";//汉字数字千位
            string RoundC = "";//汉字数字百位
            string RoundB = "";//汉字数字十位
            string RoundA = "";//汉字数字个位
            int E = 0;//数字万位
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
                RoundE = "";
                RoundD = "";
                RoundC = "";
                RoundB = "";
                RoundA = "零";
            }
            else if (num > 99999)
            {
                RoundE = "九万";
                RoundD = "九千";
                RoundC = "九百";
                RoundB = "九十";
                RoundA = "九";
            }
            else
            {
                //数字万位
                if (num > 9999)
                {
                    E = Mathf.FloorToInt(num / 10000);
                    switch (E)
                    {
                        case 1: RoundE = "一万"; break;
                        case 2: RoundE = "二万"; break;
                        case 3: RoundE = "三万"; break;
                        case 4: RoundE = "四万"; break;
                        case 5: RoundE = "五万"; break;
                        case 6: RoundE = "六万"; break;
                        case 7: RoundE = "七万"; break;
                        case 8: RoundE = "八万"; break;
                        case 9: RoundE = "九万"; break;
                        default: RoundE = ""; break;
                    }
                }
                //数字千位
                if (num > 999)
                {
                    D = Mathf.FloorToInt((num - E * 10000) / 1000);
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
                    C = Mathf.FloorToInt((num - E * 10000 - D * 1000) / 100);
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
                    B = Mathf.FloorToInt((num - E * 10000 - D * 1000 - C * 100) / 10);
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
                A = Mathf.FloorToInt(num - E * 10000 - D * 1000 - C * 100 - B * 10);
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

                if (E == 0 && D == 0 && C == 0 && B == 1)
                {
                    RoundB = "十";
                }
                else if (E > 0 && D == 0 && C > 0 && B == 0 && A > 0)
                {
                    RoundD = "零";
                    RoundB = "零";
                }
                else if (C > 0 && B == 0 && A > 0)
                {
                    RoundB = "零";
                }
                else if ((D > 0 && C == 0 && B > 0) || (D > 0 && C == 0 && B == 0 && A > 0))
                {
                    RoundC = "零";
                }
                else if ((E > 0 && D == 0 && C == 0 && B == 0 && A > 0) || (E > 0 && D == 0 && C == 0 && B > 0) || (E > 0 && D == 0 && C > 0))
                {
                    RoundD = "零";
                }
            }
            NumZ = (N < 0 ? "负" : "") + RoundE + RoundD + RoundC + RoundB + RoundA;
            //Debug.Log(NumZ);
            return NumZ;
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
    /// <summary>
    /// 屏幕键盘
    /// </summary>
    public static class VirtualKeyboard
    {

        private const uint WM_SYSCOMMAND = 274;
        private const int SC_CLOSE = 61536;
        private static Process ExternalCall(string filename, string arguments, bool hideWindow)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filename;
            startInfo.Arguments = arguments;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (hideWindow)
            {
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
            }
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            return process;
        }
        /// <summary>
        /// 识别系统打开虚拟键盘
        /// </summary>
        public static void OpenKeyboard()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            try
            {
                if (System.IO.File.Exists("C:\\Program Files\\Common Files\\Microsoft Shared\\ink\\TabTip.exe")
                    //&& FindWindow("IPTip_Main_Window", null) != IntPtr.Zero 
                    && SystemInfo.processorType.IndexOf("Atom", StringComparison.OrdinalIgnoreCase) < 0)//测试发现CPU为Atom型号的Win10平板有TabTip.exe但没成功实现切换，因此排除
                {
                    OpenTabTip();
                }
                else
                {
                    OpenOSK();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
#endif
        }
        /// <summary>
        /// 打开旧版屏幕键盘Osk.exe
        /// </summary>
        public static void OpenOSK()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Process.Start("C:\\Windows\\System32\\osk.exe");
#endif
        }
        /// <summary>
        /// 打开屏幕键盘TabTip.exe
        /// </summary>
        public static void OpenTabTip()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            //HideTabTip();
            ExternalCall("C:\\Program Files\\Common Files\\Microsoft Shared\\ink\\TabTip.exe", null, false);
#endif
        }
        /// <summary>
        /// 隐藏屏幕键盘TabTip.exe
        /// </summary>
        public static void HideTabTip()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            try
            {
                IntPtr ptr = WindowsAPI.User32.FindWindow("IPTip_Main_Window", null);
                if (ptr == IntPtr.Zero)
                    return;
                WindowsAPI.User32.PostMessage(ptr, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
#endif
        }
    }
}
