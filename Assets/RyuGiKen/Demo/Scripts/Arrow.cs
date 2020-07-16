using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
using RyuGiKen.Tools;
public class Arrow : MonoBehaviour
{
    [Tooltip("刚体")] public Rigidbody m_Rigidbody;
    public BoxCollider boxCollider;
    public CapsuleCollider capsuleCollider;
    [Tooltip("命中粒子效果位置")] public Transform HitEffectPos;
    [Tooltip("命中效果")] public GameObject HitEffect;
    [Tooltip("通常运动轨迹")] public TrailRenderer moveTrack;
    [Tooltip("测试模式运动轨迹")] public TrailRenderer testTrack;
    [Tooltip("命中音效")] [SerializeField] AudioClip[] HitSound;
    [Tooltip("爆炸音效")] [SerializeField] AudioClip[] BoomSound;
    [Tooltip("自动跟踪对象")] public Transform Target;
    [Tooltip("自动跟踪对象角度偏移量")] [SerializeField] Vector3 offset;
    [Tooltip("是否曾命中目标")] [SerializeField] bool isTouched;
    public AudioSource m_AudioSoure;
    public float ForcePercent;
    private void Awake()
    {
        if (!m_Rigidbody)
            m_Rigidbody = this.GetComponentInChildren<Rigidbody>();
        if (!boxCollider)
            boxCollider = this.GetComponentInChildren<BoxCollider>();
        if (!capsuleCollider)
            capsuleCollider = this.GetComponentInChildren<CapsuleCollider>();
        ForcePercent = Mathf.InverseLerp(CrossBowControl.instance.Force_Min, CrossBowControl.instance.Force_Max, CrossBowControl.instance.Force);
    }
    private void OnEnable()
    {
        Target = new GameObject().transform;
        Target.parent = this.transform;
        Target.position = HitEffectPos.position;
        Target.rotation = this.transform.rotation;
    }
    private void Start()
    {
        Destroy(this.gameObject, 30f);//避免箭在场景中过长时间留存

        if (TrackArrowMain.instance.hitInfo.transform)
        {
            EnemyBody enemyBody = TrackArrowMain.instance.hitInfo.transform.GetComponent<EnemyBody>();
            if (enemyBody)
                Target = enemyBody.enemy.AimPos[Random.Range(0, enemyBody.enemy.AimPos.Length)];//获取跟踪对象为敌人胸口
        }
        HitEffectPos.LookAt(Target);//获取初期朝向

        Debug.Log(this.transform.name + " 目标 " + Target.name);
        //offset = this.transform.rotation.eulerAngles - HitEffect.transform.rotation.eulerAngles;
        //Debug.Log(offset);//方向
        //Debug.Log(Quaternion.Angle(HitEffect.transform.rotation, this.transform.rotation));//角度
    }
    private void Update()
    {
        //m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, Mathf.Clamp(m_Rigidbody.velocity.y, -0.05f, 10f), m_Rigidbody.velocity.z).normalized * m_Rigidbody.velocity.magnitude;//限制向下的速度向量
        if (!isTouched)
        {
            if (Quaternion.Angle(HitEffectPos.rotation, this.transform.rotation) < 15 && (HitEffectPos.position - Target.position).magnitude > 0.5f)//初期朝向角度过大时不进行跟踪
            {
                //改变自身朝向
                this.transform.forward = ValueAdjust.Lerp(this.transform.forward, (Target.transform.position - this.transform.position).normalized, Time.deltaTime, (this.transform.forward - (Target.transform.position - this.transform.position).normalized) * 15);
            }
            m_Rigidbody.velocity = this.transform.forward.normalized * (m_Rigidbody.velocity.magnitude + 30 * Time.deltaTime);//改变速度方向为目标方向
        }
    }
    private void LateUpdate()
    {
        if (testTrack && TrackArrowMain.instance)
            testTrack.enabled = TrackArrowMain.instance.testCanvas.activeInHierarchy;
        if (moveTrack && TrackArrowMain.instance)
            moveTrack.enabled = !TrackArrowMain.instance.testCanvas.activeInHierarchy;
        if ((this.transform.position - Camera.main.transform.position).magnitude > 200)//超出目视范围
        {
            Destroy(this.gameObject, 1);
        }
        if (moveTrack)
            moveTrack.time = m_Rigidbody.velocity.magnitude < 1 ? 0.1f : Mathf.Clamp((m_Rigidbody.velocity.magnitude - 30) / 5, 0.5f, 8f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.GetComponent<CrossBowControl>() && !isTouched && !collision.collider.isTrigger)
        {
            Debug.Log("命中：" + collision.collider.name);
            isTouched = true;
            GameObject point = Instantiate(HitEffectPos.gameObject, HitEffectPos.position, HitEffectPos.rotation, this.transform);
            point.name = "Step01";
            if (collision.transform.GetComponent<EnemyBody>())
                HitEnemy(collision.collider);
            else
                Hit(collision.collider);
        }
    }
    /// <summary>
    /// 播放粒子效果
    /// </summary>
    void ShowEffect(float ForcePercent = 0.15f)
    {
        GameObject hit;

        hit = Instantiate(HitEffect, HitEffectPos.position, HitEffectPos.rotation, HitEffectPos);
        hit.transform.localScale = Vector3.one * (2.5f + ForcePercent);

        HitEffectPos.gameObject.SetActive(true);
        Destroy(hit, 5);
        hit.transform.parent = null;
    }
    /// <summary>
    /// 命中敌人
    /// </summary>
    /// <param name="other"></param>
    void HitEnemy(Collider other)
    {
        Hit(other);
        try
        {
            other.GetComponent<EnemyBody>().enemy.BeHit();
        }
        catch { }
        capsuleCollider.enabled = false;
    }
    /// <summary>
    /// 命中
    /// </summary>
    /// <param name="other"></param>
    void Hit(Collider other)
    {
        m_Rigidbody.isKinematic = true;
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.useGravity = false;
        boxCollider.enabled = false;
        this.transform.parent = other.transform;
        //moveTrack.gameObject.SetActive(false);

        ShowEffect();
        m_AudioSoure.PlayOneShot(HitSound[Random.Range(0, HitSound.Length)]);

    }
}
