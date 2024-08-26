using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    public class PropertyTextGroup : MonoBehaviour
    {
        public Text NameText;
        public Text ValueText;
        public void SetText(string name, string value)
        {
            NameText.SetText(name);
            ValueText.SetText(value);
        }
    }
}
