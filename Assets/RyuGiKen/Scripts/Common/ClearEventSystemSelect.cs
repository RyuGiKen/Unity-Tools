using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 松开时清空选择
/// </summary>
[DisallowMultipleComponent]
public class ClearEventSystemSelect : MonoBehaviour
{
    public GameObject Selected;
    void Update()
    {
        Selected = EventSystem.current.currentSelectedGameObject;
    }
    void LateUpdate()
    {
        Selectable selectable = null;
        if (EventSystem.current.currentSelectedGameObject)
            EventSystem.current.currentSelectedGameObject.TryGetComponent<Selectable>(out selectable);
        if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        {
            if (selectable && selectable is InputField && (selectable as InputField).interactable)
            {
                InputField inputField = selectable as InputField;
                if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
                {
                    if (!inputField.multiLine)
                        EventSystem.current.SetSelectedGameObject(null);
                }
            }
            else
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
