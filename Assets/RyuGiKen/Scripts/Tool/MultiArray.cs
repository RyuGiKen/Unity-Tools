using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using RyuGiKen;
namespace RyuGiKen
{
    public abstract class MultiArrayBase
    {
        public static implicit operator bool(MultiArrayBase exists) { return exists != null; }
    }
    public interface IGetRandomOne<T>
    {
        T GetRandomOne(int index1);
    }
    public interface IGetItem<T>
    {
        T GetItem(int index1, int index2);
    }
    public interface IGetLength
    {
        int Length { get; }
    }
    public abstract class MultiArrayExtension<T> : MultiArray<T>
    {
        public abstract new T GetRandomOne(int index1);
        public abstract new T GetItem(int index1, int index2);
        public abstract new int Length { get; }
    }
    /// <summary>
    /// 交错数组
    /// </summary>
    [System.Serializable]
    public class MultiArray<T> : MultiArrayBase, IGetRandomOne<T>, IGetItem<T>, IGetLength
    {
        public List<ReorderableList<T>> items;
        public MultiArray()
        {
            this.items = new List<ReorderableList<T>>();
        }
        public MultiArray(T[][] array)
        {
            this.items = new List<ReorderableList<T>>();
            for (int i = 0; i < array.Length; i++)
            {
                items.Add(new ReorderableList<T>(array[i]));
            }
        }
        public MultiArray(T[,] array)
        {
            this.items = new List<ReorderableList<T>>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                T[] temp = new T[array.GetLength(1)];
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    temp[j] = array[i, j];
                }
                items.Add(new ReorderableList<T>(temp));
            }
        }
        public T GetRandomOne(int index1)
        {
            //return items[index1].items.GetRandomItem();
            return items != null ? items.GetRandomOne(index1) : default(T);
        }
        public T GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default(T);
            }
            //return items.GetItem(index1, index2);
        }
        public int Length
        {
            get
            {
                return items != null ? items.GetLength() : 0;
            }
        }
    }
    public static partial class MultiArrayExtension
    {
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <returns></returns>
        public static T[][] ConvertArray<T>(this MultiArray<T> multiArray)
        {
            if (multiArray == null || multiArray.items == null || multiArray.Length < 1)
                return null;
            T[][] result = new T[multiArray.items.Count][];
            for (int i = 0; i < multiArray.items.Count; i++)
            {
                result[i] = new T[multiArray.items[i].Count];
                for (int j = 0; j < result[i].Length; j++)
                {
                    result[i][j] = multiArray.GetItem(i, j);
                }
            }
            return result;
        }
        /// <summary>
        /// 数组转换
        /// </summary>
        /// <returns></returns>
        public static MultiArray<T> ConvertArray<T>(this T[][] array)
        {
            if (array == null || array.Length < 1)
                return null;
            List<ReorderableList<T>> temp = new List<ReorderableList<T>>();
            for (int i = 0; i < array.Length; i++)
            {
                temp[i] = new ReorderableList<T>();
                temp[i].items = new List<T>();
                for (int j = 0; j < array[i].Length; j++)
                {
                    temp[i].items[j] = array[i][j];
                }
            }
            MultiArray<T> result = new MultiArray<T>();
            result.items = temp;
            return result;
        }
        /// <summary>
        /// 转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="multiArray"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this MultiArray<T> multiArray)
        {
            if (multiArray == null)
                return null;

            List<T> result = new List<T>();
            for (int i = 0; i < multiArray.items.Count; i++)
            {
                for (int j = 0; j < multiArray.items[i].Count; j++)
                    result.Add(multiArray.items[i].items[j]);
            }
            return result.ToArray();
        }
        /// <summary>
        /// 打印数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string PrintMutiArray<T>(this MultiArray<T> array)
        {
            return PrintMutiArray(array);
        }
        /// <summary>
        /// 打印数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string PrintMutiArray<T>(this T[][] array)
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
        /// 打印列表元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string PrintMutiArray<T>(this List<List<T>> list)
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
    }
    [Serializable]
    public class MultiArrayBoolean : MultiArrayExtension<bool>
    {
        public new List<ReorderableListBoolean> items;
        public MultiArrayBoolean(bool[][] array)
        {
            this.items = new List<ReorderableListBoolean>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListBoolean(array[i]));
        }
        public MultiArrayBoolean(ReorderableList<bool>[] array)
        {
            this.items = new List<ReorderableListBoolean>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListBoolean(array[i].ToArray()));
        }
        public MultiArrayBoolean(ReorderableListBoolean[] array)
        {
            this.items = new List<ReorderableListBoolean>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayBoolean(List<ReorderableList<bool>> list)
        {
            this.items = new List<ReorderableListBoolean>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListBoolean(list[i].ToArray()));
        }
        public MultiArrayBoolean(List<ReorderableListBoolean> list) { this.items = list; }
        public override bool GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override bool GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayInteger : MultiArrayInteger32
    {
        public MultiArrayInteger(int[][] array)
        {
            this.items = new List<ReorderableListInteger32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListInteger32(array[i]));
        }
        public MultiArrayInteger(ReorderableList<int>[] array)
        {
            this.items = new List<ReorderableListInteger32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListInteger32(array[i].ToArray()));
        }
        public MultiArrayInteger(ReorderableListInteger32[] array)
        {
            this.items = new List<ReorderableListInteger32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayInteger(List<ReorderableList<int>> list)
        {
            this.items = new List<ReorderableListInteger32>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListInteger32(list[i].ToArray()));
        }
        public MultiArrayInteger(List<ReorderableListInteger32> list) { this.items = list; }
    }
    [Serializable]
    public class MultiArrayInteger16 : MultiArrayExtension<short>
    {
        public new List<ReorderableListInteger16> items;
        public MultiArrayInteger16(short[][] array)
        {
            this.items = new List<ReorderableListInteger16>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListInteger16(array[i]));
        }
        public MultiArrayInteger16(ReorderableList<short>[] array)
        {
            this.items = new List<ReorderableListInteger16>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListInteger16(array[i].ToArray()));
        }
        public MultiArrayInteger16(ReorderableListInteger16[] array)
        {
            this.items = new List<ReorderableListInteger16>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayInteger16(List<ReorderableList<short>> list)
        {
            this.items = new List<ReorderableListInteger16>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListInteger16(list[i].ToArray()));
        }
        public MultiArrayInteger16(List<ReorderableListInteger16> list) { this.items = list; }
        public override short GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override short GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayInteger32 : MultiArrayExtension<int>
    {
        public new List<ReorderableListInteger32> items;
        public MultiArrayInteger32() { this.items = new List<ReorderableListInteger32>(); }
        public MultiArrayInteger32(int[][] array)
        {
            this.items = new List<ReorderableListInteger32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListInteger32(array[i]));
        }
        public MultiArrayInteger32(ReorderableList<int>[] array)
        {
            this.items = new List<ReorderableListInteger32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListInteger32(array[i].ToArray()));
        }
        public MultiArrayInteger32(ReorderableListInteger32[] array)
        {
            this.items = new List<ReorderableListInteger32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayInteger32(List<ReorderableList<int>> list)
        {
            this.items = new List<ReorderableListInteger32>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListInteger32(list[i].ToArray()));
        }
        public MultiArrayInteger32(List<ReorderableListInteger32> list) { this.items = list; }
        public override int GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override int GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayInteger64 : MultiArrayExtension<long>
    {
        public new List<ReorderableListInteger64> items;
        public MultiArrayInteger64(long[][] array)
        {
            this.items = new List<ReorderableListInteger64>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListInteger64(array[i]));
        }
        public MultiArrayInteger64(ReorderableList<long>[] array)
        {
            this.items = new List<ReorderableListInteger64>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListInteger64(array[i].ToArray()));
        }
        public MultiArrayInteger64(ReorderableListInteger64[] array)
        {
            this.items = new List<ReorderableListInteger64>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayInteger64(List<ReorderableList<long>> list)
        {
            this.items = new List<ReorderableListInteger64>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListInteger64(list[i].ToArray()));
        }
        public MultiArrayInteger64(List<ReorderableListInteger64> list) { this.items = list; }
        public override long GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override long GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayUInteger : MultiArrayExtension<uint>
    {
        public new List<ReorderableListUInteger> items;
        public MultiArrayUInteger(uint[][] array)
        {
            this.items = new List<ReorderableListUInteger>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListUInteger(array[i]));
        }
        public MultiArrayUInteger(ReorderableList<uint>[] array)
        {
            this.items = new List<ReorderableListUInteger>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListUInteger(array[i].ToArray()));
        }
        public MultiArrayUInteger(ReorderableListUInteger[] array)
        {
            this.items = new List<ReorderableListUInteger>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayUInteger(List<ReorderableList<uint>> list)
        {
            this.items = new List<ReorderableListUInteger>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListUInteger(list[i].ToArray()));
        }
        public MultiArrayUInteger(List<ReorderableListUInteger> list) { this.items = list; }
        public override uint GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override uint GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayFloat : MultiArrayExtension<float>
    {
        public new List<ReorderableListFloat> items;
        public MultiArrayFloat(float[][] array)
        {
            this.items = new List<ReorderableListFloat>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListFloat(array[i]));
        }
        public MultiArrayFloat(ReorderableList<float>[] array)
        {
            this.items = new List<ReorderableListFloat>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListFloat(array[i].ToArray()));
        }
        public MultiArrayFloat(ReorderableListFloat[] array)
        {
            this.items = new List<ReorderableListFloat>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayFloat(List<ReorderableList<float>> list)
        {
            this.items = new List<ReorderableListFloat>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListFloat(list[i].ToArray()));
        }
        public MultiArrayFloat(List<ReorderableListFloat> list) { this.items = list; }
        public override float GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override float GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayDouble : MultiArrayExtension<double>
    {
        public new List<ReorderableListDouble> items;
        public MultiArrayDouble(double[][] array)
        {
            this.items = new List<ReorderableListDouble>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListDouble(array[i]));
        }
        public MultiArrayDouble(ReorderableList<double>[] array)
        {
            this.items = new List<ReorderableListDouble>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListDouble(array[i].ToArray()));
        }
        public MultiArrayDouble(ReorderableListDouble[] array)
        {
            this.items = new List<ReorderableListDouble>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayDouble(List<ReorderableList<double>> list)
        {
            this.items = new List<ReorderableListDouble>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListDouble(list[i].ToArray()));
        }
        public MultiArrayDouble(List<ReorderableListDouble> list) { this.items = list; }
        public override double GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override double GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayDecimal : MultiArrayExtension<decimal>
    {
        public new List<ReorderableListDecimal> items;
        public MultiArrayDecimal(decimal[][] array)
        {
            this.items = new List<ReorderableListDecimal>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListDecimal(array[i]));
        }
        public MultiArrayDecimal(ReorderableList<decimal>[] array)
        {
            this.items = new List<ReorderableListDecimal>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListDecimal(array[i].ToArray()));
        }
        public MultiArrayDecimal(ReorderableListDecimal[] array)
        {
            this.items = new List<ReorderableListDecimal>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayDecimal(List<ReorderableList<decimal>> list)
        {
            this.items = new List<ReorderableListDecimal>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListDecimal(list[i].ToArray()));
        }
        public MultiArrayDecimal(List<ReorderableListDecimal> list) { this.items = list; }
        public override decimal GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override decimal GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayString : MultiArrayExtension<string>
    {
        public new List<ReorderableListString> items;
        public MultiArrayString(string[][] array)
        {
            this.items = new List<ReorderableListString>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListString(array[i]));
        }
        public MultiArrayString(ReorderableList<string>[] array)
        {
            this.items = new List<ReorderableListString>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListString(array[i].ToArray()));
        }
        public MultiArrayString(ReorderableListString[] array)
        {
            this.items = new List<ReorderableListString>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayString(List<ReorderableList<string>> list)
        {
            this.items = new List<ReorderableListString>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListString(list[i].ToArray()));
        }
        public MultiArrayString(List<ReorderableListString> list) { this.items = list; }
        public override string GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override string GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
#if UNITY_EDITOR || UNITY_STANDALONE
    [Serializable]
    public class MultiArrayVector2 : MultiArrayExtension<Vector2>
    {
        public new List<ReorderableListVector2> items;
        public MultiArrayVector2(Vector2[][] array)
        {
            this.items = new List<ReorderableListVector2>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector2(array[i]));
        }
        public MultiArrayVector2(ReorderableList<Vector2>[] array)
        {
            this.items = new List<ReorderableListVector2>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector2(array[i].ToArray()));
        }
        public MultiArrayVector2(ReorderableListVector2[] array)
        {
            this.items = new List<ReorderableListVector2>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayVector2(List<ReorderableList<Vector2>> list)
        {
            this.items = new List<ReorderableListVector2>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListVector2(list[i].ToArray()));
        }
        public MultiArrayVector2(List<ReorderableListVector2> list) { this.items = list; }
        public override Vector2 GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Vector2 GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayVector2Int : MultiArrayExtension<Vector2Int>
    {
        public new List<ReorderableListVector2Int> items;
        public MultiArrayVector2Int(Vector2Int[][] array)
        {
            this.items = new List<ReorderableListVector2Int>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector2Int(array[i]));
        }
        public MultiArrayVector2Int(ReorderableList<Vector2Int>[] array)
        {
            this.items = new List<ReorderableListVector2Int>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector2Int(array[i].ToArray()));
        }
        public MultiArrayVector2Int(ReorderableListVector2Int[] array)
        {
            this.items = new List<ReorderableListVector2Int>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayVector2Int(List<ReorderableList<Vector2Int>> list)
        {
            this.items = new List<ReorderableListVector2Int>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListVector2Int(list[i].ToArray()));
        }
        public MultiArrayVector2Int(List<ReorderableListVector2Int> list) { this.items = list; }
        public override Vector2Int GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Vector2Int GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayVector3 : MultiArrayExtension<Vector3>
    {
        public new List<ReorderableListVector3> items;
        public MultiArrayVector3(Vector3[][] array)
        {
            this.items = new List<ReorderableListVector3>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector3(array[i]));
        }
        public MultiArrayVector3(ReorderableList<Vector3>[] array)
        {
            this.items = new List<ReorderableListVector3>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector3(array[i].ToArray()));
        }
        public MultiArrayVector3(ReorderableListVector3[] array)
        {
            this.items = new List<ReorderableListVector3>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayVector3(List<ReorderableList<Vector3>> list)
        {
            this.items = new List<ReorderableListVector3>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListVector3(list[i].ToArray()));
        }
        public MultiArrayVector3(List<ReorderableListVector3> list) { this.items = list; }
        public override Vector3 GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Vector3 GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayVector3Int : MultiArrayExtension<Vector3Int>
    {
        public new List<ReorderableListVector3Int> items;
        public MultiArrayVector3Int(Vector3Int[][] array)
        {
            this.items = new List<ReorderableListVector3Int>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector3Int(array[i]));
        }
        public MultiArrayVector3Int(ReorderableList<Vector3Int>[] array)
        {
            this.items = new List<ReorderableListVector3Int>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector3Int(array[i].ToArray()));
        }
        public MultiArrayVector3Int(ReorderableListVector3Int[] array)
        {
            this.items = new List<ReorderableListVector3Int>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayVector3Int(List<ReorderableList<Vector3Int>> list)
        {
            this.items = new List<ReorderableListVector3Int>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListVector3Int(list[i].ToArray()));
        }
        public MultiArrayVector3Int(List<ReorderableListVector3Int> list) { this.items = list; }
        public override Vector3Int GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Vector3Int GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayVector4 : MultiArrayExtension<Vector4>
    {
        public new List<ReorderableListVector4> items;
        public MultiArrayVector4(Vector4[][] array)
        {
            this.items = new List<ReorderableListVector4>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector4(array[i]));
        }
        public MultiArrayVector4(ReorderableList<Vector4>[] array)
        {
            this.items = new List<ReorderableListVector4>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListVector4(array[i].ToArray()));
        }
        public MultiArrayVector4(ReorderableListVector4[] array)
        {
            this.items = new List<ReorderableListVector4>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayVector4(List<ReorderableList<Vector4>> list)
        {
            this.items = new List<ReorderableListVector4>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListVector4(list[i].ToArray()));
        }
        public MultiArrayVector4(List<ReorderableListVector4> list) { this.items = list; }
        public override Vector4 GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Vector4 GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayQuaternion : MultiArrayExtension<Quaternion>
    {
        public new List<ReorderableListQuaternion> items;
        public MultiArrayQuaternion(Quaternion[][] array)
        {
            this.items = new List<ReorderableListQuaternion>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListQuaternion(array[i]));
        }
        public MultiArrayQuaternion(ReorderableList<Quaternion>[] array)
        {
            this.items = new List<ReorderableListQuaternion>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListQuaternion(array[i].ToArray()));
        }
        public MultiArrayQuaternion(ReorderableListQuaternion[] array)
        {
            this.items = new List<ReorderableListQuaternion>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayQuaternion(List<ReorderableList<Quaternion>> list)
        {
            this.items = new List<ReorderableListQuaternion>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListQuaternion(list[i].ToArray()));
        }
        public MultiArrayQuaternion(List<ReorderableListQuaternion> list) { this.items = list; }
        public override Quaternion GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Quaternion GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayColor : MultiArrayExtension<Color>
    {
        public new List<ReorderableListColor> items;
        public MultiArrayColor(Color[][] array)
        {
            this.items = new List<ReorderableListColor>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListColor(array[i]));
        }
        public MultiArrayColor(ReorderableList<Color>[] array)
        {
            this.items = new List<ReorderableListColor>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListColor(array[i].ToArray()));
        }
        public MultiArrayColor(ReorderableListColor[] array)
        {
            this.items = new List<ReorderableListColor>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayColor(List<ReorderableList<Color>> list)
        {
            this.items = new List<ReorderableListColor>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListColor(list[i].ToArray()));
        }
        public MultiArrayColor(List<ReorderableListColor> list) { this.items = list; }
        public override Color GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Color GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayColor32 : MultiArrayExtension<Color32>
    {
        public new List<ReorderableListColor32> items;
        public MultiArrayColor32(Color32[][] array)
        {
            this.items = new List<ReorderableListColor32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListColor32(array[i]));
        }
        public MultiArrayColor32(ReorderableList<Color32>[] array)
        {
            this.items = new List<ReorderableListColor32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListColor32(array[i].ToArray()));
        }
        public MultiArrayColor32(ReorderableListColor32[] array)
        {
            this.items = new List<ReorderableListColor32>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayColor32(List<ReorderableList<Color32>> list)
        {
            this.items = new List<ReorderableListColor32>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListColor32(list[i].ToArray()));
        }
        public MultiArrayColor32(List<ReorderableListColor32> list) { this.items = list; }
        public override Color32 GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Color32 GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
#endif
    [Serializable]
    public class MultiArrayHSVColor : MultiArrayExtension<HSVColor>
    {
        public new List<ReorderableListHSVColor> items;
        public MultiArrayHSVColor(HSVColor[][] array)
        {
            this.items = new List<ReorderableListHSVColor>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListHSVColor(array[i]));
        }
        public MultiArrayHSVColor(ReorderableList<HSVColor>[] array)
        {
            this.items = new List<ReorderableListHSVColor>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListHSVColor(array[i].ToArray()));
        }
        public MultiArrayHSVColor(ReorderableListHSVColor[] array)
        {
            this.items = new List<ReorderableListHSVColor>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayHSVColor(List<ReorderableList<HSVColor>> list)
        {
            this.items = new List<ReorderableListHSVColor>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListHSVColor(list[i].ToArray()));
        }
        public MultiArrayHSVColor(List<ReorderableListHSVColor> list) { this.items = list; }
        public override HSVColor GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override HSVColor GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
#if UNITY_EDITOR || UNITY_STANDALONE
    [Serializable]
    public class MultiArrayObject : MultiArrayExtension<Object>
    {
        public new List<ReorderableListObject> items;
        public MultiArrayObject(Object[][] array)
        {
            this.items = new List<ReorderableListObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListObject(array[i]));
        }
        public MultiArrayObject(ReorderableList<Object>[] array)
        {
            this.items = new List<ReorderableListObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListObject(array[i].ToArray()));
        }
        public MultiArrayObject(ReorderableListObject[] array)
        {
            this.items = new List<ReorderableListObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayObject(List<ReorderableList<Object>> list)
        {
            this.items = new List<ReorderableListObject>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListObject(list[i].ToArray()));
        }
        public MultiArrayObject(List<ReorderableListObject> list) { this.items = list; }
        public override Object GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Object GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayScriptableObject : MultiArrayExtension<ScriptableObject>
    {
        public new List<ReorderableListScriptableObject> items;
        public MultiArrayScriptableObject(ScriptableObject[][] array)
        {
            this.items = new List<ReorderableListScriptableObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListScriptableObject(array[i]));
        }
        public MultiArrayScriptableObject(ReorderableList<ScriptableObject>[] array)
        {
            this.items = new List<ReorderableListScriptableObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListScriptableObject(array[i].ToArray()));
        }
        public MultiArrayScriptableObject(ReorderableListScriptableObject[] array)
        {
            this.items = new List<ReorderableListScriptableObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayScriptableObject(List<ReorderableList<ScriptableObject>> list)
        {
            this.items = new List<ReorderableListScriptableObject>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListScriptableObject(list[i].ToArray()));
        }
        public MultiArrayScriptableObject(List<ReorderableListScriptableObject> list) { this.items = list; }
        public override ScriptableObject GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override ScriptableObject GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayGameObject : MultiArrayExtension<GameObject>
    {
        public new List<ReorderableListGameObject> items;
        public MultiArrayGameObject(GameObject[][] array)
        {
            this.items = new List<ReorderableListGameObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListGameObject(array[i]));
        }
        public MultiArrayGameObject(ReorderableList<GameObject>[] array)
        {
            this.items = new List<ReorderableListGameObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListGameObject(array[i].ToArray()));
        }
        public MultiArrayGameObject(ReorderableListGameObject[] array)
        {
            this.items = new List<ReorderableListGameObject>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayGameObject(List<ReorderableList<GameObject>> list)
        {
            this.items = new List<ReorderableListGameObject>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListGameObject(list[i].ToArray()));
        }
        public MultiArrayGameObject(List<ReorderableListGameObject> list) { this.items = list; }
        public override GameObject GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override GameObject GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayComponent : MultiArrayExtension<Component>
    {
        public new List<ReorderableListComponent> items;
        public MultiArrayComponent(Component[][] array)
        {
            this.items = new List<ReorderableListComponent>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListComponent(array[i]));
        }
        public MultiArrayComponent(ReorderableList<Component>[] array)
        {
            this.items = new List<ReorderableListComponent>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListComponent(array[i].ToArray()));
        }
        public MultiArrayComponent(ReorderableListComponent[] array)
        {
            this.items = new List<ReorderableListComponent>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayComponent(List<ReorderableList<Component>> list)
        {
            this.items = new List<ReorderableListComponent>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListComponent(list[i].ToArray()));
        }
        public MultiArrayComponent(List<ReorderableListComponent> list) { this.items = list; }
        public override Component GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Component GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayTransform : MultiArrayExtension<Transform>
    {
        public new List<ReorderableListTransform> items;
        public MultiArrayTransform(Transform[][] array)
        {
            this.items = new List<ReorderableListTransform>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListTransform(array[i]));
        }
        public MultiArrayTransform(ReorderableList<Transform>[] array)
        {
            this.items = new List<ReorderableListTransform>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListTransform(array[i].ToArray()));
        }
        public MultiArrayTransform(ReorderableListTransform[] array)
        {
            this.items = new List<ReorderableListTransform>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayTransform(List<ReorderableList<Transform>> list)
        {
            this.items = new List<ReorderableListTransform>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListTransform(list[i].ToArray()));
        }
        public MultiArrayTransform(List<ReorderableListTransform> list) { this.items = list; }
        public override Transform GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Transform GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayRectTransform : MultiArrayExtension<RectTransform>
    {
        public new List<ReorderableListRectTransform> items;
        public MultiArrayRectTransform(RectTransform[][] array)
        {
            this.items = new List<ReorderableListRectTransform>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListRectTransform(array[i]));
        }
        public MultiArrayRectTransform(ReorderableList<RectTransform>[] array)
        {
            this.items = new List<ReorderableListRectTransform>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListRectTransform(array[i].ToArray()));
        }
        public MultiArrayRectTransform(ReorderableListRectTransform[] array)
        {
            this.items = new List<ReorderableListRectTransform>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayRectTransform(List<ReorderableList<RectTransform>> list)
        {
            this.items = new List<ReorderableListRectTransform>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListRectTransform(list[i].ToArray()));
        }
        public MultiArrayRectTransform(List<ReorderableListRectTransform> list) { this.items = list; }
        public override RectTransform GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override RectTransform GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayBehaviour : MultiArrayExtension<Behaviour>
    {
        public new List<ReorderableListBehaviour> items;
        public MultiArrayBehaviour(Behaviour[][] array)
        {
            this.items = new List<ReorderableListBehaviour>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListBehaviour(array[i]));
        }
        public MultiArrayBehaviour(ReorderableList<Behaviour>[] array)
        {
            this.items = new List<ReorderableListBehaviour>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListBehaviour(array[i].ToArray()));
        }
        public MultiArrayBehaviour(ReorderableListBehaviour[] array)
        {
            this.items = new List<ReorderableListBehaviour>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayBehaviour(List<ReorderableList<Behaviour>> list)
        {
            this.items = new List<ReorderableListBehaviour>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListBehaviour(list[i].ToArray()));
        }
        public MultiArrayBehaviour(List<ReorderableListBehaviour> list) { this.items = list; }
        public override Behaviour GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Behaviour GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayMonoBehaviour : MultiArrayExtension<MonoBehaviour>
    {
        public new List<ReorderableListMonoBehaviour> items;
        public MultiArrayMonoBehaviour(MonoBehaviour[][] array)
        {
            this.items = new List<ReorderableListMonoBehaviour>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListMonoBehaviour(array[i]));
        }
        public MultiArrayMonoBehaviour(ReorderableList<MonoBehaviour>[] array)
        {
            this.items = new List<ReorderableListMonoBehaviour>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListMonoBehaviour(array[i].ToArray()));
        }
        public MultiArrayMonoBehaviour(ReorderableListMonoBehaviour[] array)
        {
            this.items = new List<ReorderableListMonoBehaviour>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayMonoBehaviour(List<ReorderableList<MonoBehaviour>> list)
        {
            this.items = new List<ReorderableListMonoBehaviour>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListMonoBehaviour(list[i].ToArray()));
        }
        public MultiArrayMonoBehaviour(List<ReorderableListMonoBehaviour> list) { this.items = list; }
        public override MonoBehaviour GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override MonoBehaviour GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayGraphic : MultiArrayExtension<Graphic>
    {
        public new List<ReorderableListGraphic> items;
        public MultiArrayGraphic(Graphic[][] array)
        {
            this.items = new List<ReorderableListGraphic>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListGraphic(array[i]));
        }
        public MultiArrayGraphic(ReorderableList<Graphic>[] array)
        {
            this.items = new List<ReorderableListGraphic>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListGraphic(array[i].ToArray()));
        }
        public MultiArrayGraphic(ReorderableListGraphic[] array)
        {
            this.items = new List<ReorderableListGraphic>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayGraphic(List<ReorderableList<Graphic>> list)
        {
            this.items = new List<ReorderableListGraphic>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListGraphic(list[i].ToArray()));
        }
        public MultiArrayGraphic(List<ReorderableListGraphic> list) { this.items = list; }
        public override Graphic GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Graphic GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayText : MultiArrayExtension<Text>
    {
        public new List<ReorderableListText> items;
        public MultiArrayText(Text[][] array)
        {
            this.items = new List<ReorderableListText>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListText(array[i]));
        }
        public MultiArrayText(ReorderableList<Text>[] array)
        {
            this.items = new List<ReorderableListText>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListText(array[i].ToArray()));
        }
        public MultiArrayText(ReorderableListText[] array)
        {
            this.items = new List<ReorderableListText>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayText(List<ReorderableList<Text>> list)
        {
            this.items = new List<ReorderableListText>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListText(list[i].ToArray()));
        }
        public MultiArrayText(List<ReorderableListText> list) { this.items = list; }
        public override Text GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Text GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayImage : MultiArrayExtension<Image>
    {
        public new List<ReorderableListImage> items;
        public MultiArrayImage(Image[][] array)
        {
            this.items = new List<ReorderableListImage>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListImage(array[i]));
        }
        public MultiArrayImage(ReorderableList<Image>[] array)
        {
            this.items = new List<ReorderableListImage>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListImage(array[i].ToArray()));
        }
        public MultiArrayImage(ReorderableListImage[] array)
        {
            this.items = new List<ReorderableListImage>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayImage(List<ReorderableList<Image>> list)
        {
            this.items = new List<ReorderableListImage>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListImage(list[i].ToArray()));
        }
        public MultiArrayImage(List<ReorderableListImage> list) { this.items = list; }
        public override Image GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Image GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayRenderer : MultiArrayExtension<Renderer>
    {
        public new List<ReorderableListRenderer> items;
        public MultiArrayRenderer(Renderer[][] array)
        {
            this.items = new List<ReorderableListRenderer>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListRenderer(array[i]));
        }
        public MultiArrayRenderer(ReorderableList<Renderer>[] array)
        {
            this.items = new List<ReorderableListRenderer>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListRenderer(array[i].ToArray()));
        }
        public MultiArrayRenderer(ReorderableListRenderer[] array)
        {
            this.items = new List<ReorderableListRenderer>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayRenderer(List<ReorderableList<Renderer>> list)
        {
            this.items = new List<ReorderableListRenderer>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListRenderer(list[i].ToArray()));
        }
        public MultiArrayRenderer(List<ReorderableListRenderer> list) { this.items = list; }
        public override Renderer GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Renderer GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayCamera : MultiArrayExtension<Camera>
    {
        public new List<ReorderableListCamera> items;
        public MultiArrayCamera(Camera[][] array)
        {
            this.items = new List<ReorderableListCamera>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListCamera(array[i]));
        }
        public MultiArrayCamera(ReorderableList<Camera>[] array)
        {
            this.items = new List<ReorderableListCamera>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListCamera(array[i].ToArray()));
        }
        public MultiArrayCamera(ReorderableListCamera[] array)
        {
            this.items = new List<ReorderableListCamera>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayCamera(List<ReorderableList<Camera>> list)
        {
            this.items = new List<ReorderableListCamera>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListCamera(list[i].ToArray()));
        }
        public MultiArrayCamera(List<ReorderableListCamera> list) { this.items = list; }
        public override Camera GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Camera GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayCollider : MultiArrayExtension<Collider>
    {
        public new List<ReorderableListCollider> items;
        public MultiArrayCollider(Collider[][] array)
        {
            this.items = new List<ReorderableListCollider>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListCollider(array[i]));
        }
        public MultiArrayCollider(ReorderableList<Collider>[] array)
        {
            this.items = new List<ReorderableListCollider>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListCollider(array[i].ToArray()));
        }
        public MultiArrayCollider(ReorderableListCollider[] array)
        {
            this.items = new List<ReorderableListCollider>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayCollider(List<ReorderableList<Collider>> list)
        {
            this.items = new List<ReorderableListCollider>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListCollider(list[i].ToArray()));
        }
        public MultiArrayCollider(List<ReorderableListCollider> list) { this.items = list; }
        public override Collider GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Collider GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayCollider2D : MultiArrayExtension<Collider2D>
    {
        public new List<ReorderableListCollider2D> items;
        public MultiArrayCollider2D(Collider2D[][] array)
        {
            this.items = new List<ReorderableListCollider2D>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListCollider2D(array[i]));
        }
        public MultiArrayCollider2D(ReorderableList<Collider2D>[] array)
        {
            this.items = new List<ReorderableListCollider2D>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListCollider2D(array[i].ToArray()));
        }
        public MultiArrayCollider2D(ReorderableListCollider2D[] array)
        {
            this.items = new List<ReorderableListCollider2D>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayCollider2D(List<ReorderableList<Collider2D>> list)
        {
            this.items = new List<ReorderableListCollider2D>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListCollider2D(list[i].ToArray()));
        }
        public MultiArrayCollider2D(List<ReorderableListCollider2D> list) { this.items = list; }
        public override Collider2D GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Collider2D GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayTexture : MultiArrayExtension<Texture>
    {
        public new List<ReorderableListTexture> items;
        public MultiArrayTexture(Texture[][] array)
        {
            this.items = new List<ReorderableListTexture>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListTexture(array[i]));
        }
        public MultiArrayTexture(ReorderableList<Texture>[] array)
        {
            this.items = new List<ReorderableListTexture>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListTexture(array[i].ToArray()));
        }
        public MultiArrayTexture(ReorderableListTexture[] array)
        {
            this.items = new List<ReorderableListTexture>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayTexture(List<ReorderableList<Texture>> list)
        {
            this.items = new List<ReorderableListTexture>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListTexture(list[i].ToArray()));
        }
        public MultiArrayTexture(List<ReorderableListTexture> list) { this.items = list; }
        public override Texture GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Texture GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArraySprite : MultiArrayExtension<Sprite>
    {
        public new List<ReorderableListSprite> items;
        public MultiArraySprite(Sprite[][] array)
        {
            this.items = new List<ReorderableListSprite>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListSprite(array[i]));
        }
        public MultiArraySprite(ReorderableList<Sprite>[] array)
        {
            this.items = new List<ReorderableListSprite>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListSprite(array[i].ToArray()));
        }
        public MultiArraySprite(ReorderableListSprite[] array)
        {
            this.items = new List<ReorderableListSprite>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArraySprite(List<ReorderableList<Sprite>> list)
        {
            this.items = new List<ReorderableListSprite>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListSprite(list[i].ToArray()));
        }
        public MultiArraySprite(List<ReorderableListSprite> list) { this.items = list; }
        public override Sprite GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Sprite GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayAudioClip : MultiArrayExtension<AudioClip>
    {
        public new List<ReorderableListAudioClip> items;
        public MultiArrayAudioClip(AudioClip[][] array)
        {
            this.items = new List<ReorderableListAudioClip>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListAudioClip(array[i]));
        }
        public MultiArrayAudioClip(ReorderableList<AudioClip>[] array)
        {
            this.items = new List<ReorderableListAudioClip>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListAudioClip(array[i].ToArray()));
        }
        public MultiArrayAudioClip(ReorderableListAudioClip[] array)
        {
            this.items = new List<ReorderableListAudioClip>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayAudioClip(List<ReorderableList<AudioClip>> list)
        {
            this.items = new List<ReorderableListAudioClip>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListAudioClip(list[i].ToArray()));
        }
        public MultiArrayAudioClip(List<ReorderableListAudioClip> list) { this.items = list; }
        public override AudioClip GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override AudioClip GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayMaterial : MultiArrayExtension<Material>
    {
        public new List<ReorderableListMaterial> items;
        public MultiArrayMaterial(Material[][] array)
        {
            this.items = new List<ReorderableListMaterial>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListMaterial(array[i]));
        }
        public MultiArrayMaterial(ReorderableList<Material>[] array)
        {
            this.items = new List<ReorderableListMaterial>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListMaterial(array[i].ToArray()));
        }
        public MultiArrayMaterial(ReorderableListMaterial[] array)
        {
            this.items = new List<ReorderableListMaterial>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayMaterial(List<ReorderableList<Material>> list)
        {
            this.items = new List<ReorderableListMaterial>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListMaterial(list[i].ToArray()));
        }
        public MultiArrayMaterial(List<ReorderableListMaterial> list) { this.items = list; }
        public override Material GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override Material GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayAnimationClip : MultiArrayExtension<AnimationClip>
    {
        public new List<ReorderableListAnimationClip> items;
        public MultiArrayAnimationClip(AnimationClip[][] array)
        {
            this.items = new List<ReorderableListAnimationClip>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListAnimationClip(array[i]));
        }
        public MultiArrayAnimationClip(ReorderableList<AnimationClip>[] array)
        {
            this.items = new List<ReorderableListAnimationClip>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListAnimationClip(array[i].ToArray()));
        }
        public MultiArrayAnimationClip(ReorderableListAnimationClip[] array)
        {
            this.items = new List<ReorderableListAnimationClip>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayAnimationClip(List<ReorderableList<AnimationClip>> list)
        {
            this.items = new List<ReorderableListAnimationClip>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListAnimationClip(list[i].ToArray()));
        }
        public MultiArrayAnimationClip(List<ReorderableListAnimationClip> list) { this.items = list; }
        public override AnimationClip GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override AnimationClip GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
#endif
    [Serializable]
    public class MultiArrayValueRange : MultiArrayExtension<ValueRange>
    {
        public new List<ReorderableListValueRange> items;
        public MultiArrayValueRange(ValueRange[][] array)
        {
            this.items = new List<ReorderableListValueRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListValueRange(array[i]));
        }
        public MultiArrayValueRange(ReorderableList<ValueRange>[] array)
        {
            this.items = new List<ReorderableListValueRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListValueRange(array[i].ToArray()));
        }
        public MultiArrayValueRange(ReorderableListValueRange[] array)
        {
            this.items = new List<ReorderableListValueRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayValueRange(List<ReorderableList<ValueRange>> list)
        {
            this.items = new List<ReorderableListValueRange>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListValueRange(list[i].ToArray()));
        }
        public MultiArrayValueRange(List<ReorderableListValueRange> list) { this.items = list; }
        public override ValueRange GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override ValueRange GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayValueInRange : MultiArrayExtension<ValueInRange>
    {
        public new List<ReorderableListValueInRange> items;
        public MultiArrayValueInRange(ValueInRange[][] array)
        {
            this.items = new List<ReorderableListValueInRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListValueInRange(array[i]));
        }
        public MultiArrayValueInRange(ReorderableList<ValueInRange>[] array)
        {
            this.items = new List<ReorderableListValueInRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListValueInRange(array[i].ToArray()));
        }
        public MultiArrayValueInRange(ReorderableListValueInRange[] array)
        {
            this.items = new List<ReorderableListValueInRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayValueInRange(List<ReorderableList<ValueInRange>> list)
        {
            this.items = new List<ReorderableListValueInRange>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListValueInRange(list[i].ToArray()));
        }
        public MultiArrayValueInRange(List<ReorderableListValueInRange> list) { this.items = list; }
        public override ValueInRange GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override ValueInRange GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
    [Serializable]
    public class MultiArrayValueWithRange : MultiArrayExtension<ValueWithRange>
    {
        public new List<ReorderableListValueWithRange> items;
        public MultiArrayValueWithRange(ValueWithRange[][] array)
        {
            this.items = new List<ReorderableListValueWithRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListValueWithRange(array[i]));
        }
        public MultiArrayValueWithRange(ReorderableList<ValueWithRange>[] array)
        {
            this.items = new List<ReorderableListValueWithRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(new ReorderableListValueWithRange(array[i].ToArray()));
        }
        public MultiArrayValueWithRange(ReorderableListValueWithRange[] array)
        {
            this.items = new List<ReorderableListValueWithRange>();
            if (array != null)
                for (int i = 0; i < array.Length; i++)
                    items.Add(array[i]);
        }
        public MultiArrayValueWithRange(List<ReorderableList<ValueWithRange>> list)
        {
            this.items = new List<ReorderableListValueWithRange>();
            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    items.Add(new ReorderableListValueWithRange(list[i].ToArray()));
        }
        public MultiArrayValueWithRange(List<ReorderableListValueWithRange> list) { this.items = list; }
        public override ValueWithRange GetRandomOne(int index1)
        {
            return items[index1].items.GetRandomItem();
        }
        public override ValueWithRange GetItem(int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default;
            }
        }
        public override int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i])
                        result += items[i].Count;
                }
                return result;
            }
        }
    }
}
namespace RyuGiKenEditor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MultiArrayBase))]
    public class MultiArrayPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
    }
#endif
}
