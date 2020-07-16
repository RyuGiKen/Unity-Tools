using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using RyuGiKen.Tools;
public class TrackArrowMain : MonoBehaviour
{
    public static TrackArrowMain instance;
    [Tooltip("测试面板")] public GameObject testCanvas;
    [Tooltip("横向[-1,1]")] public Slider m_SliderX;
    [Tooltip("纵向[-1,1]")] public Slider m_SliderY;
    [SerializeField] bool useMouse;
    public Text[] m_Text;
    [Tooltip("弩")] public CrossBowControl CrossBow;
    bool fire;
    [Tooltip("以弩坐标为摄像机父对象")] GameObject CameraParent;
    public Image AimImage;
    public RaycastHit hitInfo;
    public GameObject HitPoint;
    [SerializeField] Vector3 CrossBowStartPos;
    Vector2[] lastPos = new Vector2[10];//[-1,1]
    Vector2 lastPosM;
    Vector2 curPos;//[-1,1]
    void Awake()
    {
        if (gameObject.activeInHierarchy)
            if (!instance) instance = this;
        if (instance != this)
            Destroy(this.gameObject);
        CameraParent = new GameObject("CameraParent");
        HitPoint = new GameObject("HitPoint");
    }
    void Start()
    {
        if (!testCanvas) { testCanvas = GameObject.Find("TestCanvas"); }
        if (testCanvas) { testCanvas.SetActive(false); }
        CrossBowStartPos = CrossBow.transform.position;
        CameraParent.transform.position = CrossBowStartPos;
        Camera.main.transform.parent = CameraParent.transform;
        m_SliderX.maxValue = 1;
        m_SliderX.minValue = -1;
    }
    void Update()
    {
        if (useMouse)
            fire = Input.GetMouseButton(0);
        else
            fire = Input.GetKey(KeyCode.Space);

        if (useMouse)
        {
            curPos = new Vector2(Input.mousePosition.x / Screen.width * 2 - 1, Input.mousePosition.y / Screen.height * 2 - 1);
        }
        else
        {
            curPos = new Vector2(Mathf.Clamp(m_SliderX.value, -1, 1), Mathf.Clamp(m_SliderY.value, -1, 1));
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                m_SliderX.value -= Time.deltaTime * 0.5f;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                m_SliderX.value += Time.deltaTime * 0.5f;
            }
        }
        curPos = new Vector2(Mathf.Clamp(curPos.x, -1, 1), Mathf.Clamp(curPos.y, -1, 1));
        m_SliderX.value = curPos.x;
        m_SliderY.value = curPos.y;
        GameParameter.ControlXY = (curPos + lastPosM) / (lastPos.Length + 1);

        CrossBow.gameObject.SetActive(true);
        CrossBow.transform.localEulerAngles = new Vector3(-GameParameter.ControlXY.y * 12f, GameParameter.ControlXY.x * 45f, 0);
        CrossBow.transform.position = CrossBowStartPos;// + new Vector3(0, 0, 3.3f) * GameParameter.ControlXY.x;
        CameraParent.transform.localEulerAngles = new Vector3(-GameParameter.ControlXY.y * 10f, GameParameter.ControlXY.x * 40f, 0);
        CameraParent.transform.position = CrossBowStartPos;// + new Vector3(0, 0, 3f) * GameParameter.ControlXY.x;
        if (CrossBow.Fire)
            Camera.main.fieldOfView = ValueAdjust.Lerp(Camera.main.fieldOfView, 20, Time.deltaTime, 10);
        else
            Camera.main.fieldOfView = ValueAdjust.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime, 40);

        GameParameter.Fire = fire;
        lastPosM = Vector2.zero;
        for (int i = lastPos.Length - 1; i >= 0; i--)
        {
            if (i == 0)
            { lastPos[0] = curPos; }
            else
            { lastPos[i] = lastPos[i - 1]; }
            lastPosM += lastPos[i];
        }
    }
    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.F8))//切换测试模式
        {
            if (testCanvas)
                testCanvas.SetActive(!testCanvas.activeInHierarchy);
        }
        Physics.Raycast(CrossBow.boresight.transform.position, CrossBow.boresight.transform.forward, out hitInfo);
        CrossBow.boresight.SetPosition(1, Vector3.forward * (hitInfo.transform ? Mathf.Clamp((hitInfo.point - CrossBow.boresight.transform.position).magnitude, 0, 100) : 100));
        SetAim(hitInfo);
        if (testCanvas.activeInHierarchy)
        {
            m_Text[0].text = "目标：" + (hitInfo.transform && hitInfo.transform.GetComponent<EnemyBody>() && !hitInfo.transform.GetComponent<EnemyBody>().enemy.isDie ? hitInfo.transform.GetComponent<EnemyBody>().enemy.name : "");
            m_Text[1].text = "ControlXY：" + GameParameter.ControlXY.ToString("F3");
        }
    }
    /// <summary>
    /// 刷新准心位置
    /// </summary>
    /// <param name="hitInfo"></param>
    void SetAim(RaycastHit hitInfo)
    {
        if (hitInfo.transform)
        {
            HitPoint.transform.position = hitInfo.point;
            if (hitInfo.transform.GetComponent<EnemyBody>() && !hitInfo.transform.GetComponent<EnemyBody>().enemy.isDie)
            {
                AimImage.gameObject.SetActive(true);
                //HitPoint.transform.position = hitInfo.transform.GetComponent<EnemyBody>().enmy.transform.position;
                //HitPoint.transform.position = hitInfo.transform.GetComponent<EnemyBody>().enmy.transform.GetChild(3).position;
                HitPoint.transform.position = hitInfo.transform.GetComponent<EnemyBody>().enemy.AimPos[0].position;
                //AimImage.transform.position = Camera.main.WorldToScreenPoint(hitInfo.point);
                AimImage.transform.position = Camera.main.WorldToScreenPoint(HitPoint.transform.position);
            }
            else
            {
                AimImage.gameObject.SetActive(false);
            }
            m_Text[2].text = "方向：" + hitInfo.point;
        }
        else
        {
            AimImage.gameObject.SetActive(false);
        }
    }
}
