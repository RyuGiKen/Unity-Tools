using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 残影特效
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/残影特效")]
    public class GhostShadowRender : MonoBehaviour
    {
        [Tooltip("生成")] public bool Enable = true;
        [Tooltip("持续时间")] public float survivalTime = 1;
        [Tooltip("生成间隔")] public float intervalTime = 0.2f;
        [Tooltip("生成计时")] private float buildTimer = 0;
        [Tooltip("初始透明度")] [Range(0, 1)] public float initialAlpha = 1.0f;
        /// <summary>
        /// 替换模型材质，为空时使用模型材质
        /// </summary>
        [Tooltip("残影材质")] public Material GhostShadowMaterial;
        [Tooltip("渲染模式")] public RenderingMode renderMode = RenderingMode.Fade;
        [Tooltip("残影列表")] private List<GhostShadow> shadowList;
        [SerializeField] Renderer[] meshRenderer;
        void Awake()
        {
            shadowList = new List<GhostShadow>();
            meshRenderer = this.GetComponentsInChildren<SkinnedMeshRenderer>().ToList<Renderer>().AddList(this.GetComponentsInChildren<MeshRenderer>().ToList<Renderer>()).ToArray();
        }
        private void Start()
        {
            meshRenderer = meshRenderer.ClearRepeatingItem();
        }
        void LateUpdate()
        {
            if (shadowList != null)
            {
                if (meshRenderer.Length < 1)
                {
                    return;
                }
                buildTimer += Time.deltaTime;
                if (buildTimer >= intervalTime && Enable)
                {
                    buildTimer = 0;
                    for (int i = 0; i < meshRenderer.Length; i++)
                    {
                        if (meshRenderer[i] is SkinnedMeshRenderer)
                            CreateGhostShadow(meshRenderer[i] as SkinnedMeshRenderer);
                        else if (meshRenderer[i] is MeshRenderer)
                            CreateGhostShadow(meshRenderer[i] as MeshRenderer);
                    }
                }
                UpdateGhostShadow();
            }
        }
        /// <summary>
        /// 生成残影
        /// </summary>
        void CreateGhostShadow(SkinnedMeshRenderer renderer)
        {
            Mesh mesh = new Mesh();
            renderer.BakeMesh(mesh);
            Material[] materials = new Material[renderer.materials.Length];
            float[] alpha = new float[renderer.materials.Length];
            if (GhostShadowMaterial)
            {
                if (GhostShadowMaterial.HasProperty("_BaseColor"))
                {
                    alpha.SetArrayAll(GhostShadowMaterial.GetColor("_BaseColor").a);
                }
                else if (GhostShadowMaterial.HasProperty("_Color"))
                {
                    alpha.SetArrayAll(GhostShadowMaterial.color.a);
                }
                for (int i = 0; i < renderer.materials.Length; i++)
                    materials[i] = new Material(GhostShadowMaterial);
                shadowList.Add(new GhostShadow(mesh, materials, this.transform.localToWorldMatrix, alpha, survivalTime));
            }
            else
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    materials[i] = new Material(renderer.materials[i]);
                    SetMaterialRenderingMode(materials[i], renderMode);
                    if (materials[i].HasProperty("_BaseColor"))
                    {
                        alpha[i] = materials[i].GetColor("_BaseColor").a;
                    }
                    else if (materials[i].HasProperty("_Color"))
                    {
                        alpha[i] = materials[i].color.a;
                    }
                }
                shadowList.Add(new GhostShadow(mesh, materials, this.transform.localToWorldMatrix, alpha, survivalTime));
            }
        }
        /// <summary>
        /// 生成残影
        /// </summary>
        void CreateGhostShadow(MeshRenderer renderer)
        {
            Mesh mesh = renderer.GetComponent<MeshFilter>().mesh;
            Material[] materials = new Material[renderer.materials.Length];
            float[] alpha = new float[renderer.materials.Length];
            if (GhostShadowMaterial)
            {
                if (GhostShadowMaterial.HasProperty("_BaseColor"))
                {
                    alpha.SetArrayAll(GhostShadowMaterial.GetColor("_BaseColor").a);
                }
                else if (GhostShadowMaterial.HasProperty("_Color"))
                {
                    alpha.SetArrayAll(GhostShadowMaterial.color.a);
                }
                for (int i = 0; i < renderer.materials.Length; i++)
                    materials[i] = new Material(GhostShadowMaterial);
                shadowList.Add(new GhostShadow(mesh, materials, this.transform.localToWorldMatrix, alpha, survivalTime));
            }
            else
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    materials[i] = new Material(renderer.materials[i]);
                    SetMaterialRenderingMode(materials[i], renderMode);
                    if (materials[i].HasProperty("_BaseColor"))
                    {
                        alpha[i] = materials[i].GetColor("_BaseColor").a;
                    }
                    else if (materials[i].HasProperty("_Color"))
                    {
                        alpha[i] = materials[i].color.a;
                    }
                }
                shadowList.Add(new GhostShadow(mesh, materials, this.transform.localToWorldMatrix, alpha, survivalTime));
            }
        }
        /// <summary>
        /// 刷新残影
        /// </summary>
        void UpdateGhostShadow()
        {
            //刷新残影，根据时间销毁已过时的残影
            for (int i = shadowList.Count - 1; i >= 0; i--)
            {
                shadowList[i].duration -= Time.deltaTime;
                if (shadowList[i].duration <= 0)
                {
                    Destroy(shadowList[i]);
                    shadowList.Remove(shadowList[i]);
                    continue;
                }
                for (int j = 0; j < shadowList[i].material.Length; j++)
                {
                    Color color;
                    if (shadowList[i].material[j].HasProperty("_BaseColor"))
                    {
                        color = shadowList[i].material[j].GetColor("_BaseColor");
                        color.a = shadowList[i].alpha[j] * (shadowList[i].duration * 1f / shadowList[i].durationMax);
                        shadowList[i].material[j].SetColor("_BaseColor", color);
                    }
                    else if (shadowList[i].material[j].HasProperty("_Color"))
                    {
                        color = shadowList[i].material[j].color;
                        color.a = shadowList[i].alpha[j] * (shadowList[i].duration * 1f / shadowList[i].durationMax);
                        shadowList[i].material[j].SetColor("_Color", color);
                    }
                    Graphics.DrawMesh(shadowList[i].mesh, shadowList[i].matrix, shadowList[i].material[j], gameObject.layer, null, j);
                }
            }
        }
        /// <summary>
        /// 设置纹理渲染模式
        /// </summary>
        void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
        {
            try
            {
                switch (renderingMode)
                {
                    case RenderingMode.Opaque:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = -1;
                        break;
                    case RenderingMode.Cutout:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.EnableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 2450;
                        break;
                    case RenderingMode.Fade:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                    case RenderingMode.Transparent:
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                }
            }
            catch { }
        }
    }
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }
    /// <summary>
    /// 残影
    /// </summary>
    public class GhostShadow : Object
    {
        [Tooltip("网格")] public Mesh mesh;
        [Tooltip("材质")] public Material[] material;
        [Tooltip("位置")] public Matrix4x4 matrix;
        [Tooltip("初始透明度")] public float[] alpha;
        [Tooltip("最大保留时间")] public float durationMax;
        [Tooltip("剩余时间")] public float duration;
        public GhostShadow(Mesh Mesh, Material Material, Matrix4x4 Matrix4x4, float Alpha, float Duration)
        {
            mesh = Mesh;
            material = new Material[] { Material };
            matrix = Matrix4x4;
            alpha = new float[] { Alpha };
            durationMax = Duration;
            duration = durationMax;
        }
        public GhostShadow(Mesh Mesh, Material[] Material, Matrix4x4 Matrix4x4, float[] Alpha, float Duration)
        {
            mesh = Mesh;
            material = Material;
            matrix = Matrix4x4;
            alpha = Alpha;
            durationMax = Duration;
            duration = durationMax;
        }
    }
}
