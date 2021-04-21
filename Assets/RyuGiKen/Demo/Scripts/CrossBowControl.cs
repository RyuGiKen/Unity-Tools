using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.TrackArrow
{
    public class CrossBowControl : MonoBehaviour
    {
        public static CrossBowControl instance;
        public Animator anim;
        public AudioClip[] audioClips;//0为拉弦音效，其他为射出音效
        public AudioSource m_Audio;
        [Tooltip("生成箭预制件")] public Transform Arrow_Prefab;
        [Tooltip("动画中的箭")] public GameObject Arrow_Anim;
        public bool Holding;
        public bool Fire;
        [Tooltip("射出力量")] public float Force;
        [Tooltip("最大力量")] public float Force_Min = 10;
        [Tooltip("最小力量")] public float Force_Max = 90;
        [Tooltip("射出箭数")] int ShotNum = 0;
        [HideInInspector] public bool canPlayFireEffect;
        public LineRenderer boresight;
        public float BoresightTime;
        public bool canShowBoresight;
        [SerializeField] [Tooltip("发射冷却计时")] float ColdDownTime = 0;
        [Tooltip("发射冷却")] public float ColdDownTime_Max = 2;
        void Awake()
        {
            if (gameObject.activeInHierarchy)
                if (!instance) instance = this;
        }
        private void OnEnable()
        {
            Force = 0;
            ColdDownTime = 0;
        }
        void Start()
        {
            Arrow_Anim.GetComponent<Renderer>().enabled = false;
            boresight.material.color = ColorAdjust.ColorAlphaChange(boresight.material.color, 0);
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                if (ColdDownTime >= -ColdDownTime_Max)
                    ColdDownTime -= Time.deltaTime;
                Fire = GameParameter.Fire && ColdDownTime < -ColdDownTime_Max;
            }
            anim.SetBool("Fire", Fire);
            Holding = anim.GetBool("Holding");
            if (Fire)
            {
                Force += Time.deltaTime * (Force_Max - Force_Min);
            }
            Force = Mathf.Clamp(Force, Force_Min, Force_Max);
            if (BoresightTime >= 0)
            {
                BoresightTime -= Time.deltaTime;
                canShowBoresight = false;
            }
            else
                canShowBoresight = true;
        }
        private void LateUpdate()
        {
            if (canPlayFireEffect)//发射
            {
                ColdDownTime = 0;
                ShotNum++;
                canPlayFireEffect = false;
                m_Audio.PlayOneShot(audioClips[Random.Range(1, audioClips.Length)]);//播放射出音效
                Arrow_Anim.GetComponent<Renderer>().enabled = false;
                Transform newArrow = Instantiate(Arrow_Prefab, Arrow_Anim.transform.position, this.transform.rotation);
                //newArrow.localScale = this.transform.localScale + Vector3.one * (Force - Force_Min) / (Force_Max - Force_Min);
                newArrow.GetComponent<Arrow>().m_Rigidbody.AddForce(this.transform.forward * Force);
                newArrow.name += ShotNum + "_" + Force.ToString("F2");
                Force = Force_Min;
                BoresightTime = 1f;
            }
            if (Holding)
            {
                if (!m_Audio.isPlaying)
                    m_Audio.PlayOneShot(audioClips[0]);//播放拉弦音效
                Arrow_Anim.GetComponent<Renderer>().enabled = true;
            }
            if (canShowBoresight)
            {
                boresight.material.color = ColorAdjust.ColorAlphaChange(boresight.material.color, ValueAdjust.Lerp(boresight.material.color.a, 0.6f, Time.deltaTime, 5f));
            }
            else
            {
                boresight.material.color = ColorAdjust.ColorAlphaChange(boresight.material.color, ValueAdjust.Lerp(boresight.material.color.a, 0f, Time.deltaTime, 2f));
            }
        }
    }
}
