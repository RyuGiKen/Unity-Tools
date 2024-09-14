using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Direction = UnityEngine.UI.Slider.Direction;
#if UNITY_EDITOR
using UnityEditor;
#endif
[AddComponentMenu("UI/Effects/UITextGradient")]
public class UITextGradient : BaseMeshEffect
{
    public Direction direction = Direction.TopToBottom;
    [SerializeField]
    private Color32 topColor = Color.white;
    [SerializeField]
    private Color32 bottomColor = Color.black;

    private List<UIVertex> mVertexList;
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        if (mVertexList == null)
        {
            mVertexList = new List<UIVertex>();
        }

        vh.GetUIVertexStream(mVertexList);
        ApplyGradient(mVertexList);

        vh.Clear();
        vh.AddUIVertexTriangleStream(mVertexList);
    }
    protected virtual void ApplyGradient(List<UIVertex> vertexList)
    {
        Direction direction = this.direction;
        if (!(graphic is Text))
        {
            switch (direction)
            {
                case Direction.LeftToRight:
                    direction = Direction.TopToBottom;
                    break;
                case Direction.RightToLeft:
                    direction = Direction.BottomToTop;
                    break;
                case Direction.BottomToTop:
                    direction = Direction.RightToLeft;
                    break;
                case Direction.TopToBottom:
                    direction = Direction.LeftToRight;
                    break;
            }
        }
        Color color1 = topColor;
        Color color2 = bottomColor;
        switch (direction)
        {
            case Direction.LeftToRight:
            case Direction.BottomToTop:
                color1 = bottomColor;
                color2 = topColor;
                break;
        }
        for (int i = 0; i < vertexList.Count;)
        {
            switch (direction)
            {
                case Direction.LeftToRight:
                case Direction.RightToLeft:
                    ChangeColor(vertexList, i, color1);
                    ChangeColor(vertexList, i + 1, color2);
                    ChangeColor(vertexList, i + 2, color2);
                    ChangeColor(vertexList, i + 3, color2);
                    ChangeColor(vertexList, i + 4, color1);
                    ChangeColor(vertexList, i + 5, color1);
                    break;
                default:
                case Direction.BottomToTop:
                case Direction.TopToBottom:
                    ChangeColor(vertexList, i, color1);
                    ChangeColor(vertexList, i + 1, color1);
                    ChangeColor(vertexList, i + 2, color2);
                    ChangeColor(vertexList, i + 3, color2);
                    ChangeColor(vertexList, i + 4, color2);
                    ChangeColor(vertexList, i + 5, color1);
                    break;
            }
            i += 6;
        }
    }
    protected virtual void ChangeColor(List<UIVertex> verList, int index, Color color)
    {
        UIVertex temp = verList[index];
        temp.color = color;
        verList[index] = temp;
    }
    public virtual void SetColor(Color? color1, Color? color2)
    {
        if (color1.HasValue)
            topColor = color1.Value;//.ToColor32();
        if (color2.HasValue)
            bottomColor = color2.Value;//.ToColor32();
    }
    public static void SetTextStyle(Text text, TextStyleGroup style, Outline outline, Shadow shadow, UITextGradient gradient)
    {
        if (!text)
            return;
        if (style.font)
            text.font = style.font;
        if (style.fontStyle.HasValue)
            text.fontStyle = style.fontStyle.Value;
        if (style.fontSize.HasValue)
            text.fontSize = style.fontSize.Value;
        if (style.color.HasValue)
            text.color = style.color.Value;
        if (outline)
        {
            if (style.outlineEnable.HasValue)
                outline.enabled = style.outlineEnable.Value;
            if (style.oulineColor.HasValue)
                outline.effectColor = style.oulineColor.Value;
            if (style.outlineSize.HasValue)
                outline.effectDistance = style.outlineSize.Value;
        }
        if (shadow)
        {
            if (style.shadowEnable.HasValue)
                shadow.enabled = style.shadowEnable.Value;
            if (style.shadowColor.HasValue)
                shadow.effectColor = style.shadowColor.Value;
            if (style.shadowSize.HasValue)
                shadow.effectDistance = style.shadowSize.Value;
        }
        if (gradient)
        {
            if (style.gradientEnable.HasValue)
                gradient.enabled = style.gradientEnable.Value;
            gradient.SetColor(style.gradientColor1, style.gradientColor2);
        }
    }
}
public struct TextStyleGroup
{
    public Font font;
    public FontStyle? fontStyle;
    public int? fontSize;
    public Color? color;
    public bool? outlineEnable;
    public Color? oulineColor;
    public Vector2? outlineSize;
    public bool? shadowEnable;
    public Color? shadowColor;
    public Vector2? shadowSize;
    public bool? gradientEnable;
    public Color? gradientColor1;
    public Color? gradientColor2;
}
#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomEditor(typeof(UITextGradient), true)]
    public class UITextGradientEditor : Editor
    {
        SerializedProperty topColor;
        SerializedProperty bottomColor;
        void OnEnable()
        {
            topColor = serializedObject.FindProperty("topColor");
            bottomColor = serializedObject.FindProperty("bottomColor");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[1];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "交换颜色";
                    break;
                default:
                    ButtonName[0] = "Exchange Color";
                    break;
            }
            bool changeValue = false;
            if (GUILayout.Button(ButtonName[0]))
            {
                changeValue = true;
                //(target as UITextGradient).ExchangeColor();
                Color temp = topColor.colorValue;
                topColor.colorValue = bottomColor.colorValue;
                bottomColor.colorValue = temp;
            }
            if (changeValue)
            {
                //serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif