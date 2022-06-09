using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 松开时清空选择
/// </summary>
[DisallowMultipleComponent]
public class ClearEventSystemSelect : MonoBehaviour
{
    //public GameObject Selected;
    void Update()
    {
        //Selected = EventSystem.current.currentSelectedGameObject;
    }
    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
            EventSystem.current.SetSelectedGameObject(null);
    }
}
