using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.MSGame
{
    public class UserInput : MonoBehaviour
    {
        public Controller m_Controller;
        public Camera m_Camera;
        public Text[] m_Text;
        [Range(-1, 1)] public float m_SliderX;
        [Range(-1, 1)] public float m_SliderY;
        [Range(-1, 1)] public float m_SliderZ;
        public KeyCode Key_Up = KeyCode.Space;
        public KeyCode Key_Down = KeyCode.LeftControl;
        public KeyCode Key_Left = KeyCode.A;
        public KeyCode Key_Right = KeyCode.D;
        public KeyCode Key_Forward = KeyCode.W;
        public KeyCode Key_Back = KeyCode.S;
        [SerializeField] Vector2 MousePositon_Start;
        void Start()
        {
            m_Camera = Camera.main;
        }
        void Update()
        {
            if (Input.GetKey(Key_Left) || Input.GetKey(Key_Right))
            {
                if (Input.GetKey(Key_Left))
                {
                    m_SliderX = ValueAdjust.Lerp(m_SliderX, -1, Time.deltaTime);
                }
                else if (Input.GetKey(Key_Right))
                {
                    m_SliderX = ValueAdjust.Lerp(m_SliderX, 1, Time.deltaTime);
                }
            }
            else
            {
                m_SliderX = ValueAdjust.Lerp(m_SliderX, 0, Time.deltaTime);
            }
            if (Input.GetKey(Key_Up) || Input.GetKey(Key_Down))
            {
                if (Input.GetKey(Key_Up))
                {
                    m_SliderY = ValueAdjust.Lerp(m_SliderY, 1, Time.deltaTime);
                }
                else if (Input.GetKey(Key_Down))
                {
                    m_SliderY = ValueAdjust.Lerp(m_SliderY, -1, Time.deltaTime);
                }
            }
            else
            {
                m_SliderY = ValueAdjust.Lerp(m_SliderY, 0, Time.deltaTime);
            }
            if (Input.GetKey(Key_Forward) || Input.GetKey(Key_Back))
            {
                if (Input.GetKey(Key_Forward))
                {
                    m_SliderZ = ValueAdjust.Lerp(m_SliderZ, 1, Time.deltaTime);
                }
                else if (Input.GetKey(Key_Back))
                {
                    m_SliderZ = ValueAdjust.Lerp(m_SliderZ, -1, Time.deltaTime);
                }
            }
            else
            {
                m_SliderZ = ValueAdjust.Lerp(m_SliderZ, 0, Time.deltaTime);
            }
            if (Input.GetMouseButtonDown(1))
            {
                MousePositon_Start = Input.mousePosition;
            }
            if (Input.GetMouseButton(1) && m_Controller)
            {
                Vector2 MouseOnScreen = new Vector2((Input.mousePosition.x - MousePositon_Start.x) / Screen.width, (Input.mousePosition.y - MousePositon_Start.y) / Screen.height) * 2;
                m_Text[0].text = "Mouse：" + MouseOnScreen;
                //m_Camera.transform.RotateAround(m_Controller.transform.position, Vector3.up, MouseOnScreen.x * Time.deltaTime * 10);
                //m_Camera.transform.RotateAround(m_Controller.transform.position, Vector3.right, MouseOnScreen.y * Time.deltaTime * 10);
            }
        }
        private void LateUpdate()
        {
            if (!m_Controller)
                return;
            Vector3 Direction = new Vector3(m_SliderX, m_SliderY, m_SliderZ);
            m_Text[1].text = "Slider：" + Direction;
            if (Direction.magnitude > 0.1f)
            {
                m_Controller.Move(m_Camera.transform.TransformDirection(Direction));
                m_Controller.Throttle = Direction.magnitude;
            }
            else
            {
                m_Controller.Stop();
            }
        }
    }
}
