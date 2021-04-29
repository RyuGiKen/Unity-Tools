using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 残影特效
    /// </summary>
    public class GhostShadowRender : MonoBehaviour
    {
        [Tooltip("持续时间")] public float survivalTime = 1;
        [Tooltip("生成间隔")] public float intervalTime = 0.2f;
        [Tooltip("生成计时")] private float buildTimer = 0;
        [Tooltip("初始透明度")] [Range(0, 1)] public float initialAlpha = 1.0f;

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
        void Update()
        {
            if (shadowList != null)
            {
                if (meshRenderer.Length < 1)
                {
                    return;
                }
                buildTimer += Time.deltaTime;
                if (buildTimer >= intervalTime)
                {
                    buildTimer = 0;
                    for (int i = 0; i < meshRenderer.Length; i++)
                    {
                        if (meshRenderer[i] is SkinnedMeshRenderer)
                            CreateAfterImage(meshRenderer[i] as SkinnedMeshRenderer);
                        else if (meshRenderer[i] is MeshRenderer)
                            CreateAfterImage(meshRenderer[i] as MeshRenderer);
                    }
                }
                UpdateAfterImage();
            }
        }
        /// <summary>
        /// 生成残影
        /// </summary>
        void CreateAfterImage(SkinnedMeshRenderer renderer)
        {
            Mesh mesh = new Mesh();
            renderer.BakeMesh(mesh);
            Material material = new Material(renderer.material);
            SetMaterialRenderingMode(material, RenderingMode.Fade);
            shadowList.Add(new GhostShadow(mesh, material, this.transform.localToWorldMatrix, initialAlpha, survivalTime));
        }
        /// <summary>
        /// 生成残影
        /// </summary>
        void CreateAfterImage(MeshRenderer renderer)
        {
            Mesh mesh = renderer.GetComponent<MeshFilter>().mesh;
            Material material = new Material(renderer.material);
            SetMaterialRenderingMode(material, RenderingMode.Fade);
            shadowList.Add(new GhostShadow(mesh, material, this.transform.localToWorldMatrix, initialAlpha, survivalTime));
        }
        /// <summary>
        /// 刷新残影
        /// </summary>
        void UpdateAfterImage()
        {
            //刷新残影，根据生存时间销毁已过时的残影
            for (int i = shadowList.Count - 1; i >= 0; i--)
            {
                shadowList[i].duration -= Time.deltaTime;

                if (shadowList[i].duration <= 0)
                {
                    shadowList.Remove(shadowList[i]);
                    Destroy(shadowList[i]);
                    continue;
                }
                for (int j = 0; j < shadowList.Count; j++)
                {
                    Color color;
                    if (shadowList[i].material.HasProperty("_BaseColor"))
                    {
                        color = shadowList[i].material.GetColor("_BaseColor");
                        shadowList[i].alpha = (shadowList[i].duration * 1f / shadowList[i].durationMax);
                        color.a = shadowList[i].alpha;
                        shadowList[i].material.SetColor("_BaseColor", color);
                    }
                    else if (shadowList[i].material.HasProperty("_Color"))
                    {
                        color = shadowList[i].material.color;
                        shadowList[i].alpha = (shadowList[i].duration * 1f / shadowList[i].durationMax);
                        color.a = shadowList[i].alpha;
                        shadowList[i].material.SetColor("_Color", color);
                    }

                    Graphics.DrawMesh(shadowList[i].mesh, shadowList[i].matrix, shadowList[i].material, gameObject.layer);
                }
            }
        }
        /// <summary>
        /// 设置纹理渲染模式
        /// </summary>
        void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
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
    class GhostShadow : Object
    {
        [Tooltip("网格")] public Mesh mesh;
        [Tooltip("材质")] public Material material;
        [Tooltip("位置")] public Matrix4x4 matrix;
        [Tooltip("透明度")] public float alpha;
        [Tooltip("最大保留时间")] public float durationMax;
        [Tooltip("剩余时间")] public float duration;
        public GhostShadow(Mesh Mesh, Material Material, Matrix4x4 Matrix4x4, float Alpha, float Duration)
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
