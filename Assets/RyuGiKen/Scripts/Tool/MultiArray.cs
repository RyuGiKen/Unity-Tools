using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace RyuGiKen
{
    public class MultiArrayBase
    {
        public static implicit operator bool(MultiArrayBase exists) { return exists != null; }
    }
    /// <summary>
    /// 交错数组
    /// </summary>
    [System.Serializable]
    public class MultiArray<T> : MultiArrayBase
    {
        public List<ReorderableList<T>> items;
        public MultiArray()
        {
            this.items = new List<ReorderableList<T>>();
        }
        public MultiArray(T[][] array)
        {
            this.items = new List<ReorderableList<T>>();
            for(int i = 0; i < array.Length; i++)
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
            return items[index1].items.GetRandomItem();
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
        }
        public T[] AllItems
        {
            get
            {
                List<T> result = new List<T>();
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Count > 0)
                        result.AddList(items[i].items);
                }
                return result.ToArray();
            }
        }
        public T this[int index]
        {
            get
            {
                try
                {
                    return AllItems[index];
                }
                catch
                {
                    return default(T);
                }
            }
        }
        public int Length
        {
            get
            {
                int result = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    result += items[i].Count;
                }
                return result;
            }
        }
    }
    public static partial class Extension
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
        /// 转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="multiArray"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this MultiArray<T> multiArray)
        {
            return multiArray ? multiArray.AllItems : null;
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
    [Serializable] public class MultiArrayBoolean : MultiArray<bool> { public new List<ReorderableListBoolean> items; }
    [Serializable] public class MultiArrayInteger : MultiArrayInteger32 { }
    [Serializable] public class MultiArrayInteger16 : MultiArray<short> { public new List<ReorderableListInteger16> items; }
    [Serializable] public class MultiArrayInteger32 : MultiArray<int> { public new List<ReorderableListInteger32> items; }
    [Serializable] public class MultiArrayInteger64 : MultiArray<long> { public new List<ReorderableListInteger64> items; }
    [Serializable] public class MultiArrayUInteger : MultiArray<uint> { public new List<ReorderableListUInteger> items; }
    [Serializable] public class MultiArrayFloat : MultiArray<float> { public new List<ReorderableListFloat> items; }
    [Serializable] public class MultiArrayDouble : MultiArray<double> { public new List<ReorderableListDouble> items; }
    [Serializable] public class MultiArrayDecimal : MultiArray<decimal> { public new List<ReorderableListDecimal> items; }
    [Serializable] public class MultiArrayString : MultiArray<string> { public new List<ReorderableListString> items; }
#if UNITY_EDITOR || UNITY_STANDALONE
    [Serializable] public class MultiArrayVector2 : MultiArray<Vector2> { public new List<ReorderableListVector2> items; }
    [Serializable] public class MultiArrayVector2Int : MultiArray<Vector2Int> { public new List<ReorderableListVector2Int> items; }
    [Serializable] public class MultiArrayVector3 : MultiArray<Vector3> { public new List<ReorderableListVector3> items; }
    [Serializable] public class MultiArrayVector3Int : MultiArray<Vector3Int> { public new List<ReorderableListVector3Int> items; }
    [Serializable] public class MultiArrayVector4 : MultiArray<Vector4> { public new List<ReorderableListVector4> items; }
    [Serializable] public class MultiArrayQuaternion : MultiArray<Quaternion> { public new List<ReorderableListQuaternion> items; }
    [Serializable] public class MultiArrayColor : MultiArray<Color> { public new List<ReorderableListColor> items; }
    [Serializable] public class MultiArrayColor32 : MultiArray<Color32> { public new List<ReorderableListColor32> items; }
#endif
    [Serializable] public class MultiArrayHSVColor : MultiArray<HSVColor> { public new List<ReorderableListHSVColor> items; }
#if UNITY_EDITOR || UNITY_STANDALONE
    [Serializable] public class MultiArrayObject : MultiArray<UnityEngine.Object> { public new List<ReorderableListObject> items; }
    [Serializable] public class MultiArrayScriptableObject : MultiArray<ScriptableObject> { public new List<ReorderableListScriptableObject> items; }
    [Serializable] public class MultiArrayGameObject : MultiArray<GameObject> { public new List<ReorderableListGameObject> items; }
    [Serializable] public class MultiArrayComponent : MultiArray<Component> { public new List<ReorderableListComponent> items; }
    [Serializable] public class MultiArrayTransform : MultiArray<Transform> { public new List<ReorderableListTransform> items; }
    [Serializable] public class MultiArrayBehaviour : MultiArray<Behaviour> { public new List<ReorderableListBehaviour> items; }
    [Serializable] public class MultiArrayMonoBehaviour : MultiArray<MonoBehaviour> { public new List<ReorderableListMonoBehaviour> items; }
    [Serializable] public class MultiArrayGraphic : MultiArray<Graphic> { public new List<ReorderableListGraphic> items; }
    [Serializable] public class MultiArrayText : MultiArray<Text> { public new List<ReorderableListText> items; }
    [Serializable] public class MultiArrayImage : MultiArray<Image> { public new List<ReorderableListImage> items; }
    [Serializable] public class MultiArrayRenderer : MultiArray<Renderer> { public new List<ReorderableListRenderer> items; }
    [Serializable] public class MultiArrayCamera : MultiArray<Camera> { public new List<ReorderableListCamera> items; }
#endif
    [Serializable] public class MultiArrayValueInRange : MultiArray<ValueInRange> { public new List<ReorderableListValueInRange> items; }

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
