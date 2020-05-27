using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Tools;
public class Test : MonoBehaviour
{
    public Text[] m_Text;
    void Start()
    {
        
    }

    void Update()
    {
        m_Text[0].text = ValueAdjust.ShowTime(Time.time);
    }
}
