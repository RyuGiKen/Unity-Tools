using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using RyuGiKen;
[RequireComponent(typeof(CinemachineFreeLook))]
public class CinemachineFreeLookController : MonoBehaviour
{
    public CinemachineFreeLook FreeLook;
    public Vector2 defaultXYValue;
    bool HoldingButton;

    public RectTransform ScreenRect;
    public Rect ScreenRectBounds;
    public Vector2 InputVector;
    Vector2 LastVector;
    Vector2 LastPos;
    private void Awake()
    {
        FreeLook = GetComponent<CinemachineFreeLook>();
    }
    private void Start()
    {
        FreeLook.enabled = true;
    }
    void Update()
    {
        bool input = !EventSystem.current.currentSelectedGameObject && Input.GetMouseButton(1);
        if (ScreenRect)
        {
            ScreenRectBounds = ScreenRect.GetRectInCanvas(ScreenRect.root.GetComponent<Canvas>().transform.localScale);
            Vector2 mousePos = Input.mousePosition;
            bool inRange = mousePos.x.InRange(ScreenRectBounds.xMin, ScreenRectBounds.xMax) && mousePos.y.InRange(ScreenRectBounds.yMin, ScreenRectBounds.yMax);
            Vector2 radio = Vector2.zero;
            radio.x = ValueAdjust.ToPercentPlusMinus01(mousePos.x, ScreenRectBounds.xMin, ScreenRectBounds.xMax, 1, false);
            radio.y = ValueAdjust.ToPercentPlusMinus01(mousePos.y, ScreenRectBounds.yMin, ScreenRectBounds.yMax, 1, false);
            if (!HoldingButton && input && inRange)
                HoldingButton = true;
            else if (HoldingButton && !input)
                HoldingButton = false;
            if (HoldingButton)
            {
                InputVector = (radio - LastVector) / 10f / Time.deltaTime;
            }
            else
            {
                InputVector = Vector2.zero;
            }
            LastVector = radio;
        }
        else
        {
            HoldingButton = input;
            InputVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
        if (FreeLook.isActiveAndEnabled)
        {
            FreeLook.m_XAxis.m_InputAxisValue = HoldingButton ? InputVector.x : 0;
            FreeLook.m_YAxis.m_InputAxisValue = HoldingButton ? InputVector.y : 0;
        }
    }
    [ContextMenu("÷ÿ÷√Œª÷√")]
    public void ReSetPos()
    {
        if (!FreeLook)
            return;
        FreeLook.m_XAxis.Value = defaultXYValue.x;
        FreeLook.m_YAxis.Value = defaultXYValue.y;
    }
}
